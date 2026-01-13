namespace PetSociety.API.Models
{
    public class EcpaySettings
    {
        public string MerchantID { get; set; } = string.Empty;
        public string HashKey { get; set; } = string.Empty;
        public string HashIV { get; set; } = string.Empty;
        public string ServiceURL { get; set; } = string.Empty;
        public string ReturnURL { get; set; } = string.Empty;
        public string ClientBackURL { get; set; } = string.Empty;
    }
}
