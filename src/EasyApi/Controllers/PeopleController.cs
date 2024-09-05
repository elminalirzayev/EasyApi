using Application.Contracts.Persistence;
using Application.Exceptions;
using Domain.Entities;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Prometheus;
using System.Text;

namespace EasyApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly IPersonRepository _personRepository;
        private IDistributedCache _distributedCache;

        private static readonly Counter MyCounter = Metrics.CreateCounter("easy_api_counter", "Counts requests to PeopleController.");


        public PeopleController(IPersonRepository personRepository, IDistributedCache distributedCache)
        {
            _personRepository = personRepository;
            _distributedCache = distributedCache;
        }


        // GET: api/People
        /// <summary>
        /// Get Person List
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        /// GET: api/People
        /// </remarks>
        /// <returns>This endpoint will return all People list</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Person>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<Person>> GetPerson()
        {
            MyCounter.Inc();


            string cachedPeople = await _distributedCache.GetStringAsync("people");
            if (!string.IsNullOrEmpty(cachedPeople))
            {
                return JsonConvert.DeserializeObject<List<Person>>(cachedPeople);
            }
            //add to redis cache
            var people = await _personRepository.ListAllAsync();
            var serializedData = JsonConvert.SerializeObject(people);
            await _distributedCache.SetStringAsync("people", serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });

            return people;
        }

        // GET: api/People/1
        /// <summary>
        /// Get Person by Id
        /// </summary>
        /// <param name="id">Id of Person</param>
        /// <remarks>
        /// Sample request:
        /// 
        /// GET: api/People/1
        /// </remarks>
        /// <returns>This endpoint will return Person by Id</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Person), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = await _personRepository.GetByIdAsync(id);

            if (person == null)
            {
                throw new NotFoundException($"PersonId : {id}", id);
            }

            return person;
        }


        // POST: api/People
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Create new Person
        /// </summary>
        /// <param name="person">Person</param>
        /// <remarks>
        /// Sample request:
        /// 
        /// POST: api/People
        /// {
        ///     "name":"elmin",
        ///     "surname": "alirzayev",
        ///     "email": "elmin.alirzayev@gmail.com",
        ///     "gender": 1,
        ///     "birthDate": "1994-12-21"
        /// }
        /// </remarks>
        /// <returns>This endpoint will create new Person</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Person), StatusCodes.Status201Created)]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {

            PersonValidator validator = new PersonValidator();
            ValidationResult results = validator.Validate(person);

            if (!results.IsValid)
            {
                throw new ModelValidationException(results);
            }


            await _personRepository.AddAsync(person);

            return CreatedAtAction("GetPerson", new { id = person.Id }, person);
        }
        // PUT: api/People/1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Update existed Person
        /// </summary>
        /// <param name="person">Person</param>
        /// <param name="id">Id of Person</param>
        /// <remarks>
        /// Sample request:
        /// 
        /// POST: api/People/1
        /// {
        ///     "id": 1,
        ///     "name":"elmin",
        ///     "surname": "alirzayev",
        ///     "email": "elmin.alirzayev@gmail.com",
        ///     "gender": 1,
        ///     "birthDate": "1994-12-21"
        /// }
        /// </remarks>
        /// <returns>This endpoint will update existed Person in Db</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutPerson(int id, Person person)
        {
            if (id != person.Id)
            {
                throw new BadRequestException($"PersonId : {id} , Person {JsonConvert.SerializeObject(person)}");
            }

            PersonValidator validator = new PersonValidator();
            ValidationResult results = validator.Validate(person);

            if (!results.IsValid)
            {
                throw new ModelValidationException(results);
            }

            try
            {
                await _personRepository.UpdateAsync(person);
            }
            catch (Exception ex)
            {
                if (!_personRepository.PersonExists(id).Result)
                {
                    throw new NotFoundException($"Person : {id}", JsonConvert.SerializeObject(person));
                }
                else
                {
                    throw new ApiException($"PersonId : {id} , Exception : {ex.Message}");
                }
            }

            return NoContent();
        }

        // DELETE: api/People/1
        /// <summary>
        /// Delete existed Person
        /// </summary>
        /// <param name="id">Id of Person</param>
        /// <remarks>
        /// Sample request:
        /// 
        /// DELETE: api/People/1
        /// </remarks>
        /// <returns>This endpoint will delete existed Person in Db</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person == null)
            {
                throw new NotFoundException($"PersonId : {id}", id);
            }

            await _personRepository.DeleteAsync(person);

            return NoContent();
        }


    }
}
