using System;
using System.Collections.Generic;
using Microservice.Badge.Domain.Enums;

namespace Microservice.Badge.Application.Consumers.Dtos
{
    public class EPortfolioDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid EPortfolioTypeId { get; set; }

        public string Url { get; set; }

        public EPortfolioVisibility Visibility { get; set; }

        public EPortfolioTypeDto EPortfolioType { get; set; }

        public string EPortfolioTypeName { get; set; }

        public string Title { get; set; }

        public string About { get; set; }

        public string Interest { get; set; }

        public string Achievement { get; set; }

        public int No { get; set; }

        public int ShowcaseItems { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public List<ReflectionDto> Reflections { get; set; }

        public bool IsExperience() => this.EPortfolioType?.Url == "myexperience";

        /// <summary>
        /// Get activity type based on type of eportfolio.
        /// </summary>
        /// <returns>ActivityType.</returns>
        public ActivityType GetActivityType() => this.IsExperience() ? ActivityType.CreateReflection : ActivityType.CreateSharedReflection;
    }
}
