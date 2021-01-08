using System.Collections.Generic;
using System.Linq;
using Microservice.Badge.Domain.Constants;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Domain.ValueObjects;
using MongoDB.Driver;

namespace Microservice.Badge.Infrastructure
{
    public class BadgeSeeder
    {
        private readonly BadgeDbContext _badgeDbContext;

        public BadgeSeeder(BadgeDbContext badgeDbContext)
        {
            _badgeDbContext = badgeDbContext;
        }

        public void Seed()
        {
            var badgeDefinitions = new List<BadgeWithCriteria<BaseBadgeCriteria>>
            {
                new(BadgeIdsConstants._collaborativeLearnersBadgeId, BadgeType.Level)
                {
                    Name = "Collaborative Learner Badge",
                    LevelImages = new Dictionary<BadgeLevelEnum, string>
                    {
                        { BadgeLevelEnum.Level1, "/permanent/digital-badging/community-builder-lv1.png" },
                        { BadgeLevelEnum.Level2, "/permanent/digital-badging/community-builder-lv2.png" },
                        { BadgeLevelEnum.Level3, "/permanent/digital-badging/community-builder-lv3.png" },
                    },
                    SeedVersion = 1,
                    Criteria = new CollaborativeLearnersBadgeCriteria
                    {
                        SumOfFollow = 5,
                        SumOfForward = 5,
                        SumOfPostAndLike = 5,
                        SumOfPostsResponded = 5,
                        Total = 20,
                        Limitation = RewardBadgeLimitation.NewPercentBadgeLimitation(5)
                    }
                },
                new(BadgeIdsConstants._reflectiveLearnersBadgeId, BadgeType.Level)
                {
                    Name = "Reflective Learner Badge",
                    LevelImages = new Dictionary<BadgeLevelEnum, string>
                    {
                        { BadgeLevelEnum.Level1, "/permanent/digital-badging/reflective-learner-lv1.png" },
                        { BadgeLevelEnum.Level2, "/permanent/digital-badging/reflective-learner-lv2.png" },
                        { BadgeLevelEnum.Level3, "/permanent/digital-badging/reflective-learner-lv3.png" },
                    },
                    SeedVersion = 1,
                    Criteria = new ReflectiveLearnersBadgeCriteria
                    {
                        SumOfReflection = 5,
                        SumOfSharedReflection = 5,
                        Limitation = RewardBadgeLimitation.NewPercentBadgeLimitation(5)
                    }
                },
                new(BadgeIdsConstants._digitalLearnersBadgeId, BadgeType.Level)
                {
                    Name = "Digital Learner Badge",
                    LevelImages = new Dictionary<BadgeLevelEnum, string>
                    {
                        { BadgeLevelEnum.Level1, "/permanent/digital-badging/community-builder-lv1.png" },
                        { BadgeLevelEnum.Level2, "/permanent/digital-badging/community-builder-lv2.png" },
                        { BadgeLevelEnum.Level3, "/permanent/digital-badging/community-builder-lv3.png" },
                    },
                    SeedVersion = 1,
                    Criteria = new DigitalLearnersBadgeCriteria
                    {
                        NumOfCompletedElearning = 10,
                        NumOfCompletedDigitalResources = 10,
                        NumOfCompletedMLU = 10,
                        Limitation = RewardBadgeLimitation.NewPercentBadgeLimitation(5)
                    }
                },
                new(BadgeIdsConstants._lifeLongBadgeId, BadgeType.Level)
                {
                    Name = "Life Long Badge",
                    LevelImages = new Dictionary<BadgeLevelEnum, string>
                    {
                        { BadgeLevelEnum.Level1, "/permanent/digital-badging/community-builder-lv1.png" },
                        { BadgeLevelEnum.Level2, "/permanent/digital-badging/community-builder-lv2.png" },
                        { BadgeLevelEnum.Level3, "/permanent/digital-badging/community-builder-lv3.png" },
                    },
                    SeedVersion = 1,
                    Criteria = new LifeLongBadgeCriteria
                    {
                        Limitation = RewardBadgeLimitation.NewCombinedBadgeLimitation(5, 50)
                    }
                },
                new(BadgeIdsConstants._linkCuratorBadgeId, BadgeType.Tag)
                {
                    Name = "Link Curator Badge",
                    TagImage = "/permanent/digital-badging/link-curator.png",
                    SeedVersion = 1,
                    Criteria = new LinkCuratorBadgeCriteria
                    {
                        NumOfQualifiedLinkCuratorPost = 3,
                        NumOfResponse = 10,
                        Limitation = RewardBadgeLimitation.NewPercentBadgeLimitation(5)
                    }
                },
                new(BadgeIdsConstants._visualStorytellerBadgeId, BadgeType.Tag)
                {
                    Name = "Visual Storyteller Badge",
                    TagImage = "/permanent/digital-badging/visual-story-teller.png",
                    SeedVersion = 1,
                    Criteria = new VisualStorytellerBadgeCriteria
                    {
                        NumOfQualifiedVisualPost = 1,
                        NumOfMultimedia = 3,
                        NumOfResponse = 10,
                        Limitation = RewardBadgeLimitation.NewPercentBadgeLimitation(5)
                    }
                },
                new(BadgeIdsConstants._conversationStarterBadgeId, BadgeType.Tag)
                {
                    Name = "Conversation Starter Badge",
                    TagImage = "/permanent/digital-badging/conversation-starter.png",
                    SeedVersion = 1,
                    Criteria = new ConversationStarterBadgeCriteria
                    {
                        NumOfQualifiedPost = 3,
                        NumOfResponse = 10,
                        Limitation = RewardBadgeLimitation.NewPercentBadgeLimitation(5)
                    }
                },
                new(BadgeIdsConstants._conversationBoosterBadgeId, BadgeType.Tag)
                {
                    Name = "Conversation Booster Badge",
                    TagImage = "/permanent/digital-badging/conversation-booster.png",
                    SeedVersion = 1,
                    Criteria = new ConversationBoosterBadgeCriteria
                    {
                        Limitation = RewardBadgeLimitation.NewPercentBadgeLimitation(5)
                    }
                },
                new(BadgeIdsConstants._activeContributorBadgeId, BadgeType.Level)
                {
                    Name = "Active Contributor Badge",
                    LevelImages = new Dictionary<BadgeLevelEnum, string>
                    {
                        { BadgeLevelEnum.Level1, "/permanent/digital-badging/community-builder-lv1.png" },
                        { BadgeLevelEnum.Level2, "/permanent/digital-badging/community-builder-lv2.png" },
                        { BadgeLevelEnum.Level3, "/permanent/digital-badging/community-builder-lv3.png" },
                    },
                    SeedVersion = 1,
                    Criteria = new ActiveContributorsBadgeCriteria
                    {
                        Limitation = RewardBadgeLimitation.NewMaximumPeopleBadgeLimitation(100, 50)
                    }
                }
            };

            var existedBadges = _badgeDbContext
                .BadgeCollection
                .AsQueryable()
                .Select(b => new
                {
                    b.Id,
                    b.SeedVersion
                })
                .ToList();

            var newBadges = new List<BadgeEntity>();

            foreach (var badgeDefinition in badgeDefinitions)
            {
                var existedBadge = existedBadges.Find(b => b.Id == badgeDefinition.Id);
                if (existedBadge == null)
                {
                    newBadges.Add(badgeDefinition);
                    continue;
                }

                // Update badge in case the badge was existed but diff seed version.
                if (existedBadge.SeedVersion != badgeDefinition.SeedVersion)
                {
                    _badgeDbContext.BadgeCollection.ReplaceOne(
                        Builders<BadgeEntity>.Filter.Eq(b => b.Id, badgeDefinition.Id),
                        badgeDefinition);
                }
            }

            if (newBadges.Count > 0)
            {
                _badgeDbContext.BadgeCollection.InsertMany(newBadges);
            }
        }
    }
}
