using Domain;
using Xunit;
using Moq;
namespace Unittests.domain.services
{
    public class ProcessingServicesTests
    {
        [Fact]
        public async Task Authorize_CallsNeo4jServiceAndReturnsResult()
        {
            // Arrange
            var neo4jServiceMock = new Mock<INeo4jService>();
            neo4jServiceMock
                .Setup(s => s.RunQueryAsync(It.IsAny<Auth>()))
                .ReturnsAsync(true);

            var service = new ProcessingServices(neo4jServiceMock.Object);

            // Act
            var result = await service.Authorize("T2", "PERM1", "smith@ocbc.com");

            // Assert
            Assert.True(result);
        }
    }
}
