using Dapper;
using ModelLayer.Models.Common;
namespace DataAccessLayer

{
    internal interface IDBFactoryService
    {
        Task<ResponseModel<List<T>>> Execute_SP_Select_List<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData = null);
        Task<ResponseModel<List<T>>> Execute_SP_Select_List_With_ConnectionString<T>(string SPName, DynamicParameters objparams, string Db_ConnectionString);
        Task<ResponseModel<List<T>>> Execute_SP_Select_List_With_Pagination<T>(string SPName, DynamicParameters objparams, PaginationModel pagination, LoggedUserDataModel? LoggedUserData = null);
        Task<ResponseModel<T>> Execute_SP_Select_Single<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData = null);
        Task<ResponseModel<T>> Execute_SP_Select_Scalar<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData = null);
        Task<ResponseModel<T>> Execute_SP_Insert<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData = null);
        Task<ResponseModel<T>> Execute_SP_Update<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData = null);
        Task<ResponseModel<T>> Execute_SP_Delete<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData = null);
        Task<ResponseModel<List<object>>> Execute_SP_Select_N_Multiple(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData = null, params (Type type, bool isList)[] resultTypes);
    }
}
