using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class GetBlockoutDateDependenciesModel
    {
        public IEnumerable<BlockoutDateModel> MatchedBlockoutDates { get; set; } = new List<BlockoutDateModel>();

        /// <summary>
        /// This will be not null and have items if FromDate request is not null and existed blockout dates matched with the ToDate.
        /// </summary>
        public IEnumerable<BlockoutDateModel> MatchedFromDateBlockoutDates { get; set; } = new List<BlockoutDateModel>();

        /// <summary>
        /// This will be not null and have items if ToDate request is not null and existed blockout dates matched with the ToDate.
        /// </summary>
        public IEnumerable<BlockoutDateModel> MatchedToDateBlockoutDates { get; set; } = new List<BlockoutDateModel>();

        public static GetBlockoutDateDependenciesModel Create(List<BlockoutDate> matchedBlockoutDates, DateTime? fromDate, DateTime? toDate)
        {
            return new GetBlockoutDateDependenciesModel()
            {
                MatchedBlockoutDates = matchedBlockoutDates.Select(p => new BlockoutDateModel(p)),
                MatchedFromDateBlockoutDates = matchedBlockoutDates.Where(p => p.IsMatchDate(fromDate)).Select(p => new BlockoutDateModel(p)),
                MatchedToDateBlockoutDates = matchedBlockoutDates.Where(p => p.IsMatchDate(toDate)).Select(p => new BlockoutDateModel(p))
            };
        }
    }
}
