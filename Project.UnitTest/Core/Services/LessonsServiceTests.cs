using Moq;
using Project.Core.Entities;
using Project.Core.Interfaces.Repositories;
using Project.Core.Services;

namespace Project.UnitTest.Core.Services
{
    public class LessonsServiceTests
    {
        private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
        private readonly LessonsService _lessonsService;

        public LessonsServiceTests()
        {
            _scheduleRepositoryMock = new Mock<IScheduleRepository>();
            _lessonsService = new LessonsService(_scheduleRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllLessons_CallsRepositoryWithCorrectArguments()
        {
            var searchKey = "group-101";
            var lessons = new List<Lesson>
            {
                new Lesson { Id = "1", Discipline = "Math" }
            }.ToAsyncEnumerable();

            _scheduleRepositoryMock
                .Setup(r => r.GetLessons(searchKey, It.IsAny<CancellationToken>()))
                .Returns(lessons);

            var result = await _lessonsService.GetAllLessons(searchKey, CancellationToken.None);

            _scheduleRepositoryMock.Verify(r => r.GetLessons(searchKey, It.IsAny<CancellationToken>()), Times.Once);
            Assert.Single(result);
        }

        [Fact]
        public void GetInconveniences_ReturnsEmpty_WhenLessonsIsNull()
        {
            IEnumerable<Lesson>? lessons = null;

            var inconveniences = _lessonsService.GetInconveniences(lessons);

            Assert.Empty(inconveniences);
        }

        [Fact]
        public void GetInconveniences_ReturnsNoWindows_WhenAllLessonsConsecutiveOrDifferentDates()
        {
            var day = DateTimeOffset.Now.Date;
            var lessons = new List<Lesson>
            {
                new Lesson
                {
                    Id = "1",
                    Start = new DateTimeOffset(day, TimeSpan.Zero).AddHours(8),
                    End = new DateTimeOffset(day, TimeSpan.Zero).AddHours(9),
                    Campus = "Main"
                },
                new Lesson
                {
                    Id = "2",
                    Start = new DateTimeOffset(day, TimeSpan.Zero).AddHours(9),
                    End = new DateTimeOffset(day, TimeSpan.Zero).AddHours(10),
                    Campus = "Main"
                },
                new Lesson
                {
                    Id = "3",
                    Start = new DateTimeOffset(day, TimeSpan.Zero).AddDays(1).AddHours(8),
                    End = new DateTimeOffset(day, TimeSpan.Zero).AddDays(1).AddHours(9),
                    Campus = "Main"
                },
            };

            var inconveniences = _lessonsService.GetInconveniences(lessons);

            Assert.Empty(inconveniences);
        }

        [Fact]
        public void GetInconveniences_FindsWindow_WhenBreakIsOneHourOrMore()
        {
            var day = DateTimeOffset.Now.Date;
            var lessons = new List<Lesson>
            {
                new Lesson
                {
                    Id = "1",
                    Start = new DateTimeOffset(day, TimeSpan.Zero).AddHours(8),
                    End = new DateTimeOffset(day, TimeSpan.Zero).AddHours(9),
                    Campus = "Main"
                },
                new Lesson
                {
                    Id = "2",
                    Start = new DateTimeOffset(day, TimeSpan.Zero).AddHours(11),
                    End = new DateTimeOffset(day, TimeSpan.Zero).AddHours(12),
                    Campus = "Main"
                }
            };

            var inconveniences = _lessonsService.GetInconveniences(lessons).ToList();

            Assert.Single(inconveniences);
            Assert.Equal("WINDOW", inconveniences[0].Type);
            Assert.Equal("1", inconveniences[0].FromLessonId);
            Assert.Equal("2", inconveniences[0].ToLessonId);
        }

        [Fact]
        public void GetInconveniences_FindsDiffCampus_WhenCampusDifferent()
        {
            var day = DateTimeOffset.Now.Date;
            var lessons = new List<Lesson>
            {
                new Lesson
                {
                    Id = "1",
                    Start = new DateTimeOffset(day, TimeSpan.Zero).AddHours(8),
                    End = new DateTimeOffset(day, TimeSpan.Zero).AddHours(9),
                    Campus = "Main"
                },
                new Lesson
                {
                    Id = "2",
                    Start = new DateTimeOffset(day, TimeSpan.Zero).AddHours(9),
                    End = new DateTimeOffset(day, TimeSpan.Zero).AddHours(10),
                    Campus = "Second"
                }
            };

            var inconveniences = _lessonsService.GetInconveniences(lessons).ToList();

            Assert.Single(inconveniences);
            Assert.Equal("DIFF_CAMPUS", inconveniences[0].Type);
            Assert.Equal("1", inconveniences[0].FromLessonId);
            Assert.Equal("2", inconveniences[0].ToLessonId);
        }

        [Fact]
        public void GetInconveniences_CanReturnMultipleTypes_WhenBothWindowAndDiffCampus()
        {
            var day = DateTimeOffset.Now.Date;
            var lessons = new List<Lesson>
            {
                new Lesson
                {
                    Id = "1",
                    Start = new DateTimeOffset(day, TimeSpan.Zero).AddHours(8),
                    End = new DateTimeOffset(day, TimeSpan.Zero).AddHours(9),
                    Campus = "Main"
                },
                new Lesson
                {
                    Id = "2",
                    Start = new DateTimeOffset(day, TimeSpan.Zero).AddHours(11),
                    End = new DateTimeOffset(day, TimeSpan.Zero).AddHours(12),
                    Campus = "Second"
                }
            };

            var inconveniences = _lessonsService.GetInconveniences(lessons).ToList();

            Assert.Equal(2, inconveniences.Count);

            Assert.Contains(inconveniences, i => i.Type == "WINDOW");
            Assert.Contains(inconveniences, i => i.Type == "DIFF_CAMPUS");
        }
    }
}
