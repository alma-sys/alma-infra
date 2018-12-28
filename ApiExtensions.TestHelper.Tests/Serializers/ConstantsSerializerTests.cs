using Xunit;

namespace Alma.ApiExtensions.Serializers.Tests
{
    public class ConstantsSerializer
    {
        enum ValuesToTest
        {
            Value1 = 1,
            [System.ComponentModel.AmbientValue("VALUE-2")]
            Value2 = 2,
            [System.ComponentModel.Description("The Value Is Three")]
            Value3 = 3
        }


        [Fact]
        public void ShouldSerializeEnumValuesIntoIdName()
        {

            var resultado = ConstantSerializer.SerializeEnum(typeof(ValuesToTest), true, true);
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

            var resultado = ConstantSerializer.SerializeEnumChar(typeof(ValuesToTest), true, true);
            System.Console.WriteLine(resultado);
            Assert.Equal(@"[
  {
    ""code"": ""Value1"",
    ""description"": ""Value1""
  },
  {
    ""code"": ""VALUE-2"",
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