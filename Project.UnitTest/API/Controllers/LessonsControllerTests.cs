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
            string? searchKey = "test";
            var lessonList = new List<Lesson>
            {
                new Lesson { Id = "1", Discipline = "Math", Start = DateTimeOffset.Now, End = DateTimeOffset.Now.AddHours(1) }
            };

            _lessonsServiceMock
                .Setup(s => s.GetAllLessons(searchKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(lessonList);

            var result = await _controller.GetAllLessons(searchKey, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsAssignableFrom<IEnumerable<Lesson>>(okResult.Value);
            Assert.Single(returnedValue);
        }

        [Fact]
        public async Task GetAllInconveniences_ReturnsBadRequest_WhenSearchKeyIsNullOrEmpty()
        {
            string? searchKey = null;

            var result = await _controller.GetAllInconveniences(searchKey, CancellationToken.None);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Search Key is invalid!", badRequest.Value);
        }

        [Fact]
        public async Task GetAllInconveniences_ReturnsOkWithInconveniences_WhenSearchKeyIsValid()
        {
            string? searchKey = "group-101";
            var lessonList = new List<Lesson>
            {
                new Lesson
                {
                    Id = "1", 
                    Discipline = "Math", 
                    Campus = "Main", 
                    Start = DateTimeOffset.Now, 
                    End = DateTimeOffset.Now.AddHours(1)
                },
                new Lesson
                {
                    Id = "2", 
                    Discipline = "Physics", 
                    Campus = "Second", 
                    Start = DateTimeOffset.Now.AddHours(2), 
                    End = DateTimeOffset.Now.AddHours(3)
                }
            };

            var inconvenienceList = new List<Inconvenience>
            {
                new Inconvenience { Type = "WINDOW", FromLessonId = "1", ToLessonId = "2" },
                new Inconvenience { Type = "DIFF_CAMPUS", FromLessonId = "1", ToLessonId = "2" }
            };

            _lessonsServiceMock
                .Setup(s => s.GetAllLessons(searchKey, It.IsAny<CancellationToken>()))
                .ReturnsAsync(lessonList);

            _lessonsServiceMock
                .Setup(s => s.GetInconveniences(It.IsAny<IEnumerable<Lesson>>()))
                .Returns(inconvenienceList);

            var result = await _controller.GetAllInconveniences(searchKey, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsAssignableFrom<IEnumerable<Inconvenience>>(okResult.Value);
            Assert.Equal(2, (returnedValue as List<Inconvenience>)?.Count);
        }
    }
}
