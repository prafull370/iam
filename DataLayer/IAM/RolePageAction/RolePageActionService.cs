using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.RolePageAction;
using ModelLayer.Models.IAM.User;
using RepositoryLayer.IAM.IRolePageActionService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataLayer.IAM.RolePageAction
{
    internal class RolePageActionService : IRolePageActionService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public RolePageActionService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        #region Get Role Page Action
        public async Task<ResponseModel<RolePageActionResponseModel>> Get(int Product_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_product_id", Product_Id);
            var resultTypes = new List<RefCursorResultModel>
                              {
                                  new RefCursorResultModel(typeof(PermissionRoleModel),   true),
                                  new RefCursorResultModel(typeof(PermissionPageModel),   true),
                                  new RefCursorResultModel(typeof(PermissionActionModel), true),
                                  new RefCursorResultModel(typeof(PermissionGrantModel),  true),
                                  new RefCursorResultModel(typeof(PageAvailableActionModel),  true),
                              };
            var raw = await FactoryService.dbmanager.Execute_SP_Select_N_Multiple("iam.sp_role_page_action_get", dynParameters, LoggedUserData, resultTypes);
            var response = new ResponseModel<RolePageActionResponseModel>
            {
                Status = raw.Status,
                Message = raw.Message,
                ResponseType = raw.ResponseType,
                StatusCode = raw.StatusCode
            };
            if (raw.Status && raw.ResponseData != null && raw.ResponseData.Count == 5)
            {
                var roles = ((List<object>)raw.ResponseData[0]).Cast<PermissionRoleModel>().ToList();

                var pages = ((List<object>)raw.ResponseData[1]).Cast<PermissionPageModel>().ToList();

                var actions = ((List<object>)raw.ResponseData[2]).Cast<PermissionActionModel>().ToList();

                var grants = ((List<object>)raw.ResponseData[3]).Cast<PermissionGrantModel>().ToList();
                var available = ((List<object>)raw.ResponseData[4]).Cast<PageAvailableActionModel>().ToList();   // NEW

                foreach (var page in pages.Where(x => x.Is_Parent == true))
                {
                    page.subMenus = pages
                        .Where(x => x.Parent_Id == page.Page_Id)
                        .Select(x => new ModelLayer.Models.IAM.RolePageAction.SubMenuModel
                        {
                            Page_Id = x.Page_Id,
                            Page_Name = x.Page_Name,
                            Parent_Id = x.Parent_Id ?? 0
                        })
                        .ToList();
                }

                response.ResponseData = new RolePageActionResponseModel
                {
                    Roles = roles,
                    Pages = pages,
                    Actions = actions,
                    Grants = grants,
                    AvailableActions = available 
                };
            }
            return response;
        }
        #endregion

        #region Save Role Page Action
        public async Task<ResponseModel<string>> Save(RolePageActionRequestModel objmodel)
        {
            var jsonData = JsonSerializer.Serialize(
                objmodel.Permissions.Select(x => new
                {
                    page_id = x.Page_Id,
                    role_id = x.Role_Id,
                    action_id = x.Action_Id,
                    is_allow = x.Is_Allow
                })
            );

            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_json_data", jsonData);

            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_role_page_action_save", dynParameters, LoggedUserData);
        }
        #endregion

        #region Download Role Page Action
        //public async Task<ResponseModel<Dictionary<string, object>>> DownloadList(int Product_ID)
        //{
        //    DynamicParameters dynParameters = new DynamicParameters();
        //    dynParameters.Add("p_product_id_val", Product_ID);
        //    var datatableResponseData = await FactoryService.dbmanager.Execute_SP_Select_DataTable("iam.sp_export_role_page_action", dynParameters, LoggedUserData);
        //    var table = datatableResponseData.ResponseData;
        //    var list = new List<Dictionary<string, object>>();
        //    if (table?.Rows.Count > 0)
        //    {
        //        foreach (DataRow row in table.Rows)
        //        {
        //            var dict = new Dictionary<string, object>();
        //            foreach (DataColumn col in table.Columns)
        //            {
        //                dict[col.ColumnName] = row[col];
        //            }
        //            list.Add(dict);
        //        }
        //    }
        //    return new ResponseModel<Dictionary<string, object>>
        //    {
        //        Status = datatableResponseData.Status,
        //        Message = datatableResponseData.Message,
        //        ResponseType = datatableResponseData.ResponseType,
        //    };
        //}
        #endregion

    }
}
