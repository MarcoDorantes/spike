using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace valuex;

public struct Struct1(string firstName, string lastName, string[] phoneNumbers)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public string[] PhoneNumbers { get; } = phoneNumbers;

    public static bool operator ==(Struct1 a, Struct1 b) => a.Equals(b);
    public static bool operator !=(Struct1 a, Struct1 b) => !a.Equals(b);

    public override bool Equals([NotNullWhen(true)] object other) => (other is Struct1) && other.GetHashCode() == GetHashCode();
    public override int GetHashCode()
    {
        HashCode result = new();
        result.Add(FirstName);
        result.Add(LastName);
        Array.ForEach(PhoneNumbers, h => result.Add(h));
        return result.ToHashCode();
    }
}