using cxOrganization.Domain.DomainEnums;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cxOrganization.Domain.Settings
{
    public class PropertyMappingSetting
    {
        public PropertyMappingSetting()
        {

        }

        public int SkipStartLine { get; set; }
        public int SkipEndLine { get; set; }
        public string Encoding { get; set; }
        public List<ColumnInfo> Columns { get; set; }
        public ColumnType ColumnType { get; set; }
        public string Delimiter { get; set; }
        public string DateFormat { get; set; }

        public Encoding GetEncoding()
        {
            if (!string.IsNullOrEmpty(Encoding))
            {
                return System.Text.Encoding.GetEncoding(Encoding);
            }

            return System.Text.Encoding.UTF8;
        }

        public Dictionary<string, string> GetExportFields() 
            => Columns.ToDictionary(x => x.Name, y => y.Caption);
    }

    public class ColumnInfo
    {
        public string Name { get; set; }
        /// <summary>
        /// Order of column. Start from 1.
        /// </summary>
        public int Order { get; set; }
        public int Length { get; set; }
        public int StartIndex { get; set; }
        public string Format { get; set; }
        public string Caption { get; set; }
    }
}
