using System.Data;
using Dapper;
using ModelLayer.Models.Common;
using Npgsql;

namespace DataAccessLayer
{
    internal class DBManager : IDBManager
    {
        private readonly string? Connectionstring;

        public DBManager(IDBConnection dbConnection)
        {
            Connectionstring = dbConnection.GetConnectionString();
        }

        // Add fixed input parameters
        public static void AddFixedInputParameters(ref DynamicParameters parameters, ref LoggedUserDataModel LoggedUserData,bool AddIP=false)
        {
            parameters.Add("p_user_id", LoggedUserData.User_Id, dbType: DbType.Int32);
            if (AddIP)
            {
                parameters.Add("p_ip_address", LoggedUserData.IP_Address, dbType: DbType.String);
            }
            

        }

        // Add fixed output parameters
        public static void AddFixedOutputParameters(ref DynamicParameters parameters)
        {
            parameters.Add("p_status", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            parameters.Add("p_message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
            parameters.Add("p_responsetype", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
           
        }

        // Add pagination parameters
        public void AddFixedPaginationParameters(ref DynamicParameters parameter, ref PaginationModel pageBase)
        {
            parameter.Add("p_search_val",pageBase.Search_Val, DbType.String);
            parameter.Add("p_flag",pageBase.Flag, DbType.String);
            parameter.Add("p_start",pageBase.Start, DbType.Int32);
            parameter.Add("p_length", pageBase.Length, DbType.Int32);
            parameter.Add("p_sort_column", pageBase.SortColumn.ToLower()=="string"?"created_on": pageBase.SortColumn, DbType.String);
            parameter.Add("p_sort_by", pageBase.SortColumnDirection.ToLower()=="string"?"desc": pageBase.SortColumnDirection, DbType.String);
            parameter.Add("p_total_records", dbType: DbType.Int32, size: 100, direction: ParameterDirection.Output);
        }

        // Retrieve fixed output parameters and form response
        public static ResponseModel<T> GetFixedOutputResponse<T>(ref DynamicParameters parameters, ref T? Result, bool PaginationApplied = false,bool GetRecordId=false)
        {
            ResponseModel<T> dBResponseModel = new ResponseModel<T>
            {
                Status = parameters.Get<bool>("@p_status"),
                Message = parameters.Get<string>("@p_message"),
                ResponseType = parameters.Get<string>("@p_responsetype"),
                ResponseData = Result,
                StatusCode = parameters.Get<bool>("@p_status") ? "200" : "400"
            };
            if (GetRecordId)
            {
                dBResponseModel.RecordId = parameters.Get<int>("@p_record_id");
            }
            if (PaginationApplied)
            {
                dBResponseModel.TotalRecords = parameters.Get<int>("@p_total_records");
            }

            return dBResponseModel;
        }

        // Execute stored procedure and return the list
        public async Task<ResponseModel<List<T>>> Execute_SP_Select_List<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData = null)
        {
            try
            {
                using (var connection = new NpgsqlConnection(Connectionstring))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        if (LoggedUserData != null)
                        {
                            AddFixedInputParameters(ref objparams, ref LoggedUserData);
                        }

                        AddFixedOutputParameters(ref objparams);

                        // Important: Set the cursor parameter as refcursor, not string
                        objparams.Add("ref_result_data1", dbType: DbType.String, direction: ParameterDirection.Output);

                        // Step 1: Call the stored procedure
                        await connection.ExecuteAsync(SPName, objparams, commandType: CommandType.StoredProcedure, transaction: transaction);

                        // Step 2: Get the cursor name returned
                        var cursorName = objparams.Get<string>("ref_result_data1");
                        if (!string.IsNullOrEmpty(cursorName)) {
                            // Step 3: Fetch data using the cursor
                            var result = (await connection.QueryAsync<T>($"FETCH ALL FROM \"{cursorName}\"", transaction: transaction)).ToList();

                            // Step 4: Commit the transaction (closes cursor too)
                            await transaction.CommitAsync();

                            return GetFixedOutputResponse<List<T>>(ref objparams, ref result);
                        }
                        else
                        {
                            // Step 4: Commit the transaction (closes cursor too)
                            await transaction.CommitAsync();
                            List<T> result= new List<T>();
                            return GetFixedOutputResponse<List<T>>(ref objparams, ref result);
                        }
                       
                    }
                }
            }
            catch (Exception)
            {
                // Add error logging if needed
                throw;
            }
        }
        // Execute stored procedure and return the list
        public async Task<ResponseModel<List<T>>> Execute_SP_Select_List_With_Pagination<T>(string SPName, DynamicParameters objparams, PaginationModel pagination, LoggedUserDataModel? LoggedUserData = null)
        {
            try
            {
                using (var connection = new NpgsqlConnection(Connectionstring))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        if (LoggedUserData != null)
                        {
                            AddFixedInputParameters(ref objparams, ref LoggedUserData);
                        }
                        AddFixedPaginationParameters(ref objparams, ref pagination);
                        AddFixedOutputParameters(ref objparams);

                        // Correct: Only declare output, don't set value
                        objparams.Add("ref_result_data1", dbType: DbType.String, direction: ParameterDirection.Output);

                        // Step 1: Execute stored procedure
                        await connection.ExecuteAsync(SPName, objparams, commandType: CommandType.StoredProcedure, transaction: transaction);

                        // Step 2: Get cursor name
                        var cursorName = objparams.Get<string>("ref_result_data1");
                        if (!string.IsNullOrEmpty(cursorName))
                        {

                            // Step 3: Fetch data without quotes
                            var result = (await connection.QueryAsync<T>($"FETCH ALL FROM {cursorName}", transaction: transaction)).ToList();

                            // Step 4: Commit
                            await transaction.CommitAsync();

                            return GetFixedOutputResponse<List<T>>(ref objparams, ref result, true);
                        }
                        else
                        {
                            // Step 4: Commit
                            await transaction.CommitAsync();
                            List<T> result = new List<T>();
                            return GetFixedOutputResponse<List<T>>(ref objparams, ref result, true);
                        }


                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ResponseModel<DataTable>> Execute_SP_Select_DataTable(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData = null)
        {
            try
            {
                using (var connection = new NpgsqlConnection(Connectionstring))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        if (LoggedUserData != null)
                        {
                            AddFixedInputParameters(ref objparams, ref LoggedUserData);
                        }

                        AddFixedOutputParameters(ref objparams);

                        // Important: Set the cursor parameter as refcursor, not string
                        objparams.Add("ref_result_data1", dbType: DbType.String, direction: ParameterDirection.Output);

                        // Step 1: Execute the stored procedure
                        await connection.ExecuteAsync(SPName, objparams, commandType: CommandType.StoredProcedure, transaction: transaction);

                        // Step 2: Get the cursor name
                        var cursorName = objparams.Get<string>("ref_result_data1");

                        // Step 3: Read the cursor result into a DataTable
                        DataTable dataTable = new DataTable();

                        if (!string.IsNullOrEmpty(cursorName))
                        {
                            using (var cmd = new NpgsqlCommand($"FETCH ALL FROM \"{cursorName}\"", connection, transaction))
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                dataTable.Load(reader);
                            }
                        }

                        // Step 4: Commit transaction (automatically closes the cursor)
                        await transaction.CommitAsync();

                        return GetFixedOutputResponse(ref objparams, ref dataTable);
                    }
                }
            }
            catch (Exception)
            {
                // Add error logging if needed
                throw;
            }
        }

