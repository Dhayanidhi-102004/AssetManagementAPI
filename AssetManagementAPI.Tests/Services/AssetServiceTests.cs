using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetManagementAPI.Data;
using AssetManagementAPI.DTOs.Asset;
using AssetManagementAPI.Models;
using AssetManagementAPI.Services;
using AutoMapper;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace AssetManagementAPI.Tests.Services
{
    public class AssetServiceTests
    {
        private AppDbContext _context;
        private AssetService _service;
        private Mock<IMapper> _mapper;
        private Mock<ILogger<AssetService>> _logger;
        private Mock<IAuditService> _auditService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .Options;
            _context = new AppDbContext(options);
            _mapper = new Mock<IMapper>();
            _logger = new Mock<ILogger<AssetService>>();
            _auditService = new Mock<IAuditService>();
            _service = new AssetService(
                _context,
                _mapper.Object,
                _logger.Object,
                _auditService.Object);
        }
        [TearDown]
        public void TearDown()
        {
            _context.Dispose(); 
        }
        [Test]
        public void DeleteAssetById_ShouldReturnTrue_WhenAssetExists()
        {
            _context.Assets.Add(new Asset
            {
                Id = 1,
                AssetName = "Laptop",
                Category = "Electronics",
                Status = "Available"
            });

            _context.SaveChanges();

            var result = _service.DeleteAssetById(1);

            Assert.That(result, Is.True);
            Assert.That(_context.Assets.Count(), Is.EqualTo(0));
        }
        [Test]
        public void DeleteAssetById_ShouldReturnFalse_WhenAssetNotFound()
        {
            var result = _service.DeleteAssetById(999);

            Assert.That(result, Is.False);
            Assert.That(_context.Assets.Count(), Is.EqualTo(0));
        }
        [Test]
        public void ReturnAsset_ShouldReturnTrue_WhenAllocationExists()
        {
            _context.Assets.Add(new Asset
            {
                Id = 1,
                AssetName = "Laptop",
                Status = "Assigned",
                Category = "Electronics"
            });

            _context.AssetAllocations.Add(new AssetAllocation
            {
                Id = 1,
                AssetId = 1,
                EmployeeId = 1,
                AssignedDate = DateTime.Now,
                Status = "Active"
            });

            _context.SaveChanges();

            var result = _service.ReturnAsset(1);

            var asset = _context.Assets.First();
            var allocation = _context.AssetAllocations.First();

            Assert.That(result, Is.True);
            Assert.That(asset.Status, Is.EqualTo("Available"));
            Assert.That(allocation.Status, Is.EqualTo("Returned"));
            Assert.That(allocation.ReturnedDate, Is.Not.Null);
        }
        [Test]
        public void ReturnAsset_ShouldReturnFalse_WhenAllocationNotFound()
        {
            var result = _service.ReturnAsset(999);

            Assert.That(result, Is.False);
        }
        [Test]
        public void CreateAsset_ShouldReturnSuccess_WhenValid()
        {
            var dto = new CreateAssetDTO
            {
                AssetName = "Laptop",
                Category = "Electronics",
                SerialNumber = "SN001"
            };

            var mappedAsset = new Asset
            {
                Id = 1,
                AssetName = "Laptop",
                Category = "Electronics",
                SerialNumber = "SN001",
                Status = "Available"
            };

            _mapper.Setup(m =>
                m.Map<Asset>(It.IsAny<CreateAssetDTO>()))
                .Returns(mappedAsset);

            var result = _service.CreateAsset(dto);

            Assert.That(result, Is.EqualTo("Asset created successfully"));
            Assert.That(_context.Assets.Count(), Is.EqualTo(1));
        }
        [Test]
        public void GetAssetById_ShouldReturnAsset_WhenAssetExists()
        {
            _context.Assets.Add(new Asset
            {
                Id = 1,
                AssetName = "Laptop",
                Category = "Electronics",
                Status = "Available"
            });

            _context.SaveChanges();

            _mapper.Setup(m =>
                m.Map<AssetResponseDTO>(It.IsAny<Asset>()))
                .Returns(new AssetResponseDTO
                {
                    Id = 1,
                    AssetName = "Laptop",
                    Category = "Electronics",
                    Status = "Available"
                });

            var result = _service.GetAssetById(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AssetName, Is.EqualTo("Laptop"));
        }
        [Test]
        public void GetAssetById_ShouldReturnNull_WhenAssetNotFound()
        {
            var result = _service.GetAssetById(999);

            Assert.That(result, Is.Null);
        }
        [Test]
        public void UpdateAsset_ShouldReturnTrue_WhenAssetExists()
        {
            _context.Assets.Add(new Asset
            {
                Id = 1,
                AssetName = "Laptop",
                Category = "Electronics",
                Status = "Available"
            });

            _context.SaveChanges();

            var dto = new UpdateAssetDTO
            {
                AssetName = "Updated Laptop"
            };

            _mapper.Setup(m =>
                m.Map(It.IsAny<UpdateAssetDTO>(), It.IsAny<Asset>()))
                .Callback<UpdateAssetDTO, Asset>((src, dest) =>
                {
                    dest.AssetName = src.AssetName;
                });

            var result = _service.UpdateAsset(1, dto);

            Assert.That(result, Is.True);
            Assert.That(_context.Assets.First().AssetName,
                Is.EqualTo("Updated Laptop"));
        }
        [Test]
        public void UpdateAsset_ShouldReturnFalse_WhenAssetNotFound()
        {
            var dto = new UpdateAssetDTO();

            var result = _service.UpdateAsset(999, dto);

            Assert.That(result, Is.False);
        }
        [Test]
        public void GetAvailableAssets_ShouldReturnOnlyAvailableAssets()
        {
            _context.Assets.AddRange(
                new Asset
                {
                    Id = 1,
                    AssetName = "Laptop",
                    Status = "Available"
                },
                new Asset
                {
                    Id = 2,
                    AssetName = "Phone",
                    Status = "Assigned"
                });

            _context.SaveChanges();

            _mapper.Setup(m =>
                m.Map<List<AssetResponseDTO>>(It.IsAny<List<Asset>>()))
                .Returns(new List<AssetResponseDTO>
                {
            new AssetResponseDTO
            {
                Id = 1,
                AssetName = "Laptop"
            }
                });

            var result = _service.GetAvailableAssets(1, 10);

            Assert.That(result.Data.Count, Is.EqualTo(1));
        }
        [Test]
        public void GetMyAssets_ShouldReturnEmployeeAssets()
        {
            _context.Employees.Add(new Employee
            {
                Id = 1,
                Email = "test@gmail.com",
                Name = "Dhaya"
            });

            _context.Assets.Add(new Asset
            {
                Id = 1,
                AssetName = "Laptop",
                Status = "Assigned"
            });

            _context.AssetAllocations.Add(new AssetAllocation
            {
                AssetId = 1,
                EmployeeId = 1,
                Status = "Active"
            });

            _context.SaveChanges();

            _mapper.Setup(m =>
                m.Map<List<AssetResponseDTO>>(It.IsAny<List<Asset>>()))
                .Returns(new List<AssetResponseDTO>
                {
            new AssetResponseDTO
            {
                Id = 1,
                AssetName = "Laptop"
            }
                });

            var result = _service.GetMyAssets("test@gmail.com");

            Assert.That(result.Count, Is.EqualTo(1));
        }
    }
}
