using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.Interfaces;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Validation;
using Thunder.Service.Authentication;

namespace Microservice.Course.Domain.Entities
{
    public class BlockoutDate : FullAuditedEntity, ISoftDelete, IFullTextSearchable
    {
        private IEnumerable<string> _serviceSchemes = new List<string>();

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        public int StartDay { get; set; }

        public int StartMonth { get; set; }

        public int EndDay { get; set; }

        public int EndMonth { get; set; }

        public int ValidYear { get; set; }

        public DateTime StartDate
        {
            get => new DateTime(ValidYear, StartMonth, StartDay);
            private set { }
        }

        public DateTime EndDate
        {
            get => new DateTime(ValidYear, EndMonth, EndDay);
            private set { }
        }

        public Guid PlanningCycleId { get; set; }

        public bool IsConfirmed { get; set; }

        public BlockoutDateStatus Status { get; set; } = BlockoutDateStatus.Draft;

        [JsonIgnore]
        public virtual CoursePlanningCycle PlanningCycle { get; set; }

        public IEnumerable<string> ServiceSchemes
        {
            get => _serviceSchemes ??= new List<string>();
            set => _serviceSchemes = value;
        }

        /// <summary>
        /// This column to support filter equivalent to ServiceSchemes.Contain([some user id]) by using full-text search.
        /// </summary>
        public string ServiceSchemesFullTextSearch
        {
            get => ServiceSchemes != null ? JsonSerializer.Serialize(ServiceSchemes) : null;
            set { }
        }

        /// <summary>
        /// This column is a unique full-text search key column. It also support OrderBy. For now we order by CreatedDate.
        /// </summary>
        public string FullTextSearchKey
        {
            get => $"{ValidYear:0000}/{StartMonth:00}/{StartDay:00}_{Id.ToString().ToUpperInvariant()}";
            set { }
        }

        public string FullTextSearch
        {
            get => $"{Title ?? string.Empty}  {Description ?? string.Empty}";
            set { }
        }

        public static Expression<Func<BlockoutDate, bool>> IsMatchServiceSchemesExpr(List<string> serviceSchemeIds)
        {
            return p => p.ServiceSchemes.Any() && serviceSchemeIds.Any() &&
                        p.ServiceSchemes.ContainsAll(serviceSchemeIds.Select(serviceSchemeId => serviceSchemeId.ToString()).ToArray());
        }

        public static Expression<Func<BlockoutDate, bool>> IsMatchDateExpr(DateTime? date)
        {
            return p => date.HasValue
                        && date.Value.Year == p.ValidYear
                        && ((date.Value.Month == p.EndMonth && date.Value.Day <= p.EndDay) || date.Value.Month < p.EndMonth)
                        && ((date.Value.Month == p.StartMonth && date.Value.Day >= p.StartDay) || date.Value.Month > p.StartMonth);
        }

        public static Expression<Func<BlockoutDate, bool>> IsMatchDateRangeExpr(DateTime fromDate, DateTime toDate)
        {
            return p =>
                fromDate.Year == p.ValidYear && ((fromDate.Month == p.EndMonth && fromDate.Day <= p.EndDay) || fromDate.Month < p.EndMonth) &&
                toDate.Year == p.ValidYear && ((toDate.Month == p.StartMonth && toDate.Day >= p.StartDay) || toDate.Month > p.StartMonth);
        }

        public static bool HasCrudPermission(Guid? userId, List<string> userRoles)
        {
            return userId == null || UserRoles.IsSysAdministrator(userRoles) || userRoles.Any(r => r == UserRoles.CoursePlanningCoordinator);
        }

        public bool IsMatchServiceSchemes(List<string> serviceSchemeIds)
        {
            return IsMatchServiceSchemesExpr(serviceSchemeIds).Compile()(this);
        }

        public bool IsMatchDate(DateTime? date)
        {
            return IsMatchDateExpr(date).Compile()(this);
        }

        public Validation ValidateCanBeDeleted()
        {
            return Validation.ValidIf(IsConfirmed == false, "You can not delete this blockout date because it is already confirmed.");
        }
    }
}
