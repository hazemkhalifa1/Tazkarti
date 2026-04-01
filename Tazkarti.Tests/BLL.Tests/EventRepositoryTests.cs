using BLL.Interfaces;
using DAL.Entities;
using Moq;

namespace Tazkarti.Tests.BLL.Tests
{
    public class EventRepositoryTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenaricRepository<Event>> _eventRepositoryMock;
        private readonly Mock<ITicketRepository> _ticketRepositoryMock;

        public EventRepositoryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _eventRepositoryMock = new Mock<IGenaricRepository<Event>>();
            _ticketRepositoryMock = new Mock<ITicketRepository>();

            // Setup the mock repositories and unit of work
            _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.TicketRepository).Returns(_ticketRepositoryMock.Object);
        }

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_ShouldCallRepositoryAddAsync_WithCorrectEvent()
        {
            // Arrange
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Football Match",
                NameAr = "مباراة كرة قدم",
                place = "Cairo Stadium",
                placeAr = "ستاد القاهرة",
                Time = new DateTime(2026, 5, 15, 20, 0, 0),
                NoOfTickets = 1000,
                Price = 150.00m,
                Info = "Semi-final match",
                InfoAr = "مباراة نصف النهائي",
                ImageName = "match.jpg"
            };

            _eventRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Event>()))
                .Returns(Task.CompletedTask);

            // Act
            await _unitOfWorkMock.Object.EventRepository.AddAsync(newEvent);

            // Assert
            _eventRepositoryMock.Verify(r => r.AddAsync(newEvent), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ThenSaveChanges_ShouldCallBothMethods()
        {
            // Arrange
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Concert",
                NameAr = "حفلة موسيقية",
                place = "Opera House",
                placeAr = "دار الأوبرا",
                Time = new DateTime(2026, 6, 20, 21, 0, 0),
                NoOfTickets = 500,
                Price = 200.00m,
                Info = "Live concert",
                InfoAr = "حفلة مباشرة"
            };

            _eventRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Event>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _unitOfWorkMock.Object.EventRepository.AddAsync(newEvent);
            await _unitOfWorkMock.Object.SaveChangesAsync();

            // Assert
            _eventRepositoryMock.Verify(r => r.AddAsync(newEvent), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WithNullImageName_ShouldStillAddEvent()
        {
            // Arrange
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Workshop",
                NameAr = "ورشة عمل",
                place = "Convention Center",
                placeAr = "مركز المؤتمرات",
                Time = new DateTime(2026, 4, 10, 10, 0, 0),
                NoOfTickets = 100,
                Price = 50.00m,
                Info = "Tech workshop",
                InfoAr = "ورشة تقنية",
                ImageName = null
            };

            _eventRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Event>()))
                .Returns(Task.CompletedTask);

            // Act
            await _unitOfWorkMock.Object.EventRepository.AddAsync(newEvent);

            // Assert
            _eventRepositoryMock.Verify(r => r.AddAsync(
                It.Is<Event>(e => e.ImageName == null)), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldPreserveAllEventProperties()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var eventTime = new DateTime(2026, 7, 1, 18, 30, 0);
            Event? capturedEvent = null;

            _eventRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Event>()))
                .Callback<Event>(e => capturedEvent = e)
                .Returns(Task.CompletedTask);

            var newEvent = new Event
            {
                Id = eventId,
                Name = "Final Match",
                NameAr = "المباراة النهائية",
                place = "National Stadium",
                placeAr = "الاستاد الوطني",
                Time = eventTime,
                NoOfTickets = 5000,
                Price = 300.00m,
                Info = "Championship final",
                InfoAr = "نهائي البطولة",
                ImageName = "final.png"
            };

            // Act
            await _unitOfWorkMock.Object.EventRepository.AddAsync(newEvent);

            // Assert
            Assert.NotNull(capturedEvent);
            Assert.Equal(eventId, capturedEvent.Id);
            Assert.Equal("Final Match", capturedEvent.Name);
            Assert.Equal("المباراة النهائية", capturedEvent.NameAr);
            Assert.Equal("National Stadium", capturedEvent.place);
            Assert.Equal("الاستاد الوطني", capturedEvent.placeAr);
            Assert.Equal(eventTime, capturedEvent.Time);
            Assert.Equal(5000, capturedEvent.NoOfTickets);
            Assert.Equal(300.00m, capturedEvent.Price);
            Assert.Equal("Championship final", capturedEvent.Info);
            Assert.Equal("نهائي البطولة", capturedEvent.InfoAr);
            Assert.Equal("final.png", capturedEvent.ImageName);
        }

        [Fact]
        public async Task AddAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Test Event",
                NameAr = "حدث تجريبي",
                place = "Test Place",
                placeAr = "مكان تجريبي",
                Time = DateTime.Now,
                NoOfTickets = 10,
                Price = 25.00m
            };

            _eventRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Event>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _unitOfWorkMock.Object.EventRepository.AddAsync(newEvent));
        }

        [Fact]
        public async Task AddAsync_WhenSaveChangesFails_ShouldThrowException()
        {
            // Arrange
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Event",
                NameAr = "حدث",
                place = "Place",
                placeAr = "مكان",
                Time = DateTime.Now,
                NoOfTickets = 50,
                Price = 75.00m
            };

            _eventRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Event>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new Exception("Save failed"));

            // Act
            await _unitOfWorkMock.Object.EventRepository.AddAsync(newEvent);

            // Assert - SaveChangesAsync should throw
            await Assert.ThrowsAsync<Exception>(
                () => _unitOfWorkMock.Object.SaveChangesAsync());
        }

        [Fact]
        public async Task AddAsync_MultipleTimes_ShouldCallRepositoryForEachEvent()
        {
            // Arrange
            var event1 = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Event 1",
                NameAr = "حدث 1",
                place = "Place 1",
                placeAr = "مكان 1",
                Time = DateTime.Now,
                NoOfTickets = 100,
                Price = 50.00m
            };

            var event2 = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Event 2",
                NameAr = "حدث 2",
                place = "Place 2",
                placeAr = "مكان 2",
                Time = DateTime.Now.AddDays(1),
                NoOfTickets = 200,
                Price = 100.00m
            };

            _eventRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Event>()))
                .Returns(Task.CompletedTask);

            // Act
            await _unitOfWorkMock.Object.EventRepository.AddAsync(event1);
            await _unitOfWorkMock.Object.EventRepository.AddAsync(event2);

            // Assert
            _eventRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Event>()), Times.Exactly(2));
            _eventRepositoryMock.Verify(r => r.AddAsync(event1), Times.Once);
            _eventRepositoryMock.Verify(r => r.AddAsync(event2), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WithZeroPrice_ShouldAddEvent()
        {
            // Arrange
            var freeEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Free Event",
                NameAr = "حدث مجاني",
                place = "Park",
                placeAr = "حديقة",
                Time = DateTime.Now,
                NoOfTickets = 300,
                Price = 0m,
                Info = "Free admission",
                InfoAr = "دخول مجاني"
            };

            _eventRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Event>()))
                .Returns(Task.CompletedTask);

            // Act
            await _unitOfWorkMock.Object.EventRepository.AddAsync(freeEvent);

            // Assert
            _eventRepositoryMock.Verify(r => r.AddAsync(
                It.Is<Event>(e => e.Price == 0m)), Times.Once);
        }

        [Fact]
        public async Task AddAsync_EventShouldHaveNewGuidId()
        {
            // Arrange
            Event? capturedEvent = null;

            _eventRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Event>()))
                .Callback<Event>(e => capturedEvent = e)
                .Returns(Task.CompletedTask);

            var newEvent = new Event
            {
                Name = "ID Test Event",
                NameAr = "حدث تجربة المعرف",
                place = "Venue",
                placeAr = "مكان",
                Time = DateTime.Now,
                NoOfTickets = 50,
                Price = 99.99m
            };
            // BaseEntity auto-generates a new Guid
            newEvent.Id = Guid.NewGuid();

            // Act
            await _unitOfWorkMock.Object.EventRepository.AddAsync(newEvent);

            // Assert
            Assert.NotNull(capturedEvent);
            Assert.NotEqual(Guid.Empty, capturedEvent.Id);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_ShouldCallRepositoryUpdate_WithCorrectEvent()
        {
            // Arrange
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Football Match",
                NameAr = "مباراة كرة قدم",
                place = "Cairo Stadium",
                placeAr = "ستاد القاهرة",
                Time = new DateTime(2026, 5, 15, 20, 0, 0),
                NoOfTickets = 1000,
                Price = 150.00m,
                Info = "Semi-final match",
                InfoAr = "مباراة نصف النهائي",
                ImageName = "match.jpg"
            };

            _eventRepositoryMock
                .Setup(r => r.Update(It.IsAny<Event>()));

            // Act
            _unitOfWorkMock.Object.EventRepository.Update(newEvent);

            // Assert
            _eventRepositoryMock.Verify(r => r.Update(newEvent), Times.Once);
        }

        [Fact]
        public async Task Update_ThenSaveChanges_ShouldCallBothMethods()
        {
            // Arrange
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Concert",
                NameAr = "حفلة موسيقية",
                place = "Opera House",
                placeAr = "دار الأوبرا",
                Time = new DateTime(2026, 6, 20, 21, 0, 0),
                NoOfTickets = 500,
                Price = 200.00m,
                Info = "Live concert",
                InfoAr = "حفلة مباشرة"
            };

            _eventRepositoryMock
                .Setup(r => r.Update(It.IsAny<Event>()));

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            _unitOfWorkMock.Object.EventRepository.Update(newEvent);
            await _unitOfWorkMock.Object.SaveChangesAsync();

            // Assert
            _eventRepositoryMock.Verify(r => r.Update(newEvent), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public void Update_WithNullImageName_ShouldStillUpdate()
        {
            // Arrange
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Workshop",
                NameAr = "ورشة عمل",
                place = "Convention Center",
                placeAr = "مركز المؤتمرات",
                Time = new DateTime(2026, 4, 10, 10, 0, 0),
                NoOfTickets = 100,
                Price = 50.00m,
                Info = "Tech workshop",
                InfoAr = "ورشة تقنية",
                ImageName = null
            };

            _eventRepositoryMock
                .Setup(r => r.Update(It.IsAny<Event>()));

            // Act
            _unitOfWorkMock.Object.EventRepository.Update(newEvent);

            // Assert
            _eventRepositoryMock.Verify(r => r.Update(
                It.Is<Event>(e => e.ImageName == null)), Times.Once);
        }

        [Fact]
        public void Update_ShouldPreserveAllEventProperties()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var eventTime = new DateTime(2026, 7, 1, 18, 30, 0);
            Event? capturedEvent = null;

            _eventRepositoryMock
                .Setup(r => r.Update(It.IsAny<Event>()))
                .Callback<Event>(e => capturedEvent = e);

            var newEvent = new Event
            {
                Id = eventId,
                Name = "Final Match",
                NameAr = "المباراة النهائية",
                place = "National Stadium",
                placeAr = "الاستاد الوطني",
                Time = eventTime,
                NoOfTickets = 5000,
                Price = 300.00m,
                Info = "Championship final",
                InfoAr = "نهائي البطولة",
                ImageName = "final.png"
            };

            // Act
            _unitOfWorkMock.Object.EventRepository.Update(newEvent);

            // Assert
            Assert.NotNull(capturedEvent);
            Assert.Equal(eventId, capturedEvent.Id);
            Assert.Equal("Final Match", capturedEvent.Name);
            Assert.Equal("المباراة النهائية", capturedEvent.NameAr);
            Assert.Equal("National Stadium", capturedEvent.place);
            Assert.Equal("الاستاد الوطني", capturedEvent.placeAr);
            Assert.Equal(eventTime, capturedEvent.Time);
            Assert.Equal(5000, capturedEvent.NoOfTickets);
            Assert.Equal(300.00m, capturedEvent.Price);
            Assert.Equal("Championship final", capturedEvent.Info);
            Assert.Equal("نهائي البطولة", capturedEvent.InfoAr);
            Assert.Equal("final.png", capturedEvent.ImageName);
        }

        [Fact]
        public void Update_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Test Event",
                NameAr = "حدث تجريبي",
                place = "Test Place",
                placeAr = "مكان تجريبي",
                Time = DateTime.Now,
                NoOfTickets = 10,
                Price = 25.00m
            };

            _eventRepositoryMock
                .Setup(r => r.Update(It.IsAny<Event>()))
                .Throws(new InvalidOperationException("Database error"));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(
               () => _unitOfWorkMock.Object.EventRepository.Update(newEvent));
        }

        [Fact]
        public async Task Update_WhenSaveChangesFails_ShouldThrowException()
        {
            // Arrange
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Event",
                NameAr = "حدث",
                place = "Place",
                placeAr = "مكان",
                Time = DateTime.Now,
                NoOfTickets = 50,
                Price = 75.00m
            };

            _eventRepositoryMock
                .Setup(r => r.Update(It.IsAny<Event>()));

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new Exception("Save failed"));

            // Act
            _unitOfWorkMock.Object.EventRepository.Update(newEvent);

            // Assert - SaveChangesAsync should throw
            await Assert.ThrowsAsync<Exception>(
                () => _unitOfWorkMock.Object.SaveChangesAsync());
        }

        [Fact]
        public void Update_MultipleTimes_ShouldCallRepositoryForEachEvent()
        {
            // Arrange
            var event1 = new Event { Id = Guid.NewGuid(), Name = "Event 1", NameAr = "حدث 1", place = "Place 1", placeAr = "مكان 1", Time = DateTime.Now, NoOfTickets = 100, Price = 50.00m };
            var event2 = new Event { Id = Guid.NewGuid(), Name = "Event 2", NameAr = "حدث 2", place = "Place 2", placeAr = "مكان 2", Time = DateTime.Now.AddDays(1), NoOfTickets = 200, Price = 100.00m };

            _eventRepositoryMock.Setup(r => r.Update(It.IsAny<Event>()));

            // Act
            _unitOfWorkMock.Object.EventRepository.Update(event1);
            _unitOfWorkMock.Object.EventRepository.Update(event2);

            // Assert
            _eventRepositoryMock.Verify(r => r.Update(It.IsAny<Event>()), Times.Exactly(2));
            _eventRepositoryMock.Verify(r => r.Update(event1), Times.Once);
            _eventRepositoryMock.Verify(r => r.Update(event2), Times.Once);
        }

        [Fact]
        public void Update_WithZeroPrice_ShouldUpdateEvent()
        {
            // Arrange
            var freeEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Free Event",
                NameAr = "حدث مجاني",
                place = "Park",
                placeAr = "حديقة",
                Time = DateTime.Now,
                NoOfTickets = 300,
                Price = 0m,
                Info = "Free admission",
                InfoAr = "دخول مجاني"
            };

            _eventRepositoryMock.Setup(r => r.Update(It.IsAny<Event>()));

            // Act
            _unitOfWorkMock.Object.EventRepository.Update(freeEvent);

            // Assert
            _eventRepositoryMock.Verify(r => r.Update(
                It.Is<Event>(e => e.Price == 0m)), Times.Once);
        }

        [Fact]
        public void Update_EventShouldHaveNewGuidId()
        {
            // Arrange
            Event? capturedEvent = null;

            _eventRepositoryMock
                .Setup(r => r.Update(It.IsAny<Event>()))
                .Callback<Event>(e => capturedEvent = e);

            var newEvent = new Event
            {
                Name = "ID Test Event",
                NameAr = "حدث تجربة المعرف",
                place = "Venue",
                placeAr = "مكان",
                Time = DateTime.Now,
                NoOfTickets = 50,
                Price = 99.99m
            };
            newEvent.Id = Guid.NewGuid();

            // Act
            _unitOfWorkMock.Object.EventRepository.Update(newEvent);

            // Assert
            Assert.NotNull(capturedEvent);
            Assert.NotEqual(Guid.Empty, capturedEvent.Id);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEvents()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Event 1",
                    NameAr = "حدث 1",
                    place = "Place 1",
                    placeAr = "مكان 1",
                    Time = DateTime.Now,
                    NoOfTickets = 100,
                    Price = 50.00m
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Event 2",
                    NameAr = "حدث 2",
                    place = "Place 2",
                    placeAr = "مكان 2",
                    Time = DateTime.Now.AddDays(1),
                    NoOfTickets = 200,
                    Price = 100.00m
                }
            };

            _eventRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(events);

            // Act
            var result = await _unitOfWorkMock.Object.EventRepository.GetAllAsync();

            // Assert
            Assert.Equal(events, result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoEvents()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Event>());

            // Act
            var result = await _unitOfWorkMock.Object.EventRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListType()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Event>());

            // Act
            var result = await _unitOfWorkMock.Object.EventRepository.GetAllAsync();

            // Assert
            Assert.IsAssignableFrom<List<Event>>(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _unitOfWorkMock.Object.EventRepository.GetAllAsync());
        }

        #endregion

        #region GetbyIdAsync Tests

        [Fact]
        public async Task GetbyIdAsync_ShouldReturnEvent_WhenEventExists()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var expectedEvent = new Event
            {
                Id = eventId,
                Name = "Football Match",
                NameAr = "مباراة كرة قدم",
                place = "Cairo Stadium",
                placeAr = "ستاد القاهرة",
                Time = new DateTime(2026, 5, 15, 20, 0, 0),
                NoOfTickets = 1000,
                Price = 150.00m,
                Info = "Semi-final match",
                InfoAr = "مباراة نصف النهائي",
                ImageName = "match.jpg"
            };

            _eventRepositoryMock
                .Setup(r => r.GetbyIdAsync(eventId))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _unitOfWorkMock.Object.EventRepository.GetbyIdAsync(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventId, result.Id);
            Assert.Equal("Football Match", result.Name);
            Assert.Equal("مباراة كرة قدم", result.NameAr);
        }

        [Fact]
        public async Task GetbyIdAsync_ShouldReturnNull_WhenEventDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            _eventRepositoryMock
                .Setup(r => r.GetbyIdAsync(nonExistentId))
                .ReturnsAsync((Event?)null);

            // Act
            var result = await _unitOfWorkMock.Object.EventRepository.GetbyIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetbyIdAsync_ShouldCallRepository_ExactlyOnce()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _eventRepositoryMock
                .Setup(r => r.GetbyIdAsync(eventId))
                .ReturnsAsync(new Event { Id = eventId, Name = "Test", NameAr = "تجربة", place = "Place", placeAr = "مكان" });

            // Act
            await _unitOfWorkMock.Object.EventRepository.GetbyIdAsync(eventId);

            // Assert
            _eventRepositoryMock.Verify(r => r.GetbyIdAsync(eventId), Times.Once);
        }

        [Fact]
        public async Task GetbyIdAsync_ShouldPreserveAllProperties_WhenEventFound()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var eventTime = new DateTime(2026, 8, 10, 19, 0, 0);
            var expectedEvent = new Event
            {
                Id = eventId,
                Name = "Concert Night",
                NameAr = "ليلة حفل",
                place = "Opera House",
                placeAr = "دار الأوبرا",
                Time = eventTime,
                NoOfTickets = 2000,
                Price = 250.00m,
                Info = "Live music show",
                InfoAr = "عرض موسيقى حي",
                ImageName = "concert.jpg"
            };

            _eventRepositoryMock
                .Setup(r => r.GetbyIdAsync(eventId))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _unitOfWorkMock.Object.EventRepository.GetbyIdAsync(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventId, result.Id);
            Assert.Equal("Concert Night", result.Name);
            Assert.Equal("ليلة حفل", result.NameAr);
            Assert.Equal("Opera House", result.place);
            Assert.Equal("دار الأوبرا", result.placeAr);
            Assert.Equal(eventTime, result.Time);
            Assert.Equal(2000, result.NoOfTickets);
            Assert.Equal(250.00m, result.Price);
            Assert.Equal("Live music show", result.Info);
            Assert.Equal("عرض موسيقى حي", result.InfoAr);
            Assert.Equal("concert.jpg", result.ImageName);
        }

        [Fact]
        public async Task GetbyIdAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _eventRepositoryMock
                .Setup(r => r.GetbyIdAsync(eventId))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _unitOfWorkMock.Object.EventRepository.GetbyIdAsync(eventId));
        }

        #endregion

        #region Search Tests

        [Fact]
        public async Task Search_WithValidSearchValue_ShouldReturnMatchingEvents()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var searchValue = eventId.ToString().Substring(0, 8);
            var matchingEvents = new List<Event>
            {
                new Event
                {
                    Id = eventId,
                    Name = "Matched Event",
                    NameAr = "حدث مطابق",
                    place = "Stadium",
                    placeAr = "ملعب",
                    Time = DateTime.Now,
                    NoOfTickets = 500,
                    Price = 100.00m
                }
            };

            _eventRepositoryMock
                .Setup(r => r.Search(searchValue))
                .ReturnsAsync(matchingEvents);

            // Act
            var result = await _unitOfWorkMock.Object.EventRepository.Search(searchValue);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Matched Event", result.First().Name);
        }

        [Fact]
        public async Task Search_WithNullSearchValue_ShouldReturnAllEvents()
        {
            // Arrange
            var allEvents = new List<Event>
            {
                new Event { Id = Guid.NewGuid(), Name = "Event 1", NameAr = "حدث 1", place = "Place 1", placeAr = "مكان 1", Time = DateTime.Now, NoOfTickets = 100, Price = 50m },
                new Event { Id = Guid.NewGuid(), Name = "Event 2", NameAr = "حدث 2", place = "Place 2", placeAr = "مكان 2", Time = DateTime.Now, NoOfTickets = 200, Price = 75m },
                new Event { Id = Guid.NewGuid(), Name = "Event 3", NameAr = "حدث 3", place = "Place 3", placeAr = "مكان 3", Time = DateTime.Now, NoOfTickets = 300, Price = 100m }
            };

            _eventRepositoryMock
                .Setup(r => r.Search(null))
                .ReturnsAsync(allEvents);

            // Act
            var result = await _unitOfWorkMock.Object.EventRepository.Search(null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task Search_WithNonMatchingValue_ShouldReturnEmptyList()
        {
            // Arrange
            var searchValue = "nonexistent-value-xyz";

            _eventRepositoryMock
                .Setup(r => r.Search(searchValue))
                .ReturnsAsync(new List<Event>());

            // Act
            var result = await _unitOfWorkMock.Object.EventRepository.Search(searchValue);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Search_ShouldCallRepository_WithExactSearchValue()
        {
            // Arrange
            var searchValue = "test-search";

            _eventRepositoryMock
                .Setup(r => r.Search(It.IsAny<string>()))
                .ReturnsAsync(new List<Event>());

            // Act
            await _unitOfWorkMock.Object.EventRepository.Search(searchValue);

            // Assert
            _eventRepositoryMock.Verify(r => r.Search(searchValue), Times.Once);
        }

        [Fact]
        public async Task Search_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            _eventRepositoryMock
                .Setup(r => r.Search(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Search failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _unitOfWorkMock.Object.EventRepository.Search("test"));
        }

        #endregion

        #region Delete Tests

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WithCorrectEvent()
        {
            // Arrange
            var eventToDelete = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Event to Delete",
                NameAr = "حدث للحذف",
                place = "Old Venue",
                placeAr = "مكان قديم",
                Time = DateTime.Now,
                NoOfTickets = 100,
                Price = 50.00m,
                ImageName = "old.jpg"
            };

            _eventRepositoryMock
                .Setup(r => r.Delete(It.IsAny<Event>()));

            // Act
            _unitOfWorkMock.Object.EventRepository.Delete(eventToDelete);

            // Assert
            _eventRepositoryMock.Verify(r => r.Delete(eventToDelete), Times.Once);
        }

        [Fact]
        public async Task Delete_ThenSaveChanges_ShouldCallBothMethods()
        {
            // Arrange
            var eventToDelete = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Delete Me",
                NameAr = "احذفني",
                place = "Venue",
                placeAr = "مكان",
                Time = DateTime.Now,
                NoOfTickets = 50,
                Price = 30.00m
            };

            _eventRepositoryMock
                .Setup(r => r.Delete(It.IsAny<Event>()));

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            _unitOfWorkMock.Object.EventRepository.Delete(eventToDelete);
            await _unitOfWorkMock.Object.SaveChangesAsync();

            // Assert
            _eventRepositoryMock.Verify(r => r.Delete(eventToDelete), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public void Delete_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var eventToDelete = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Problem Event",
                NameAr = "حدث مشكلة",
                place = "Place",
                placeAr = "مكان",
                Time = DateTime.Now,
                NoOfTickets = 10,
                Price = 20.00m
            };

            _eventRepositoryMock
                .Setup(r => r.Delete(It.IsAny<Event>()))
                .Throws(new InvalidOperationException("Cannot delete tracked entity"));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => _unitOfWorkMock.Object.EventRepository.Delete(eventToDelete));
        }

        [Fact]
        public void Delete_ShouldCaptureCorrectEntity()
        {
            // Arrange
            Event? capturedEvent = null;
            var eventId = Guid.NewGuid();

            var eventToDelete = new Event
            {
                Id = eventId,
                Name = "Captured Event",
                NameAr = "حدث ملتقط",
                place = "Captured Place",
                placeAr = "مكان ملتقط",
                Time = new DateTime(2026, 12, 25, 20, 0, 0),
                NoOfTickets = 400,
                Price = 120.00m,
                ImageName = "captured.png"
            };

            _eventRepositoryMock
                .Setup(r => r.Delete(It.IsAny<Event>()))
                .Callback<Event>(e => capturedEvent = e);

            // Act
            _unitOfWorkMock.Object.EventRepository.Delete(eventToDelete);

            // Assert
            Assert.NotNull(capturedEvent);
            Assert.Equal(eventId, capturedEvent.Id);
            Assert.Equal("Captured Event", capturedEvent.Name);
            Assert.Equal("حدث ملتقط", capturedEvent.NameAr);
            Assert.Equal("captured.png", capturedEvent.ImageName);
        }

        [Fact]
        public void Delete_MultipleTimes_ShouldCallRepositoryForEachEvent()
        {
            // Arrange
            var event1 = new Event { Id = Guid.NewGuid(), Name = "Event 1", NameAr = "حدث 1", place = "P1", placeAr = "م1", Time = DateTime.Now, NoOfTickets = 10, Price = 10m };
            var event2 = new Event { Id = Guid.NewGuid(), Name = "Event 2", NameAr = "حدث 2", place = "P2", placeAr = "م2", Time = DateTime.Now, NoOfTickets = 20, Price = 20m };
            var event3 = new Event { Id = Guid.NewGuid(), Name = "Event 3", NameAr = "حدث 3", place = "P3", placeAr = "م3", Time = DateTime.Now, NoOfTickets = 30, Price = 30m };

            _eventRepositoryMock.Setup(r => r.Delete(It.IsAny<Event>()));

            // Act
            _unitOfWorkMock.Object.EventRepository.Delete(event1);
            _unitOfWorkMock.Object.EventRepository.Delete(event2);
            _unitOfWorkMock.Object.EventRepository.Delete(event3);

            // Assert
            _eventRepositoryMock.Verify(r => r.Delete(It.IsAny<Event>()), Times.Exactly(3));
            _eventRepositoryMock.Verify(r => r.Delete(event1), Times.Once);
            _eventRepositoryMock.Verify(r => r.Delete(event2), Times.Once);
            _eventRepositoryMock.Verify(r => r.Delete(event3), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenSaveChangesFails_ShouldThrowException()
        {
            // Arrange
            var eventToDelete = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Delete Fail",
                NameAr = "فشل الحذف",
                place = "Place",
                placeAr = "مكان",
                Time = DateTime.Now,
                NoOfTickets = 5,
                Price = 15.00m
            };

            _eventRepositoryMock.Setup(r => r.Delete(It.IsAny<Event>()));

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new Exception("Foreign key constraint violation"));

            // Act
            _unitOfWorkMock.Object.EventRepository.Delete(eventToDelete);

            // Assert
            await Assert.ThrowsAsync<Exception>(
                () => _unitOfWorkMock.Object.SaveChangesAsync());
        }

        #endregion

        #region SaveChangesAsync Tests

        [Fact]
        public async Task SaveChangesAsync_ShouldBeCalledSuccessfully()
        {
            // Arrange
            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _unitOfWorkMock.Object.SaveChangesAsync();

            // Assert
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SaveChangesAsync_CalledMultipleTimes_ShouldTrackAllCalls()
        {
            // Arrange
            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _unitOfWorkMock.Object.SaveChangesAsync();
            await _unitOfWorkMock.Object.SaveChangesAsync();
            await _unitOfWorkMock.Object.SaveChangesAsync();

            // Assert
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(3));
        }

        [Fact]
        public async Task SaveChangesAsync_WhenThrowsException_ShouldPropagateException()
        {
            // Arrange
            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new Exception("Database connection lost"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(
                () => _unitOfWorkMock.Object.SaveChangesAsync());
            Assert.Equal("Database connection lost", ex.Message);
        }

        [Fact]
        public async Task SaveChangesAsync_AfterAddAndDelete_ShouldBeCalledOnce()
        {
            // Arrange
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "New Event",
                NameAr = "حدث جديد",
                place = "New Place",
                placeAr = "مكان جديد",
                Time = DateTime.Now,
                NoOfTickets = 100,
                Price = 60.00m
            };

            var eventToDelete = new Event
            {
                Id = Guid.NewGuid(),
                Name = "Old Event",
                NameAr = "حدث قديم",
                place = "Old Place",
                placeAr = "مكان قديم",
                Time = DateTime.Now.AddDays(-30),
                NoOfTickets = 0,
                Price = 40.00m
            };

            _eventRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Event>())).Returns(Task.CompletedTask);
            _eventRepositoryMock.Setup(r => r.Delete(It.IsAny<Event>()));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _unitOfWorkMock.Object.EventRepository.AddAsync(newEvent);
            _unitOfWorkMock.Object.EventRepository.Delete(eventToDelete);
            await _unitOfWorkMock.Object.SaveChangesAsync();

            // Assert
            _eventRepositoryMock.Verify(r => r.AddAsync(newEvent), Times.Once);
            _eventRepositoryMock.Verify(r => r.Delete(eventToDelete), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        #endregion
    }
}
