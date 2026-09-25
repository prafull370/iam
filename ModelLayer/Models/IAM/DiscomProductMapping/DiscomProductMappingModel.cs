namespace ModelLayer.Models.IAM.DiscomProductMapping
{
    public class DiscomProductMappingRequestModel
    {
        public int Discom_Id { get; set; }
        public List<int> Product_Ids { get; set; } = new();
    }
    public class DiscomProductMappingResponseModel
    {
        public int Discom_Id { get; set; }
        public int Product_Id { get; set; }
        public string? Product_Name { get; set; }
        public string? Discom_Name { get; set; }
        public string? Product_Short_Name { get; set; }
        public string? Product_Logo { get; set; }
        public bool Is_Mapped { get; set; }
    }
}
