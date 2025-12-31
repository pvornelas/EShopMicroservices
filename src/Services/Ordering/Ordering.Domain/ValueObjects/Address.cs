namespace Ordering.Domain.ValueObjects
{
    public record Address
    {
        public string FirstName { get; } = default!;
        public string LastName { get; } = default!;
        public string? EmailAddress { get; } = default!;
        public string AddressLine { get; } = default!;
        public string Country { get; } = default!;
        public string State { get; } = default!;
        public string ZipCode { get; } = default!;

        protected Address()
        {
        }

        private Address(string fisrtName, string lastName, string emailAdress,
            string addressLine, string country, string state, string zipCode)
        {
            FirstName = fisrtName;
            LastName = lastName;
            EmailAddress = emailAdress;
            AddressLine = addressLine;
            Country = country;
            State = state;
            ZipCode = zipCode;
        }

        public static Address Of(string fisrtName, string lastName, string emailAdress,
            string addressLine, string country, string state, string zipCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(emailAdress);
            ArgumentException.ThrowIfNullOrWhiteSpace(addressLine);

            return new Address(fisrtName, lastName, emailAdress, addressLine, country, state, zipCode);
        }
    }
}
