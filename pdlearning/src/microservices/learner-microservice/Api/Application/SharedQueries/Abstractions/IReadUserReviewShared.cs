using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Learner.Application.Common;

namespace Microservice.Learner.Application.SharedQueries.Abstractions
{
    public interface IReadUserReviewShared : ISharedQuery
    {
        /// <summary>
        /// Get user reviews for courses or digital-contents. </summary>
        /// <param name="itemIds">The list of identifier that could be course id or digital-content id.</param>
        /// <returns>A dictionary that with key is course id or digital-content id,
        /// and Value is review summary of each course/digital-content.
        /// </returns>
        Task<Dictionary<Guid, ReviewSummary>> GetReviewSummary(List<Guid> itemIds);
    }
}
