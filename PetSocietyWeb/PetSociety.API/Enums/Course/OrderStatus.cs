using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetSociety.API.Enums.Course
{
    public enum OrderStatus
    {
        Unpaid = 0,
        Paid = 1,
        Failed = 2,
        Cancelled = 3,
    }
}
