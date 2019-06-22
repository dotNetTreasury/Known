﻿using Known.Core.Entities;
using Known.Mapping;

namespace Known.Core
{
    class UserService : CoreServiceBase<IUserRepository>
    {
        public UserService(Context context) : base(context)
        {
        }

        #region View
        public PagingResult QueryUsers(PagingCriteria criteria)
        {
            return Repository.QueryUsers(criteria);
        }

        public Result DeleteUsers(string[] ids)
        {
            var users = Repository.QueryListById<Module>(ids);
            if (users == null || users.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            var info = Repository.Transaction("删除", rep =>
            {
                foreach (var item in users)
                {
                    rep.Delete(item);
                }
            });

            return info;
        }
        #endregion

        #region Form
        public User GetUser(string id)
        {
            return Repository.QueryById<User>(id);
        }

        public Result SaveUser(dynamic model)
        {
            if (model == null)
                return Result.Error("不能提交空数据！");

            var id = (string)model.Id;
            var entity = Repository.QueryById<User>(id);
            if (entity == null)
                entity = new User();

            EntityHelper.FillModel(entity, model);

            if (string.IsNullOrWhiteSpace(entity.AppId))
                entity.AppId = Setting.Instance.AppId;

            var vr = EntityHelper.Validate(entity);
            if (vr.HasError)
                return Result.Error(vr.ErrorMessage);

            Repository.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }
        #endregion
    }
}
