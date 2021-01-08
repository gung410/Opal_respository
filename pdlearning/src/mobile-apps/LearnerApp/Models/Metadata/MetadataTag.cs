using System;
using System.Collections.Generic;

namespace LearnerApp.Models
{
    public class MetadataTag : ICloneable
    {
        public int Id { get; set; }

        public string TagId { get; set; }

        public string DisplayText { get; set; }

        public string FullStatement { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string GroupCode { get; set; }

        public string Type { get; set; }

        public string ParentTagId { get; set; }

        public bool IsCollected { get; set; }

        public string Name { get; set; }

        public List<MetadataTag> Children { get; set; } = new List<MetadataTag>();

        public object Clone()
        {
            MetadataTag cloneObject = (MetadataTag)this.MemberwiseClone();
            cloneObject.Children = new List<MetadataTag>();
            foreach (var child in this.Children)
            {
                cloneObject.Children.Add((MetadataTag)child.Clone());
            }

            return cloneObject;
        }
    }
}
