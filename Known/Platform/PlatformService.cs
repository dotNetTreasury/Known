﻿using System;
using System.Collections.Generic;
using System.Linq;
using Known.Web;

namespace Known.Platform
{
    public class PlatformService
    {
        private readonly PlatformHelper helper;

        public PlatformService(ApiClient client = null, IPlatformRepository repository = null)
        {
            if (Setting.Instance.IsMonomer)
            {
                this.helper = new DbPlatformHelper(repository);
            }
            else
            {
                this.helper = new ApiPlatformHelper(client);
            }
        }

        public string GetApiBaseUrl(string apiId)
        {
            return helper.GetApiBaseUrl(apiId);
        }

        public Dictionary<string, object> GetCodes()
        {
            return helper.GetCodes(Setting.Instance.AppId);
        }

        public Module GetModule(string id)
        {
            var module = helper.GetModule(id);
            if (module != null)
            {
                SetParentModule(module);
            }

            return module;
        }

        public List<Module> GetModules()
        {
            var modules = helper.GetModules(Setting.Instance.AppId);
            if (modules == null || modules.Count == 0)
                return null;

            return modules.OrderBy(m => m.ParentId)
                          .ThenBy(m => m.Sort)
                          .ToList();
        }

        public List<Module> GetUserModules(string userName)
        {
            var modules = helper.GetUserModules(Setting.Instance.AppId, userName);
            if (modules == null || modules.Count == 0)
                return new List<Module>();

            return GetHierarchicalModules(modules);
        }

        public User GetUser(string userName)
        {
            return helper.GetUser(userName);
        }

        public Result<User> ValidateLogin(string token)
        {
            var user = helper.GetUserByToken(token);
            if (user == null)
                return Result.Error<User>("用户不存在！");

            return Result.Success("登录成功！", user);
        }

        public Result<User> ValidateLogin(string userName, string password)
        {
            var user = GetUser(userName);
            if (user == null)
                return Result.Error<User>("用户不存在！");

            if (user.Password != password)
                return Result.Error<User>("用户密码不正确！");

            return Result.Success("登录成功！", user);
        }

        public Result<User> SignIn(string userName, string password)
        {
            var result = ValidateLogin(userName, password);
            if (!result.IsValid)
                return Result.Error<User>(result.Message);

            var user = result.Data;
            user.Token = Utils.NewGuid;
            if (!user.FirstLoginTime.HasValue)
                user.FirstLoginTime = DateTime.Now;
            user.LastLoginTime = DateTime.Now;
            helper.SaveUser(user);

            return Result.Success("登录成功！", user);
        }

        public Result SignOut(string userName)
        {
            var user = GetUser(userName);
            if (user == null)
                return Result.Error("用户不存在！");

            user.Token = string.Empty;
            helper.SaveUser(user);

            return Result.Success("注销成功！");
        }

        private void SetParentModule(Module module)
        {
            if (module.ParentId == "0")
                return;

            module.Parent = helper.GetModule(module.ParentId);
            SetParentModule(module.Parent);
        }

        private List<Module> GetHierarchicalModules(List<Module> source)
        {
            var modules = new List<Module>();
            var topModules = source.Where(m => m.ParentId == "0")
                                   .OrderBy(m => m.Sort)
                                   .ToList();
            foreach (var item in topModules)
            {
                modules.Add(item);
                SetModuleChildren(source, modules, item);
            }

            return modules;
        }

        private void SetModuleChildren(List<Module> source, List<Module> modules, Module module)
        {
            var children = source.Where(m => m.ParentId == module.Id)
                                 .OrderBy(m => m.Sort)
                                 .ToList();
            if (children == null || children.Count == 0)
                return;

            if (module.Children == null)
                module.Children = new List<Module>();

            foreach (var item in children)
            {
                item.Parent = module;
                module.Children.Add(item);
                SetModuleChildren(source, modules, item);
            }
        }
    }
}
