namespace cxOrganization.Business.CandidateList
{
    public class CompletenessRange
    {

        /// <summary>
        /// The value which the completeness should be greater or equal
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// The value which the completeness should be less or equal
        /// </summary>
        public int MaxValue { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}", MinValue, MaxValue);
        }

    }
}
