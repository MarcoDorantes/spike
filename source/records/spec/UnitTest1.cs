namespace spec;

using System.Linq;

[TestClass]
public class RecordTypesMinimalSpec
{
    [TestMethod]
    public void RecordEqual()
    {
        var phoneNumbers = new string[2];
        Person person1 = new("Nancy", "Davolio", phoneNumbers);
        Person person2 = new("Nancy", "Davolio", phoneNumbers);
        Assert.IsTrue(person1 == person2);

        person1.PhoneNumbers[0] = "555-1234";
        Assert.IsTrue(person1 == person2);

        Assert.IsFalse(ReferenceEquals(person1, person2));
    }

    [TestMethod]
    public void RecordNotEqual()
    {
        var phoneNumbers1 = new string[2];
        var phoneNumbers2 = new string[2];
        Person person1 = new("Nancy", "Davolio", phoneNumbers1);
        Person person2 = new("Nancy", "Davolio", phoneNumbers2);
        Assert.IsFalse(person1 == person2);

        person1.PhoneNumbers[0] = "555-1234";
        Assert.IsFalse(person1 == person2);

        Assert.IsFalse(ReferenceEquals(person1, person2));
    }

    [TestMethod]
    public void record_props()
    {
        Person p = new("name1", "last1", null);
        var properties = p.GetType().GetProperties();
        Assert.AreEqual(3, properties.Length);
        Assert.AreEqual("FirstName,LastName,PhoneNumbers", string.Join(',', properties.Select(p => p.Name)));
    }

    [TestMethod]
    public void class_props()
    {
        objectx.Class1 p = new("name1", "last1", null);
        var properties = p.GetType().GetProperties();
        Assert.AreEqual(3, properties.Length);
        Assert.AreEqual("FirstName,LastName,PhoneNumbers", string.Join(',', properties.Select(p => p.Name)));
    }

    [TestMethod]
    public void recordclass_props()
    {
        recordx.RecordClass1 p = new("name1", "last1", null);
        var properties = p.GetType().GetProperties();
        Assert.AreEqual(3, properties.Length);
        Assert.AreEqual("FirstName,LastName,PhoneNumbers", string.Join(',', properties.Select(p => p.Name)));
    }
}