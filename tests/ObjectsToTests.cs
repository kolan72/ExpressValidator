﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressValidator.Tests
{
    public class SubObjWithComplexProperty : ObjWithTwoPublicProps
    {
        public Contact Contact { get; set; }
    }

    public class SubObjWithComplexCollectionProperty : ObjWithTwoPublicProps
    {
        public IEnumerable<Contact> Contacts { get; set; }
    }

    public class ObjWithTwoPublicProps
    {
        public int I { get; set; }
        public string S { get; set; }
        public string _sField;
        public int _iField;
        public int PercentValue1 { get; set; }
        public int PercentValue2 { get; set; }
    }

    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            RuleFor(x => x.Name).NotNull();
            RuleFor(x => x.Email).NotNull();
        }
    }

    public class Contact
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public string K { get; set; }
    }

    class ObjWithNullable
    {
        public string Value { get; set; } = "Test";
    }

    enum SetPropertyNameType
    {
        NotSetExplicitly,
        Override,
        WithName
    }
}
