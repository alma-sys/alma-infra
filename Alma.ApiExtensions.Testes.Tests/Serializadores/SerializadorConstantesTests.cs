using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Alma.ApiExtensions.Serializadores.Tests
{
    [TestClass()]
    public class SerializadorConstantesTests
    {
        enum ParaTestar
        {
            Valor1,
            Valor2,
            [System.ComponentModel.Description("Valor 3")]
            Valor3
        }


        [TestMethod()]
        public void DeveSerializarEnumsEmCodigoNome()
        {

            var resultado = SerializadorConstantes.SerializarEnum(typeof(ParaTestar), true, true);
            Assert.AreEqual(@"[
  {
    ""codigo"": 0,
    ""nome"": ""Valor1""
  },
  {
    ""codigo"": 1,
    ""nome"": ""Valor2""
  },
  {
    ""codigo"": 2,
    ""nome"": ""Valor 3""
  }
]", resultado);
        }
    }
}