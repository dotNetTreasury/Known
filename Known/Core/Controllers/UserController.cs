﻿using System.Web.Http;
using Known.Web;

namespace Known.Core.Controllers
{
    /// <summary>
    /// 用户控制器类。
    /// </summary>
    public class UserController : ApiControllerBase
    {
        private UserService Service
        {
            get { return LoadService<UserService>(); }
        }

        #region Platform
        /// <summary>
        /// 用户登录操作。
        /// </summary>
        /// <param name="userName">用户名。</param>
        /// <param name="password">登录密码。</param>
        /// <param name="backUrl">登录成功后，跳转的地址，默认空。</param>
        /// <returns>操作结果对象。</returns>
        [AllowAnonymous, Route("signin")]
        public object SignIn(string userName, string password, string backUrl = null)
        {
            userName = userName.ToLower();
            var result = PlatformService.SignIn(userName, password);
            if (!result.IsValid)
                return ApiResult.Error(result.Message);

            return ApiResult.Success("登录成功，正在跳转页面......", new
            {
                user = new
                {
                    result.Data.CompanyId,
                    result.Data.DepartmentId,
                    result.Data.UserName,
                    result.Data.Name,
                    result.Data.Email,
                    result.Data.Mobile,
                    result.Data.Phone,
                    result.Data.Token
                },
                backUrl
            });
        }

        /// <summary>
        /// 退出登录操作。
        /// </summary>
        /// <returns>操作结果对象。</returns>
        [Route("signout")]
        public object SignOut()
        {
            PlatformService.SignOut(UserName);
            return ApiResult.Success("");
        }

        /// <summary>
        /// 获取当前用户菜单模块及系统代码集合。
        /// </summary>
        /// <returns>菜单模块及系统代码集合。</returns>
        public object GetModules()
        {
            var menus = Menu.GetUserMenus(PlatformService, UserName);
            var codes = Code.GetCodes(PlatformService);
            return new { menus, codes };
        }
        #endregion

        #region View
        /// <summary>
        /// 查询分页数据对象。
        /// </summary>
        /// <param name="criteria">查询条件对象。</param>
        /// <returns>分页查询结果。</returns>
        protected override PagingResult QueryDatas(PagingCriteria criteria)
        {
            return Service.QueryUsers(criteria);
        }

        /// <summary>
        /// 删除一个或多个实体对象。
        /// </summary>
        /// <param name="ids">实体对象 Id 数组。</param>
        /// <returns>删除结果。</returns>
        protected override Result DeleteDatas(string[] ids)
        {
            return Service.DeleteUsers(ids);
        }
        #endregion

        #region Form
        /// <summary>
        /// 获取实体对象。
        /// </summary>
        /// <param name="id">实体 id。</param>
        /// <returns>实体对象。</returns>
        public override object GetData(string id)
        {
            return Service.GetUser(id);
        }

        /// <summary>
        /// 保存实体对象。
        /// </summary>
        /// <param name="model">实体对象。</param>
        /// <returns>保存结果。</returns>
        protected override Result SaveData(dynamic model)
        {
            return Service.SaveUser(model);
        }
        #endregion
    }
}