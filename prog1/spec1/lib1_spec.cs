using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace spec1
{
    [TestClass]
    public class lib1_spec
    {
        [TestMethod]
        public void call_lib1()
        {
            //Arrange
            var expectation = $"{nameof(lib1.Class1.f)}";

            //Act
            var result=lib1.Class1.f();

            //Assert
            Assert.AreEqual(expectation,result);
        }
    }
}