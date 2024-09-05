using Domain.Common;
using Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Person : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
       

    }

    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Name).Length(1, 30).Matches(@"^[A-Za-z\s]*$").WithMessage("'{PropertyName}' should only contain letters.");
            RuleFor(x => x.Surname).Length(1, 30);
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.BirthDate).LessThanOrEqualTo(DateTime.Now.Date);
            RuleFor(x => x.Gender).IsInEnum();
        }
    }
}
