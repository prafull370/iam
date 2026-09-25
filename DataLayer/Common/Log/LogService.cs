using Dapper;
using DataAccessLayer;
using ModelLayer.Models.Common;
using RepositoryLayer.Common.ILog;

namespace DataLayer.Common.Log
{
    internal class LogService : ILogService
    {
        public readonly DataAccessDependencyService FactoryService;

        public LogService(DataAccessDependencyService factoryservice)
        {
            FactoryService = factoryservice;
        }
        //public async void Save_Logs(ErrorLogModel errormodel)
        //{
        //    DynamicParameters dynParameters = new DynamicParameters();
        //    dynParameters.Add("ErrorType", errormodel.ErrorType);
        //    dynParameters.Add("ErrorSource", errormodel.ErrorSource);
        //    dynParameters.Add("ErrorAction", errormodel.ErrorAction);
        //    dynParameters.Add("ErrorMessage", errormodel.ErrorMessage);
        //    var Response = await FactoryService.dbfactory.Execute_SP_Insert<string>("system.sp_errorlog_save", dynParameters);

        //}
        public string GetTest()
        {
            return "Test Success";
        }
        public async Task<ResponseModel<List<ErrorLogModel>>> Get()
        {
            DynamicParameters dynParameters = new DynamicParameters();
            var Response = await FactoryService.dbmanager.Execute_SP_Select_List<ErrorLogModel>("mst.sp_errorlog_get", dynParameters);
            return Response;
        }
        public async Task<ResponseModel<ErrorViewModel>> GetMultiple()
        {
            var Result = new ErrorViewModel();
            var resultTypes = new List<RefCursorResultModel>
                              {
                                  new RefCursorResultModel(typeof(ErrorLogModel), false),
                                  new RefCursorResultModel(typeof(ErrorLogModel), true)
                              };
            DynamicParameters dynParameters = new DynamicParameters();
            var Response = await FactoryService.dbmanager.Execute_SP_Select_N_Multiple("mst.sp_get_multiple_resultsets", dynParameters,null, resultTypes);
            Result.singleerror = Response.ResponseData[0] as ErrorLogModel;
            Result.multipleerrors = ((List<object>)Response.ResponseData[1]).Cast<ErrorLogModel>().ToList();
            return new ResponseModel<ErrorViewModel>
            {
                Status = Response.Status,
                Message = Response.Message,
                ResponseType = Response.ResponseType,
                RecordId = Response.RecordId,
                ResponseData = Result,
                StatusCode = Response.StatusCode
            };
        }
        public async Task<ResponseModel<string>> Save(ErrorLogModel errormodel)
        {
            DynamicParameters dynParameters = new DynamicParameters();
            dynParameters.Add("p_errortype", errormodel.ErrorType);
            dynParameters.Add("p_errorsource", errormodel.ErrorSource);
            dynParameters.Add("p_erroraction", errormodel.ErrorAction);
            dynParameters.Add("p_errormessage", errormodel.ErrorMessage);
            var Response = await FactoryService.dbmanager.Execute_SP_Insert<string>("mst.sp_errorlog_web_save", dynParameters);
            return Response;
        }
    }
}
