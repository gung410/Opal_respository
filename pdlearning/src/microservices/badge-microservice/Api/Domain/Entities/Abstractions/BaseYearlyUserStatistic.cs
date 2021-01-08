using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Badge.Domain.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;
using Thunder.Platform.Core.Extensions;
using GuidGenerator = MongoDB.Bson.Serialization.IdGenerators.GuidGenerator;

namespace Microservice.Badge.Domain.Entities
{
    /// <summary>
    /// This entity defines activity statistic of users each year.
    /// It also contains detail statistic of user each month, each day for ensuring ability to track data for analytic.
    /// </summary>
    /// <typeparam name="T">Statistic type.</typeparam>
    public abstract class BaseYearlyUserStatistic<T> where T : BaseStatistic
    {
        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public int Year { get; init; }

        public T YearlyStatistic { get; private set; }

        public T LatestDailyStatistic { get; private set; }

        public T LatestMonthlyStatistics { get; private set; }

        /// <summary>
        /// We will have a recurring job which run daily to scan UserActivity in previous 24h and insert new 1 item.
        /// The first item will be the latest statistical data of the user that summarize day by day.
        /// </summary>
        public IEnumerable<T> DailyStatistics { get; private set; } = new List<T>();

        /// <summary>
        /// We will have a recurring job which run monthly to scan UserActivity in a previous month and insert new 1 item.
        /// The first item will be the latest statistical data of the user that summarizes monthly.
        /// </summary>
        public IEnumerable<T> MonthlyStatistics { get; private set; } = new List<T>();

        /// <summary>
        /// Insert or Update a daily statistic record to <see cref="DailyStatistics"/>.
        /// Make sure the order has not change.
        /// </summary>
        /// <param name="dailyStatistic">daily statistic data.</param>
        /// <returns>Yearly statistic data.</returns>
        public BaseYearlyUserStatistic<T> SetDailyStatistic(T dailyStatistic)
        {
            var dailyStatistics = DailyStatistics.ToList();
            var index = dailyStatistics.FindIndex(x =>
                dailyStatistic.ExecutedDate.StartOfDateInSystemTimeZone().ToUtcFromSystemTimeZone() <= x.ExecutedDate
                && x.ExecutedDate <= dailyStatistic.ExecutedDate.EndOfDateInSystemTimeZone().ToUtcFromSystemTimeZone());

            if (index < 0)
            {
                dailyStatistics.Insert(0, dailyStatistic);
            }
            else
            {
                dailyStatistics[index] = dailyStatistic;
            }

            DailyStatistics = dailyStatistics;
            return this;
        }

        /// <summary>
        /// Insert or Update a monthly statistic record to <see cref="MonthlyStatistics"/>.
        /// Make sure the order has not change.
        /// </summary>
        /// <param name="monthlyStatistic">monthly statistic data.</param>
        /// <returns>Yearly statistic data.</returns>
        public BaseYearlyUserStatistic<T> SetMonthlyStatistic(T monthlyStatistic)
        {
            var monthlyStatistics = MonthlyStatistics.ToList();
            var index = monthlyStatistics.FindIndex(x =>
                monthlyStatistic.ExecutedDate.Month == x.ExecutedDate.Month);

            if (index < 0)
            {
                monthlyStatistics.Insert(0, monthlyStatistic);
            }
            else
            {
                monthlyStatistics[index] = monthlyStatistic;
            }

            MonthlyStatistics = monthlyStatistics;
            return this;
        }

        /// <summary>
        /// Insert or Update a monthly statistic record to <see cref="MonthlyStatistics"/>.
        /// Make sure the order has not change.
        /// </summary>
        /// <param name="yearlyStatistic">monthly statistic data.</param>
        /// <returns>Yearly statistic data.</returns>
        public BaseYearlyUserStatistic<T> SetYearlyStatistic(T yearlyStatistic)
        {
            if (YearlyStatistic.ExecutedDate.Year == yearlyStatistic.ExecutedDate.Year)
            {
                YearlyStatistic = yearlyStatistic;
            }

            return this;
        }
    }
}
