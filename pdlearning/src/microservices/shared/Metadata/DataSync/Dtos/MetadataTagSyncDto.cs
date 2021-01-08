using System;
using System.Collections.Generic;
using Conexus.Opal.Microservice.Metadata.Entities;

namespace Conexus.Opal.Microservice.Metadata.Dtos
{
    public class MetadataTagSyncDto
    {
        public Guid Id { get; set; }

        public string FullStatement { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        public string DisplayText { get; set; }

        public string Code { get; set; }

        public string CodingScheme { get; set; }

        public string AbbreviatedStatement { get; set; }

        public Guid? TypeId { get; set; }

        public IEnumerable<MetadataTagSyncDto> Childs { get; set; }

        public static IEnumerable<MetadataTagSyncDto> FlatTree(MetadataTagSyncDto root)
        {
            var result = new List<MetadataTagSyncDto>
            {
                root
            };

            if (root.Childs != null)
            {
                foreach (var item in root.Childs)
                {
                    result.AddRange(FlatTree(item));
                }
            }

            return result;
        }

        public MetadataTag ToEntity(string groupCode, Guid? parentTagId = null)
        {
            return new MetadataTag
            {
                TagId = Id,
                FullStatement = FullStatement,
                Note = Note,
                DisplayText = DisplayText,
                CodingScheme = CodingScheme,
                GroupCode = groupCode,
                ParentTagId = parentTagId,
                Type = Type
            };
        }
    }
}
