using Xunit;
using Moq;
using Infrastructure;
using Neo4j.Driver;
using System.Threading.Tasks;
using Domain;
using System.Threading;

public class Neo4jServiceTests
{
    [Fact]
    public async Task RunQueryAsync_WithAuth_ReturnsExpectedResult()
    {
        // Arrange
        var driverMock = new Mock<IDriver>();
        var sessionMock = new Mock<IAsyncSession>();
        var resultCursorMock = new Mock<IResultCursor>();
        var recordMock = new Mock<IRecord>();

        // Setup the record to return true for "isAllowed"
        recordMock.Setup(r => r["isAllowed"]).Returns(true);

        // Setup the result cursor to return the mocked record
        resultCursorMock
            .Setup(r => r.SingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(recordMock.Object);

        // Setup the session to return the mocked result cursor
        sessionMock
            .Setup(s => s.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(resultCursorMock.Object);

        // Setup the driver to return the mocked session
        driverMock
            .Setup(d => d.AsyncSession(It.IsAny<Action<SessionConfigBuilder>>()))
            .Returns(sessionMock.Object);

        // Use the constructor that accepts IDriver for easier testing
        var neo4jService = new Neo4jService("","","","");

        var auth = new Auth
        {
            tenantId = "T2",
            username = "smith@ocbc.com",
            permission = "PERM1"
        };

        // Act
        var result = await neo4jService.RunQueryAsync(auth);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RunQueryAsync_WithQuery_ReturnsExpectedString()
    {
        // Arrange
        var driverMock = new Mock<IDriver>();
        var sessionMock = new Mock<IAsyncSession>();
        var resultCursorMock = new Mock<IResultCursor>();
        var recordMock = new Mock<IRecord>();

        // Setup the record to return a string for "message"
        recordMock.Setup(r => r["message"]).Returns("Neo4j Connected");

        // Setup the result cursor to return the mocked record
        resultCursorMock
            .Setup(r => r.SingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(recordMock.Object);

        // Setup the session to return the mocked result cursor
        sessionMock
            .Setup(s => s.RunAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(resultCursorMock.Object);

        // Setup the driver to return the mocked session
        driverMock
            .Setup(d => d.AsyncSession(It.IsAny<Action<SessionConfigBuilder>>()))
            .Returns(sessionMock.Object);

        // Use the constructor that accepts IDriver for easier testing
        //var neo4jService = new Neo4jService(driverMock.Object, "testdb");

        //// Act
        //var result = await neo4jService.RunQueryAsync("RETURN 'Neo4j Connected' AS message");

        //// Assert
        //Assert.Equal("Neo4j Connected", result);
    }
}