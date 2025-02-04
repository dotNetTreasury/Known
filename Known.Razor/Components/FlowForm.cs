﻿namespace Known.Razor.Components;

class FlowFormOption
{
    public Size? Size { get; set; }
    public string Status { get; set; }
    public string UserRole { get; set; }
    public string UserLabel { get; set; }
    public string NoteLabel { get; set; }
    public string ConfirmText { get; set; }
    public bool NoteRequired { get; set; }
    public FlowFormInfo Model { get; set; }
    public Action<FlowFormInfo> OnConfirm { get; set; }
}

class FlowForm : Form
{
    private readonly SysUserList PickUser = new();

    [Parameter] public FlowFormOption Option { get; set; }

    protected override void BuildFields(RenderTreeBuilder builder)
    {
        builder.Hidden(nameof(FlowFormInfo.BizId));
        if (Context.IsMobile)
            BuildDivFields(builder);
        else
            BuildTableFields(builder);
    }

    private void BuildDivFields(RenderTreeBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(Option.Status))
            builder.Field<Text>("状态", nameof(FlowFormInfo.BizStatus)).Value(Option.Status).ReadOnly(true).Build();
        if (!string.IsNullOrWhiteSpace(Option.UserRole))
        {
            PickUser.SetRole(Option.UserRole);
            builder.Field<Picker>(Option.UserLabel, nameof(FlowFormInfo.User), true).Set(f => f.Pick, PickUser).Build();
        }
        builder.Field<TextArea>(Option.NoteLabel, nameof(FlowFormInfo.Note), Option.NoteRequired).Build();
    }

    private void BuildTableFields(RenderTreeBuilder builder)
    {
        builder.Table(table =>
        {
            if (!string.IsNullOrWhiteSpace(Option.Status))
            {
                table.Tr(attr =>
                {
                    table.Field<Text>("状态", nameof(FlowFormInfo.BizStatus)).Value(Option.Status).ReadOnly(true).Build();
                });
            }
            if (!string.IsNullOrWhiteSpace(Option.UserRole))
            {
                PickUser.SetRole(Option.UserRole);
                table.Tr(attr =>
                {
                    table.Field<Picker>(Option.UserLabel, nameof(FlowFormInfo.User), true).Set(f => f.Pick, PickUser).Build();
                });
            }
            table.Tr(attr =>
            {
                table.Field<TextArea>(Option.NoteLabel, nameof(FlowFormInfo.Note), Option.NoteRequired).Build();
            });
        });
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.OK, Callback(OnSave));
        builder.Button(FormButton.Cancel, Callback(OnCancel));
    }

    private void OnSave()
    {
        if (!Validate())
            return;

        UI.Confirm(Option.ConfirmText, () =>
        {
            var info = Utils.MapTo<FlowFormInfo>(Data);
            Option.OnConfirm?.Invoke(info);
        });
    }
}