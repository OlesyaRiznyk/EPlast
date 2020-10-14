﻿using AutoMapper;
using EPlast.BLL.DTO.Admin;
using EPlast.BLL.DTO.City;
using EPlast.BLL.Interfaces.Admin;
using EPlast.BLL.Services.City;
using EPlast.DataAccess.Entities;
using EPlast.DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EPlast.Tests.Services.City
{
    [TestFixture]
    public class CityAdministrationServiceTests
    {
        private CityAdministrationService _cityAdministrationService;
        private Mock<IRepositoryWrapper> _repoWrapper;
        private Mock<IMapper> _mapper;
        private Mock<IAdminTypeService> _adminTypeService;
        private Mock<IUserStore<User>> _user;
        private Mock<UserManager<User>> _userManager;

        [SetUp]
        public void SetUp()
        {
            _repoWrapper = new Mock<IRepositoryWrapper>();
            _mapper = new Mock<IMapper>();
            _adminTypeService = new Mock<IAdminTypeService>();
            _user = new Mock<IUserStore<User>>();
            _userManager = new Mock<UserManager<User>>(_user.Object, null, null, null, null, null, null, null, null);
            _cityAdministrationService = new CityAdministrationService(_repoWrapper.Object, _mapper.Object, _adminTypeService.Object, _userManager.Object);
        }

        [Test]
        public async Task GetAdministrationByIdAsync_ReturnsAdministrations()
        {
            // Arrange
            _repoWrapper
                .Setup(r => r.CityAdministration.GetAllAsync(It.IsAny<Expression<Func<CityAdministration, bool>>>(),
                 It.IsAny<Func<IQueryable<CityAdministration>, IIncludableQueryable<CityAdministration, object>>>()))
                     .ReturnsAsync(new List<CityAdministration> { new CityAdministration() { ID = fakeId} });
            _mapper
                .Setup(m => m.Map<IEnumerable<CityAdministration>, IEnumerable<CityAdministrationDTO>>(It.IsAny<IEnumerable<CityAdministration>>()))
                .Returns(new List<CityAdministrationDTO> { new CityAdministrationDTO { ID = fakeId } });

            // Act
            var result = await _cityAdministrationService.GetAdministrationByIdAsync(It.IsAny<int>());

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.FirstOrDefault().ID, fakeId);
        }

        [Test]
        public async Task AddAdministratorAsync_ReturnsAdministrator()
        {
            //Arrange
            _repoWrapper
                .Setup(s => s.CityAdministration.CreateAsync(cityAdm));
            _adminTypeService
                .Setup(a => a.GetAdminTypeByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new AdminTypeDTO());

            //Act
            var result = await _cityAdministrationService.AddAdministratorAsync(cityAdmDTO);

            //Assert
            Assert.IsInstanceOf<CityAdministrationDTO>(result);
        }

        [Test]
        public async Task EditAdministratorAsync_ReturnsEditedAdministratorWithSameId()
        {
            //Arrange
            _adminTypeService
                .Setup(a => a.GetAdminTypeByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new AdminTypeDTO());
            _repoWrapper
                .Setup(r => r.CityAdministration.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CityAdministration, bool>>>(),
                It.IsAny<Func<IQueryable<CityAdministration>,
                IIncludableQueryable<CityAdministration, object>>>()))
                .ReturnsAsync(new CityAdministration());
            _repoWrapper
                .Setup(r => r.CityAdministration.Update(It.IsAny<CityAdministration>()));
            _repoWrapper
                .Setup(r => r.SaveAsync());

            //Act
            var result = await _cityAdministrationService.EditAdministratorAsync(cityAdmDTO);

            //Assert
            _repoWrapper.Verify();
            Assert.IsInstanceOf<CityAdministrationDTO>(result);
        }

        [Test]
        public async Task EditAdministratorAsync_ReturnsEditedAdministratorWithDifferentId()
        {
            //Arrange
            _repoWrapper
                .Setup(r => r.CityAdministration.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CityAdministration, bool>>>(),
                It.IsAny<Func<IQueryable<CityAdministration>,
                IIncludableQueryable<CityAdministration, object>>>()))
                .ReturnsAsync(new CityAdministration());
            _adminTypeService
               .Setup(a => a.GetAdminTypeByNameAsync(It.IsAny<string>()))
               .ReturnsAsync(new AdminTypeDTO
               {
                   AdminTypeName = "Голова Станиці",
                   ID = 3
               });
            _adminTypeService
                .Setup(a => a.GetAdminTypeByIdAsync(It.IsAny<int>()))
               .ReturnsAsync(new AdminTypeDTO
               {
                   AdminTypeName = "Голова Станиці",
                   ID = 3
               });

            //Act
            var result = await _cityAdministrationService.EditAdministratorAsync(cityFakeAdmDTO);

            //Assert
            _repoWrapper.Verify();
            Assert.IsInstanceOf<CityAdministrationDTO>(result);
        }

        [Test]
        public void RemoveAdministratorAsync_ReturnsCorrect()
        {
            //Arrange
            _repoWrapper
                .Setup(r => r.CityAdministration.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<CityAdministration, bool>>>(),
                It.IsAny<Func<IQueryable<CityAdministration>,
                IIncludableQueryable<CityAdministration, object>>>()))
                .ReturnsAsync(cityAdm);
            _adminTypeService
                .Setup(a => a.GetAdminTypeByIdAsync(It.IsAny<int>()))
                .Returns(() => Task<AdminTypeDTO>.Factory.StartNew(() => AdminType));
            _userManager
                .Setup(u => u.FindByIdAsync(It.IsAny<string>()));
            _userManager
                .Setup(u => u.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()));
            _repoWrapper
                .Setup(r => r.CityAdministration.Update(It.IsAny<CityAdministration>()));
            _repoWrapper
                .Setup(r => r.SaveAsync());

            //Act
            var result = _cityAdministrationService.RemoveAdministratorAsync(It.IsAny<int>());

            //Assert
            _repoWrapper.Verify();
            Assert.NotNull(result);
        }

        [Test]
        public void CheckPreviousAdministratorsToDelete_ReturnsCorrect()
        {
            //Arrange
            _repoWrapper.Setup(r => r.CityAdministration.GetAllAsync(It.IsAny<Expression<Func<CityAdministration, bool>>>(),
                 It.IsAny<Func<IQueryable<CityAdministration>, IIncludableQueryable<CityAdministration, object>>>()))
                     .ReturnsAsync(new List<CityAdministration> { new CityAdministration() { ID = fakeId } });
            _adminTypeService
               .Setup(a => a.GetAdminTypeByNameAsync(It.IsAny<string>()))
               .ReturnsAsync(new AdminTypeDTO
               {
                   AdminTypeName = "Голова Станиці",
                   ID = 3
               });
            _userManager
                .Setup(u => u.FindByIdAsync(It.IsAny<string>()));
            _userManager
                .Setup(u => u.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()));

            //Act
            var result = _cityAdministrationService.CheckPreviousAdministratorsToDelete();

            //Assert
            _repoWrapper.Verify();
            Assert.NotNull(result);
        }


        [Test]
        public async Task GetAdministrationsOfUserAsync_ReturnsCorrectAdministrations()
        {
            //Arrange
            _repoWrapper.Setup(r => r.CityAdministration.GetAllAsync(It.IsAny<Expression<Func<CityAdministration, bool>>>(),
                 It.IsAny<Func<IQueryable<CityAdministration>, IIncludableQueryable<CityAdministration, object>>>()))
                     .ReturnsAsync(new List<CityAdministration> { new CityAdministration() { ID = 3 } });
            _mapper
                .Setup(m => m.Map<IEnumerable<CityAdministration>, IEnumerable<CityAdministrationDTO>>(It.IsAny<IEnumerable<CityAdministration>>()))
                .Returns(GetTestCityAdministration());

            //Act
            var result = await _cityAdministrationService.GetAdministrationsOfUserAsync(It.IsAny<string>());

            //Assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<IEnumerable<CityAdministrationDTO>>(result);
        }

        private IEnumerable<CityAdministrationDTO> GetTestCityAdministration()
        {
            return new List<CityAdministrationDTO>
            {
                new CityAdministrationDTO{UserId = "Голова Станиці"},
                new CityAdministrationDTO{UserId = "Голова Станиці"}
            }.AsEnumerable();
        }

        private static AdminTypeDTO AdminType = new AdminTypeDTO
        {
            AdminTypeName = "Голова Станиці",
            ID = 1
        };

        private CityAdministrationDTO cityAdmDTO = new CityAdministrationDTO
        {
            ID = 1,
            AdminType = AdminType,
            CityId = 1,
            AdminTypeId = 1,
            EndDate = DateTime.Today,
            StartDate = DateTime.Now,
            User = new CityUserDTO(),
            UserId = "Голова Станиці"
        };

        private CityAdministration cityAdm = new CityAdministration
        {
            ID = 1,
            AdminType = new AdminType()
            {
                AdminTypeName = "Голова Станиці",
                ID = 1
            },
            AdminTypeId = AdminType.ID,
            UserId = "Голова Станиці"
        };

        private CityAdministrationDTO cityFakeAdmDTO = new CityAdministrationDTO
        {
            ID = 2,
            AdminType = AdminType,
            CityId = 2,
            AdminTypeId = 2,
            EndDate = DateTime.Today,
            StartDate = DateTime.Now,
            User = new CityUserDTO(),
            UserId = "Голова Станиці"
        };

        private int fakeId = 3;
    }
}
