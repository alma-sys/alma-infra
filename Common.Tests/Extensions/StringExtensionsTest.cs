using Xunit;

namespace Alma.Common.Tests.Extensions
{
    public class StringExtensions
    {
        [Fact]
        public void ShouldRemoveAccentuation()
        {
            var accentedPhrase = "Se hoje é o dia das crianças... Ontem eu disse: o dia da criança é o dia da mãe, dos pais, das professoras, mas também é o dia dos animais, sempre que você olha uma criança, há sempre uma figura oculta, que é um cachorro atrás. O que é algo muito importante!";
            var expectedPhrase = "Se hoje e o dia das criancas... Ontem eu disse: o dia da crianca e o dia da mae, dos pais, das professoras, mas tambem e o dia dos animais, sempre que voce olha uma crianca, ha sempre uma figura oculta, que e um cachorro atras. O que e algo muito importante!";

            var actual = accentedPhrase.RemoveAccentuation();

            Assert.Equal(expectedPhrase, actual);
        }
    }
}
