﻿using System;
using System.Collections.Generic;
using EPlast.BLL.DTO.ActiveMembership;
using EPlast.BLL.Interfaces.ActiveMembership;
using Moq;
using EPlast.WebApi.Controllers;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EPlast.Tests.Controllers
{
    class ActiveMembershipControllerTests
    {
        private Mock<IPlastDegreeService> _plastDegreeService;
        private Mock<IAccessLevelService> _accessLevelService;
        private Mock<IUserDatesService> _userDatesService;

        private ActiveMembershipController _activeMembershipController =>
            new ActiveMembershipController(_plastDegreeService.Object, _accessLevelService.Object, _userDatesService.Object);

        public ActiveMembershipControllerTests()
        {
            _plastDegreeService = new Mock<IPlastDegreeService>();
            _accessLevelService = new Mock<IAccessLevelService>();
            _userDatesService = new Mock<IUserDatesService>();
        }

        [Test]
        public async Task GetAllDergees_Valid_Test()
        {
            //Arrange
            _plastDegreeService.Setup(cs => cs.GetDergeesAsync()).ReturnsAsync(new List<PlastDegreeDTO>());

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.GetAllDergees();
            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.NotNull(result);
        }

        [TestCase("2")]
        public async Task GetAccessLevel_Valid_Test(string id)
        {
            //Arrange
            _accessLevelService.Setup(m => m.GetUserAccessLevelsAsync(It.IsAny<string>())).ReturnsAsync(new List<string>());

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.GetAccessLevel(id);
            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.NotNull(result);
        }

        [TestCase("2")]
        public async Task GetUserDergees_Valid_Test(string id)
        {
            //Arrange
            _plastDegreeService.Setup(cs => cs.GetUserPlastDegreesAsync(It.IsAny<string>())).ReturnsAsync(new List<UserPlastDegreeDTO>());

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.GetUserDegrees(id);
            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.NotNull(result);
        }

        [TestCase(2)]
        public async Task AddPlastDegreeForUser_Valid_Test(int id)
        {
            //Arrange
            bool successfulAdded = true;
            _plastDegreeService.Setup(cs => cs.AddPlastDegreeForUserAsync(It.IsAny<UserPlastDegreePostDTO>())).ReturnsAsync(successfulAdded);

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.AddPlastDegreeForUser(new UserPlastDegreePostDTO() { PlastDegreeId = id });
            //Assert
            Assert.IsInstanceOf<CreatedResult>(result);
            var cr = (CreatedResult)result;
            Assert.NotNull(cr.Value);
            Assert.IsInstanceOf<int>(cr.Value);
            Assert.AreEqual(id, cr.Value);
        }

        [TestCase(2)]
        public async Task AddPlastDegreeForUser_InValid_Test(int id)
        {
            //Arrange
            bool successfulAdded = false;
            _plastDegreeService.Setup(cs => cs.AddPlastDegreeForUserAsync(It.IsAny<UserPlastDegreePostDTO>())).ReturnsAsync(successfulAdded);

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.AddPlastDegreeForUser(new UserPlastDegreePostDTO() { PlastDegreeId = id });
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task DeletePlastDegreeForUser_Valid_Test()
        {
            //Arrange
            bool successfulDeleted = true;
            _plastDegreeService.Setup(cs => cs.DeletePlastDegreeForUserAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(successfulDeleted);

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.DeletePlastDegreeForUser("", 0);
            //Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeletePlastDegreeForUser_InValid_Test()
        {
            //Arrange
            bool successfulDeleted = false;
            _plastDegreeService.Setup(cs => cs.DeletePlastDegreeForUserAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(successfulDeleted);

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.DeletePlastDegreeForUser("", 0);
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task SetPlastDegreeAsCurrent_Valid_Test()
        {
            //Arrange
            bool successfulSetPD = true;
            _plastDegreeService.Setup(cs => cs.SetPlastDegreeForUserAsCurrentAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(successfulSetPD);

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.SetPlastDegreeAsCurrent("", 0);
            //Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task SetPlastDegreeAsCurrent_InValid_Test()
        {
            //Arrange
            bool successfulSetPD = false;
            _plastDegreeService.Setup(cs => cs.SetPlastDegreeForUserAsCurrentAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(successfulSetPD);

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.SetPlastDegreeAsCurrent("", 0);
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task AddEndDatePlastDegreeForUser_Valid_Test()
        {
            //Arrange
            bool successfulAddedEndDate = true;
            _plastDegreeService.Setup(cs => cs.AddEndDateForUserPlastDegreeAsync(It.IsAny<UserPlastDegreePutDTO>()))
                               .ReturnsAsync(successfulAddedEndDate);

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.AddEndDatePlastDegreeForUser(new UserPlastDegreePutDTO());
            //Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task AddEndDatePlastDegreeForUser_InValid_Test()
        {
            //Arrange
            bool successfulAddedEndDate = false;
            _plastDegreeService.Setup(cs => cs.AddEndDateForUserPlastDegreeAsync(It.IsAny<UserPlastDegreePutDTO>()))
                               .ReturnsAsync(successfulAddedEndDate);

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.AddEndDatePlastDegreeForUser(new UserPlastDegreePutDTO());
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [TestCase("2")]
        public async Task GetUserDates_Valid_ReturnsOK(string id)
        {
            //Arrange
            _userDatesService.Setup(cs => cs.GetUserMembershipDatesAsync(It.IsAny<string>())).ReturnsAsync(new UserMembershipDatesDTO());

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.GetUserDates(id);
            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsInstanceOf<UserMembershipDatesDTO>(((OkObjectResult)result).Value);
        }

        [Test]
        public async Task GetUserDates_InValid_ThrowException()
        {
            //Arrange
            _userDatesService.Setup(cs => cs.GetUserMembershipDatesAsync(It.IsAny<string>())).ThrowsAsync(new InvalidOperationException());

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.GetUserDates(null);
            //Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task ChangeUserDates_Valid_Test()
        {
            //Arrange
            bool successfulChangedDates = true;
            //Arrange
            _userDatesService.Setup(cs => cs.ChangeUserMembershipDatesAsync(It.IsAny<UserMembershipDatesDTO>()))
                             .ReturnsAsync(successfulChangedDates);

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.ChangeUserDates(new UserMembershipDatesDTO());
            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsInstanceOf<UserMembershipDatesDTO>(((OkObjectResult)result).Value);
        }

        [Test]
        public async Task ChangeUserDates_InValid_Test()
        {
            //Arrange
            bool successfulChangedDates = false;
            //Arrange
            _userDatesService.Setup(cs => cs.ChangeUserMembershipDatesAsync(It.IsAny<UserMembershipDatesDTO>()))
                             .ReturnsAsync(successfulChangedDates);

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.ChangeUserDates(new UserMembershipDatesDTO());
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [TestCase("2")]
        public async Task InitializeUserDates_Valid_Test(string userId)
        {
            //Arrange
            bool successfulInitedDates = true;
            //Arrange
            _userDatesService.Setup(cs => cs.AddDateEntryAsync(It.IsAny<string>()))
                             .ReturnsAsync(successfulInitedDates);

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.InitializeUserDates("2");
            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            Assert.IsInstanceOf<string>(((OkObjectResult)result).Value);
        }

        [Test]
        public async Task InitializeUserDates_InValid_Test()
        {
            //Arrange
            bool successfulInitedDates = false;
            //Arrange
            _userDatesService.Setup(cs => cs.AddDateEntryAsync(It.IsAny<string>()))
                             .ReturnsAsync(successfulInitedDates);

            ActiveMembershipController activeMembershipController = _activeMembershipController;

            //Act
            var result = await activeMembershipController.InitializeUserDates("");
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

    }
}
