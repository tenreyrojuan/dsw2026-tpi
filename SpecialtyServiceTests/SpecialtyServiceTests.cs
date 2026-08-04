using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Services;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Dsw2026Tpi.Tests;

public class SpecialtyServiceTests
{
    private readonly IPersistence _mockPersistence = Substitute.For<IPersistence>();

    [Fact]
    public async Task GetAll_WhenCalled_ReturnsPaginatedMappedResponse()
    {
        // Arrange
        int pageSize = 10;
        int pageIndex = 1;
        var specialtiesList = new List<Specialty>
        {
            new Specialty("Cardiología", "Atención cardiovascular") { Id = Guid.NewGuid() },
            new Specialty("Dermatología", "Cuidado de la piel") { Id = Guid.NewGuid() }
        };

        var paginatedResult = new Pagination<Specialty>(pageSize, pageIndex, specialtiesList, specialtiesList.Count);

        _mockPersistence.Paginate<Specialty, string>(pageSize, pageIndex, Arg.Any<Expression<Func<Specialty, bool>>>(),Arg.Any<Expression<Func<Specialty, string>>>())
            .Returns(paginatedResult);

        var service = new SpecialtyService(_mockPersistence);

        // Act
        var result = await service.GetAll(pageSize, pageIndex);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(specialtiesList.Count, result.Data.Count());
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(pageIndex, result.PageIndex);
        Assert.IsType<SpecialtyModel.Response>(result.Data.First());
        Assert.Equal("Cardiología", result.Data.First().Name);

        await _mockPersistence.Received(1).Paginate<Specialty, string>(pageSize, pageIndex, Arg.Any<Expression<Func<Specialty, bool>>>(), Arg.Any<Expression<Func<Specialty, string>>>());
    }

    [Fact]
    public async Task AddSpecialty_WhenDataIsValidAndDoesNotExist_ShouldCreateAndReturnResponse()
    {
        // Arrange
        var name = "Pediatría";
        var description = "Atención médica infantil";

        _mockPersistence.Any<Specialty>(Arg.Any<Expression<Func<Specialty, bool>>>())
            .Returns(false);

        var createdSpecialty = new Specialty(name, description) { Id = Guid.NewGuid() };

        _mockPersistence.Add(Arg.Any<Specialty>())
            .Returns(createdSpecialty);

        var service = new SpecialtyService(_mockPersistence);

        // Act
        var result = await service.AddSpecialty(name, description);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<SpecialtyModel.Response>(result);
        Assert.Equal(createdSpecialty.Id, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(description, result.Description);

        await _mockPersistence.Received(1).Add(Arg.Any<Specialty>());
    }
    
    [Fact]
    public async Task UpdateSpecialty_WhenEntityExistsAndNameIsValid_ShouldUpdateAndReturnResponse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingSpecialty = new Specialty("Nombre Viejo", "Descripción Vieja") { Id = id };
        var newName = "Neurología";
        var newDescription = "Especialidad del sistema nervioso";

        _mockPersistence.GetById<Specialty>(id)
            .Returns(existingSpecialty);

        _mockPersistence.Update(Arg.Any<Specialty>())
            .Returns(existingSpecialty);

        var service = new SpecialtyService(_mockPersistence);

        // Act
        var result = await service.UpdateSpecialty(id, newName, newDescription);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<SpecialtyModel.Response>(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(newName, result.Name);
        Assert.Equal(newDescription, result.Description);

        await _mockPersistence.Received(1).Update(Arg.Any<Specialty>());
    }

    [Fact]
    public async Task DeleteSpecialty_WhenEntityDoesNotExist_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockPersistence.GetById<Specialty>(id)
            .Returns((Specialty?)null);

        var service = new SpecialtyService(_mockPersistence);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => service.DeleteSpecialty(id));
        await _mockPersistence.DidNotReceive().Update(Arg.Any<Specialty>());
    }
}