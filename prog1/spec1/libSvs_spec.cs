using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace spec1
{
    [TestClass]
    public class libSvs_spec
    {
        [TestMethod]
        public void call_libSvs()
        {
            //Arrange
            var expectation = $"{nameof(libSvs.Class1.f)}";

            //Act
            var result=libSvs.Class1.f();

            //Assert
            Assert.AreEqual(expectation,result);
        }
    }
}