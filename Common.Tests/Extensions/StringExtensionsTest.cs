using Xunit;

namespace Alma.Common.Tests.Extensions
{
    public class StringExtensions
    {
        [Fact]
        public void ShouldRemoveAccentuation()
        {
            var accentedPhrase = "Se hoje � o dia das crian�as... Ontem eu disse: o dia da crian�a � o dia da m�e, dos pais, das professoras, mas tamb�m � o dia dos animais, sempre que voc� olha uma crian�a, h� sempre uma figura oculta, que � um cachorro atr�s. O que � algo muito importante!";
            var expectedPhrase = "Se hoje e o dia das criancas... Ontem eu disse: o dia da crianca e o dia da mae, dos pais, das professoras, mas tambem e o dia dos animais, sempre que voce olha uma crianca, ha sempre uma figura oculta, que e um cachorro atras. O que e algo muito importante!";

            var actual = accentedPhrase.RemoveAccentuation();

            Assert.Equal(expectedPhrase, actual);
        }
    }
}
