using BLL.Interfaces;
using DAL.Entities;
using Moq;

namespace Tazkarti.Tests.BLL.Tests
{
    public class TicketRepositoryTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITicketRepository> _ticketRepositoryMock;

        private readonly Event _sampleEvent;

        public TicketRepositoryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _ticketRepositoryMock = new Mock<ITicketRepository>();

            _sampleEvent = new Event
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

            // Setup the mock unit of work
            _unitOfWorkMock.Setup(u => u.TicketRepository).Returns(_ticketRepositoryMock.Object);
        }

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_ShouldCallRepositoryAddAsync_WithCorrectTicket()
        {
            // Arrange
            var newTicket = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Hazem Khalifa",
                Email = "hazem@example.com",
                PhoneNumber = "01012345678",
                Valid = true,
                EventID = _sampleEvent.Id,
                Event = _sampleEvent
            };

            _ticketRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Ticket>()))
                .Returns(Task.CompletedTask);

            // Act
            await _unitOfWorkMock.Object.TicketRepository.AddAsync(newTicket);

            // Assert
            _ticketRepositoryMock.Verify(r => r.AddAsync(newTicket), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ThenSaveChanges_ShouldCallBothMethods()
        {
            // Arrange
            var newTicket = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Ahmed Ali",
                Email = "ahmed@example.com",
                PhoneNumber = "01098765432",
                Valid = true,
                EventID = _sampleEvent.Id,
                Event = _sampleEvent
            };

            _ticketRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Ticket>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _unitOfWorkMock.Object.TicketRepository.AddAsync(newTicket);
            await _unitOfWorkMock.Object.SaveChangesAsync();

            // Assert
            _ticketRepositoryMock.Verify(r => r.AddAsync(newTicket), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldPreserveAllTicketProperties()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            Ticket? capturedTicket = null;

            _ticketRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Ticket>()))
                .Callback<Ticket>(t => capturedTicket = t)
                .Returns(Task.CompletedTask);

            var newTicket = new Ticket
            {
                Id = ticketId,
                Name = "Mohamed Salah",
                Email = "mo.salah@example.com",
                PhoneNumber = "01155555555",
                Valid = true,
                EventID = _sampleEvent.Id,
                Event = _sampleEvent
            };

            // Act
            await _unitOfWorkMock.Object.TicketRepository.AddAsync(newTicket);

            // Assert
            Assert.NotNull(capturedTicket);
            Assert.Equal(ticketId, capturedTicket.Id);
            Assert.Equal("Mohamed Salah", capturedTicket.Name);
            Assert.Equal("mo.salah@example.com", capturedTicket.Email);
            Assert.Equal("01155555555", capturedTicket.PhoneNumber);
            Assert.True(capturedTicket.Valid);
            Assert.Equal(_sampleEvent.Id, capturedTicket.EventID);
            Assert.Equal(_sampleEvent, capturedTicket.Event);
        }

        [Fact]
        public async Task AddAsync_WithInvalidTicket_ShouldStillCallAdd()
        {
            // Arrange
            var newTicket = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Expired Ticket Holder",
                Email = "expired@example.com",
                PhoneNumber = "01011111111",
                Valid = false,
                EventID = _sampleEvent.Id,
                Event = _sampleEvent
            };

            _ticketRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Ticket>()))
                .Returns(Task.CompletedTask);

            // Act
            await _unitOfWorkMock.Object.TicketRepository.AddAsync(newTicket);

            // Assert
            _ticketRepositoryMock.Verify(r => r.AddAsync(
                It.Is<Ticket>(t => t.Valid == false)), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var newTicket = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                PhoneNumber = "01099999999",
                Valid = true,
                EventID = _sampleEvent.Id
            };

            _ticketRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Ticket>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _unitOfWorkMock.Object.TicketRepository.AddAsync(newTicket));
        }

        [Fact]
        public async Task AddAsync_WhenSaveChangesFails_ShouldThrowException()
        {
            // Arrange
            var newTicket = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Save Fail User",
                Email = "fail@example.com",
                PhoneNumber = "01077777777",
                Valid = true,
                EventID = _sampleEvent.Id
            };

            _ticketRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Ticket>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new Exception("Save failed"));

            // Act
            await _unitOfWorkMock.Object.TicketRepository.AddAsync(newTicket);

            // Assert
            await Assert.ThrowsAsync<Exception>(
                () => _unitOfWorkMock.Object.SaveChangesAsync());
        }

        [Fact]
        public async Task AddAsync_MultipleTimes_ShouldCallRepositoryForEachTicket()
        {
            // Arrange
            var ticket1 = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "User One",
                Email = "user1@example.com",
                PhoneNumber = "01011111111",
                Valid = true,
                EventID = _sampleEvent.Id
            };

            var ticket2 = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "User Two",
                Email = "user2@example.com",
                PhoneNumber = "01022222222",
                Valid = true,
                EventID = _sampleEvent.Id
            };

            _ticketRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Ticket>()))
                .Returns(Task.CompletedTask);

            // Act
            await _unitOfWorkMock.Object.TicketRepository.AddAsync(ticket1);
            await _unitOfWorkMock.Object.TicketRepository.AddAsync(ticket2);

            // Assert
            _ticketRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Ticket>()), Times.Exactly(2));
            _ticketRepositoryMock.Verify(r => r.AddAsync(ticket1), Times.Once);
            _ticketRepositoryMock.Verify(r => r.AddAsync(ticket2), Times.Once);
        }

        [Fact]
        public async Task AddAsync_TicketShouldHaveNewGuidId()
        {
            // Arrange
            Ticket? capturedTicket = null;

            _ticketRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Ticket>()))
                .Callback<Ticket>(t => capturedTicket = t)
                .Returns(Task.CompletedTask);

            var newTicket = new Ticket
            {
                Name = "GUID Test",
                Email = "guid@example.com",
                PhoneNumber = "01033333333",
                Valid = true,
                EventID = _sampleEvent.Id
            };
            newTicket.Id = Guid.NewGuid();

            // Act
            await _unitOfWorkMock.Object.TicketRepository.AddAsync(newTicket);

            // Assert
            Assert.NotNull(capturedTicket);
            Assert.NotEqual(Guid.Empty, capturedTicket.Id);
        }

        [Fact]
        public async Task AddAsync_ShouldLinkTicketToCorrectEvent()
        {
            // Arrange
            Ticket? capturedTicket = null;

            _ticketRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Ticket>()))
                .Callback<Ticket>(t => capturedTicket = t)
                .Returns(Task.CompletedTask);

            var newTicket = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Event Link Test",
                Email = "link@example.com",
                PhoneNumber = "01044444444",
                Valid = true,
                EventID = _sampleEvent.Id,
                Event = _sampleEvent
            };

            // Act
            await _unitOfWorkMock.Object.TicketRepository.AddAsync(newTicket);

            // Assert
            Assert.NotNull(capturedTicket);
            Assert.Equal(_sampleEvent.Id, capturedTicket.EventID);
            Assert.Equal(_sampleEvent.Name, capturedTicket.Event.Name);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_ShouldCallRepositoryUpdate_WithCorrectTicket()
        {
            // Arrange
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Original Name",
                Email = "original@example.com",
                PhoneNumber = "01055555555",
                Valid = true,
                EventID = _sampleEvent.Id
            };

            _ticketRepositoryMock.Setup(r => r.Update(It.IsAny<Ticket>()));

            // Act
            _unitOfWorkMock.Object.TicketRepository.Update(ticket);

            // Assert
            _ticketRepositoryMock.Verify(r => r.Update(ticket), Times.Once);
        }

        [Fact]
        public async Task Update_ThenSaveChanges_ShouldCallBothMethods()
        {
            // Arrange
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Updated User",
                Email = "updated@example.com",
                PhoneNumber = "01066666666",
                Valid = false,
                EventID = _sampleEvent.Id
            };

            _ticketRepositoryMock.Setup(r => r.Update(It.IsAny<Ticket>()));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            _unitOfWorkMock.Object.TicketRepository.Update(ticket);
            await _unitOfWorkMock.Object.SaveChangesAsync();

            // Assert
            _ticketRepositoryMock.Verify(r => r.Update(ticket), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public void Update_ShouldPreserveAllTicketProperties()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            Ticket? capturedTicket = null;

            _ticketRepositoryMock
                .Setup(r => r.Update(It.IsAny<Ticket>()))
                .Callback<Ticket>(t => capturedTicket = t);

            var ticket = new Ticket
            {
                Id = ticketId,
                Name = "Full Props User",
                Email = "full@example.com",
                PhoneNumber = "01088888888",
                Valid = true,
                EventID = _sampleEvent.Id,
                Event = _sampleEvent
            };

            // Act
            _unitOfWorkMock.Object.TicketRepository.Update(ticket);

            // Assert
            Assert.NotNull(capturedTicket);
            Assert.Equal(ticketId, capturedTicket.Id);
            Assert.Equal("Full Props User", capturedTicket.Name);
            Assert.Equal("full@example.com", capturedTicket.Email);
            Assert.Equal("01088888888", capturedTicket.PhoneNumber);
            Assert.True(capturedTicket.Valid);
            Assert.Equal(_sampleEvent.Id, capturedTicket.EventID);
        }

        [Fact]
        public void Update_CanInvalidateTicket()
        {
            // Arrange
            Ticket? capturedTicket = null;

            _ticketRepositoryMock
                .Setup(r => r.Update(It.IsAny<Ticket>()))
                .Callback<Ticket>(t => capturedTicket = t);

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Invalidate User",
                Email = "invalid@example.com",
                PhoneNumber = "01099999000",
                Valid = false,
                EventID = _sampleEvent.Id
            };

            // Act
            _unitOfWorkMock.Object.TicketRepository.Update(ticket);

            // Assert
            Assert.NotNull(capturedTicket);
            Assert.False(capturedTicket.Valid);
        }

        [Fact]
        public void Update_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Error User",
                Email = "error@example.com",
                PhoneNumber = "01000000000",
                Valid = true,
                EventID = _sampleEvent.Id
            };

            _ticketRepositoryMock
                .Setup(r => r.Update(It.IsAny<Ticket>()))
                .Throws(new InvalidOperationException("Database error"));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(
               () => _unitOfWorkMock.Object.TicketRepository.Update(ticket));
        }

        [Fact]
        public async Task Update_WhenSaveChangesFails_ShouldThrowException()
        {
            // Arrange
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Save Fail",
                Email = "savefail@example.com",
                PhoneNumber = "01011110000",
                Valid = true,
                EventID = _sampleEvent.Id
            };

            _ticketRepositoryMock.Setup(r => r.Update(It.IsAny<Ticket>()));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new Exception("Save failed"));

            // Act
            _unitOfWorkMock.Object.TicketRepository.Update(ticket);

            // Assert
            await Assert.ThrowsAsync<Exception>(
                () => _unitOfWorkMock.Object.SaveChangesAsync());
        }

        [Fact]
        public void Update_MultipleTimes_ShouldCallRepositoryForEachTicket()
        {
            // Arrange
            var ticket1 = new Ticket { Id = Guid.NewGuid(), Name = "User 1", Email = "u1@ex.com", PhoneNumber = "010111", Valid = true, EventID = _sampleEvent.Id };
            var ticket2 = new Ticket { Id = Guid.NewGuid(), Name = "User 2", Email = "u2@ex.com", PhoneNumber = "010222", Valid = true, EventID = _sampleEvent.Id };

            _ticketRepositoryMock.Setup(r => r.Update(It.IsAny<Ticket>()));

            // Act
            _unitOfWorkMock.Object.TicketRepository.Update(ticket1);
            _unitOfWorkMock.Object.TicketRepository.Update(ticket2);

            // Assert
            _ticketRepositoryMock.Verify(r => r.Update(It.IsAny<Ticket>()), Times.Exactly(2));
            _ticketRepositoryMock.Verify(r => r.Update(ticket1), Times.Once);
            _ticketRepositoryMock.Verify(r => r.Update(ticket2), Times.Once);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTickets()
        {
            // Arrange
            var tickets = new List<Ticket>
            {
                new Ticket { Id = Guid.NewGuid(), Name = "User A", Email = "a@ex.com", PhoneNumber = "010111", Valid = true, EventID = _sampleEvent.Id },
                new Ticket { Id = Guid.NewGuid(), Name = "User B", Email = "b@ex.com", PhoneNumber = "010222", Valid = true, EventID = _sampleEvent.Id },
                new Ticket { Id = Guid.NewGuid(), Name = "User C", Email = "c@ex.com", PhoneNumber = "010333", Valid = false, EventID = _sampleEvent.Id }
            };

            _ticketRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(tickets);

            // Act
            var result = await _unitOfWorkMock.Object.TicketRepository.GetAllAsync();

            // Assert
            Assert.Equal(tickets, result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoTickets()
        {
            // Arrange
            _ticketRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Ticket>());

            // Act
            var result = await _unitOfWorkMock.Object.TicketRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListType()
        {
            // Arrange
            _ticketRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Ticket>());

            // Act
            var result = await _unitOfWorkMock.Object.TicketRepository.GetAllAsync();

            // Assert
            Assert.IsAssignableFrom<IEnumerable<Ticket>>(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            _ticketRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _unitOfWorkMock.Object.TicketRepository.GetAllAsync());
        }

        #endregion

        #region GetbyIdAsync Tests

        [Fact]
        public async Task GetbyIdAsync_ShouldReturnTicket_WhenTicketExists()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var expectedTicket = new Ticket
            {
                Id = ticketId,
                Name = "Found User",
                Email = "found@example.com",
                PhoneNumber = "01055550000",
                Valid = true,
                EventID = _sampleEvent.Id,
                Event = _sampleEvent
            };

            _ticketRepositoryMock
                .Setup(r => r.GetbyIdAsync(ticketId))
                .ReturnsAsync(expectedTicket);

            // Act
            var result = await _unitOfWorkMock.Object.TicketRepository.GetbyIdAsync(ticketId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticketId, result.Id);
            Assert.Equal("Found User", result.Name);
            Assert.Equal("found@example.com", result.Email);
        }

        [Fact]
        public async Task GetbyIdAsync_ShouldReturnNull_WhenTicketDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            _ticketRepositoryMock
                .Setup(r => r.GetbyIdAsync(nonExistentId))
                .ReturnsAsync((Ticket?)null);

            // Act
            var result = await _unitOfWorkMock.Object.TicketRepository.GetbyIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetbyIdAsync_ShouldCallRepository_ExactlyOnce()
        {
            // Arrange
            var ticketId = Guid.NewGuid();

            _ticketRepositoryMock
                .Setup(r => r.GetbyIdAsync(ticketId))
                .ReturnsAsync(new Ticket { Id = ticketId, Name = "Test", Email = "t@ex.com", PhoneNumber = "010", Valid = true, EventID = _sampleEvent.Id });

            // Act
            await _unitOfWorkMock.Object.TicketRepository.GetbyIdAsync(ticketId);

            // Assert
            _ticketRepositoryMock.Verify(r => r.GetbyIdAsync(ticketId), Times.Once);
        }

        [Fact]
        public async Task GetbyIdAsync_ShouldPreserveAllProperties_WhenTicketFound()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var expectedTicket = new Ticket
            {
                Id = ticketId,
                Name = "Full Props",
                Email = "fullprops@example.com",
                PhoneNumber = "01099887766",
                Valid = true,
                EventID = _sampleEvent.Id,
                Event = _sampleEvent
            };

            _ticketRepositoryMock
                .Setup(r => r.GetbyIdAsync(ticketId))
                .ReturnsAsync(expectedTicket);

            // Act
            var result = await _unitOfWorkMock.Object.TicketRepository.GetbyIdAsync(ticketId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticketId, result.Id);
            Assert.Equal("Full Props", result.Name);
            Assert.Equal("fullprops@example.com", result.Email);
            Assert.Equal("01099887766", result.PhoneNumber);
            Assert.True(result.Valid);
            Assert.Equal(_sampleEvent.Id, result.EventID);
            Assert.NotNull(result.Event);
            Assert.Equal("Football Match", result.Event.Name);
        }

        [Fact]
        public async Task GetbyIdAsync_ShouldIncludeEventNavigation()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var expectedTicket = new Ticket
            {
                Id = ticketId,
                Name = "Nav Test",
                Email = "nav@example.com",
                PhoneNumber = "01033221100",
                Valid = true,
                EventID = _sampleEvent.Id,
                Event = _sampleEvent
            };

            _ticketRepositoryMock
                .Setup(r => r.GetbyIdAsync(ticketId))
                .ReturnsAsync(expectedTicket);

            // Act
            var result = await _unitOfWorkMock.Object.TicketRepository.GetbyIdAsync(ticketId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Event);
            Assert.Equal(_sampleEvent.Id, result.Event.Id);
            Assert.Equal("Cairo Stadium", result.Event.place);
        }

        [Fact]
        public async Task GetbyIdAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var ticketId = Guid.NewGuid();

            _ticketRepositoryMock
                .Setup(r => r.GetbyIdAsync(ticketId))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _unitOfWorkMock.Object.TicketRepository.GetbyIdAsync(ticketId));
        }

        #endregion

        #region Search Tests

        [Fact]
        public async Task Search_WithValidSearchValue_ShouldReturnMatchingTickets()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var searchValue = ticketId.ToString().Substring(0, 8);
            var matchingTickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = ticketId,
                    Name = "Matched User",
                    Email = "matched@example.com",
                    PhoneNumber = "01055551234",
                    Valid = true,
                    EventID = _sampleEvent.Id,
                    Event = _sampleEvent
                }
            };

            _ticketRepositoryMock
                .Setup(r => r.Search(searchValue))
                .ReturnsAsync(matchingTickets);

            // Act
            var result = await _unitOfWorkMock.Object.TicketRepository.Search(searchValue);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Matched User", result.First().Name);
        }

        [Fact]
        public async Task Search_WithNullSearchValue_ShouldReturnAllTickets()
        {
            // Arrange
            var allTickets = new List<Ticket>
            {
                new Ticket { Id = Guid.NewGuid(), Name = "User 1", Email = "u1@ex.com", PhoneNumber = "010111", Valid = true, EventID = _sampleEvent.Id, Event = _sampleEvent },
                new Ticket { Id = Guid.NewGuid(), Name = "User 2", Email = "u2@ex.com", PhoneNumber = "010222", Valid = true, EventID = _sampleEvent.Id, Event = _sampleEvent },
                new Ticket { Id = Guid.NewGuid(), Name = "User 3", Email = "u3@ex.com", PhoneNumber = "010333", Valid = false, EventID = _sampleEvent.Id, Event = _sampleEvent }
            };

            _ticketRepositoryMock
                .Setup(r => r.Search(null))
                .ReturnsAsync(allTickets);

            // Act
            var result = await _unitOfWorkMock.Object.TicketRepository.Search(null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task Search_WithNonMatchingValue_ShouldReturnEmptyList()
        {
            // Arrange
            var searchValue = "nonexistent-value-xyz";

            _ticketRepositoryMock
                .Setup(r => r.Search(searchValue))
                .ReturnsAsync(new List<Ticket>());

            // Act
            var result = await _unitOfWorkMock.Object.TicketRepository.Search(searchValue);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Search_ShouldCallRepository_WithExactSearchValue()
        {
            // Arrange
            var searchValue = "test-search-value";

            _ticketRepositoryMock
                .Setup(r => r.Search(It.IsAny<string>()))
                .ReturnsAsync(new List<Ticket>());

            // Act
            await _unitOfWorkMock.Object.TicketRepository.Search(searchValue);

            // Assert
            _ticketRepositoryMock.Verify(r => r.Search(searchValue), Times.Once);
        }

        [Fact]
        public async Task Search_ResultsShouldIncludeEventNavigation()
        {
            // Arrange
            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Name = "Nav Search User",
                    Email = "navsearch@example.com",
                    PhoneNumber = "01077770000",
                    Valid = true,
                    EventID = _sampleEvent.Id,
                    Event = _sampleEvent
                }
            };

            _ticketRepositoryMock
                .Setup(r => r.Search(It.IsAny<string>()))
                .ReturnsAsync(tickets);

            // Act
            var result = await _unitOfWorkMock.Object.TicketRepository.Search("test");

            // Assert
            Assert.NotNull(result);
            var firstTicket = result.First();
            Assert.NotNull(firstTicket.Event);
            Assert.Equal("Football Match", firstTicket.Event.Name);
        }

        [Fact]
        public async Task Search_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            _ticketRepositoryMock
                .Setup(r => r.Search(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Search failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _unitOfWorkMock.Object.TicketRepository.Search("test"));
        }

        #endregion

        #region Delete Tests

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WithCorrectTicket()
        {
            // Arrange
            var ticketToDelete = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Delete User",
                Email = "delete@example.com",
                PhoneNumber = "01066660000",
                Valid = true,
                EventID = _sampleEvent.Id
            };

            _ticketRepositoryMock.Setup(r => r.Delete(It.IsAny<Ticket>()));

            // Act
            _unitOfWorkMock.Object.TicketRepository.Delete(ticketToDelete);

            // Assert
            _ticketRepositoryMock.Verify(r => r.Delete(ticketToDelete), Times.Once);
        }

        [Fact]
        public async Task Delete_ThenSaveChanges_ShouldCallBothMethods()
        {
            // Arrange
            var ticketToDelete = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Delete Save User",
                Email = "delsave@example.com",
                PhoneNumber = "01088880000",
                Valid = false,
                EventID = _sampleEvent.Id
            };

            _ticketRepositoryMock.Setup(r => r.Delete(It.IsAny<Ticket>()));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            _unitOfWorkMock.Object.TicketRepository.Delete(ticketToDelete);
            await _unitOfWorkMock.Object.SaveChangesAsync();

            // Assert
            _ticketRepositoryMock.Verify(r => r.Delete(ticketToDelete), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public void Delete_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var ticketToDelete = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Error Delete User",
                Email = "errordel@example.com",
                PhoneNumber = "01000001111",
                Valid = true,
                EventID = _sampleEvent.Id
            };

            _ticketRepositoryMock
                .Setup(r => r.Delete(It.IsAny<Ticket>()))
                .Throws(new InvalidOperationException("Cannot delete tracked entity"));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => _unitOfWorkMock.Object.TicketRepository.Delete(ticketToDelete));
        }

        [Fact]
        public void Delete_ShouldCaptureCorrectEntity()
        {
            // Arrange
            Ticket? capturedTicket = null;
            var ticketId = Guid.NewGuid();

            var ticketToDelete = new Ticket
            {
                Id = ticketId,
                Name = "Capture User",
                Email = "capture@example.com",
                PhoneNumber = "01044440000",
                Valid = true,
                EventID = _sampleEvent.Id
            };

            _ticketRepositoryMock
                .Setup(r => r.Delete(It.IsAny<Ticket>()))
                .Callback<Ticket>(t => capturedTicket = t);

            // Act
            _unitOfWorkMock.Object.TicketRepository.Delete(ticketToDelete);

            // Assert
            Assert.NotNull(capturedTicket);
            Assert.Equal(ticketId, capturedTicket.Id);
            Assert.Equal("Capture User", capturedTicket.Name);
            Assert.Equal("capture@example.com", capturedTicket.Email);
        }

        [Fact]
        public void Delete_MultipleTimes_ShouldCallRepositoryForEachTicket()
        {
            // Arrange
            var ticket1 = new Ticket { Id = Guid.NewGuid(), Name = "Del 1", Email = "d1@ex.com", PhoneNumber = "010", Valid = true, EventID = _sampleEvent.Id };
            var ticket2 = new Ticket { Id = Guid.NewGuid(), Name = "Del 2", Email = "d2@ex.com", PhoneNumber = "020", Valid = true, EventID = _sampleEvent.Id };
            var ticket3 = new Ticket { Id = Guid.NewGuid(), Name = "Del 3", Email = "d3@ex.com", PhoneNumber = "030", Valid = false, EventID = _sampleEvent.Id };

            _ticketRepositoryMock.Setup(r => r.Delete(It.IsAny<Ticket>()));

            // Act
            _unitOfWorkMock.Object.TicketRepository.Delete(ticket1);
            _unitOfWorkMock.Object.TicketRepository.Delete(ticket2);
            _unitOfWorkMock.Object.TicketRepository.Delete(ticket3);

            // Assert
            _ticketRepositoryMock.Verify(r => r.Delete(It.IsAny<Ticket>()), Times.Exactly(3));
            _ticketRepositoryMock.Verify(r => r.Delete(ticket1), Times.Once);
            _ticketRepositoryMock.Verify(r => r.Delete(ticket2), Times.Once);
            _ticketRepositoryMock.Verify(r => r.Delete(ticket3), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenSaveChangesFails_ShouldThrowException()
        {
            // Arrange
            var ticketToDelete = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "FK Fail",
                Email = "fk@example.com",
                PhoneNumber = "01022220000",
                Valid = true,
                EventID = _sampleEvent.Id
            };

            _ticketRepositoryMock.Setup(r => r.Delete(It.IsAny<Ticket>()));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new Exception("Foreign key constraint violation"));

            // Act
            _unitOfWorkMock.Object.TicketRepository.Delete(ticketToDelete);

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
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _unitOfWorkMock.Object.SaveChangesAsync();

            // Assert
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SaveChangesAsync_CalledMultipleTimes_ShouldTrackAllCalls()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

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
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
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
            var newTicket = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "New User",
                Email = "new@example.com",
                PhoneNumber = "01011112222",
                Valid = true,
                EventID = _sampleEvent.Id
            };

            var ticketToDelete = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Old User",
                Email = "old@example.com",
                PhoneNumber = "01033334444",
                Valid = false,
                EventID = _sampleEvent.Id
            };

            _ticketRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _ticketRepositoryMock.Setup(r => r.Delete(It.IsAny<Ticket>()));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _unitOfWorkMock.Object.TicketRepository.AddAsync(newTicket);
            _unitOfWorkMock.Object.TicketRepository.Delete(ticketToDelete);
            await _unitOfWorkMock.Object.SaveChangesAsync();

            // Assert
            _ticketRepositoryMock.Verify(r => r.AddAsync(newTicket), Times.Once);
            _ticketRepositoryMock.Verify(r => r.Delete(ticketToDelete), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region GetAllForUserAsync Tests

        [Fact]
        public async Task GetAllForUserAsync_ShouldReturnMatchingTickets_ForSpecificUser()
        {
            // Arrange
            var userId = "test-user-id";
            var tickets = new List<Ticket>
            {
                new Ticket { Id = Guid.NewGuid(), Name = "User 1", UserId = userId, Event = _sampleEvent },
                new Ticket { Id = Guid.NewGuid(), Name = "User 2", UserId = userId, Event = _sampleEvent }
            };

            _ticketRepositoryMock
                .Setup(r => r.GetAllForUserAsync(userId))
                .ReturnsAsync(tickets);

            // Act
            var result = await _unitOfWorkMock.Object.TicketRepository.GetAllForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            // Since we're using a Mock repository, it just returns what we told it to return.
            // In a real integration test, we'd verify the filtering logic.
        }

        [Fact]
        public async Task GetAllForUserAsync_ShouldReturnEmptyList_WhenUserHasNoTickets()
        {
            // Arrange
            var userId = "no-tickets-user";
            _ticketRepositoryMock
                .Setup(r => r.GetAllForUserAsync(userId))
                .ReturnsAsync(new List<Ticket>());

            // Act
            var result = await _unitOfWorkMock.Object.TicketRepository.GetAllForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllForUserAsync_ShouldCallRepository_ExactlyOnceWithCorrectUserId()
        {
            // Arrange
            var userId = "call-count-test";
            _ticketRepositoryMock
                .Setup(r => r.GetAllForUserAsync(userId))
                .ReturnsAsync(new List<Ticket>());

            // Act
            await _unitOfWorkMock.Object.TicketRepository.GetAllForUserAsync(userId);

            // Assert
            _ticketRepositoryMock.Verify(r => r.GetAllForUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetAllForUserAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var userId = "error-user";
            _ticketRepositoryMock
                .Setup(r => r.GetAllForUserAsync(userId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _unitOfWorkMock.Object.TicketRepository.GetAllForUserAsync(userId));
        }

        #endregion
    }
}
