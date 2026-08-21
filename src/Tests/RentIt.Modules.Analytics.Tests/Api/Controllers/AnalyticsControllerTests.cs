using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RentIt.Modules.Analytics.Api.Controllers;
using RentIt.Modules.Analytics.Application.Queries;
using RentIt.Shared.Abstractions.Results;
using Xunit;

namespace RentIt.Modules.Analytics.Tests.Api.Controllers;

public class AnalyticsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AnalyticsController _controller;

    public AnalyticsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AnalyticsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetPropertyStats_ShouldReturnOk_WhenQueryIsSuccessful()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var expectedStats = new PropertyStatsDto(propertyId, 10, 5, 4.8);
        var expectedResult = Result.Success(expectedStats);

        _mediatorMock.Setup(m => m.Send(It.Is<GetPropertyStatsQuery>(q => q.PropertyId == propertyId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var response = await _controller.GetPropertyStats(propertyId, CancellationToken.None);

        // Assert
        var okResult = response.Should().BeOfType<OkObjectResult>().Subject;
        var statsDto = okResult.Value.Should().BeOfType<PropertyStatsDto>().Subject;
        
        statsDto.PropertyId.Should().Be(propertyId);
        statsDto.TotalBookings.Should().Be(10);
        statsDto.TotalReviews.Should().Be(5);
        statsDto.AverageRating.Should().Be(4.8);
    }

    [Fact]
    public async Task GetPropertyStats_ShouldReturnBadRequest_WhenQueryFails()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var error = new Error("Property.NotFound", "Property could not be found.");
        var expectedResult = Result.Failure<PropertyStatsDto>(error);

        _mediatorMock.Setup(m => m.Send(It.Is<GetPropertyStatsQuery>(q => q.PropertyId == propertyId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var response = await _controller.GetPropertyStats(propertyId, CancellationToken.None);

        // Assert
        var badRequestResult = response.Should().BeOfType<BadRequestObjectResult>().Subject;
        var returnedError = badRequestResult.Value.Should().BeOfType<Error>().Subject;
        
        returnedError.Code.Should().Be("Property.NotFound");
        returnedError.Message.Should().Be("Property could not be found.");
    }
}
