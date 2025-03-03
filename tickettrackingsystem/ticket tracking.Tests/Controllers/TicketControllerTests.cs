using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ticket_tracking_system.Controllers;
using ticket_tracking_system.Data;
using ticket_tracking_system.DTOs;
using ticket_tracking_system.Models;
using ticket_tracking_system.Services;

namespace ticket_tracking.Tests.Controllers
{
    [TestFixture]
    public class CustomerTicketsControllerTests
    {
        private AppDbContext _mockContext;
        private Mock<IEmailService> _mockEmailService;
        private CustomerTicketsController _controller;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TicketTrackingTest")
                .Options;

            _mockContext = new AppDbContext(options);
            _mockEmailService = new Mock<IEmailService>();
            _controller = new CustomerTicketsController(_mockContext, _mockEmailService.Object);

            // Mock the ClaimsPrincipal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "Test User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            // Set the User in the controller context
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            ClearDatabase();
            SeedDatabase();
        }

        private void ClearDatabase()
        {
            _mockContext.Users.RemoveRange(_mockContext.Users);
            _mockContext.Tickets.RemoveRange(_mockContext.Tickets);
            _mockContext.SaveChanges();
        }

        private void SeedDatabase()
        {
            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "testuser@example.com",
                Role = "Customer",
                PasswordHash = "hashedpassword"
            };

            var ticket = new Ticket
            {
                Id = 1,
                Title = "Test Ticket",
                Description = "Test Description",
                Priority = "High",
                Status = "Open",
                CustomerId = 1,
                CreatedAt = DateTime.UtcNow
            };

            _mockContext.Users.Add(user);
            _mockContext.Tickets.Add(ticket);
            _mockContext.SaveChanges();
        }

        [Test]
        public async Task CreateTicket_ReturnsOkResult_WithCreatedTicket()
        {
            // Arrange
            var dto = new CreateTicketDto
            {
                Title = "New Ticket",
                Description = "New Description",
                Priority = "Medium"
            };

            // Act
            var result = await _controller.CreateTicket(dto);

            // Assert
            var okResult = result as OkObjectResult;

            var returnTicket = okResult.Value as Ticket;
            Assert.That(dto.Title, Is.EqualTo(returnTicket.Title));
            Assert.That(dto.Description, Is.EqualTo(returnTicket.Description));
            Assert.That(dto.Priority, Is.EqualTo(returnTicket.Priority));
        }

        [Test]
        public async Task GetTicket_ReturnsOkResult_WithTicket()
        {
            // Arrange
            var ticketId = 1;

            // Act
            var result = await _controller.GetTicket(ticketId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetTicket_ReturnsNotFound_WhenTicketNotFound()
        {
            // Arrange
            var ticketId = 99;

            // Act
            var result = await _controller.GetTicket(ticketId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdateTicket_ReturnsOkResult_WithUpdatedTicket()
        {
            // Arrange
            var ticketId = 1;
            var dto = new TicketDto
            {
                Id = ticketId,
                Title = "Updated Title",
                Description = "Updated Description",
                Priority = "Low",
                Status = "Closed"
            };

            // Act
            var result = await _controller.UpdateTicket(ticketId, dto);

            // Assert
            var okResult = result as OkObjectResult;
            var returnTicket = okResult.Value as Ticket;
            Assert.That(dto.Title, Is.EqualTo(returnTicket.Title));
        }

        [Test]
        public async Task UpdateTicket_ReturnsNotFound_WhenTicketNotFound()
        {
            // Arrange
            var ticketId = 99;
            var dto = new TicketDto
            {
                Id = ticketId,
                Title = "Updated Title",
                Description = "Updated Description",
                Priority = "Low",
                Status = "Closed"
            };

            // Act
            var result = await _controller.UpdateTicket(ticketId, dto);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteTicket_ReturnsNoContent_WhenTicketDeleted()
        {
            // Arrange
            var ticketId = 1;

            // Act
            var result = await _controller.DeleteTicket(ticketId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteTicket_ReturnsNotFound_WhenTicketNotFound()
        {
            // Arrange
            var ticketId = 99;

            // Act
            var result = await _controller.DeleteTicket(ticketId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}


