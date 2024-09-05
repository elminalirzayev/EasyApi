using Domain.Entities;

namespace Application.Contracts.Persistence
{
    public interface IPersonRepository : IGenericRepositoryAsync<Person>
    {
        Task<bool> PersonExists(int id);
    }
}
