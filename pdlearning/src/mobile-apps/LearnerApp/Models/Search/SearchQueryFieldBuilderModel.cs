namespace LearnerApp.Models.Search
{
    public class SearchQueryFieldBuilderModel
    {
        public SearchQueryFieldBuilderModel(string fieldName, string @operator, params string[] values)
        {
            FieldName = fieldName;
            Operator = @operator;
            Values = values;
        }

        public string FieldName { get; set; }

        public string Operator { get; set; }

        public string[] Values { get; set; }
    }
}
