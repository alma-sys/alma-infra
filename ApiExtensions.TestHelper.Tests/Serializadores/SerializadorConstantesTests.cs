using Xunit;

namespace Alma.ApiExtensions.Serializadores.Tests
{
    public class ConstantsSerializer
    {
        enum ValuesToTest
        {
            Value1 = 1,
            Value2 = 2,
            [Alma.Core.Description("The Value Is Three")]
            Value3 = 3
        }


        [Fact]
        public void ShouldSerializeEnumValuesIntoIdName()
        {

            var resultado = SerializadorConstantes.SerializarEnum(typeof(ValuesToTest), true, true);
            Assert.Equal(@"[
  {
    ""id"": 1,
    ""name"": ""Value1""
  },
  {
    ""id"": 2,
    ""name"": ""Value2""
  },
  {
    ""id"": 3,
    ""name"": ""The Value Is Three""
  }
]", resultado);
        }


        [Fact()]
        public void ShouldSerializeEnumValuesIntoCodeDescription()
        {

            var resultado = SerializadorConstantes.SerializarEnumChar(typeof(ValuesToTest), true, true);
            Assert.Equal(@"[
  {
    ""code"": ""Value1"",
    ""description"": ""Value1""
  },
  {
    ""code"": ""Value2"",
    ""description"": ""Value2""
  },
  {
    ""code"": ""Value3"",
    ""description"": ""The Value Is Three""
  }
]", resultado);
        }
    }
}