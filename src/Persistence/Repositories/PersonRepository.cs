using Application.Contracts.Persistence;
using Domain.Entities;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Persistence.Repositories
{
    public class PersonRepository : GenericRepository<Person>, IPersonRepository
    {
        public PersonRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public Task<bool> PersonExists(int id)
        {
            var matches = _dbContext.Person.Any(e => e.Id == id);
            return Task.FromResult(matches); ;
        }
    }
}
