using System.Collections.Generic;

namespace LearnerApp.Models
{
    public class ListCSLResultDto<T>
    {
        public int Total { get; set; }

        public int Page { get; set; }

        public int Pages { get; set; }

        public List<T> Results { get; set; }
    }
}
