using PetSociety.API.Models;

namespace PetSociety.API.Services.Class.Interfaces
{
    public interface ICourseOrderService
    {
        /// <summary>
        /// Creates a new course order for the specified member and course detail.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member placing the order. Must be a valid, existing member ID.</param>
        /// <param name="courseDetailId">The unique identifier of the course detail to order. Must refer to an available course detail.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see
        /// cref="CourseOrder"/> instance.</returns>
        Task<CourseOrder> CreateOrderAsync(int memberId, int courseDetailId);
        /// <summary>
        /// Updates the status of the specified course order to indicate that payment has been received.
        /// </summary>
        /// <param name="merchantTradeNo">The unique merchant trade number that identifies the course order to update. Cannot be <see
        /// langword="null"/> or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="CourseOrder"/> with its payment status set to paid.</returns>
        Task UpdateOrderToPaidAsync(string merchantTradeNo, string ecpayTradeNo);
    }
}
