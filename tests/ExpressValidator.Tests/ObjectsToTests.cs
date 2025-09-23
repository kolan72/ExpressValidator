using FluentValidation;
using System;
using System.Collections.Generic;
using System.Threading;
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

    public class ObjWithTwoPublicPropsOptions
    {
        public int IGreaterThanValue { get; set; }
        public int IGreaterThanValue2 { get; set; }
        public int SMaximumLengthValue { get; set; }
        public int SFieldMaximumLengthValue { get; set; }
        public int PercentSumMinValue { get; set; }
        public int PercentSumMaxValue { get; set; }
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

    internal class ObjWithNullable
    {
        public string Value { get; set; } = "Test";
    }

    internal enum SetPropertyNameType
    {
        NotSetExplicitly,
        Override,
        WithName
    }

    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public decimal CustomerDiscount { get; set; }
        public bool IsPreferredCustomer { get; set; }
    }

    public class SomeExternalWebApiClient
    {
        private readonly int _existedId;

		public SomeExternalWebApiClient(int existedId)
        {
            _existedId = existedId;
		}

        public async Task<bool> IdExistsAsync(int id, CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromTicks(1), token);
            return _existedId == id;
        }

	}
}
