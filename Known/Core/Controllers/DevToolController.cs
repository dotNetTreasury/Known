﻿using Known.Core.Services;
using Known.Web;
using Known.WebApi;

namespace Known.Core.Controllers
{
    /// <summary>
    /// 开发工具管理控制器类。
    /// </summary>
    public class DevToolController : ApiControllerBase
    {
        private DevToolService Service
        {
            get { return Container.Resolve<DevToolService>(); }
        }

        #region DevDatabase
        /// <summary>
        /// 查询示例分页数据对象。
        /// </summary>
        /// <param name="data">查询条件对象。</param>
        /// <returns>分页数据对象。</returns>
        public object QueryDatas(CriteriaData data)
        {
            var criteria = data.ToPagingCriteria();
            var result = Service.QueryDatas(criteria);
            return ApiResult.ToPageData(result);
        }
        #endregion
    }
}