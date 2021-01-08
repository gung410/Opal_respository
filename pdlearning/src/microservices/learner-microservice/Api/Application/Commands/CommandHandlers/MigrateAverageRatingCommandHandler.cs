using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Events;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class MigrateAverageRatingCommandHandler : BaseCommandHandler<MigrateAverageRatingCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<UserReview> _userReviewRepository;

        public MigrateAverageRatingCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IThunderCqrs thunderCqrs,
            IRepository<UserReview> userReviewRepository) : base(unitOfWorkManager, userContext)
        {
            _thunderCqrs = thunderCqrs;
            _userReviewRepository = userReviewRepository;
        }

        protected override async Task HandleAsync(MigrateAverageRatingCommand command, CancellationToken cancellationToken)
        {
            if (!UserContext.IsSysAdministrator())
            {
                throw new UnauthorizedAccessException("The user must be in role SysAdministrator!");
            }

            var batchSize = command.BatchSize;
            var itemType = ItemType.DigitalContent;
            for (int i = 0; i < command.DigitalContentIds.Count; i += command.BatchSize)
            {
                var digitalContentIds = command.DigitalContentIds.Take(batchSize).Skip(i).ToList();
                var userReviews = await _userReviewRepository
                    .GetAll()
                    .Where(p => digitalContentIds.Contains(p.ItemId) && p.ItemType == itemType && p.Rate.HasValue)
                    .Select(p => new { Rating = p.Rate.Value, p.ItemId })
                    .ToListAsync(cancellationToken);

                batchSize += command.BatchSize;

                foreach (var digitalContentId in digitalContentIds)
                {
                    var rateReviews = userReviews.Where(p => p.ItemId == digitalContentId).Select(p => p.Rating).ToList();
                    await _thunderCqrs.SendEvent(
                        new AverageReviewRatingChangedEvent(
                            CreateAverageReviewRating(rateReviews, digitalContentId, itemType),
                            AverageReviewRatingType.Updated),
                        cancellationToken);
                }
            }
        }

        private AverageReviewRatingModel CreateAverageReviewRating(List<double> rateReviews, Guid itemId, ItemType itemType)
        {
            return new AverageReviewRatingModel
            {
                ItemId = itemId,
                ItemType = itemType,
                AverageRating = rateReviews.Any() ? Math.Round(rateReviews.Average(), 1) : 0,
                ReviewsCount = rateReviews.Count
            };
        }
    }
}
