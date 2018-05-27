﻿using System.Web.Http;
using Known.Platform;

namespace Known.Web.Api.Controllers
{
    public class UserController : BaseApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public object SignIn(string userName, string password)
        {
            if (userName != "13")
                return ApiResult.Error("用户名不存在！");

            return ApiResult.Success(new User { UserName = userName });
        }
    }
}