using System.Collections.Generic;

namespace LearnerApp.Models
{
    public class ListResultDto<T>
    {
        public int TotalCount { get; set; }

        public List<T> Items { get; set; }
    }
}
