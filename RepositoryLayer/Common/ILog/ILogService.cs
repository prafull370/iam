using ModelLayer.Models.Common;

namespace RepositoryLayer.Common.ILog
{
    public interface ILogService
    {
        //void Save_Logs(ErrorLogModel errormodel);
        string GetTest();
        Task<ResponseModel<List<ErrorLogModel>>> Get();
        Task<ResponseModel<string>> Save(ErrorLogModel errormodel);
        Task<ResponseModel<ErrorViewModel>> GetMultiple();
    }
}
 