using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Models
{
    public class BlockoutDateModel
    {
        public BlockoutDateModel()
        {
        }

        public BlockoutDateModel(BlockoutDate entity)
        {
            Id = entity.Id;
            CreatedBy = entity.CreatedBy;
            StartDay = entity.StartDay;
            StartMonth = entity.StartMonth;
            EndDay = entity.EndDay;
            EndMonth = entity.EndMonth;
            PlanningCycleId = entity.PlanningCycleId;
            ValidYear = entity.ValidYear;
            ServiceSchemes = entity.ServiceSchemes;
            Title = entity.Title;
            Description = entity.Description;
            Status = entity.Status;
            IsConfirmed = entity.IsConfirmed;
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public BlockoutDateStatus Status { get; set; }

        public Guid Id { get; set; }

        public Guid? CreatedBy { get; set; }

        public int StartDay { get; set; }

        public int StartMonth { get; set; }

        public int EndDay { get; set; }

        public int EndMonth { get; set; }

        public int ValidYear { get; set; }

        public Guid PlanningCycleId { get; set; }

        public bool IsConfirmed { get; set; }

        public IEnumerable<string> ServiceSchemes { get; set; } = new List<string>();
    }
}
