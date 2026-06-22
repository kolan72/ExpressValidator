using FluentValidation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressValidator.Tests.Net8
{
	public class SubObjWithComplexProperty : ObjWithTwoPublicProps
    {
        public Contact? Contact { get; set; }
    }

    public class SubObjWithComplexCollectionProperty : ObjWithTwoPublicProps
    {
        public IEnumerable<Contact>? Contacts { get; set; }
    }

    public class ObjWithTwoPublicProps
    {
        public int I { get; set; }
        public string? S { get; set; }
        public string? _sField;
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

    public class SimpleContactValidator : AbstractValidator<Contact>
    {
        public SimpleContactValidator()
        {
            RuleFor(x => x.Name).NotNull();
            RuleFor(x => x.Email).NotNull();
        }
    }

	public class ContactValidator : AbstractValidator<Contact>
	{
		public ContactValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty()
				.MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
		}
	}

	public class Contact
    {
        public string? Name { get; set; }
        public string? Email { get; set; }

        public string? K { get; set; }
    }

	public class ContactNullableStructValidator : AbstractValidator<ContactStruct?>
	{
		public ContactNullableStructValidator()
		{
			RuleFor(x => x!.Value.Name)
				.NotEmpty()
				.MaximumLength(100);

			RuleFor(x => x!.Value.Email)
				.NotEmpty()
				.EmailAddress();
		}
	}

	public struct ContactStruct
	{
		public string Name { get; set; }
		public string Email { get; set; }
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
        public string? Name { get; set; }
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

	public sealed class CatsOptions
	{
		public int CatsCount { get; set; }
		public int CatsMinimum { get; set; }
	}

	public class Person
	{
		public IList<Cat> Cats { get; set; } = new List<Cat>();
		public int Id { get; set; } = 20;
	}

#pragma warning disable S2094 // Classes should not be empty
	public class Cat { }
#pragma warning restore S2094 // Classes should not be empty

	public class ObjWithSingleIndexer
	{
		private readonly string[] _items;

		public ObjWithSingleIndexer(params string[] items)
		{
			_items = items;
		}

		public int Length => _items.Length;

#pragma warning disable CS8603 // Possible null reference return
		public string this[int index] => _items[index];
#pragma warning restore CS8603 // Possible null reference return
	}

	public class ObjWithStringIndexer
	{
		private readonly Dictionary<string, double> _data = new();

		public void Set(string key, double value)
		{
			_data[key] = value;
		}

		public bool TryGetValue(string key, out double value)
		{
			return _data.TryGetValue(key, out value);
		}

#pragma warning disable CS8603 // Possible null reference return
		public double this[string key] => _data[key];
#pragma warning restore CS8603 // Possible null reference return
	}

	public class PersonValidator : AbstractValidator<Person>
	{
		public PersonValidator()
		{
			RuleFor(person => person.Cats)
				.SetExpressValidator(builder => builder.Configure
									((b) =>
										b.AddProperty(p => p.Count)
											.WithValidation((o, p) =>
															p
															.LessThan(o.CatsCount)
															.GreaterThanOrEqualTo(o.CatsMinimum)))
									.WithMessageTemplate((_, __, ___)
										=> "{PropertyName} must contain fewer than {MaxElements} items " +
										"and greater than or equal {MinElements} items.")
									.WithTemplateArgument("MaxElements", (po) => po.CatsCount)
									.WithTemplateArgument("MinElements", (po) => po.CatsMinimum)
									, new CatsOptions() { CatsCount = 14, CatsMinimum = 1 });
			RuleFor(person => person.Id)
				.SetExpressValidator(
					(config) =>
							config.Configure((b) =>
								b.AddFunc(p => p, "Id")
								.WithValidation((o, p) =>
									p.LessThan(o)
									.LessThanOrEqualTo(o)))
							, 1);
		}
	}
}
