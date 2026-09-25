using IAM_WEB.Common.Helper.APIHelper;
using IAM_WEB.Common.Helper.Pagination;
using IAM_WEB.Common.Helper.Validation;
using IAM_WEB.Common.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ModelLayer.Models.Common;
using ModelLayer.Models.IAM.Department;
using UtilityLayer.Interfaces;

namespace IAM_WEB.Areas.IAM.Controllers.Department
{
    [Area("IAM")]
    public class DepartmentController : Controller
    {
        private readonly IAPIHelper FactoryService;
        private readonly ICryptographyService _cryptography;
        private readonly string BaseApi = "Department";
        public DepartmentController(IAPIHelper aPIHelper,ICryptographyService cryptography)
        {
            this.FactoryService = aPIHelper;
            this._cryptography = cryptography;
        }

        #region Department List
        public IActionResult Index()
        {
            return View("~/Areas/IAM/Views/Department/Index.cshtml");
        }
        #endregion

        #region Add Department
        public IActionResult Add()
        {
            DepartmentRequestModel model = new DepartmentRequestModel();
            return View("~/Areas/IAM/Views/Department/Add.cshtml", model);
        }
        #endregion

        #region Edit Department
        public async Task<IActionResult> Edit(string Id)
        {

            var Result = await Get_By_Id<DepartmentRequestModel>(Convert.ToInt32(_cryptography.Decrypt(Id)));
            return View("~/Areas/IAM/Views/Department/Add.cshtml", Result.ResponseData);
        }
        #endregion

        #region Department Details
        public async Task<IActionResult> Details(int Id)
        {
            var Result = await Get_By_Id<DepartmentResponseModel>(Id);
            return Json(Result);
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> Save(DepartmentRequestModel model)
        {
            var Validation = ValidationHelper.ValidateModel<DepartmentRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var result= await FactoryService.PostAsync<string>(BaseApi + "/save", model, "");
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Update(DepartmentRequestModel model)
        {
            var Validation = ValidationHelper.ValidateModel<DepartmentRequestModel>(model, ModelState);
            if (!Validation.Status)
            {
                return Json(Validation);
            }
            var result = await FactoryService.PutAsync<string>(BaseApi + "/update", model, "");
            return Json(result);
        }

        
        [HttpPost]
        [PermissionAction("Delete")] //  Add permission check for Delete action
        public async Task<IActionResult> Delete(int DepartmentId)
        {
            // Add delete implementation here
            return Json(new { Status = false, Message = "Delete functionality not yet implemented" });
        }

        public async Task<ResponseModel<T>> Get_By_Id<T>(int DepartmentId)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["Department_Id"] = DepartmentId.ToString(),
            };
            string url = QueryHelpers.AddQueryString(BaseApi + "/getbyid", queryParams);

            return await FactoryService.GetAsync<T>(url, "");
        }

        [HttpPost]
        public async Task<IActionResult> Get(string? Search_Val)
        {
            PaginationModel pagination = new PaginationModel();
            if(Search_Val != null)
            {
                pagination.Search_Val = Search_Val;
            }
            PaginationHelper.PagingData(Request.Form, ref pagination);
            var result=await FactoryService.PostAsync<List<DepartmentResponseModel>>(BaseApi + "/get", pagination, "");
            List<DepartmentResponseModel> data = result.ResponseData;
            if (data?.Any() == true)
            {
                data.Select(c =>
                {
                    c.ENC_Department_Id = _cryptography.Encrypt(c.Department_Id.ToString());
                    return c;
                }).ToList();
            }
            return Json(new { iTotalRecords = result.TotalRecords, iTotalDisplayRecords = result.TotalRecords, data });
        }
    }
}
