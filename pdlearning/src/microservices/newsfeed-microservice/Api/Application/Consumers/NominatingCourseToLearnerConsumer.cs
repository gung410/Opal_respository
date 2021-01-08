using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.NewsFeed.Application.Consumers.Messages.NominatingCourseToLearner;
using Microservice.NewsFeed.Domain.Entities;
using Microservice.NewsFeed.Infrastructure;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Microservice.NewsFeed.Application.Consumers
{
    [OpalConsumer("cx-competence-api.pdplan.create.actionitem")]
    public class NominatingCourseToLearnerConsumer : OpalMessageConsumer<NominatingCourseToLearnerMessage>
    {
        private readonly ILogger<NominatingCourseToLearnerConsumer> _logger;
        private readonly NewsFeedDbContext _dbContext;

        public NominatingCourseToLearnerConsumer(
            ILogger<NominatingCourseToLearnerConsumer> logger,
            NewsFeedDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(NominatingCourseToLearnerMessage message)
        {
            if (!message.Result.IsCourseRecommended())
            {
                return;
            }

            Guid courseId = message.Result.AdditionalProperties.CourseId;
            Guid learnerId = message.Result.ObjectiveInfo.Identity.ExtId;

            // Insert to course subscription
            await InsertIntoCourseSubscription(courseId, learnerId);
        }

        private async Task InsertIntoCourseSubscription(Guid courseId, Guid learnerId)
        {
            var existedSubscription = await _dbContext.LearnerCourseSubscriptionCollection
                .AsQueryable()
                .Where(_ => _.CourseId == courseId)
                .FirstOrDefaultAsync();

            var subscription = new LearnerCourseSubscription();
            if (existedSubscription == null)
            {
                subscription.CourseId = courseId;
                subscription.LearnerIds = new List<Guid>
                {
                    learnerId
                };

                await _dbContext.LearnerCourseSubscriptionCollection.InsertOneAsync(subscription);
                return;
            }

            subscription = existedSubscription;
            subscription.LearnerIds.Add(learnerId);

            await _dbContext.LearnerCourseSubscriptionCollection.ReplaceOneAsync(
                _ => _.CourseId == courseId,
                subscription,
                new ReplaceOptions { IsUpsert = true });
        }
    }
}