        // For Datatable
        public async Task<ResponseModel<DataTable>> Execute_SP_Select_DataTable_With_Pagination(string SPName, DynamicParameters objparams, PaginationModel pagination, LoggedUserDataModel? LoggedUserData = null)
        {
            try
            {
                using (var connection = new NpgsqlConnection(Connectionstring))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        if (LoggedUserData != null)
                        {
                            AddFixedInputParameters(ref objparams, ref LoggedUserData);
                        }
                        AddFixedPaginationParameters(ref objparams, ref pagination);
                        AddFixedOutputParameters(ref objparams);

                        // Correct: Only declare output, don't set value
                        objparams.Add("ref_result_data1", dbType: DbType.String, direction: ParameterDirection.Output);

                        // Step 1: Execute stored procedure
                        await connection.ExecuteAsync(SPName, objparams, commandType: CommandType.StoredProcedure, transaction: transaction);

                        // Step 2: Get cursor name
                        var cursorName = objparams.Get<string>("ref_result_data1");

                        // Step 3: Fetch data as a DataTable
                        var dataTable = new DataTable();
                        using (var reader = await connection.ExecuteReaderAsync($"FETCH ALL FROM {cursorName}", transaction: transaction))
                        {
                            dataTable.Load(reader);  // Load the result into a DataTable
                        }

                        // Step 4: Commit transaction
                        await transaction.CommitAsync();

                        return GetFixedOutputResponse<DataTable>(ref objparams, ref dataTable, true);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle accordingly
                throw;
            }
        }

