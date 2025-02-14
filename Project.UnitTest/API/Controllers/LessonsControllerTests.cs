using Microsoft.AspNetCore.Mvc;
using Moq;
using Project.API.Controllers;
using Project.Core.Entities;
using Project.Core.Interfaces.Services;

namespace Project.UnitTest.API.Controllers
{
    public class LessonsControllerTests
    {
        private readonly Mock<ILessonsService> _lessonsServiceMock;
        private readonly LessonsController _controller;

        public LessonsControllerTests()
        {
            _lessonsServiceMock = new Mock<ILessonsService>();
            _controller = new LessonsController(_lessonsServiceMock.Object);
        }

        [Fact]
        public async Task GetAllLessons_ReturnsBadRequest_WhenSearchKeyIsNull()
        {
            string? searchKey = null;

            var result = await _controller.GetAllLessons(searchKey, CancellationToken.None);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Search Key is invalid!", badRequest.Value);
        }

        [Fact]
        public async Task GetAllLessons_ReturnsBadRequest_WhenSearchKeyIsEmpty()
        {
            string? searchKey = string.Empty;

            var result = await _controller.GetAllLessons(searchKey, CancellationToken.None);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Search Key is invalid!", badRequest.Value);
        }

        [Fact]
        public async Task GetAllLessons_ReturnsOkWithLessons_WhenSearchKeyIsValid()
        {
            string searchKey = "test";
            var lessons = new List<Lesson>
            {
                new Lesson
                {
                    Id = "1",
                    Discipline = "Math",
                    Start = DateTimeOffset.Now,
                    End = DateTimeOffset.Now.AddHours(1)
                }
            };

            _lessonsServiceMock
                .Setup(s => s.SearchAllLessons(searchKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(lessons);

            var result = await _controller.GetAllLessons(searchKey, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsAssignableFrom<IEnumerable<Lesson>>(okResult.Value);
            Assert.Single(returnedValue);
            Assert.Equal("1", returnedValue.First().Id);
        }

        [Fact]
        public async Task GetAllInconveniences_ReturnsBadRequest_WhenSearchKeyIsNull()
        {
            string? searchKey = null;

            var result = await _controller.GetAllInconveniences(searchKey, CancellationToken.None);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Search Key is invalid!", badRequest.Value);
        }

        [Fact]
        public async Task GetAllInconveniences_ReturnsBadRequest_WhenSearchKeyIsEmpty()
        {
            string? searchKey = string.Empty;

            var result = await _controller.GetAllInconveniences(searchKey, CancellationToken.None);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Search Key is invalid!", badRequest.Value);
        }

        [Fact]
        public async Task GetAllInconveniences_ReturnsOkWithInconveniences_WhenSearchKeyIsValid()
        {
            string searchKey = "group-101";
            
            var inconvenienceList = new List<SearchInconvenience>
            {
                new SearchInconvenience()
                {
                        FromLessonId = "1",
                        ToLessonId = "2",
                        Type = "WINDOW",
                        SearchKey = searchKey 
                },
                new SearchInconvenience()
                {
                    FromLessonId = "1",
                    ToLessonId = "2",
                    Type = "DIFF_CAMPUS",
                    SearchKey = searchKey 
                }
            };

            _lessonsServiceMock
                .Setup(s => s.SearchInconveniences(searchKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(inconvenienceList);

            var result = await _controller.GetAllInconveniences(searchKey, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsAssignableFrom<IEnumerable<SearchInconvenience>>(okResult.Value);

            Assert.Equal(2, returnedValue.Count());
            Assert.Contains(returnedValue, i => i.Type == "WINDOW");
            Assert.Contains(returnedValue, i => i.Type == "DIFF_CAMPUS");
        }
    }
}
