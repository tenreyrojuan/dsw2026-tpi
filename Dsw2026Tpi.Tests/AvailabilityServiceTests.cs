using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Services;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using NSubstitute;

namespace Dsw2026Tpi.Tests;

public class AvailabilityServiceTests
{
    private readonly IPersistence _mockPersistence = Substitute.For<IPersistence>();
    private readonly IHolidayProviderService _mockHolidayProvider = Substitute.For<IHolidayProviderService>();

    public AvailabilityServiceTests()
    {
        _mockHolidayProvider.GetHolidays()
            .Returns(Enumerable.Empty<DateOnly>());

        _mockPersistence.AddRange(Arg.Any<IEnumerable<AvailabilityRule>>())!
            .Returns(callInfo => callInfo.Arg<IEnumerable<AvailabilityRule>>());

        _mockPersistence.RemoveRange<AvailabilityRule>(Arg.Any<IEnumerable<AvailabilityRule>>())!
            .Returns(callInfo => callInfo.Arg<IEnumerable<AvailabilityRule>>());
    }

    [Fact]
    public async Task AddAvailability_WhenDoctorExistsAndDaysAreValid_ShouldPersistRulesAndReturnResponse()
    {
        var specialty = new Specialty("Cardiología", "Atención cardiovascular") { Id = Guid.NewGuid() };

        var doctor = Doctor.Create("Carlos Gómez", "MP-48213", specialty);
        var request = CreateRequest(doctor.Id,
            new AvailabilityModel.DayScheduleRequest("lunes", new TimeOnly(8, 0), new TimeOnly(12, 0)),
            new AvailabilityModel.DayScheduleRequest("miercoles", new TimeOnly(14, 0), new TimeOnly(18, 0)));

        _mockPersistence.GetById<Doctor>(doctor.Id, Arg.Any<string[]>())
            .Returns(doctor);

        var service = new AvailabilityService(_mockPersistence, _mockHolidayProvider);

        var result = await service.AddAvailability(request);

        Assert.NotNull(result);
        Assert.Equal(doctor.Id, result.DoctorId);
        Assert.Equal(2, result.Days.Count());

        var monday = result.Days.First();
        Assert.Equal(DayOfWeek.Monday.ToString(), monday.Day);
        Assert.Equal(new TimeOnly(8, 0), monday.StartTime);
        Assert.Equal(new TimeOnly(12, 0), monday.EndTime);

        var wednesday = result.Days.Last();
        Assert.Equal(DayOfWeek.Wednesday.ToString(), wednesday.Day);
        Assert.Equal(new TimeOnly(14, 0), wednesday.StartTime);
        Assert.Equal(new TimeOnly(18, 0), wednesday.EndTime);

        await _mockHolidayProvider.Received(1).GetHolidays();
        await _mockPersistence.Received(1).AddRange(Arg.Is<IEnumerable<AvailabilityRule>>(r => r.Count() == 2));
    }

    [Fact]
    public async Task AddAvailability_WhenDoctorDoesNotExist_ShouldThrowEntityNotFoundException()
    {
        var doctorId = Guid.NewGuid();
        var request = CreateRequest(doctorId,
            new AvailabilityModel.DayScheduleRequest("lunes", new TimeOnly(8, 0), new TimeOnly(12, 0)));

        _mockPersistence.GetById<Doctor>(doctorId, Arg.Any<string[]>())
            .Returns((Doctor?)null);

        var service = new AvailabilityService(_mockPersistence, _mockHolidayProvider);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => service.AddAvailability(request));
        await _mockHolidayProvider.DidNotReceive().GetHolidays();
        await _mockPersistence.DidNotReceive().AddRange(Arg.Any<IEnumerable<AvailabilityRule>>());
    }

    [Fact]
    public async Task AddAvailability_WhenTimeRangeIsInvalid_ShouldThrowBusinessRuleException()
    {
        var specialty = new Specialty("Cardiología", "Atención cardiovascular") { Id = Guid.NewGuid() };

        var doctor = Doctor.Create("Carlos Gómez", "MP-48213", specialty);

        var request = CreateRequest(doctor.Id,
            new AvailabilityModel.DayScheduleRequest("martes", new TimeOnly(18, 0), new TimeOnly(9, 0)));

        _mockPersistence.GetById<Doctor>(doctor.Id, Arg.Any<string[]>())
            .Returns(doctor);

        var service = new AvailabilityService(_mockPersistence, _mockHolidayProvider);

        await Assert.ThrowsAsync<BusinessRuleException>(() => service.AddAvailability(request));
        await _mockHolidayProvider.DidNotReceive().GetHolidays();
        await _mockPersistence.DidNotReceive().AddRange(Arg.Any<IEnumerable<AvailabilityRule>>());
    }

    [Fact]
    public async Task AddAvailability_WhenDayNameIsInvalid_ShouldThrowValidationException()
    {
        var specialty = new Specialty("Cardiología", "Atención cardiovascular") { Id = Guid.NewGuid() };

        var doctor = Doctor.Create("Carlos Gómez", "MP-48213", specialty);

        var request = CreateRequest(doctor.Id,
            new AvailabilityModel.DayScheduleRequest("feriado", new TimeOnly(8, 0), new TimeOnly(12, 0)));

        _mockPersistence.GetById<Doctor>(doctor.Id, Arg.Any<string[]>())
            .Returns(doctor);

        var service = new AvailabilityService(_mockPersistence, _mockHolidayProvider);

        await Assert.ThrowsAsync<ValidationException>(() => service.AddAvailability(request));
        await _mockHolidayProvider.Received(1).GetHolidays();
        await _mockPersistence.DidNotReceive().AddRange(Arg.Any<IEnumerable<AvailabilityRule>>());
    }

    [Fact]
    public async Task UpdateAvailability_WhenDoctorExists_ShouldRemoveCurrentRulesBeforeAddingNewOnes()
    {
        var specialty = new Specialty("Cardiología", "Atención cardiovascular") { Id = Guid.NewGuid() };

        var doctor = Doctor.Create("Carlos Gómez", "MP-48213", specialty);
        
        var request = CreateRequest(doctor.Id,
            new AvailabilityModel.DayScheduleRequest("viernes", new TimeOnly(9, 0), new TimeOnly(13, 0)));

        _mockPersistence.GetById<Doctor>(doctor.Id, Arg.Any<string[]>())
            .Returns(doctor);

        var service = new AvailabilityService(_mockPersistence, _mockHolidayProvider); 

        var result = await service.UpdateAvailability(request);

        Assert.NotNull(result);
        Assert.Equal(doctor.Id, result.DoctorId);
        Assert.Single(result.Days);
        Assert.Equal(DayOfWeek.Friday.ToString(), result.Days.First().Day);
        Assert.Equal(new TimeOnly(9, 0), result.Days.First().StartTime);
        Assert.Equal(new TimeOnly(13, 0), result.Days.First().EndTime);

        Received.InOrder(() =>
        {
            _mockPersistence.RemoveRange<AvailabilityRule>(Arg.Any<IEnumerable<AvailabilityRule>>());
            _mockPersistence.AddRange(Arg.Any<IEnumerable<AvailabilityRule>>());
        });
    }

    private static AvailabilityModel.Request CreateRequest(
        Guid doctorId,
        params AvailabilityModel.DayScheduleRequest[] days)
    {
        return new AvailabilityModel.Request(doctorId, days);
    }
}
