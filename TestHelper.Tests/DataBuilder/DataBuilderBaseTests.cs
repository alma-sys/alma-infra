using Xunit;

namespace Alma.TestHelper.DataBuilder.Tests
{
    public class DataBuilderBaseTests
    {
        [Fact()]
        public void ShouldGenerateTag()
        {
            var tag = DataBuilderBase.GenerateTag();
            Assert.NotNull(tag);
        }

        [Fact()]
        public void ShouldGenerateDifferentTags()
        {
            var tag = DataBuilderBase.GenerateTag();
            var tag2 = DataBuilderBase.GenerateTag();

            Assert.NotNull(tag);
            Assert.NotNull(tag2);

            Assert.NotEqual(tag, tag2);
        }
    }
}