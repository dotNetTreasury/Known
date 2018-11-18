﻿using Known.Extensions;

namespace Known.Platform.Business
{
    public abstract class PlatformBusiness : BusinessBase
    {
        public PlatformBusiness(Context context) : base(context)
        {
        }

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

            return Database.QueryById<Module>(id);
        }
    }
}
