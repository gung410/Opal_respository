using System;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Metadata.DataSync;
using Microservice.Course.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Course.Infrastructure
{
    public class CourseSeeder : IDbContextSeeder
    {
        private readonly IServiceProvider _serviceProvider;

        public CourseSeeder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Seed<TDbContext>(TDbContext dbContext) where TDbContext : BaseThunderDbContext
        {
            _ = SyncMetaData(dbContext);

            /*
            var courseId = new Guid("E37B4DBF-9F0A-4272-A788-2F3CC5A15A7A");
            var existingForm = context.Course.IgnoreQueryFilters().FirstOrDefault(p => p.Id == courseId);
            if (existingForm != null)
            {
                return;
            }

            context.Course.AddRange(new List<Domain.Entities.Course>
            {
                new Domain.Entities.Course { Id = new Guid("E37B4DBF-9F0A-4272-A788-2F3CC5A15A7A"), CourseName = "Learn AI", Description = "Learn AI", Status = CourseStatus.Published, CreatedDate = Clock.Now.AddDays(-3), DurationMinutes = 195, CourseCode = "SCI-000211-00" },
                new Domain.Entities.Course { Id = new Guid("E37B4DBF-9F0A-4272-A788-2F3CC5A15A8B"), CourseName = "Test Course", Description = "Test Course", Status = CourseStatus.Published, CreatedDate = Clock.Now.AddDays(-3), DurationMinutes = 195, CourseCode = "SCI-000211-02" },
                new Domain.Entities.Course { Id = new Guid("6c901d54-cb20-4367-b4bc-5f687f4cb062"), CourseName = "Test Course 1", Description = "Test Course 1", Status = CourseStatus.Published, CreatedDate = Clock.Now.AddDays(-3), DurationMinutes = 343, CourseCode = "SCI-000211-02" },
                new Domain.Entities.Course { Id = new Guid("b7414da4-9a89-4ebb-b0af-12d6a3c09984"), CourseName = "Test Course 2", Description = "Test Course 2", Status = CourseStatus.Published, CreatedDate = Clock.Now.AddDays(-3), DurationMinutes = 245, CourseCode = "SCI-000212-03" },
                new Domain.Entities.Course { Id = new Guid("f6465145-a361-40bb-9bf5-e41a0d335213"), CourseName = "Test Course 3", Description = "Test Course 3", Status = CourseStatus.Published, CreatedDate = Clock.Now.AddDays(-3), DurationMinutes = 155, CourseCode = "SCI-000212-04" },
                new Domain.Entities.Course { Id = new Guid("63689f4a-9c10-48e2-be23-fe5509a9661e"), CourseName = "测试课程 4", Description = "测试课程 4", Status = CourseStatus.Published, CreatedDate = Clock.Now.AddDays(-3), DurationMinutes = 125, CourseCode = "SCI-000111-02" },
                new Domain.Entities.Course { Id = new Guid("32871bc6-c7cb-4c89-8c4a-a296c5d80550"), CourseName = "Test Course 5", Description = "Test Course 5", Status = CourseStatus.Published, CreatedDate = Clock.Now.AddDays(-3), DurationMinutes = 695, CourseCode = "SCI-000123-02" },
                new Domain.Entities.Course { Id = new Guid("b05179a8-1324-4739-a53f-ddc16c413210"), CourseName = "测试课程 6", Description = "测试课程 6", Status = CourseStatus.Published, CreatedDate = Clock.Now.AddDays(-3), DurationMinutes = 193, CourseCode = "SCI-000211-02" },
                new Domain.Entities.Course { Id = new Guid("6d81d845-cd95-4249-a442-c5f2d7aa5ee0"), CourseName = "Test Course 7", Description = "Test Course 7", Status = CourseStatus.Published, CreatedDate = Clock.Now.AddDays(-3), DurationMinutes = 597, CourseCode = "SCI-000123-01" },
                new Domain.Entities.Course { Id = new Guid("af163d14-1274-496f-b81e-a74d5cf0c1e1"), CourseName = "Test Course 8", Description = "Test Course 8", Status = CourseStatus.Published, CreatedDate = Clock.Now.AddDays(-3), DurationMinutes = 115, CourseCode = "SCI-000123-02" },
                new Domain.Entities.Course { Id = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), CourseName = "Test Course 9", Description = "Test Course 9", Status = CourseStatus.Published, CreatedDate = Clock.Now.AddDays(-3), DurationMinutes = 125, CourseCode = "SCI-000123-03" },
                new Domain.Entities.Course { Id = new Guid("d75a1573-3760-4aef-bac7-8250271d430b"), CourseName = "Test Course 10", Description = "Test Course 10", Status = CourseStatus.Published, CreatedDate = Clock.Now.AddDays(-3), DurationMinutes = 295, CourseCode = "SCI-000123-02" },
                new Domain.Entities.Course { Id = new Guid("2bbb18c8-1833-4939-9fa1-b5cd9d5c0403"), CourseName = "Test History of Course", Description = "Test Course 10", Status = CourseStatus.Published, CreatedDate = Clock.Now.AddDays(-3), DurationMinutes = 295, CourseCode = "SCI-000123-03" }
            });

            context.Section.AddRange(new List<Section>
            {
                new Section
                {
                    Id = new Guid("E37B4DBF-9F0A-2222-A788-3F3CC5A15A5A"),
                    CourseId = Guid.Parse("E37B4DBF-9F0A-4272-A788-2F3CC5A15A7A"),
                    Order = 0,
                    ParentId = Guid.Parse("E37B4DBF-9F0A-4272-A788-2F3CC5A15A7A"),
                    Title = "Introduce and history of AI",
                    Description = "Introduce and history of AI",
                    CreatedDate = Clock.Now.AddDays(-2)
                },
                new Section
                {
                    Id = new Guid("E37B4DBF-9F0A-2222-A788-3F3CC5A15A5B"),
                    CourseId = Guid.Parse("E37B4DBF-9F0A-4272-A788-2F3CC5A15A7A"),
                    Order = 1,
                    ParentId = Guid.Parse("E37B4DBF-9F0A-4272-A788-2F3CC5A15A7A"),
                    Title = "Categorization and Methodologies",
                    Description = "Categorization and Methodologies",
                    CreatedDate = Clock.Now.AddDays(-2)
                }
            });
            context.Lecture.AddRange(new List<Lecture>
            {
                new Lecture
                {
                    Id = new Guid("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8A"),
                    CourseId = Guid.Parse("E37B4DBF-9F0A-4272-A788-2F3CC5A15A7A"),
                    SectionId = Guid.Parse("E37B4DBF-9F0A-2222-A788-3F3CC5A15A5A"),
                    Order = 0,
                    ParentId = Guid.Parse("E37B4DBF-9F0A-2222-A788-3F3CC5A15A5A"),
                    ParentType = ParentType.Section,
                    Description = "Course Intro",
                    LectureName = "Course Intro",
                    CreatedDate = Clock.Now.AddDays(-1)
                },
                new Lecture
                {
                    Id = new Guid("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8B"),
                    CourseId = Guid.Parse("E37B4DBF-9F0A-4272-A788-2F3CC5A15A7A"),
                    SectionId = Guid.Parse("E37B4DBF-9F0A-2222-A788-3F3CC5A15A5B"),
                    Order = 0,
                    ParentId = Guid.Parse("E37B4DBF-9F0A-2222-A788-3F3CC5A15A5B"),
                    ParentType = ParentType.Section,
                    Description = "3 Paradigms of AI",
                    LectureName = "3 Paradigms of AI",
                    CreatedDate = Clock.Now.AddDays(-1)
                },
                new Lecture
                {
                    Id = new Guid("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8C"),
                    CourseId = Guid.Parse("E37B4DBF-9F0A-4272-A788-2F3CC5A15A7A"),
                    SectionId = Guid.Parse("E37B4DBF-9F0A-2222-A788-3F3CC5A15A5B"),
                    Order = 1,
                    ParentId = Guid.Parse("E37B4DBF-9F0A-2222-A788-3F3CC5A15A5B"),
                    ParentType = ParentType.Section,
                    Description = "Categories and types of AI",
                    LectureName = "Categories and types of AI",
                    CreatedDate = Clock.Now.AddDays(-1)
                },
                new Lecture
                {
                    Id = new Guid("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8D"),
                    CourseId = Guid.Parse("E37B4DBF-9F0A-4272-A788-2F3CC5A15A7A"),
                    SectionId = Guid.Parse("E37B4DBF-9F0A-2222-A788-3F3CC5A15A5B"),
                    Order = 2,
                    ParentId = Guid.Parse("E37B4DBF-9F0A-2222-A788-3F3CC5A15A5B"),
                    ParentType = ParentType.Section,
                    Description = "Intro to NNs",
                    LectureName = "Intro to NNs",
                    CreatedDate = Clock.Now.AddDays(-1)
                },
                new Lecture
                {
                    Id = new Guid("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8E"),
                    CourseId = Guid.Parse("E37B4DBF-9F0A-4272-A788-2F3CC5A15A7A"),
                    Order = 2,
                    ParentId = Guid.Parse("E37B4DBF-9F0A-4272-A788-2F3CC5A15A7A"),
                    ParentType = ParentType.Course,
                    Description = "Final Test",
                    LectureName = "Final Test",
                    CreatedDate = Clock.Now.AddDays(-1)
                },
            });

            context.LectureContent.AddRange(new List<LectureContent>
             {
                new LectureContent
                {
                    Id = new Guid("E37B4DBF-9F0A-5555-A788-4F3CC5A15A8E"),
                    Type = LectureContentType.DigitalContent,
                    Title = string.Empty,
                    LectureId = Guid.Parse("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8A"),
                    MimeType = "mp4",
                    ResourceId = Guid.NewGuid(),
                    CreatedDate = Clock.Now.AddDays(-1)
                },
                new LectureContent
                {
                    Id = new Guid("E37B4DBF-9F0A-5555-A788-4F3CC5A15A8B"),
                    Type = LectureContentType.InlineContent,
                    Title = string.Empty,
                    Value = "<b>Inline Content</b>",
                    LectureId = Guid.Parse("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8B"),
                    ResourceId = Guid.NewGuid(),
                    CreatedDate = Clock.Now.AddDays(-1)
                },
                new LectureContent
                {
                    Id = new Guid("E37B4DBF-9F0A-5555-A788-4F3CC5A15A8C"),
                    Type = LectureContentType.InlineContent,
                    Title = string.Empty,
                    Value = "<b>Inline Content</b>",
                    LectureId = Guid.Parse("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8C"),
                    ResourceId = Guid.NewGuid(),
                    CreatedDate = Clock.Now.AddDays(-1)
                },
                new LectureContent
                {
                    Id = new Guid("E37B4DBF-9F0A-4272-A788-4F3CC5A15A8D"),
                    Type = LectureContentType.InlineContent,
                    Title = string.Empty,
                    Value = "<b>Inline Content</b>",
                    LectureId = Guid.Parse("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8D"),
                    ResourceId = Guid.NewGuid(),
                    CreatedDate = Clock.Now.AddDays(-1)
                },
                new LectureContent
                {
                    Id = new Guid("E37B4DBF-9F0A-5555-A788-4F3CC5A15A8A"),
                    Type = LectureContentType.Quiz,
                    Title = string.Empty,
                    LectureId = Guid.Parse("E37B4DBF-9F0A-3333-A788-4F3CC5A15A8E"),
                    MimeType = string.Empty,
                    ResourceId = Guid.Parse("91bd6a35-39bd-4698-b8c1-e799b0ce5405"),
                    CreatedDate = Clock.Now.AddDays(-1)
                }
             });
             */
        }

        private async Task SyncMetaData(BaseThunderDbContext dbContext)
        {
            using var scope = _serviceProvider.CreateScope();
            var metadataSynchronizer = scope.ServiceProvider.GetRequiredService<IMetadataSynchronizer>();
            await metadataSynchronizer.Sync<MetadataTag>(true);
            await dbContext.SaveChangesAsync();
        }
    }
}
