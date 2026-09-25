using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using DataAccessLayer;
using DataLayer.Common.Helper;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DesignationRoleMapping;
using RepositoryLayer.IAM.IDesignationRoleMapping;

namespace DataLayer.IAM.DesignationRoleMapping
{
    internal class DesignationRoleMappingService: IDesignationRoleMappingService
    {
        private readonly DataAccessDependencyService FactoryService;
        private readonly LoggedUserDataModel LoggedUserData;
        public DesignationRoleMappingService(DataAccessDependencyService service)
        {
            LoggedUserData = LoggedUserDataHelper.Get();
            FactoryService = service;
        }
        public async Task<ResponseModel<string>> Save(DesignationRoleMappingRequestModel objmodel)
        {
            var jsonData = JsonSerializer.Serialize(
            objmodel.Role_Ids.Select(pid => new { role_id = pid }));

            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_department_id", objmodel.Department_Id);
            dynParameters.Add("p_designation_id", objmodel.Designation_Id);
            dynParameters.Add("p_json_data", jsonData);
            return await FactoryService.dbmanager.Execute_SP_Insert<string>("iam.sp_designation_role_mapping_save", dynParameters, LoggedUserData);
        }


        public async Task<ResponseModel<List<DesignationRoleMappingResponseModel>>> Get(DesignationRoleMappingGetModel objmodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_department_id", objmodel.Department_Id);
            dynParameters.Add("p_designation_id", objmodel.Designation_Id);
            return await FactoryService.dbmanager.Execute_SP_Select_List<DesignationRoleMappingResponseModel>("iam.sp_designation_role_mapping_get", dynParameters, LoggedUserData);
        }
    }
}
