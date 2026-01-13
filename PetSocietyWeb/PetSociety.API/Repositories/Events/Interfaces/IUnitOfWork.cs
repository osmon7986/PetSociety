namespace PetSociety.API.Repositories.Events.Interfaces
{
    public interface IUnitOfWork
    {
        //將dbcontext藏起來
        Task<int> CompleteAsync();
    }
}
