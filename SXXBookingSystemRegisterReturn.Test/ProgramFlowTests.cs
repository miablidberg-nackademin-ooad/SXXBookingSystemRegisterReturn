using NSubstitute;
using SXXBookingSystemRegisterReturn.Application.Dtos;
using SXXBookingSystemRegisterReturn.Application.Services;
using SXXBookingSystemRegisterReturn.Domain.Enums;
using SXXBookingSystemRegisterReturn.Domain.Interfaces;
using SXXBookingSystemRegisterReturn.Domain.Models;

namespace SXXBookingSystemRegisterReturn.Test;

public class ProgramFlowTests
{
    [Fact]
    public async Task CorrectInput_Should_RegisterAReturn()
    {
        // Arrange
        Guid staffId = Guid.NewGuid();
        Guid bookingId = Guid.NewGuid();
        Guid customerId = Guid.NewGuid();
        string inventoryNumber = "UT-26-015";
        string articleNumber = "UT1001";

        Booking booking = new(
            bookingId,
            "UT-20261112-001",
            DateTime.Now,
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(5)), 
            DateOnly.FromDateTime(DateTime.Now.AddDays(5)), 
            1000,
            5000,
            inventoryNumber,
            BookingStatus.Confirmed,
            customerId);

        EquipmentItem equipmentItem = new(
            inventoryNumber,
            articleNumber,
            false);

        IReturnRepository returnRepository = Substitute.For<IReturnRepository>();
        returnRepository.AddAsync(Arg.Any<Return>()).Returns(Task.FromResult(true));

        IBookingRepository bookingRepository = Substitute.For<IBookingRepository>();
        bookingRepository.GetByIdAsync(bookingId).Returns(Task.FromResult<Booking?>(booking));
        bookingRepository.UpdateAsync(booking).Returns(Task.FromResult(true));

        IEquipmentItemRepository equipmentItemRepository = Substitute.For<IEquipmentItemRepository>();
        equipmentItemRepository.GetEquipmentItemByInventoryNumberAsync(inventoryNumber).Returns(Task.FromResult(equipmentItem));
        equipmentItemRepository.UpdateAsync(Arg.Any<EquipmentItem>()).Returns(Task.FromResult(true));

        IReturnService returnService = new ReturnService(returnRepository, bookingRepository, equipmentItemRepository);

        //Act
        GetBookingResponse getBookingResponse = await returnService.GetBookingByIdAsync(bookingId);
        CheckInventoryNumberResponse chechInventoryNumberResponse = returnService.CheckInventoryNumber(inventoryNumber, getBookingResponse.Booking!);
        RegisterReturnResponse registerReturnResponse = await returnService.RegisterReturnAsync(getBookingResponse.Booking!, inventoryNumber, staffId, true);

        // Assert
        Assert.Equal(BookingStatus.Completed, booking.Status);

        Assert.True(getBookingResponse.Success);
        Assert.True(chechInventoryNumberResponse.Success);
        Assert.True(registerReturnResponse.Success);

        await bookingRepository.Received(1).GetByIdAsync(bookingId);
        await bookingRepository.Received(1).UpdateAsync(booking);

        await equipmentItemRepository.Received(1).GetEquipmentItemByInventoryNumberAsync(inventoryNumber);
        await equipmentItemRepository.Received(1).UpdateAsync(Arg.Is<EquipmentItem>(equipmentItem =>
            equipmentItem.InventoryNumber.Equals(inventoryNumber) &&
            equipmentItem.ArticleNumber == articleNumber &&
            equipmentItem.IsAvailable == true));

