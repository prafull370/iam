using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Page;
using RepositoryLayer.IAM.IPage;

namespace DataLayer.IAM.Page
{
    internal class PageService : IPageService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public PageService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(PageRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_product_id", objmodel.Product_Id);
            dynParameters.Add("p_service_id", objmodel.Service_Id);
            dynParameters.Add("p_module_id", objmodel.Module_Id);
            dynParameters.Add("p_parent_id", objmodel.Parent_Id);
            dynParameters.Add("p_access_id", objmodel.Access_Id);
            dynParameters.Add("p_page_name", objmodel.Page_Name);
            dynParameters.Add("p_page_url", objmodel.Page_Url);
            dynParameters.Add("p_page_desc", objmodel.Page_Desc);
            dynParameters.Add("p_page_logo", objmodel.Page_Logo);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            dynParameters.Add("p_is_parent", objmodel.Is_Parent);
            dynParameters.Add("p_app_id", objmodel.Access_Id == 1 ? objmodel.App_Id : 0);
            dynParameters.Add("p_display_order", objmodel.Display_Order);
            dynParameters.Add("p_activity_log", objmodel.Activity_Log);

            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_page_save", dynParameters, LoggedUserData);
        }
        public async Task<ResponseModel<string>> Update(PageRequestModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_page_id", objmodel.Page_Id);
            dynParameters.Add("p_product_id", objmodel.Product_Id);
            dynParameters.Add("p_service_id", objmodel.Service_Id);
            dynParameters.Add("p_module_id", objmodel.Module_Id);
            dynParameters.Add("p_parent_id", objmodel.Parent_Id);
            dynParameters.Add("p_access_id", objmodel.Access_Id);
            dynParameters.Add("p_page_name", objmodel.Page_Name);
            dynParameters.Add("p_page_url", objmodel.Page_Url);
            dynParameters.Add("p_page_desc", objmodel.Page_Desc);
            dynParameters.Add("p_page_logo", objmodel.Page_Logo);
            dynParameters.Add("p_display_order", objmodel.Display_Order);
            dynParameters.Add("p_is_active", objmodel.Is_Active);
            dynParameters.Add("p_is_parent", objmodel.Is_Parent);
            dynParameters.Add("p_app_id", objmodel.Access_Id == 1 ? objmodel.App_Id : 0);
            dynParameters.Add("p_activity_log", objmodel.Activity_Log);

            return await FactoryService.dbmanager.Execute_SP_Update<string>("iam.sp_page_update", dynParameters, LoggedUserData);
        }

        public async Task<ResponseModel<List<PageResponseModel>>> Get(PageGetRequestModel pagination)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_product_id", pagination.Product_Id);
            dynParameters.Add("p_service_id", pagination.Service_Id);
            dynParameters.Add("p_access_id", pagination.Access_Id);

            return await FactoryService.dbmanager.Execute_SP_Select_List_With_Pagination<PageResponseModel>("iam.sp_page_get", dynParameters, pagination, LoggedUserData);
        }

        public async Task<ResponseModel<PageResponseModel>> GetById(int Page_Id)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_page_id", Page_Id);

            return await FactoryService.dbmanager.Execute_SP_Select_Single<PageResponseModel>("iam.sp_page_get_by_id", dynParameters, LoggedUserData);
        }
    }
}
