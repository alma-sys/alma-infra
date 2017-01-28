using Microsoft.VisualStudio.TestTools.UnitTesting;
using Alma.Testes.DataBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alma.Testes.DataBuilder.Tests
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