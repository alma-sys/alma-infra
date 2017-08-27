using Alma.ApiExtensions.Serializadores;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Alma.ApiExtensions.TestHelper.Serializadores.Tests
{
    [TestClass()]
    public class SerializadorConstantesTests
    {
        enum ParaTestar
        {
            Valor1 = 1,
            Valor2 = 2,
            [System.ComponentModel.Description("Valor 3")]
            Valor3 = 3
        }


        [TestMethod()]
        public void DeveSerializarEnumsEmCodigoNome()
        {

            var resultado = SerializadorConstantes.SerializarEnum(typeof(ParaTestar), true, true);
            Assert.AreEqual(@"[
  {
    ""id"": 1,
    ""nome"": ""Valor1""
  },
  {
    ""id"": 2,
    ""nome"": ""Valor2""
  },
  {
    ""id"": 3,
    ""nome"": ""Valor 3""
  }
]", resultado);
        }


        [TestMethod()]
        public void DeveSerializarEnumsComTextoEmCodigoDescricao()
        {

            var resultado = SerializadorConstantes.SerializarEnumChar(typeof(ParaTestar), true, true);
            Assert.AreEqual(@"[
  {
    ""codigo"": ""Valor1"",
    ""descricao"": ""Valor1""
  },
  {
    ""codigo"": ""Valor2"",
    ""descricao"": ""Valor2""
  },
  {
    ""codigo"": ""Valor3"",
    ""descricao"": ""Valor 3""
  }
]", resultado);
        }
    }
}