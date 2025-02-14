using Moq;
using Project.Core.Entities;
using Project.Core.Interfaces.Repositories;
using Project.Core.Services;

namespace Project.UnitTest.Core.Services
{
    public class LessonsServiceTests
    {
        private readonly Mock<IScheduleRepository> _scheduleRepositoryMock = new();
        private readonly Mock<IInconveniencesRepository> _inconveniencesRepositoryMock = new();
        private readonly LessonsService _lessonsService;

        public LessonsServiceTests()
        {
            _inconveniencesRepositoryMock
                .Setup(r => r.GetFirst(It.IsAny<string>()))
                .ReturnsAsync((SearchInconvenience)null);

            _inconveniencesRepositoryMock
                .Setup(r => r.GetInconviences(It.IsAny<string>()))
                .Returns(new List<SearchInconvenience>().ToAsyncEnumerable());

            _inconveniencesRepositoryMock
                .Setup(r => r.AddRange(It.IsAny<IEnumerable<SearchInconvenience>>()))
                .Returns(Task.CompletedTask);

            _inconveniencesRepositoryMock
                .Setup(r => r.RemoveInconviences(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _lessonsService = new LessonsService(
                _scheduleRepositoryMock.Object,
                _inconveniencesRepositoryMock.Object
            );
        }

        [Fact]
        public async Task SearchAllLessons_CallsRepositoryWithCorrectArguments()
        {
            var searchKey = "group-101";
            var lessons = new List<Lesson>
            {
                new Lesson { Id = "1", Discipline = "Math" }
            }.ToAsyncEnumerable();

            _scheduleRepositoryMock
                .Setup(r => r.GetLessons(searchKey, It.IsAny<CancellationToken>()))
                .Returns(lessons);

            var result = await _lessonsService.SearchAllLessons(searchKey, CancellationToken.None);

            _scheduleRepositoryMock
                .Verify(r => r.GetLessons(searchKey, It.IsAny<CancellationToken>()), Times.Once);

            Assert.Single(result);
            Assert.Equal("1", result.First().Id);
        }

        [Fact]
        public async Task SearchInconveniences_ReturnsEmpty_WhenRepositoryReturnsNoLessons()
        {
            var searchKey = "test-key";
            var emptyLessons = new List<Lesson>().ToAsyncEnumerable();

            _scheduleRepositoryMock
                .Setup(r => r.GetLessons(searchKey, It.IsAny<CancellationToken>()))
                .Returns(emptyLessons);

            var inconveniences = await _lessonsService.SearchInconveniences(searchKey, CancellationToken.None);

            Assert.Empty(inconveniences);
        }

        [Fact]
        public async Task SearchInconveniences_ReturnsEmpty_WhenAllLessonsConsecutiveOrDifferentDates()
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
                    Start = new DateTimeOffset(day.AddDays(1), TimeSpan.Zero).AddHours(8),
                    End = new DateTimeOffset(day.AddDays(1), TimeSpan.Zero).AddHours(9),
                    Campus = "Main"
                },
            }.ToAsyncEnumerable();

            _scheduleRepositoryMock
                .Setup(r => r.GetLessons(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(lessons);

            var inconveniences = await _lessonsService.SearchInconveniences("irrelevant-key", CancellationToken.None);

            Assert.Empty(inconveniences);
        }

        [Fact]
        public async Task SearchInconveniences_FindsWindow_WhenBreakIsOneHourOrMore()
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
            }.ToAsyncEnumerable();

            _scheduleRepositoryMock
                .Setup(r => r.GetLessons(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(lessons);

            var inconveniences = (await _lessonsService.SearchInconveniences("some-key", CancellationToken.None)).ToList();

            Assert.Single(inconveniences);
            Assert.Equal("WINDOW", inconveniences[0].Type);
            Assert.Equal("1", inconveniences[0].FromLessonId);
            Assert.Equal("2", inconveniences[0].ToLessonId);
        }

        [Fact]
        public async Task SearchInconveniences_FindsDiffCampus_WhenCampusDifferent()
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
            }.ToAsyncEnumerable();

            _scheduleRepositoryMock
                .Setup(r => r.GetLessons(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(lessons);

            var inconveniences = (await _lessonsService.SearchInconveniences("some-key", CancellationToken.None)).ToList();

            Assert.Single(inconveniences);
            Assert.Equal("DIFF_CAMPUS", inconveniences[0].Type);
            Assert.Equal("1", inconveniences[0].FromLessonId);
            Assert.Equal("2", inconveniences[0].ToLessonId);
        }

        [Fact]
        public async Task SearchInconveniences_CanReturnMultipleTypes_WhenBothWindowAndDiffCampus()
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
            }.ToAsyncEnumerable();

            _scheduleRepositoryMock
                .Setup(r => r.GetLessons(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(lessons);

            var inconveniences = (await _lessonsService.SearchInconveniences("some-key", CancellationToken.None)).ToList();

            Assert.Equal(2, inconveniences.Count);
            Assert.Contains(inconveniences, i => i.Type == "WINDOW");
            Assert.Contains(inconveniences, i => i.Type == "DIFF_CAMPUS");
        }
    }
}
