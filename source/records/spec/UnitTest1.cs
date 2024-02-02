namespace spec;

using System.Linq;

[TestClass]
public class UnitTest1
{
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
        Assert.AreEqual(6, properties.Length);
        Assert.AreEqual("firstName,lastName,phoneNumbers,FirstName,LastName,PhoneNumbers", string.Join(',', properties.Select(p => p.Name)));
    }
}