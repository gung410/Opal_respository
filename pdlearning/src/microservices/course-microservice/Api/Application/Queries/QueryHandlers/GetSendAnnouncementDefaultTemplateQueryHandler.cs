using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetSendAnnouncementDefaultTemplateQueryHandler : BaseQueryHandler<GetSendAnnouncementDefaultTemplateQuery, SendAnnouncementEmailTemplateModel>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly IReadOnlyRepository<CourseDepartment> _courseDepartmentRepository;
        private readonly GetRegisteredClassRunSlotSharedQuery _getRegisteredClassRunSlot;

        public GetSendAnnouncementDefaultTemplateQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<Session> readSessionRepository,
            IReadOnlyRepository<CourseDepartment> courseDepartmentRepository,
            GetRegisteredClassRunSlotSharedQuery getRegisteredClassRunSlot) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _readClassRunRepository = readClassRunRepository;
            _readSessionRepository = readSessionRepository;
            _courseDepartmentRepository = courseDepartmentRepository;
            _getRegisteredClassRunSlot = getRegisteredClassRunSlot;
        }

        protected override async Task<SendAnnouncementEmailTemplateModel> HandleAsync(
            GetSendAnnouncementDefaultTemplateQuery query,
            CancellationToken cancellationToken)
        {
            switch (query.AnnouncementType)
            {
                case AnnouncementType.CoursePublicity:
                    return await Task.FromResult(
                        new SendAnnouncementEmailTemplateModel
                        {
                            Id = Guid.NewGuid(),
                            Template = @"<p>Dear #USER_NAME#,</p>
                                            <p></p>
                                            <p>We would like to inform you that the following course has been published.</p>
                                            <p></p>
                                            <p>PD Activity Title: #COURSE_TITLE#</p>
                                            <p></p>
                                            <p>We find this course suitable for you, given your profile. Please take a look and consider whether it can contribute to your professional development.</p>
                                            <p></p>
                                            <p>Thank you.</p>
                                            <p></p>
                                            <p>Best regards,</p>
                                            <p>The OPAL2.0 team</p>",
                            UserNameTagValue = "#USER_NAME#",
                            CourseTitleTagValue = "#COURSE_TITLE#"
                        });
                case AnnouncementType.CourseNomination:
                    return await Task.FromResult(
                        new SendAnnouncementEmailTemplateModel
                        {
                            Id = Guid.NewGuid(),
                            Template = @"<p>Dear #USER_NAME#,</p>
                                            <p></p>
                                            <p>We would like to inform you that the following course has been published.</p>
                                            <p></p>
                                            <p>PD Activity Title: #COURSE_TITLE#</p>
                                            <p></p>
                                            <p>We believe that this course is very informative and will contribute to your staff's professional development. Please consider and nominate the course to your staff if possible.</p>
                                            <p></p>
                                            <p>Thank you.</p>
                                            <p></p>
                                            <p>Best regards,</p>
                                            <p>The OPAL2.0 team</p>",
                            UserNameTagValue = "#USER_NAME#",
                            CourseTitleTagValue = "#COURSE_TITLE#"
                        });
                case AnnouncementType.PlacementLetter:
                    return await Task.FromResult(
                        new SendAnnouncementEmailTemplateModel
                        {
                            Id = Guid.NewGuid(),
                            Template = @"<p>Dear #USER_NAME#,</p>
                                            <p></p>
                                            <p>We are pleased to confirm that you have been enrolled in the following PD Activity:</p>
                                            <p></p>
                                            <p>PD Activity Title: #COURSE_TITLE#</p>
                                            <p>PD Activity Code: #COURSE_CODE#</p>
                                            <p></p>
                                            <p>#LIST_SESSIONS#</p>
                                            <p></p>
                                            <p>If you have any queries, please contact #COURSE_ADMIN_NAME# at #COURSE_ADMIN_EMAIL#.</p>
                                            <p></p>
                                            <p>Note:</p>
                                            <p></p>
                                            <p>1.   If this is a face-to-face session, please bring along your PS card and retain the details in this e-mail (e.g. in your mobile device). If this is a virtual session, please take note of the login details that your course administrator will send to you separately. There is no need to print this placement letter.</p>
                                            <p>2.   If you are unable to attend any of the sessions, you are required to submit the reason(s) for your absence in OPAL2.0. </p>
                                            <p>3.   If you are unable to attend the PD Activity, please click #DETAIL_URL# to withdraw.</p>
                                            <p></p>
                                            <p>This is a system-generated notification. Please do not reply.</p>
                                            <p></p>
                                            <p>Thank you.</p>
                                            <p></p>
                                            <p>Best regards,</p>
                                            <p>The OPAL2.0 team</p>",
                            UserNameTagValue = "#USER_NAME#",
                            CourseTitleTagValue = "#COURSE_TITLE#",
                            CourseCodeTagValue = "#COURSE_CODE#",
                            CourseAdminNameTagValue = "#COURSE_ADMIN_NAME#",
                            CourseAdminEmailTagValue = "#COURSE_ADMIN_EMAIL#",
                            ListSessionTagValue = "#LIST_SESSIONS#",
                            DetailUrlTagValue = "#DETAIL_URL#"
                        });
                case AnnouncementType.OrderRefreshment:
                    return new SendAnnouncementEmailTemplateModel
                    {
                        Id = Guid.NewGuid(),
                        Template = await GetOrderRefreshmentEmailContent(query.CourseId)
                    };
                default:
                    return null;
            }
        }

        private async Task<string> GetOrderRefreshmentEmailContent(Guid? courseId)
        {
            var course = courseId.HasValue ? await _readCourseRepository.GetAsync(courseId.Value) : null;
            var divisionBranchText = await GetOrderRefreshmentEmailContent_DivisionBranchText(course);
            var classrunInfoTable = await GetOrderRefreshmentEmailContent_SessionInfoTable(course);

            return $@"<table>
                        <tr>
                            <th>Course Title/Event</th>
                            <td colspan=""3"">{course?.CourseName ?? "N/A"}</td>
                        </tr>
                        <tr>
                            <th>Course Code</th>
                            <td colspan=""3"">{course?.CourseCode ?? "N/A"}</td>
                        </tr>
                        <tr>
                            <th>Cost per pax</th>
                            <td colspan=""3"">$2.50</td>
                        </tr>
                        <tr>
                            <th rowspan=""2"">Contact Person(s)</th>
                            <td></td>
                            <th rowspan = ""2"">Contact No.</th>
                            <td></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <th>Division/Branch</th>
                            <td>{divisionBranchText}</td>
                            <th>Sub BU</th>
                            <td>MOE23</td>
                        </tr>
                     </table>
                     <p></p>
                     {classrunInfoTable}";
        }

        private async Task<string> GetOrderRefreshmentEmailContent_DivisionBranchText(CourseEntity course)
        {
            if (course == null)
            {
                return "N/A";
            }

            var ownerDivisionBranchIds = course.GetOwnerDivisionBranchIds();
            var divisionBranchNames = await _courseDepartmentRepository.GetAll()
                .Where(cd => ownerDivisionBranchIds.Contains(cd.DepartmentId))
                .Select(c => c.Name)
                .ToListAsync();

            return string.Join("/", divisionBranchNames);
        }

        /// <summary>
        /// Return a table of class runs of the course which is ApplicationEndDate has been ended for less than 2 weeks (14 days).
        /// </summary>
        /// <param name="course">The target course.</param>
        /// <returns>HTML class runs table.</returns>
        private async Task<string> GetOrderRefreshmentEmailContent_SessionInfoTable(CourseEntity course)
        {
            if (course == null)
            {
                return TableBuilderFn(TableRowBuilderFn(null, null, null));
            }
            else
            {
                var classRunsQuery = _readClassRunRepository
                    .GetAll()
                    .Where(c => c.CourseId == course.Id && c.ApplicationEndDate >= Clock.Now.AddDays(-14) && c.ApplicationEndDate < Clock.Now);

                var classRunWithSessions = await classRunsQuery
                    .GroupJoin(_readSessionRepository.GetAll(), c => c.Id, s => s.ClassRunId, (classrun, session) => new { classrun, session })
                    .SelectMany(s => s.session.DefaultIfEmpty(), (gj, session) => new { Classrun = gj.classrun, Session = session })
                    .ToListAsync();
                var totalParticipantsPerClassruns =
                    await _getRegisteredClassRunSlot.CountByClassRunQuery(classRunsQuery, countParticipantOnly: true);

                var sessionTableRows = string.Join(" ", classRunWithSessions.Select(item => TableRowBuilderFn(item.Classrun, item.Session, totalParticipantsPerClassruns)));
                return TableBuilderFn(sessionTableRows);
            }

            string TableBuilderFn(string sessionTableRows) =>
                $@"
                <table>
                    <tr>
                        <th>Class run</th>
                        <th>Session</th>
                        <th>Date of Session</th>
                        <th>Delivery Time</th>
                        <th>Venue</th>
                        <th>No. of Pax</th>
                        <th>Choice of Menu (if applicable)</th>
                    </tr>
                    {sessionTableRows}
                </table>";

            string TableRowBuilderFn(ClassRun classrun, Session session, Dictionary<Guid, int> totalParticipantsPerClassruns) =>
                $@"
                <tr>
                    <td>{classrun?.ClassTitle ?? "N/A"}</td>
                    <td>{(session != null ? session.SessionTitle : string.Empty)}</td>
                    <td>{(session != null ? (TimeHelper.ConvertTimeFromUtc(session.StartDateTime.Value).ToString(DateTimeFormatConstant.DateWithTime) + " - " + TimeHelper.ConvertTimeFromUtc(session.EndDateTime.Value).ToString(DateTimeFormatConstant.DateWithTime)) : string.Empty)}</td>
                    <td></td>
                    <td></td>
                    <td>{(classrun != null && totalParticipantsPerClassruns != null ? totalParticipantsPerClassruns[classrun.Id].ToString() : "N/A")}</td>
                    <td>According to menu provided for the day.</td>
                </tr>
                ";
        }
    }
}
