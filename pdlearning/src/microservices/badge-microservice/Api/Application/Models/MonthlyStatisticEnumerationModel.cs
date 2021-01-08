using System;
using Microservice.Badge.Domain.ValueObjects;

namespace Microservice.Badge.Application.Models
{
    public class MonthlyStatisticEnumerationModel
    {
        /// <summary>
        /// Map to MongoDB _id.
        /// </summary>
#pragma warning disable SA1300 // Element should begin with upper-case letter
        public object _id { get; set; }
#pragma warning restore SA1300 // Element should begin with upper-case letter

        /// <summary>
        ///  Map to MongoDB property.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///  Map to MongoDB property.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        ///  Map to MongoDB property.
        /// </summary>
        public GeneralStatistic DailyStatistics { get; set; }

        /// <summary>
        ///  Map to MongoDB property.
        /// </summary>
        public GeneralStatistic MonthlyStatistics { get; set; }

        /// <summary>
        ///  Map to MongoDB property.
        /// </summary>
        public object YearlyStatistic { get; set; }
    }
}
