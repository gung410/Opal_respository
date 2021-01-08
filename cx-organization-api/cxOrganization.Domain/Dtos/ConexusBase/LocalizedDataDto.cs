using System;
using System.Collections.Generic;
using System.Linq;

namespace cxPlatform.Client.ConexusBase
{
    public class LocalizedDataDto : IConexusBaseDto
    {
        public const string MaxVersion = "99.99";
        public LocalizedDataDto()
        {
            Fields = new List<LocalizedDataFieldDto>();
        }
        public int Id { get; set; }
        public string LanguageCode { get; set; }
        public List<LocalizedDataFieldDto> Fields { get; set; }
        public string GetField(string fieldName)
        {
            return Fields.Where(p => p.Name == fieldName).Select(p => p.LocalizedText).FirstOrDefault();
        }
    }
}
