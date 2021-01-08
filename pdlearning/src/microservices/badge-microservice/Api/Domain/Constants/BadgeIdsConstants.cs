using System;

namespace Microservice.Badge.Domain.Constants
{
    public static class BadgeIdsConstants
    {
        public const string CollaborativeLearnersBadgeIdStr = "b747e01e-2c53-49c0-8787-1f1018c6462c";

        public const string ReflectiveLearnersBadgeIdStr = "02ae2b78-4434-11eb-b378-0242ac130002";

        public const string DigitalLearnersBadgeIdStr = "449ead06-86c6-4310-8faa-32f34bb536cf";

        public const string ActiveContributorBadgeIdStr = "da247ab1-5a7a-4486-81d1-913bc33c37c7";

        public const string LifeLongBadgeIdStr = "3d58e626-517d-4e7d-9e57-f79689d87ac7";

        public const string LinkCuratorBadgeIdStr = "e8175f1c-ce15-46a3-b525-4923d055efea";

        public const string ConversationStarterBadgeIdStr = "fdd4d905-d876-4d0c-9403-a7e0df6843b9";

        public const string ConversationBoosterBadgeIdStr = "d2f6ebb9-e6c8-4857-97c7-b2226117b872";

        public const string VisualStorytellerBadgeIdStr = "57cdfb67-b1f6-4398-81ab-c3aafa2cde08";

        public static readonly Guid _collaborativeLearnersBadgeId = new(CollaborativeLearnersBadgeIdStr);

        public static readonly Guid _reflectiveLearnersBadgeId = new(ReflectiveLearnersBadgeIdStr);

        public static readonly Guid _digitalLearnersBadgeId = new(DigitalLearnersBadgeIdStr);

        public static readonly Guid _activeContributorBadgeId = new(ActiveContributorBadgeIdStr);

        public static readonly Guid _lifeLongBadgeId = new(LifeLongBadgeIdStr);

        public static readonly Guid _conversationStarterBadgeId = new(ConversationStarterBadgeIdStr);

        public static readonly Guid _conversationBoosterBadgeId = new(ConversationBoosterBadgeIdStr);

        public static readonly Guid _visualStorytellerBadgeId = new(VisualStorytellerBadgeIdStr);

        public static readonly Guid _linkCuratorBadgeId = new(LinkCuratorBadgeIdStr);
    }
}
