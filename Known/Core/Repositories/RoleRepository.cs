﻿using Known.Core.Entities;
using Known.Data;

namespace Known.Core
{
    /// <summary>
    /// 系统角色数据仓库接口。
    /// </summary>
    public interface IRoleRepository : IRepository
    {
        /// <summary>
        /// 查询角色分页数据对象。
        /// </summary>
        /// <param name="criteria">查询条件对象。</param>
        /// <returns>分页数据对象。</returns>
        PagingResult QueryRoles(PagingCriteria criteria);

        void DeleteRoleUsers(string roleId);

        void DeleteRoleFunctions(string roleId);
    }

    internal class RoleRepository : DbRepository, IRoleRepository
    {
        public RoleRepository(Database database) : base(database)
        {
        }

        public PagingResult QueryRoles(PagingCriteria criteria)
        {
            var sql = "select * from t_plt_roles where parent_id=@pid";

            var key = (string)criteria.Parameter.key;
            if (!string.IsNullOrWhiteSpace(key))
            {
                sql += " and (code like @key or name like @key)";
                criteria.Parameter.key = $"%{key}%";
            }

            return Database.QueryPage<User>(sql, criteria);
        }

        public void DeleteRoleUsers(string roleId)
        {

        }

        public void DeleteRoleFunctions(string roleId)
        {

        }
    }
}
