using LearnerApp.Common.Helper;
using Xunit;

namespace LearnerApp.UnitTest.Helper
{
    public class TransformSearchSpecialCharactersTests
    {
        [Theory]
        [InlineData("\"Here is something cool", "\\\"Here is something cool")]
        [InlineData("\"Here is something cool\"", "\"Here is something cool\"")]
        [InlineData("\"Here is\" something cool\"", "\"Here is\" something cool\"")]
        [InlineData("\"Here is\" \"something cool", "\\\"Here is\\\" \\\"something cool")]
        public void ReplaceSingleQuoteTest(string input, string expected)
        {
            var actual = TransformSearchSpecialCharacters.TransformSpecialCharacterToApiSearchString(input);

            Assert.Equal(actual, expected);
        }

        [Theory]
        [InlineData("*Test* *String*", "\\*Test\\* \\*String\\*")]
        [InlineData("/Test/ /String/", "\\/Test\\/ \\/String\\/")]
        public void ReplaceSpecialCharacterTest(string input, string expected)
        {
            var actual = TransformSearchSpecialCharacters.TransformSpecialCharacterToApiSearchString(input);

            Assert.Equal(actual, expected);
        }

        [Theory]
        [InlineData("\"*Test* *String*\"", "\"*Test* *String*\"")]
        [InlineData("\"/Test/ /String/", "\\\"\\/Test\\/ \\/String\\/")]
        public void IntegrationTransformSpecialCharacterTest(string input, string expected)
        {
            var actual = TransformSearchSpecialCharacters.TransformSpecialCharacterToApiSearchString(input);

            Assert.Equal(actual, expected);
        }
    }
}
