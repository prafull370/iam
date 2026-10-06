using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models.IAM.BulkDeviceReset
{
    public class BulkDeviceResetRequestModel
    {
        public string? File_Name { get; set; }
        public string? File_Type { get; set; }
        public int File_Size { get; set; }
        public string? File_Path { get; set; }
        public int Total_Count { get; set; }

        [Required(ErrorMessage = "Remarks is required.")]
        [StringLength(200, ErrorMessage = "Remarks cannot exceed 200 characters.")]
        public string? Remarks { get; set; }
    }

    public class BulkDeviceResetResponseModel
    {
        public int Id { get; set; }
        public string? File_Name { get; set; }
        public string? File_Type { get; set; }
        public int File_Size { get; set; }
        public string? File_Path { get; set; }
        public int Total_Count { get; set; }
        public int Success_Count { get; set; }
        public int Failure_Count { get; set; }
        public string? Remarks { get; set; }
        public string? Status { get; set; }   // PENDING / INPROGRESS / COMPLETED / PARTIAL / FAILED
        public string? Uploaded_By { get; set; }
        public string? Uploaded_Date { get; set; }
    }

    // Excel row. Property names must match the Excel header text (User_Id, Device_Id)
    public class BulkDeviceResetExcelModel
    {
        public string? Login_Id { get; set; }
        //public string? Device_Id { get; set; }
    }


}