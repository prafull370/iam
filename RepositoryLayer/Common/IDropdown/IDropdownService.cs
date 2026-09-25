using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.Common;

namespace RepositoryLayer.Common.IDropdown
{
    public interface IDropdownService
    {
        Task<ResponseModel<List<DDLResponseModel>>> Get(DDLRequestModel model);
    }
}
