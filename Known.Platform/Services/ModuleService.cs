﻿using System.Collections.Generic;
using Known.Extensions;
using Known.Platform.Repositories;

namespace Known.Platform.Services
{
    public class ModuleService : PlatformService
    {
        public ModuleService(Context context) : base(context)
        {
        }

        public IModuleRepository Repository
        {
            get { return LoadRepository<IModuleRepository>(); }
        }

        public PagingResult<Module> QueryModules(PagingCriteria criteria)
        {
            return Repository.QueryModules(criteria);
        }

        public List<Module> GetModules(bool isTree = false)
        {
            var modules = Repository.QueryList<Module>();
            if (modules == null)
            {
                modules = new List<Module>();
            }

            if (isTree)
            {
                modules.Insert(0, new Module
                {
                    Id = "0",
                    ParentId = "-1",
                    Code = Setting.Instance.SystemId,
                    Name = Setting.Instance.SystemName
                });
            }

            return modules;
        }

        #region GetModule
        public Module GetModule(string id)
        {
            if (id == "devTool")
            {
                return new Module
                {
                    Id = "devTool",
                    Code = "DevTool",
                    Name = "开发工具",
                    ViewType = ViewType.SplitPageView,
                    Extension = new { LeftPartialName = "DevTool/LeftMenu" }.ToJson()
                };
            }

            var module = Repository.QueryById<Module>(id);
            if (module != null)
            {
                if (module.Code == "Module" && string.IsNullOrWhiteSpace(module.Extension))
                {
                    module.Extension = new
                    {
                        LeftPartialName = "System/Module/LeftMenu",
                        RightPartialName = "System/Module/ModuleGrid"
                    }.ToJson();
                }

                SetParentModule(module);
            }
            return module;
        }

        private void SetParentModule(Module module)
        {
            if (module.ParentId == "0")
                return;

            module.Parent = Repository.QueryById<Module>(module.ParentId);
            SetParentModule(module.Parent);
        }
        #endregion

        public Result<Module> SaveModule(dynamic model)
        {
            if (model == null)
                return Result.Error<Module>("不能提交空数据！");

            var id = (string)model.Id;
            var entity = Repository.QueryById<Module>(id);
            if (entity == null)
            {
                entity = new Module();
                entity.AppId = Setting.Instance.SystemId;
            }
            entity.FillModel(model);

            var vr = entity.Validate().ToResult();
            if (vr.HasError)
                return Result.Error<Module>(vr.ErrorMessage);

            Repository.Save(entity);
            return Result.Success("保存成功！", entity);
        }
    }
}
