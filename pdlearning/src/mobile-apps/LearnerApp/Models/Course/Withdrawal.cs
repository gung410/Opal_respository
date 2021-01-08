using System.Collections.Generic;

namespace LearnerApp.Models.Course
{
    public class Withdrawal
    {
        public List<string> Ids { get; set; }

        public string WithdrawalStatus { get; set; }

        public string Comment { get; set; }
    }
}
