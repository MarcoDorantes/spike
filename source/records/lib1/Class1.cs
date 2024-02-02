namespace objectx;

using System;

public class Class1(string firstName, string lastName, string[] phoneNumbers)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public string[] PhoneNumbers { get; } = phoneNumbers;
}