namespace Ordering.Application.Dtos
{
    public record AddressDto(
        string FirstName,
        string LastName,
        string EmailAdress,
        string AddressLine,
        string Country,
        string State,
        string ZipCode);
}
