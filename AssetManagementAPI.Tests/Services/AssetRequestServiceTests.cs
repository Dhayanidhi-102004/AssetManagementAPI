using NUnit.Framework;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AssetManagementAPI.Data;
using AssetManagementAPI.Models;
using AssetManagementAPI.Services;

namespace AssetManagementAPI.Tests.Services
{
    public class AssetRequestServiceTests
    {
        private AppDbContext _context;
        private AssetRequestService _service;
        private Mock<ILogger<AssetRequestService>> _logger;
        private Mock<IAuditService> _auditService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            _logger = new Mock<ILogger<AssetRequestService>>();
            _auditService = new Mock<IAuditService>();

            _service = new AssetRequestService(
                _context,
                _logger.Object,
                _auditService.Object
            );
        }
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void CreateAssetRequest_ShouldReturnTrue_WhenValid()
        {
            _context.Assets.Add(new Asset
            {
                Id=1,
                AssetName="Laptop",
                Status="Available",
                Category="Electronics"
            });
            _context.Employees.Add(new Employee
            {
                Id = 1,
                Name = "Dhayanidhi",
                Email = "test@gmail.com",
                Password = "123",
                Role = "Employee",
                Gender = "Male",
                ContactNumber = "9876543210",
                Address = "Chennai"
            });
            _context.SaveChanges();
            var result=_service.CreateAssetRequest(
                1,"test@gmail.com");
            Assert.That(result, Is.True);
            Assert.That(_context.AssetRequests.Count(),Is.EqualTo(1));
        }
        [Test]
        public void CreateAssetRequest_ShouldReturnFalse_WhenAssetNotFound()
        {
            _context.Employees.Add(new Employee
            {
                Id = 1,
                Name = "Dhayanidhi",
                Email = "test@gmail.com",
                Password = "123",
                Role = "Employee",
                Gender = "Male",
                ContactNumber = "9876543210",
                Address = "Chennai"
            });

            _context.SaveChanges();

            var result = _service.CreateAssetRequest(
                999,
                "test@gmail.com"
            );

            Assert.That(result, Is.False);
            Assert.That(_context.AssetRequests.Count(), Is.EqualTo(0));
        }
        [Test]
        public void ApproveAssetRequest_ShouldReturnTrue_WhenRequestIsPending()
        {
            _context.Assets.Add(new Asset
            {
                Id = 1,
                AssetName = "Laptop",
                Status = "Pending",
                Category = "Electronics"
            });
            _context.AssetRequests.Add(new AssetRequest
            {
                Id = 1,
                AssetId = 1,
                EmployeeId = 1,
                Status = "Pending",
                RequestDate = DateTime.Now
            });
            _context.SaveChanges();
            var result = _service.ApproveAssetRequest(1);
            var request = _context.AssetRequests.First();
            var asset = _context.Assets.First();
            Assert.That(result, Is.True);
            Assert.That(request.Status, Is.EqualTo("Approved"));
            Assert.That(asset.Status, Is.EqualTo("Assigned"));
            Assert.That(_context.AssetAllocations.Count(), Is.EqualTo(1));
        }
        [Test]
        public void ApproveAssetRequest_ShouldReturnFalse_WhenRequestNotFound()
        {
            var result = _service.ApproveAssetRequest(999);

            Assert.That(result, Is.False);
            Assert.That(_context.AssetAllocations.Count(), Is.EqualTo(0));
        }
        [Test]
        public void RejectPendingRequests_ShouldRejectRequest_WhenRequestIsPending()
        {
            _context.Assets.Add(new Asset
            {
                Id = 1,
                AssetName = "Laptop",
                Status = "Pending",
                Category = "Electronics"
            });
            _context.AssetRequests.Add(new AssetRequest
            {
                Id = 1,
                AssetId = 1,
                EmployeeId = 1,
                Status = "Pending",
                RequestDate = DateTime.Now
            });
            _context.SaveChanges();
            var result = _service.RejectPendingRequests(1);
            var request = _context.AssetRequests.First();
            var asset = _context.Assets.First();
            Assert.That(result, Is.True);
            Assert.That(request.Status, Is.EqualTo("Rejected"));
            Assert.That(asset.Status, Is.EqualTo("Available"));
            Assert.That(_context.AssetAllocations.Count(), Is.EqualTo(0));
        }
        [Test]
        public void RejectPendingRequests_ShouldReturnFalse_WhenRequestNotFound()
        {
            var result = _service.RejectPendingRequests(999);

            Assert.That(result, Is.False);
        }
    }
}