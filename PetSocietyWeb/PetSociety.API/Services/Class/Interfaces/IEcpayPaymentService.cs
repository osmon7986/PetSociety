using PetSociety.API.DTOs.Class;

namespace PetSociety.API.Services.Class.Interfaces
{
    public interface IEcpayPaymentService
    {
        /// <summary>
        /// Get payment parameters for Ecpay
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Dictionary<string, string> GetPaymentParameters(EcpayCheckOutDTO dto);

        /// <summary>
        /// Generates a CheckMacValue hash for the specified set of parameters.
        /// </summary>
        /// <param name="param">A dictionary containing the key-value pairs to include in the hash calculation. Keys and values must not be
        /// null or empty.</param>
        /// <returns>A string representing the computed CheckMacValue hash for the provided parameters.</returns>
        string GenerateCheckMacValue(Dictionary<string, string> param); 
    }
}
