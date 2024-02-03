namespace recordx;

public record class RecordClass1(string FirstName, string LastName, string[] PhoneNumbers)
{
    public string FirstName { get; set; } = FirstName;
    public string LastName { get; } = LastName;
    public string[] PhoneNumbers { get; } = PhoneNumbers;
}