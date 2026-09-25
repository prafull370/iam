using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Designation;

namespace RepositoryLayer.IAM.IDesignation
{
    public interface IDesignationService
    {
        Task<ResponseModel<string>> Save(DesignationRequestModel objmodel);
        Task<ResponseModel<string>> Update(DesignationRequestModel objmodel);
        Task<ResponseModel<List<DesignationResponseModel>>> Get(PaginationModel pagination);
        Task<ResponseModel<DesignationResponseModel>> GetById(int Designation_Id);
    }
}
