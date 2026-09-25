using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.LevelHierarchy;
using ModelLayer.Models.IAM.RolePageMapping;

namespace RepositoryLayer.IAM.ILevelHierarchy
{
	public interface ILevelHierarchyService
	{
		Task<ResponseModel<string>> Save(LevelHierarchyRequestModel objmodel);
		Task<ResponseModel<string>> Update(LevelHierarchyRequestModel objmodel);
		Task<ResponseModel<List<LevelHierarchyResponseModel>>> Get(GetLevelHierarchyModel pagination);
		Task<ResponseModel<LevelHierarchyRequestModel>> GetById(LevelHierarchyGetModel model);
	}
}
