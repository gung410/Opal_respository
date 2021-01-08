using System;

namespace cxPlatform.Client.ConexusBase
{
    public class PeriodDto
    {
        /// <summary>
        /// The name of the period 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The start date of the period 
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// The end date of the period 
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}
