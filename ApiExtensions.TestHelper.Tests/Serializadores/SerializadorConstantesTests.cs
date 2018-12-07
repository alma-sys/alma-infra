using Xunit;

namespace Alma.ApiExtensions.Serializadores.Tests
{
    public class SerializadorConstantesTests
    {
        enum ParaTestar
        {
            Valor1 = 1,
            Valor2 = 2,
            [Alma.Core.Description("Valor 3")]
            Valor3 = 3
        }


        [Fact]
        public void DeveSerializarEnumsEmCodigoNome()
        {

            var resultado = SerializadorConstantes.SerializarEnum(typeof(ParaTestar), true, true);
            Assert.Equal(@"[
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


        [Fact()]
        public void DeveSerializarEnumsComTextoEmCodigoDescricao()
        {

            var resultado = SerializadorConstantes.SerializarEnumChar(typeof(ParaTestar), true, true);
            Assert.Equal(@"[
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