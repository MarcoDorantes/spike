namespace recordx;

public record class RecordClass1(string firstName, string lastName, string[] phoneNumbers)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public string[] PhoneNumbers { get; } = phoneNumbers;
}