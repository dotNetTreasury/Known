﻿using Known.Core.Cells;

namespace Known.Core.Helpers;

public sealed class ImportHelper
{
    public const string BizType = "ImportFiles";

    private ImportHelper() { }

    internal static ImportFormInfo GetImport(string bizId, SysTask task)
    {
        var info = new ImportFormInfo { BizId = bizId, BizType = BizType, IsFinished = true };
        if (task != null)
        {
            switch (task.Status)
            {
                case Constants.TaskPending:
                    info.Message = "导入任务等待中...";
                    info.IsFinished = false;
                    break;
                case Constants.TaskRunning:
                    info.Message = "导入任务执行中...";
                    info.IsFinished = false;
                    break;
                case Constants.TaskFailed:
                    info.Message = "导入失败！";
                    info.Error = task.Note;
                    break;
                case Constants.TaskSuccess:
                    info.Message = "";
                    break;
            }
        }

        info.Columns = GetRuleColumns(bizId);
        return info;
    }

    private static List<string> GetRuleColumns(string bizId)
    {
        var columns = BaseImport.GetImportColumns(bizId);
        if (columns == null || columns.Count == 0)
            return new List<string>();

        return columns.Select(c => c.Name).ToList();
    }

    internal static byte[] GetImportRule(string bizId)
    {
        var columns = BaseImport.GetImportColumns(bizId);
        if (columns == null || columns.Count == 0)
            return Array.Empty<byte>();

        var excel = ExcelFactory.Create();
        var sheet = excel.CreateSheet("Sheet1");
        sheet.SetCellValue("A1", "提示：红色栏位为必填栏位！", new StyleInfo { IsBorder = true });
        sheet.MergeCells(0, 0, 1, columns.Count);
        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            sheet.SetColumnWidth(i, 13);
            sheet.SetCellValue(1, i, column.Note, new StyleInfo { IsBorder = true, IsTextWrapped = true });
            var fontColor = column.Required ? Color.Red : Color.White;
            sheet.SetCellValue(2, i, column.Name, new StyleInfo { IsBorder = true, FontColor = fontColor, BackgroundColor = Utils.FromHtml("#6D87C1") });
        }
        sheet.SetRowHeight(1, 30);
        var stream = excel.SaveToStream();
        return stream.ToArray();
    }

    internal static SysTask CreateTask(ImportFormInfo form)
    {
        return new SysTask
        {
            BizId = form.BizId,
            Type = form.BizType,
            Name = form.BizName,
            Target = "",
            Status = Constants.TaskPending
        };
    }

    public static Task Execute()
    {
        return Task.Run(() =>
        {
            TaskHelper.Run(BizType, (db, task) =>
            {
                var import = BaseImport.Create(task.BizId, db);
                if (import == null)
                    return Result.Error("导入方法未注册，无法执行！");

                var file = db.QueryById<SysFile>(task.Target);
                return import.Execute(file);
            });
        });
    }

    public static Result ReadFile(SysFile file, Action<ImportRow> action)
    {
        var path = KCConfig.GetUploadPath(file.Path);
        if (!File.Exists(path))
            return Result.Error("导入文件不存在！");

        if (!path.EndsWith(".txt"))
            return ReadExcelFile(path, action);

        var columns = string.IsNullOrWhiteSpace(file.Note)
                    ? new List<string>()
                    : ImportFormInfo.GetImportColumns(file.Note);
        return ReadTextFile(path, columns, action);
    }

    private static Result ReadExcelFile(string path, Action<ImportRow> action)
    {
        var excel = ExcelFactory.Create(path);
        if (excel == null)
            return Result.Error("Excel创建失败！");

        var errors = new Dictionary<int, string>();
        var lines = excel.SheetToDictionaries(0, 2);
        for (int i = 0; i < lines.Count; i++)
        {
            var item = new ImportRow();
            foreach (var line in lines[i])
            {
                item[line.Key] = line.Value;
            }
            action?.Invoke(item);
            if (!string.IsNullOrWhiteSpace(item.ErrorMessage))
                errors.Add(i, item.ErrorMessage);
        }
        return ReadResult(errors);
    }

    private static Result ReadTextFile(string path, List<string> columns, Action<ImportRow> action)
    {
        var errors = new Dictionary<int, string>();
        var lines = File.ReadAllLines(path);
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var items = line.Split('\t');
            if (items[0] == columns[0])
                continue;

            var item = new ImportRow();
            for (int j = 0; j < columns.Count; j++)
            {
                item[columns[j]] = items.Length > j ? items[j] : "";
            }
            action?.Invoke(item);
            if (!string.IsNullOrWhiteSpace(item.ErrorMessage))
                errors.Add(i, item.ErrorMessage);
        }
        return ReadResult(errors);
    }

    private static Result ReadResult(Dictionary<int, string> errors)
    {
        if (errors.Count == 0)
            return Result.Success("校验成功！");

        var error = string.Join(Environment.NewLine, errors.Select(e => $"第{e.Key}行：{e.Value}"));
        return Result.Error($"校验失败！{Environment.NewLine}{error}");
    }
}