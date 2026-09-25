using ModelLayer.Models.Common;

namespace IAM_WEB.Common.Helper.Response
{
    public static class ResponseHelper
    {
        public static ResponseModel<T> CreateErrorResponse<T>(T? Data, string ErrorType)
        {
            string Message = "";
            string StatusCode = "";
            if (ErrorType.ToLower() == "INTERNALSERVER")
            {
                Message = "Internal server error.";
                StatusCode = "500";
            }
            else if (ErrorType.ToLower() == "validationfailled")
            {
                Message = "Validation failled.";
                StatusCode = "400";
            }
            else
            {
                Message = "Error.";
                StatusCode = "400";
            }
            return new ResponseModel<T>
            {
                Status = false,
                StatusCode = StatusCode,
                Message = Message,
                ResponseData = Data,
                ResponseType = ErrorType
            };
        }
        public static ResponseModel<T> CreateSuccessResponse<T>(T? Data)
        {
            return new ResponseModel<T>
            {
                Status = true,
                StatusCode = "200",
                Message = "Request processed successfully.",
                ResponseData = Data,
                ResponseType ="Success"
            };
        }
    }
}
