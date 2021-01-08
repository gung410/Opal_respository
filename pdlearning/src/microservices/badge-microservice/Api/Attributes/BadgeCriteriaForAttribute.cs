using System;

namespace Microservice.Badge.Attributes
{
    /// <summary>
    /// Indicate badge id of the criteria.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BadgeCriteriaForAttribute : Attribute
    {
        public BadgeCriteriaForAttribute(string badgeIdStr)
        {
            BadgeIdStr = badgeIdStr;
        }

        public string BadgeIdStr { get; }

        public Guid BadgeId => new(BadgeIdStr);
    }
}
