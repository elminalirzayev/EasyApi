using Application.Contracts.Persistence;
using Application.Exceptions;
using AutoFixture;
using Domain.Entities;
using Domain.Enums;
using EasyApi.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace EasyApiTest
{
    public class EasyApiTest
    {
        public Mock<IPersonRepository> _personRepository = new Mock<IPersonRepository>();
        public Mock<ILogger<PeopleController>> _logger = new Mock<ILogger<PeopleController>>();
        public Mock<IDistributedCache> _distributedCache = new Mock<IDistributedCache>();




        [Fact]
        public void GET_PERSON_TEST()
        {
            // Arrange
            var fixture = new Fixture();
            var personFixture = fixture.Create<Person>();

            var personList = new List<Person>()
            {
                new Person{Id=1,Name="Elmin",Surname="Alirzayev",Email="elmin.alirzayev@gmail.com",Gender=Gender.Male,BirthDate= new DateTime(1994,12,21)},
                new Person{Id=2,Name="Elmin",Surname="Alirzayev",Email="elmin.alirzayev@gmail.com",Gender=Gender.Male,BirthDate= new DateTime(1994,12,21)},
                personFixture
           };
            _personRepository.Setup(x => x.ListAllAsync()).ReturnsAsync(personList);

            PeopleController _peopleController = new PeopleController(_personRepository.Object, _distributedCache.Object);

            // Act

            var result = _peopleController.GetPerson().Result;

            // Assert

            Assert.IsType<List<Person>>(result);
        }


        [Fact]
        public void GET_PERSON_BY_ID_SUCCESS_TEST()
        {
            // Arrange

            Person person = new() { Id = 1, Name = "Elmin", Surname = "Alirzayev", Email = "elmin.alirzayev@gmail.com", Gender = Gender.Male, BirthDate = new DateTime(1994, 12, 21) };
            int _searchedId = 1;
            _personRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(person);

            PeopleController _peopleController = new PeopleController(_personRepository.Object, _distributedCache.Object);

            //Act

            var result = _peopleController?.GetPerson(_searchedId)?.Result;

            // Assert

            Assert.IsType<ActionResult<Person>>(result);
            Assert.IsType<Person>(result?.Value);
            Assert.Equal(_searchedId, result?.Value?.Id);
        }


        [Fact]
        public void GET_PERSON_BY_ID_NOTFOUND_TEST()
        {
            //Arrange

            Person person = new() { Id = 1, Name = "Elmin", Surname = "Alirzayev", Email = "elmin.alirzayev@gmail.com", Gender = Gender.Male, BirthDate = new DateTime(1994, 12, 21) };
            _personRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(person);

            PeopleController _peopleController = new PeopleController(_personRepository.Object, _distributedCache.Object);



            // Assert

            var ex = Assert.Throws<AggregateException>(() => _peopleController?.GetPerson(2)?.Result.Result);
            Assert.IsType<NotFoundException>(ex.InnerException);


        }

        [Fact]
        public void CREATE_PERSON_SUCCESS_TEST()
        {
            // Arrange

            Person person = new() { Id = 1, Name = "Elmin", Surname = "Alirzayev", Email = "elmin.alirzayev@gmail.com", Gender = Gender.Male, BirthDate = new DateTime(1994, 12, 21) };

            _personRepository.Setup(x => x.AddAsync(person)).ReturnsAsync(person);
            _personRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(person);

            PeopleController _peopleController = new PeopleController(_personRepository.Object, _distributedCache.Object);

            // Act

            var result = _peopleController?.PostPerson(person).Result.Result;

            // Assert

            Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, (result as CreatedAtActionResult)?.StatusCode);

            Assert.IsType<Person>((result as CreatedAtActionResult)?.Value);
            Assert.Equal(person.Name, ((result as CreatedAtActionResult)?.Value as Person)?.Name);
            Assert.Equal(person.Surname, ((result as CreatedAtActionResult)?.Value as Person)?.Surname);
            Assert.Equal(person.Email, ((result as CreatedAtActionResult)?.Value as Person)?.Email);
            Assert.Equal(person.BirthDate, ((result as CreatedAtActionResult)?.Value as Person)?.BirthDate);



            // fluent assertion
            result.Should().BeOfType<CreatedAtActionResult>();
            (result as CreatedAtActionResult)?.StatusCode.Should().Be((int)HttpStatusCode.Created);
            (result as CreatedAtActionResult)?.Value.Should().BeOfType<Person>();
            ((result as CreatedAtActionResult)?.Value as Person)?.Name.Should().Be(person.Name);




        }
        [Fact]
        public void PERSON_VALIDATION_WHEN_NAME_CONTAINS_NUMBER_TEST()
        {
            //Arrange

            Person person = new() { Id = 1, Name = "A2222", Surname = "Alirzayev", Email = "elmin.alirzayev@gmail.com", Gender = Gender.Male, BirthDate = new DateTime(1994, 12, 21) };
            PersonValidator validationRules = new PersonValidator();

            //Act

            var result = validationRules.Validate(person);

            //Assert

            Assert.True(!result.IsValid);
        }
        [Fact]
        public void PERSON_VALIDATION_WHEN_NAME_IS_EMPTY_TEST()
        {
            //Arrange

            Person person = new() { Id = 1, Name = "", Surname = "Alirzayev", Email = "elmin.alirzayev@gmail.com", Gender = Gender.Male, BirthDate = new DateTime(1994, 12, 21) };
            PersonValidator validationRules = new PersonValidator();

            //Act

            var result = validationRules.Validate(person);

            //Assert

            Assert.True(!result.IsValid);
        }
        [Fact]
        public void PERSON_VALIDATION_WHEN_EMAIL_IS_NOT_VALID_TEST()
        {
            //Arrange

            Person person = new() { Id = 1, Name = "Elmin", Surname = "Alirzayev", Email = "elmin.alirzayevgmail.com", Gender = Gender.Male, BirthDate = new DateTime(1994, 12, 21) };
            PersonValidator validationRules = new PersonValidator();

            //Act

            var result = validationRules.Validate(person);

            //Assert

            Assert.True(!result.IsValid);
        }
        [Fact]
        public void PERSON_VALIDATION_WHEN_GENDER_IS_NOT_VALID_TEST()
        {
            //Arrange

            Person person = new() { Id = 1, Name = "Elmin", Surname = "Alirzayev", Email = "elmin.alirzayev@gmail.com", Gender = (Gender)5, BirthDate = new DateTime(1994, 12, 21) };
            PersonValidator validationRules = new PersonValidator();

            //Act

            var result = validationRules.Validate(person);

            //Assert

            Assert.True(!result.IsValid);
        }
        [Fact]
        public void PERSON_VALIDATION_WHEN_BIRHDATE_IS_NOT_VALID_TEST()
        {
            //Arrange

            Person person = new() { Id = 1, Name = "Elmin", Surname = "Alirzayev", Email = "elmin.alirzayev@gmail.com", Gender = Gender.Male, BirthDate = DateTime.Now.Date.AddDays(1) };
            PersonValidator validationRules = new PersonValidator();

            //Act

            var result = validationRules.Validate(person);

            //Assert

            Assert.True(!result.IsValid);
        }
    }
}