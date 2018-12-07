using Xunit;

namespace Alma.TestHelper.DataBuilder.Tests
{
    public class DataBuilderBaseTests
    {
        [Fact()]
        public void DeveGerarTag()
        {
            var tag = DataBuilderBase.GerarTag();
            Assert.NotNull(tag);
        }

        [Fact()]
        public void DeveGerarTagsDiferentes()
        {
            var tag = DataBuilderBase.GerarTag();
            var tag2 = DataBuilderBase.GerarTag();

            Assert.NotNull(tag);
            Assert.NotNull(tag2);

            Assert.NotEqual(tag, tag2);
        }
    }
}