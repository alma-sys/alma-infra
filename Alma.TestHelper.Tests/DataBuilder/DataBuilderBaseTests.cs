using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Alma.TestHelper.DataBuilder.Tests
{
    [TestClass()]
    public class DataBuilderBaseTests
    {
        [TestMethod()]
        public void DeveGerarTag()
        {
            var tag = DataBuilderBase.GerarTag();
            Assert.IsNotNull(tag);
        }

        [TestMethod()]
        public void DeveGerarTagsDiferentes()
        {
            var tag = DataBuilderBase.GerarTag();
            var tag2 = DataBuilderBase.GerarTag();

            Assert.IsNotNull(tag);
            Assert.IsNotNull(tag2);

            Assert.AreNotEqual(tag, tag2);
        }
    }
}