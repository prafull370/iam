

namespace ModelLayer.Models.Common
{
    public class LoggedUserDataModel
    {
        public int? User_Id { get; set; }
        public string? User_Name { get; set; }
        public int? Company_Id { get; set; }
        public int? Discom_Id { get; set; }
        public string? Company_Name { get; set; }
        public string? Email_Id { get; set; }
        public string? Role_Name { get; set; }
        public string? IP_Address { get; set; }
        public string? Token { get; set; }
        public string? Is_Superadmin { get; set; }
        public string? Is_Company_Owner { get; set; }
        public string? Login_Type { get; set; }

    }
}