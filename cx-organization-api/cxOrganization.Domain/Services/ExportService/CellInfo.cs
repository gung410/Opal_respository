
namespace cxOrganization.Domain.Services.ExportService
{
    public class CellInfo
    {
        public object Value { get; set; }
        public string ValueFormat { get; set; }
        public bool IsGroup { get; set; }
        public override string ToString()
        {
            return Value?.ToString();
        }

        public static CellInfo Create(object value, string valueFormat = null, bool isGroup = false)
        {
            return new CellInfo
            {
                Value = value,
                ValueFormat = valueFormat,
                IsGroup = isGroup
            };
        }
    }
}
