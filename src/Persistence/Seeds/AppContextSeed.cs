using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Persistence.Seeds
{
    public static class AppContextSeed
    {
        public static void ApplicationSeed(this ModelBuilder modelBuilder)
        {
            CreatePerson(modelBuilder);
        }

        private static void CreatePerson(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().HasData(PersonList());
        }

        private static List<Person> PersonList()
        {
            return new List<Person>()
            {
                new Person() {Id=1  , Name= "John", Surname= "Doe",Email="john.doe@mail.com", BirthDate=new DateTime(1990,12,21), Gender= Gender.Male},
                new Person() {Id=2  , Name= "The", Surname= "Rock",Email="rock@mail.com", BirthDate=new DateTime(1991,12,21), Gender= Gender.Male},
                new Person() {Id=3  , Name= "Charlie", Surname= "Chaplin",Email="charlie@mail.com", BirthDate=new DateTime(1992,12,21), Gender= Gender.Male},
                new Person() {Id=4  , Name= "Selena", Surname= "Gomez",Email="selene2000@mail.com", BirthDate=new DateTime(2000,12,21), Gender= Gender.Female},
            };
        }


    }
}