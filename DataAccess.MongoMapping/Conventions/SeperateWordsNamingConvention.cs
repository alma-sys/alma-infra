using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.Text.RegularExpressions;

namespace Alma.DataAccess.MongoMapping.Conventions
{
    class SeperateWordsNamingConvention : IMemberMapConvention
    {
        //Taken from:
        //http://stackoverflow.com/questions/4488969/split-a-string-by-capital-letters?answertab=votes#tab-top
        private static readonly Regex s_seperateWordRegex =
         new Regex(@"
   (?<=[A-Z])(?=[A-Z][a-z]) |
   (?<=[^A-Z])(?=[A-Z]) |
   (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

        public string Name => nameof(SeperateWordsNamingConvention);
        public void Apply(BsonMemberMap memberMap)
        {
            var replace = s_seperateWordRegex.Replace(memberMap.ElementName, "_");
            memberMap.SetElementName(replace);
        }
    }
}
