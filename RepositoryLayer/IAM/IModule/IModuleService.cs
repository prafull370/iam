using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Module;

namespace RepositoryLayer.IAM.IModule
{
    public interface IModuleService
    {
        Task<ResponseModel<string>> Save(ModuleRequestModel objmodel);
        Task<ResponseModel<string>> Update(ModuleRequestModel objmodel);
        Task<ResponseModel<List<ModuleResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<ModuleResponseModel>> GetById(int Module_Id);
    }
}
