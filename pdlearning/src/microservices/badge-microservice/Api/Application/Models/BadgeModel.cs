using System;
using System.Collections.Generic;
using Microservice.Badge.Domain.Enums;

namespace Microservice.Badge.Application.Models
{
    public class BadgeModel
    {
        public Guid Id { get; init; }

        public BadgeType Type { get; init; }

        public string Name { get; init; }

        public string TagImage { get; init; }

        /// <summary>
        /// List of level with relative path to S3 file.
        /// Eg: { Level1 : "/permanent/digital-badging/community-builder-lv1.png." }.
        /// </summary>
        public Dictionary<BadgeLevelEnum, string> LevelImages { get; init; }
    }
}
