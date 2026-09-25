using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using ModelLayer.Models.Common;
namespace DataAccessLayer

{


    internal class DBFactoryService:IDBFactoryService
    {
        private readonly string? Connectionstring;//= DBConnection.GetConnectionString();

        public DBFactoryService(IDBConnection dbConnection)
        {
            Connectionstring = dbConnection.GetConnectionString();
           
        }
        // Add fixed input parameters
        public static void AddFixedInputParameters(ref DynamicParameters parameters,ref LoggedUserDataModel LoggedUserData)
        {
            
            parameters.Add("p_user_id", LoggedUserData.User_Id, dbType: DbType.Guid);
        }
        // Add fixed output parameters
        public static void AddFixedOutputParameters(ref DynamicParameters parameters)
        {
            parameters.Add("@status", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("@message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
            parameters.Add("@responsetype", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
            parameters.Add("@recordid", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
        }
        public void AddFixedPaginationParameters(ref DynamicParameters parameter, ref PaginationModel pageBase)
        {
            parameter.Add("@Start", pageBase.Start, DbType.Int32);
            parameter.Add("@Length", pageBase.Length, DbType.Int32);
            parameter.Add("@SortColumn", pageBase.SortColumn, DbType.String);
            parameter.Add("@SortBy", pageBase.SortColumnDirection, DbType.String);
            parameter.Add("@TotalRecords", dbType: DbType.Int32, size: 100, direction: ParameterDirection.Output);
        }
        // Retrieve fixed output parameters
        public static ResponseModel<T> GetFixedOutputResponse<T>(ref DynamicParameters parameters,ref T? Result,bool PaginationApplied=false)
        {
            ResponseModel<T> dBResponseModel = new ResponseModel<T>();
            dBResponseModel.Status = parameters.Get<bool>("@status");
            dBResponseModel.Message = parameters.Get<string>("@message");
            dBResponseModel.ResponseType = parameters.Get<string>("@responsetype");
            dBResponseModel.RecordId = parameters.Get<int>("@recordid");
            dBResponseModel.ResponseData = Result;
            dBResponseModel.StatusCode = dBResponseModel.Status?"200":"400";
            if (PaginationApplied)
            {
                dBResponseModel.TotalRecords = parameters.Get<int>("@TotalRecords");
            }
            return (dBResponseModel);
        }
        // Execute the sp and return the List
        public async Task<ResponseModel<List<T>>> Execute_SP_Select_List<T>(string SPName, DynamicParameters objparams,LoggedUserDataModel? LoggedUserData=null)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(Connectionstring))
                {
                    connection.Open();
                    if (LoggedUserData!=null)
                    {
                        AddFixedInputParameters(ref objparams, ref LoggedUserData);
                    }
                    AddFixedOutputParameters(ref objparams);
                    var result = (await connection.QueryAsync<T>(
                        SPName,
                        objparams,
                        commandType: CommandType.StoredProcedure
                    )).AsList();
                    return GetFixedOutputResponse<List<T>>(ref objparams, ref result);
                }
            }
            catch (Exception) 
            {
                throw;
            }
            
        }
        // Execute the sp and return the List
        public async Task<ResponseModel<List<T>>> Execute_SP_Select_List_With_Pagination<T>(string SPName, DynamicParameters objparams,PaginationModel pagination,LoggedUserDataModel? LoggedUserData = null)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(Connectionstring))
                {
                    connection.Open();
                    if (LoggedUserData != null)
                    {
                        AddFixedInputParameters(ref objparams, ref LoggedUserData);
                    }
                    AddFixedPaginationParameters(ref objparams,ref pagination);
                    AddFixedOutputParameters(ref objparams);
                    var result = (await connection.QueryAsync<T>(
                        SPName,
                        objparams,
                        commandType: CommandType.StoredProcedure
                    )).AsList();
                    return GetFixedOutputResponse<List<T>>(ref objparams, ref result,true);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        // Execute the sp and return the List
        public async Task<ResponseModel<List<T>>> Execute_SP_Select_List_With_ConnectionString<T>(string SPName, DynamicParameters objparams, string Db_ConnectionString)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(Db_ConnectionString.Replace("\\\\", "\\")))
                {
                    connection.Open();
                    AddFixedOutputParameters(ref objparams);
                    var result = (await connection.QueryAsync<T>(
                        SPName,
                        objparams,
                        commandType: CommandType.StoredProcedure
                    )).AsList();
                    return GetFixedOutputResponse<List<T>>(ref objparams, ref result);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        // Execute the sp and return the Single Record
        public async Task<ResponseModel<T>> Execute_SP_Select_Single<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData=null)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(Connectionstring))
                {
                    connection.Open();
                    if (LoggedUserData != null)
                    {
                        AddFixedInputParameters(ref objparams, ref LoggedUserData);
                    }
                    AddFixedOutputParameters(ref objparams);
                    var result = (await connection.QueryAsync<T>(
                        SPName,
                        objparams,
                        commandType: CommandType.StoredProcedure
                    )).FirstOrDefault();
                    return GetFixedOutputResponse<T>(ref objparams, ref result);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        // Execute the stored procedure and return the scalar value
        public async Task<ResponseModel<T>> Execute_SP_Select_Scalar<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData=null)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(Connectionstring))
                {
                    connection.Open();
                    if (LoggedUserData != null)
                    {
                        AddFixedInputParameters(ref objparams, ref LoggedUserData);
                    }
                    AddFixedOutputParameters(ref objparams);
                    // Execute the stored procedure and return the scalar value
                    var result = await connection.ExecuteScalarAsync<T>(
                        SPName,
                        objparams,
                        commandType: CommandType.StoredProcedure
                    );
                    return GetFixedOutputResponse<T>(ref objparams, ref result);
                }
            }
            catch (Exception)
            {
                // Preserve the stack trace of the exception
                throw;
            }
        }
        // Execute the stored procedure to insert a record
        public async Task<ResponseModel<T>> Execute_SP_Insert<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData=null)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(Connectionstring))
                {
                    connection.Open();
                    if (LoggedUserData != null)
                    {
                        AddFixedInputParameters(ref objparams, ref LoggedUserData);
                    }
                    AddFixedOutputParameters(ref objparams);
                    // Execute the stored procedure
                    var result = (await connection.QueryAsync<T>(
                         SPName,
                         objparams,
                         commandType: CommandType.StoredProcedure
                     )).FirstOrDefault();
                    return GetFixedOutputResponse<T>(ref objparams, ref result);
                }
            }
            catch (Exception)
            {
                // Preserve the stack trace of the exception
                throw;
            }
        }
        // Execute the stored procedure to update a record
        public async Task<ResponseModel<T>> Execute_SP_Update<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData=null)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(Connectionstring))
                {
                    connection.Open();
                    if (LoggedUserData != null)
                    {
                        AddFixedInputParameters(ref objparams, ref LoggedUserData);
                    }
                    AddFixedOutputParameters(ref objparams);
                    // Execute the stored procedure
                    var result = (await connection.QueryAsync<T>(
                         SPName,
                         objparams,
                         commandType: CommandType.StoredProcedure
                     )).FirstOrDefault();
                    return GetFixedOutputResponse<T>(ref objparams, ref result);
                }
            }
            catch (Exception)
            {
                // Preserve the stack trace of the exception
                throw;
            }
        }
        // Execute the stored procedure to insert a record
        public async Task<ResponseModel<T>> Execute_SP_Delete<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData=null)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(Connectionstring))
                {
                    connection.Open();
                    if (LoggedUserData != null)
                    {
                        AddFixedInputParameters(ref objparams, ref LoggedUserData);
                    }
                    AddFixedOutputParameters(ref objparams);
                    // Execute the stored procedure
                    var result = (await connection.QueryAsync<T>(
                         SPName,
                         objparams,
                         commandType: CommandType.StoredProcedure
                     )).FirstOrDefault();
                    return GetFixedOutputResponse<T>(ref objparams, ref result);
                }
            }
            catch (Exception)
            {
                // Preserve the stack trace of the exception
                throw;
            }
        }
        public async Task<ResponseModel<List<object>>> Execute_SP_Select_N_Multiple(string SPName,DynamicParameters objparams, LoggedUserDataModel? LoggedUserData=null,params (Type type, bool isList)[] resultTypes)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SPName))
                    throw new ArgumentException("Stored procedure name cannot be empty.", nameof(SPName));

                if (objparams == null)
                    throw new ArgumentNullException(nameof(objparams), "Stored procedure parameters cannot be null.");

                using (IDbConnection connection = new SqlConnection(Connectionstring))
                {
                    connection.Open();
                    if (LoggedUserData != null)
                    {
                        AddFixedInputParameters(ref objparams, ref LoggedUserData);
                    }
                    AddFixedOutputParameters(ref objparams);

                    using (var multi = await connection.QueryMultipleAsync(SPName, objparams, commandType: CommandType.StoredProcedure))
                    {
                        if (multi == null)
                            throw new Exception("QueryMultipleAsync returned null. Check if the stored procedure is returning data.");

                        List<object> results = new List<object>();

                        foreach (var (type, isList) in resultTypes)
                        {
                            if (isList)
                            {
                                // Read as an IEnumerable and convert to List<T>
                                var result = await multi.ReadAsync(type).ConfigureAwait(false);
                                results.Add(result);  // Use AsList to dynamically cast the result
                            }
                            else
                            {
                                // Read as a single object
                                var result = await multi.ReadFirstOrDefaultAsync(type).ConfigureAwait(false);
                                results.Add(result);
                            }
                        }


                        return GetFixedOutputResponse(ref objparams, ref results);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception if needed
                throw new Exception($"Error executing stored procedure '{SPName}': {ex.Message}", ex);
            }
        }

    }
}