using FluentAssertions;
using InternalInventory.API.Data.Entities;
using InternalInventory.API.Data.Repositories;
using InternalInventory.API.Models.Options;
using InternalInventory.API.Services;
using InternalInventory.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace InternalInventory.API.Tests.Unit.Services;

public class InventoryServiceTests
{
    private readonly InventoryService sut;
    private readonly IEventSender eventSender = Substitute.For<IEventSender>();
    private readonly IInternalInventoryItemRepository internalInventoryItemRepository =
        Substitute.For<IInternalInventoryItemRepository>();

    public InventoryServiceTests()
    {
        var options = new AppOptions { DayDataCleanupTimeUtc = new TimeOnly(0, 0) };
        this.sut = new InventoryService(
            eventSender,
            Options.Create(options),
            internalInventoryItemRepository
        );
    }

    [Fact]
    public void GetInventory_ShouldReturnEmptyList_WhenNoItemsExist()
    {
        // Arrange
        internalInventoryItemRepository.Get().Returns(Enumerable.Empty<InternalInventoryItemDb>());

        // Act
        var result = sut.GetInventory("AAPL", null);

        // Assert
        result.Should().BeEmpty();
    }
}
