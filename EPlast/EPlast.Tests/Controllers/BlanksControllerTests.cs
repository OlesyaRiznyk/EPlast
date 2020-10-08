﻿using EPlast.BLL.DTO.Blank;
using EPlast.BLL.Interfaces.Blank;
using EPlast.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlast.Tests.Controllers
{
    [TestFixture]
    class BlanksControllerTests
    {
        Mock<IBlankBiographyDocumentService> _mockBiographyService;
        BlanksController _blanksController;

        [SetUp]
        public void SetUp()
        {
            _mockBiographyService = new Mock<IBlankBiographyDocumentService>();

            _blanksController = new BlanksController(_mockBiographyService.Object);
        }
        
        [Test]
        public async Task AddDocument_ReturnsOkObjectResult()
        {
            //Arrange
            _mockBiographyService
                .Setup(x => x.AddDocumentAsync(It.IsAny<BlankBiographyDocumentsDTO>()))
                .ReturnsAsync(GetBlankBiographyDocumentsDTO());
            //Act
            var document = await _blanksController.AddDocument(GetBlankBiographyDocumentsDTO());

            //Assert
            _mockBiographyService.Verify();
            Assert.NotNull(document);
            Assert.IsInstanceOf<ObjectResult>(document);
            
        }

        [Test]
        public async Task GetDocumentByUserId_ReturnsOkObjectResult()
        {
            //Arrange
            _mockBiographyService
                .Setup(x => x.GetDocumentByUserId("gh34tg"))
                .ReturnsAsync(GetBiographyDocumentsDTO());

            //Act
            var document = await _blanksController.GetDocumentByUserId("gh34tg");

            //Assert
            _mockBiographyService.Verify();
            Assert.NotNull(document);
            Assert.IsInstanceOf<ObjectResult>(document);

        }

        [Test]
        public async Task RemoveDocument_ReturnsOkResult()
        {
            //Arrange
            int documentId = 1;
            _mockBiographyService
                .Setup(x => x.DeleteFileAsync(documentId));
            
            //Act
            var document = await _blanksController.RemoveDocument(documentId);
            var statusCodeDocument = document as StatusCodeResult;

            //Assert
            _mockBiographyService.Verify();
            Assert.NotNull(document);
            Assert.AreEqual(StatusCodes.Status204NoContent, statusCodeDocument.StatusCode);
        }

        [Test]
        public async Task GetFileBase64_ReturnsOkObjectResult()
        {
            // Arrange
            _mockBiographyService
               .Setup(x => x.DownloadFileAsync(new string("Dogovir")))
               .ReturnsAsync(new string("Dogovir"));
            //Act
            var document = await _blanksController.GetFileBase64("Dogovir");

            //Assert
            _mockBiographyService.Verify();
            Assert.NotNull(document);
            Assert.IsInstanceOf<ObjectResult>(document);
        }

        private BlankBiographyDocumentsDTO GetBlankBiographyDocumentsDTO()
        {
            BlankBiographyDocumentsDTO BlankBiographyDTO = new BlankBiographyDocumentsDTO()
            {
                ID = 1,
                FileName = "Dogovir",
                BlobName = "BlobName",
                UserId = "gh34tg"

            };

            return BlankBiographyDTO;
        }

        public BlankBiographyDocumentsDTO GetBiographyDocumentsDTO()
        {
            return new BlankBiographyDocumentsDTO
            {
                ID = 1,
                FileName = "Dogovir",
                BlobName = "BlobName",
                UserId = "gh34tg"

            };
        }
    }
}
