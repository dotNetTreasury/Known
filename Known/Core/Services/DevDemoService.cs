﻿using System.Collections.Generic;
using Known.Extensions;

namespace Known.Core
{
    class DevDemoService : ServiceBase
    {
        public DevDemoService(Context context) : base(context)
        {
        }

        public PagingResult QueryUsers(PagingCriteria criteria)
        {
            var users = new List<UserInfo>();
            users.Add(new UserInfo
            {
                Id = "1",
                UserName = "admin",
                Name = "管理员",
                Email = "admin@known.com",
                Mobile = "18988888888",
                Phone = "68888888"
            });
            users.Add(new UserInfo
            {
                Id = "2",
                UserName = "zhangsan",
                Name = "张三",
                Email = "zhangsan@known.com"
            });
            for (int i = 3; i < 188; i++)
            {
                users.Add(new UserInfo
                {
                    Id = i.ToString(),
                    UserName = $"account{i}",
                    Name = $"操作员{i}"
                });
            }

            var data = users.ToPageList(criteria.PageIndex, criteria.PageSize);
            return new PagingResult(users.Count, data);
        }
    }
}
