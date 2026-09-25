
namespace ModelLayer.Models.IAM.Company
{
    public class CompanyRequestModel
    {
        public int Company_Id { get; set; }
        public int Plan_Id { get; set; }
        public string? Company_Name { get; set; }
        public string? Company_Short_Name { get; set; }
        public string? Company_Name_Regional { get; set; }
        public string? Toll_Free_Number { get; set; }
        public string? Company_Website { get; set; }
        public string? Address { get; set; }
        public string? Contact_Mobile_No { get; set; }
        public string? Contact_Email_Id { get; set; }
        public string? Contact_Person_Name { get; set; }
        public string? Company_Logo { get; set; }
        public string? Gst { get; set; }
        public string? Tin { get; set; }
        public string? Cin { get; set; }
        public string? Company_Desc { get; set; }
        public string? Remarks { get; set; }
        public bool Is_Active { get; set; } = true;
        public string? Ip_Address { get; set; }
        public string? Password { get; set; }
    }

    public class CompanyResponseModel
    {
        public int Company_Id { get; set; }
        public string? END_Company_Id { get; set; }
        public int Plan_Id { get; set; }
        public string? Company_Name { get; set; }
        public string? Company_Short_Name { get; set; }
        public string? Company_Name_Regional { get; set; }
        public string? Toll_Free_Number { get; set; }
        public string? Company_Website { get; set; }
        public string? Address { get; set; }
        public string? Contact_Mobile_No { get; set; }
        public string? Contact_Email_Id { get; set; }
        public string? Contact_Person_Name { get; set; }
        public string? Company_Logo { get; set; }
        public string? Gst { get; set; }
        public string? Tin { get; set; }
        public string? Cin { get; set; }
        public string? Company_Desc { get; set; }
        public string? Remarks { get; set; }
        public bool Is_Active { get; set; }
        public string? Created_On { get; set; }
        public string? Ip_Address { get; set; }
    }

}
