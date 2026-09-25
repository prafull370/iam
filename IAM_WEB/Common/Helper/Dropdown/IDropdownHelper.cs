using ModelLayer.Models.Common;

namespace IAM_WEB.Common.Helper.Dropdown
{
	public interface IDropdownHelper
	{
		Task<List<DDLResponseModel>> Get(DDLRequestModel model);
	}
}