        // Execute stored procedure and return the list
        public async Task<ResponseModel<T>> Execute_SP_Select_Single<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData = null)
        {
            try
            {
                using (var connection = new NpgsqlConnection(Connectionstring))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        if (LoggedUserData != null)
                        {
                            AddFixedInputParameters(ref objparams, ref LoggedUserData);
                        }

                        AddFixedOutputParameters(ref objparams);

                        // Important: Set the cursor parameter as refcursor, not string
                        objparams.Add("ref_result_data1", dbType: DbType.String, direction: ParameterDirection.Output);

                        // Step 1: Call the stored procedure
                        await connection.ExecuteAsync(SPName, objparams, commandType: CommandType.StoredProcedure, transaction: transaction);

                        // Step 2: Get the cursor name returned
                        var cursorName = objparams.Get<string>("ref_result_data1");
                        if (!string.IsNullOrEmpty(cursorName))
                        {
                            // Step 3: Fetch data using the cursor
                            var result = (await connection.QueryAsync<T>($"FETCH ALL FROM \"{cursorName}\"", transaction: transaction)).FirstOrDefault();

                            // Step 4: Commit the transaction (closes cursor too)
                            await transaction.CommitAsync();

                            return GetFixedOutputResponse<T>(ref objparams, ref result);
                        }
                        else
                        {
                            // Step 4: Commit the transaction (closes cursor too)
                            await transaction.CommitAsync();
                            T result = default(T);
                            return GetFixedOutputResponse<T>(ref objparams, ref result);
                        }

                       
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Execute stored procedure and return the list
        public async Task<ResponseModel<T>> Execute_SP_Select_Scalar<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData = null)
        {
            try
            {
                using (var connection = new NpgsqlConnection(Connectionstring))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        if (LoggedUserData != null)
                        {
                            AddFixedInputParameters(ref objparams, ref LoggedUserData);
                        }

                        AddFixedOutputParameters(ref objparams);

                        // Important: Set the cursor parameter as refcursor, not string
                        objparams.Add("ref_result_data1", dbType: DbType.String, direction: ParameterDirection.Output);

                        // Step 1: Call the stored procedure
                        await connection.ExecuteAsync(SPName, objparams, commandType: CommandType.StoredProcedure, transaction: transaction);

                        // Step 2: Get the cursor name returned
                        var cursorName = objparams.Get<string>("ref_result_data1");
                        if (!string.IsNullOrEmpty(cursorName))
                        {
                            // Step 3: Fetch data using the cursor
                            var result = await connection.ExecuteScalarAsync<T>($"FETCH ALL FROM \"{cursorName}\"", transaction: transaction);

                            // Step 4: Commit the transaction (closes cursor too)
                            await transaction.CommitAsync();

                            return GetFixedOutputResponse<T>(ref objparams, ref result);
                        }
                        else
                        {
                            // Step 4: Commit the transaction (closes cursor too)
                            await transaction.CommitAsync();
                            T result=default(T);
                            return GetFixedOutputResponse<T>(ref objparams, ref result);
                        }
                        
                    }
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
                using (IDbConnection connection = new NpgsqlConnection(Db_ConnectionString.Replace("\\\\", "\\")))
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
        public async Task<ResponseModel<T>> Execute_SP_Insert<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData = null)
        {
            try
            {
                using (var connection = new NpgsqlConnection(Connectionstring))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        if (LoggedUserData != null)
                        {
                            AddFixedInputParameters(ref objparams, ref LoggedUserData,true);
                           
                        }
                        
                        objparams.Add("p_record_id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        AddFixedOutputParameters(ref objparams); // Status, Message, ResponseType, RecordId

                        await connection.ExecuteAsync(SPName, objparams, commandType: CommandType.StoredProcedure, transaction: transaction);

                        await transaction.CommitAsync();
                        T Result = default;
                        return GetFixedOutputResponse<T>(ref objparams,ref Result,false,true); // No result needed from FETCH
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Execute stored procedure to update a record
        public async Task<ResponseModel<T>> Execute_SP_Update<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData = null)
        {
            try
            {
                using (var connection = new NpgsqlConnection(Connectionstring))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        if (LoggedUserData != null)
                        {
                            AddFixedInputParameters(ref objparams, ref LoggedUserData,true);
                        }
                        objparams.Add("p_record_id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        AddFixedOutputParameters(ref objparams); // Status, Message, ResponseType, RecordId

                        await connection.ExecuteAsync(SPName, objparams, commandType: CommandType.StoredProcedure, transaction: transaction);

                        await transaction.CommitAsync();
                        T Result = default;
                        return GetFixedOutputResponse<T>(ref objparams, ref Result,false,true); // No result needed from FETCH
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Execute stored procedure to delete a record
        public async Task<ResponseModel<T>> Execute_SP_Delete<T>(string SPName, DynamicParameters objparams, LoggedUserDataModel? LoggedUserData = null)
        {
            try
            {
                using (var connection = new NpgsqlConnection(Connectionstring))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        if (LoggedUserData != null)
                        {
                            AddFixedInputParameters(ref objparams, ref LoggedUserData, true);
                        }
                        objparams.Add("record_id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        AddFixedOutputParameters(ref objparams); // Status, Message, ResponseType, RecordId

                        await connection.ExecuteAsync(SPName, objparams, commandType: CommandType.StoredProcedure, transaction: transaction);

                        await transaction.CommitAsync();
                        T Result = default;
                        return GetFixedOutputResponse<T>(ref objparams, ref Result); // No result needed from FETCH
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Execute stored procedure with multiple result sets

        public async Task<ResponseModel<List<object>>> Execute_SP_Select_N_Multiple(
                                      string SPName,
                                      DynamicParameters objparams,
                                      LoggedUserDataModel? LoggedUserData = null,
                                      List<RefCursorResultModel> resultInfos = null
        )
        {
            try
            {
                using (var connection = new NpgsqlConnection(Connectionstring))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        if (LoggedUserData != null)
                        {
                            AddFixedInputParameters(ref objparams, ref LoggedUserData);
                        }

                        AddFixedOutputParameters(ref objparams);

                        // Add all refcursor output parameters: ref_result_data1, ref_result_data2, etc.
                        for (int i = 0; i < resultInfos.Count; i++)
                        {
                            string paramName = $"ref_result_data{i + 1}";
                            objparams.Add(paramName, dbType: DbType.String, direction: ParameterDirection.Output);
                        }

                        await connection.ExecuteAsync(SPName, objparams, commandType: CommandType.StoredProcedure, transaction: transaction);

                        List<object> results = new();

                        for (int i = 0; i < resultInfos.Count; i++)
                        {
                            string cursorName = objparams.Get<string>($"ref_result_data{i + 1}");
                            string fetchSql = $"FETCH ALL FROM \"{cursorName}\"";

                            var info = resultInfos[i];

                            if (info.IsList)
                            {
                                var listResult = await connection.QueryAsync(info.ResultType, fetchSql, transaction: transaction);
                                results.Add(listResult.ToList());
                            }
                            else
                            {
                                var singleResult = await connection.QueryFirstOrDefaultAsync(info.ResultType, fetchSql, transaction: transaction);
                                results.Add(singleResult);
                            }
                        }

                        await transaction.CommitAsync();

                        return GetFixedOutputResponse(ref objparams, ref results);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}


