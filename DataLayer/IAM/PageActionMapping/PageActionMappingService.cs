using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.PageActionMapping;
using RepositoryLayer.IAM.IPageActionMapping;

namespace DataLayer.IAM.PageActionMapping
{
    internal class PageActionMappingService : IPageActionMappingService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;

        public PageActionMappingService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }

        // ---- get mapping (2 cursors: actions + mapped ids) --------------
        public async Task<ResponseModel<PageActionMappingResponseModel>> GetMapping(int Page_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_page_id", Page_Id);

            var cursors = new List<RefCursorResultModel>
            {
                new RefCursorResultModel(typeof(ActionItemModel),     true),
                new RefCursorResultModel(typeof(MappedActionIdModel), true),
            };

            var raw = await FactoryService.dbmanager.Execute_SP_Select_N_Multiple("iam.sp_page_action_mapping_get", dynParameters, LoggedUserData, cursors);

            var response = new ResponseModel<PageActionMappingResponseModel>
            {
                Status = raw.Status,
                Message = raw.Message,
                ResponseType = raw.ResponseType,
                StatusCode = raw.StatusCode
            };

            if (raw.Status && raw.ResponseData != null && raw.ResponseData.Count == 2)
            {
                var actions = ((List<object>)raw.ResponseData[0]).Cast<ActionItemModel>().ToList();
                var mapped = ((List<object>)raw.ResponseData[1]).Cast<MappedActionIdModel>()
                                .Select(x => x.Action_Id).ToList();
                response.ResponseData = new PageActionMappingResponseModel
                {
                    Actions = actions,
                    MappedActionIds = mapped
                };
            }
            return response;
        }

        // ---- save ----
        public async Task<ResponseModel<string>> SaveMapping(PageActionMappingRequestModel objModel)
        {
            var jsonData = JsonSerializer.Serialize(
                objModel.Actions.Select(a => new { action_id = a.Action_Id }));

            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_product_id", objModel.Product_Id);
            dynParameters.Add("p_service_id", objModel.Service_Id);
            dynParameters.Add("p_module_id", objModel.Module_Id ?? 0);
            dynParameters.Add("p_page_id", objModel.Page_Id);
            dynParameters.Add("p_json_data", jsonData, dbType: System.Data.DbType.String);

            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_page_action_mapping_save", dynParameters, LoggedUserData);
        }
    }
}
