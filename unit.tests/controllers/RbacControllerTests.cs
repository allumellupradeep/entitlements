using Xunit;
using Moq;
using RbacV2.Controllers;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Unittests.controllers
{
    public class RbacControllerTests
    {
        [Fact]
        public void Authorize_ReturnsOk_WhenHeadersAreValid()
        {
            // Arrange
            var processingServicesMock = new Mock<ProcessingServices>(MockBehavior.Strict, null as INeo4jService);
            processingServicesMock
                .Setup(s => s.Authorize(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var controller = new RbacController(processingServicesMock.Object);

            // Act
            var result = controller.Authorize("subject", "permission", "username").Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(((dynamic)okResult.Value).isAuthorized);
        }
    }
}
