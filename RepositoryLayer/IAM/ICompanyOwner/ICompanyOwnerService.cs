using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.DesignationRoleMapping;
using ModelLayer.Models.IAM.LevelHierarchy;
using ModelLayer.Models.IAM.CompanyOwner;

namespace RepositoryLayer.IAM.ICompanyOwner
{
	public interface ICompanyOwnerService
	{
		//Task<ResponseModel<string>> Save(UserRequestModel objmodel);
		//Task<ResponseModel<string>> Update(UserRequestModel objmodel);
		Task<ResponseModel<List<CompanyOwnerResponseModel>>> GetList(CompanyOwnerPaginationModel pagination);
		//Task<ResponseModel<UserRequestModel>> GetById(int Id);
		//Task<ResponseModel<List<UserApplicationModel>>> Get(int Designation_id);
		//Task<ResponseModel<List<UserApplicationModel>>> GetPermissionById(int Id);
		Task<ResponseModel<List<Dictionary<string, object>>>> DownloadList(FiltersModel model);
		//Task<ResponseModel<List<DesignationRoleMappingResponseModel>>> GetRolesPermission(DesignationRoleMappingGetModel model);
		//      Task GetList(ModelLayer.Models.IAM.CompanyOwner.CompanyOwnerPaginationModel model);
	}
}