        await returnRepository.Received(1).AddAsync(Arg.Is<Return>(returnToAdd => 
            returnToAdd.ReturnId != Guid.Empty &&
            returnToAdd.BookingId == bookingId &&
            returnToAdd.InventoryNumber.Equals(inventoryNumber) &&
            returnToAdd.ReturnDate == DateOnly.FromDateTime(DateTime.Now) &&
            returnToAdd.StaffId == staffId));
    }

    [Fact]
    public async Task WrongInventoryNumber_Should_CancelReturnProcess()
    {
        // Arrange
        Guid staffId = Guid.NewGuid();
        Guid bookingId = Guid.NewGuid();
        Guid customerId = Guid.NewGuid();
        string inventoryNumber = "UT-26-015";
        string wrongInventoryNumber = "UT-26-016";

        Booking booking = new(
            bookingId,
            "UT-20261112-001",
            DateTime.Now,
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            1000,
            5000,
            inventoryNumber,
            BookingStatus.Confirmed,
            customerId);

        IReturnRepository returnRepository = Substitute.For<IReturnRepository>();

        IBookingRepository bookingRepository = Substitute.For<IBookingRepository>();
        bookingRepository.GetByIdAsync(bookingId).Returns(Task.FromResult<Booking?>(booking));

        IEquipmentItemRepository equipmentItemRepository = Substitute.For<IEquipmentItemRepository>();

        IReturnService returnService = new ReturnService(returnRepository, bookingRepository, equipmentItemRepository);

        //Act
        GetBookingResponse getBookingResponse = await returnService.GetBookingByIdAsync(bookingId);
        CheckInventoryNumberResponse chechInventoryNumberResponse = returnService.CheckInventoryNumber(wrongInventoryNumber, getBookingResponse.Booking!);
        RegisterReturnResponse registerReturnResponse = await returnService.RegisterReturnAsync(getBookingResponse.Booking!, wrongInventoryNumber, staffId, true);

        // Assert
        Assert.Equal(BookingStatus.Confirmed, booking.Status);

        Assert.True(getBookingResponse.Success);
        Assert.False(chechInventoryNumberResponse.Success);
        Assert.False(registerReturnResponse.Success);

        await bookingRepository.Received(1).GetByIdAsync(bookingId);
        await bookingRepository.Received(0).UpdateAsync(Arg.Any<Booking>());

        await equipmentItemRepository.Received(0).GetEquipmentItemByInventoryNumberAsync(inventoryNumber);
        await equipmentItemRepository.Received(0).UpdateAsync(Arg.Any<EquipmentItem>());

        await returnRepository.Received(0).AddAsync(Arg.Any<Return>());
    }

    [Theory]
    [InlineData(BookingStatus.Cancelled)]
    [InlineData(BookingStatus.Completed)]
    public async Task CanceledOrCompletedBooking_Should_NotRegisterReturn(BookingStatus status)
    {
        // Arrange
        Guid staffId = Guid.NewGuid();
        Guid bookingId = Guid.NewGuid();
        Guid customerId = Guid.NewGuid();
        string inventoryNumber = "UT-26-015";

        Booking booking = new(
            bookingId,
            "UT-20261112-001",
            DateTime.Now,
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            1000,
            5000,
            inventoryNumber,
            status,
            customerId);

        IReturnRepository returnRepository = Substitute.For<IReturnRepository>();

        IBookingRepository bookingRepository = Substitute.For<IBookingRepository>();
        bookingRepository.GetByIdAsync(bookingId).Returns(Task.FromResult<Booking?>(booking));

        IEquipmentItemRepository equipmentItemRepository = Substitute.For<IEquipmentItemRepository>();

        IReturnService returnService = new ReturnService(returnRepository, bookingRepository, equipmentItemRepository);

        //Act
        GetBookingResponse getBookingResponse = await returnService.GetBookingByIdAsync(bookingId);
        CheckInventoryNumberResponse chechInventoryNumberResponse = returnService.CheckInventoryNumber(inventoryNumber, getBookingResponse.Booking!);
        RegisterReturnResponse registerReturnResponse = await returnService.RegisterReturnAsync(getBookingResponse.Booking!, inventoryNumber, staffId, true);

        // Assert
        Assert.Equal(status, booking.Status);
        Assert.Equal("Uthyrningen är redan avslutad.", registerReturnResponse.ErrorMessage);

        Assert.True(getBookingResponse.Success);
        Assert.True(chechInventoryNumberResponse.Success);
        Assert.False(registerReturnResponse.Success);

        await bookingRepository.Received(1).GetByIdAsync(bookingId);
        await bookingRepository.Received(0).UpdateAsync(Arg.Any<Booking>());

        await equipmentItemRepository.Received(0).GetEquipmentItemByInventoryNumberAsync(inventoryNumber);
        await equipmentItemRepository.Received(0).UpdateAsync(Arg.Any<EquipmentItem>());

        await returnRepository.Received(0).AddAsync(Arg.Any<Return>());
    }

    [Fact]
    public async Task EarlyReturn_Should_NotChangeTheTotalPrice()
    {
        // Arrange
        Guid staffId = Guid.NewGuid();
        Guid bookingId = Guid.NewGuid();
        Guid customerId = Guid.NewGuid();
        string inventoryNumber = "UT-26-015";
        string articleNumber = "UT1001";

        Booking booking = new(
            bookingId,
            "UT-20261112-001",
            DateTime.Now,
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            1000,
            5000,
            inventoryNumber,
            BookingStatus.Confirmed,
            customerId);

        EquipmentItem equipmentItem = new(
            inventoryNumber,
            articleNumber,
            false);

        IReturnRepository returnRepository = Substitute.For<IReturnRepository>();
        returnRepository.AddAsync(Arg.Any<Return>()).Returns(Task.FromResult(true));

        IBookingRepository bookingRepository = Substitute.For<IBookingRepository>();
        bookingRepository.GetByIdAsync(bookingId).Returns(Task.FromResult<Booking?>(booking));
        bookingRepository.UpdateAsync(booking).Returns(Task.FromResult(true));

        IEquipmentItemRepository equipmentItemRepository = Substitute.For<IEquipmentItemRepository>();
        equipmentItemRepository.GetEquipmentItemByInventoryNumberAsync(inventoryNumber).Returns(Task.FromResult(equipmentItem));
        equipmentItemRepository.UpdateAsync(Arg.Any<EquipmentItem>()).Returns(Task.FromResult(true));

        IReturnService returnService = new ReturnService(returnRepository, bookingRepository, equipmentItemRepository);

        //Act
        GetBookingResponse getBookingResponse = await returnService.GetBookingByIdAsync(bookingId);
        CheckInventoryNumberResponse chechInventoryNumberResponse = returnService.CheckInventoryNumber(inventoryNumber, getBookingResponse.Booking!);
        RegisterReturnResponse registerReturnResponse = await returnService.RegisterReturnAsync(getBookingResponse.Booking!, inventoryNumber, staffId, true);

        // Assert
        Assert.Equal(5000, booking.AgreedTotalPrice);
        Assert.Equal(BookingStatus.Completed, booking.Status);

        Assert.True(getBookingResponse.Success);
        Assert.True(chechInventoryNumberResponse.Success);
        Assert.True(registerReturnResponse.Success);

        await bookingRepository.Received(1).GetByIdAsync(bookingId);
        await bookingRepository.Received(1).UpdateAsync(booking);

        await equipmentItemRepository.Received(1).UpdateAsync(Arg.Any<EquipmentItem>());
        await returnRepository.Received(1).AddAsync(Arg.Any<Return>());
    }

    [Fact]
    public async Task DamagedOrBlockedEquipmentItem_Should_NotBeMadeAvailableForBooking()
    {
        // Arrange
        Guid staffId = Guid.NewGuid();
        Guid bookingId = Guid.NewGuid();
        Guid customerId = Guid.NewGuid();
        string inventoryNumber = "UT-26-015";
        string articleNumber = "UT1001";

        Booking booking = new(
            bookingId,
            "UT-20261112-001",
            DateTime.Now,
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            1000,
            5000,
            inventoryNumber,
            BookingStatus.Confirmed,
            customerId);

        EquipmentItem equipmentItem = new(
            inventoryNumber,
            articleNumber,
            // sätter till true för att kunna säkerhetsställa att det ändras i flödet,
            // även om det inte skulle vara tillgängligt i ett verkligt scenario där en återlämning görs 
            true);

        IReturnRepository returnRepository = Substitute.For<IReturnRepository>();
        returnRepository.AddAsync(Arg.Any<Return>()).Returns(Task.FromResult(true));

        IBookingRepository bookingRepository = Substitute.For<IBookingRepository>();
        bookingRepository.GetByIdAsync(bookingId).Returns(Task.FromResult<Booking?>(booking));
        bookingRepository.UpdateAsync(booking).Returns(Task.FromResult(true));

        IEquipmentItemRepository equipmentItemRepository = Substitute.For<IEquipmentItemRepository>();
        equipmentItemRepository.GetEquipmentItemByInventoryNumberAsync(inventoryNumber).Returns(Task.FromResult(equipmentItem));
        equipmentItemRepository.UpdateAsync(Arg.Any<EquipmentItem>()).Returns(Task.FromResult(true));

        IReturnService returnService = new ReturnService(returnRepository, bookingRepository, equipmentItemRepository);

        //Act
        GetBookingResponse getBookingResponse = await returnService.GetBookingByIdAsync(bookingId);
        CheckInventoryNumberResponse chechInventoryNumberResponse = returnService.CheckInventoryNumber(inventoryNumber, getBookingResponse.Booking!);
        RegisterReturnResponse registerReturnResponse = await returnService.RegisterReturnAsync(getBookingResponse.Booking!, inventoryNumber, staffId, false);

        // Assert
        Assert.Equal(BookingStatus.Completed, booking.Status);
        Assert.False(equipmentItem.IsAvailable);

        Assert.True(getBookingResponse.Success);
        Assert.True(chechInventoryNumberResponse.Success);
        Assert.True(registerReturnResponse.Success);

        await bookingRepository.Received(1).GetByIdAsync(bookingId);
        await bookingRepository.Received(1).UpdateAsync(booking);

        await equipmentItemRepository.Received(1).GetEquipmentItemByInventoryNumberAsync(inventoryNumber);
        await equipmentItemRepository.Received(1).UpdateAsync(Arg.Is<EquipmentItem>(equipmentItem =>
            equipmentItem.InventoryNumber.Equals(inventoryNumber) &&
            equipmentItem.ArticleNumber == articleNumber &&
            equipmentItem.IsAvailable == false));

        await returnRepository.Received(1).AddAsync(Arg.Is<Return>(returnToAdd =>
            returnToAdd.ReturnId != Guid.Empty &&
            returnToAdd.BookingId == bookingId &&
            returnToAdd.InventoryNumber.Equals(inventoryNumber) &&
            returnToAdd.ReturnDate == DateOnly.FromDateTime(DateTime.Now) &&
            returnToAdd.StaffId == staffId));
    }

    [Fact]
    public async Task GetBookingByIdAsync_BookingNotFound_Should_ReturnErrorMessage()
    {
        // Arrange
        Guid bookingId = Guid.NewGuid();

        IReturnRepository returnRepository = Substitute.For<IReturnRepository>();

        IBookingRepository bookingRepository = Substitute.For<IBookingRepository>();
        bookingRepository.GetByIdAsync(bookingId).Returns(Task.FromResult<Booking?>(null));

        IEquipmentItemRepository equipmentItemRepository = Substitute.For<IEquipmentItemRepository>();

        IReturnService returnService = new ReturnService(returnRepository, bookingRepository, equipmentItemRepository);

        //Act
        GetBookingResponse getBookingResponse = await returnService.GetBookingByIdAsync(bookingId);

        // Assert
        Assert.False(getBookingResponse.Success);
        Assert.Equal($"Bokningen med id '{bookingId}' kunde inte hittas.", getBookingResponse.ErrorMessage);

        await bookingRepository.Received(1).GetByIdAsync(bookingId);
        await bookingRepository.Received(0).UpdateAsync(Arg.Any<Booking>());
    }
}