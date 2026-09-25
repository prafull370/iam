using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NPOI.SS.Formula.Functions;

namespace ModelLayer.Models.IAM.BulkUpload
{
    public class BulkUploadFilesRequestModel
    {
        public int Bulk_Type_Id { get; set; }
        public string? File_Name { get; set; }
        public string? File_Type { get; set; }
        public int File_Size { get; set; }
        public string? File_Path { get; set; }
        public int Total_Count { get; set; }
    }
    public class BulkUploadFilesResponseModel
    {
        public int Id { get; set; }
        public string? ENC_ID { get; set; }
        public int Bulk_Type_Id { get; set; }
        public string? Bulk_Type { get; set; }
        public string? File_Name { get; set; }
        public string? File_Type { get; set; }
        public int File_Size { get; set; }
        public string? File_Path { get; set; }
        public int Total_Count { get; set; }
        public int Success_Count { get; set; }
        public int Failure_Count { get; set; }
        public string? Uploaded_Date { get; set; }
        public string? Uploaded_By { get; set; }
        public string? Status { get; set; }
    }
    public class BulkUserUploadModel
    {
        public string? USER_NAME { get; set; }
        public string? MOBILE_NO { get; set; }
        public string? EMAIL { get; set; }
        public string? LOGIN_ID { get; set; }
        public string? IDENTITY_TYPE_NAME { get; set; }
        public string? IDENTITY_NO { get; set; }
        public string? DEPARTMENT_NAME { get; set; }
        public string? DESIGNATION_NAME { get; set; }
        public string? REPORTING_USER_NAME { get; set; }
        public string? DISCOM_NAME { get; set; }
        public string? LEVEL1_NAME { get; set; }
        public string? LEVEL2_NAME { get; set; }
        public string? LEVEL3_NAME { get; set; }
        public string? LEVEL4_NAME { get; set; }
        public string? LEVEL5_NAME { get; set; }
        public string? LEVEL6_NAME { get; set; }
        public string? APP_PERMISSION { get; set; }
        public string? VENDOR { get; set; }
    }
    public class BulkVendorMasterModel
    {
        public string? VENDOR_NAME { get; set; }
        public string? COUNTRY_NAME { get; set; }
        public string? STATE_NAME { get; set; }
        public string? DISTRICT_NAME { get; set; }
        public string? CITY_NAME { get; set; }
        public string? PINCODE { get; set; }
        public string? ADDRESS1 { get; set; }
        public string? ADDRESS2 { get; set; }
        public string? CONTACT_PERSON_NAME { get; set; }
        public string? CONTACT_PERSON_EMAIL { get; set; }
        public string? CONTACT_PERSON_MOBILE { get; set; }
    }
    public class BulkLevelMasterModel
    {
        public string? LEVEL_TYPE_NAME { get; set; }
        public string? LEVEL1_NAME { get; set; }
        public string? LEVEL1_CODE { get; set; }
        public string? LEVEL2_NAME { get; set; }
        public string? LEVEL2_CODE { get; set; }
        public string? LEVEL3_NAME { get; set; }
        public string? LEVEL3_CODE { get; set; }
        public string? LEVEL4_NAME { get; set; }
        public string? LEVEL4_CODE { get; set; }
        public string? LEVEL5_NAME { get; set; }
        public string? LEVEL5_CODE { get; set; }
        public string? LEVEL6_NAME { get; set; }
        public string? LEVEL6_CODE { get; set; }
    }
    public class BUlkDataRequestModel<T>
    {
        public int File_Id { get; set; }
        public int User { get; set; }
        public T? Bulk_Data { get; set; }
    }

}
