using System;

namespace cxOrganization.Business.PDPlanner.EmployeeList
{
    public class StatusTypeInfo: IComparable, IComparable<StatusTypeInfo>
    {
        public int? AssessmentStatusId { get; set; }
        public string AssessmentStatusCode { get; set; }
        public string AssessmentStatusName { get; set; }
        public string AssessmentStatusDescription { get; set; }
        public int No { get; set; }
        public virtual int CompareTo(object obj)
        {
            var other = obj as StatusTypeInfo;
            return this.CompareTo(other);
        }
        public int CompareTo(StatusTypeInfo other)
        {
            if (this.Equals(other)) return 0;
            if (other == null) return 1;
            var compareByNumber = No.CompareTo(other.No);
            if (compareByNumber == 0)
            {
                //If two status type same number order we compare by name
                return string.Compare(AssessmentStatusName,
                    other.AssessmentStatusName, StringComparison.CurrentCultureIgnoreCase);
            }
            return compareByNumber;
        }
    }
}
