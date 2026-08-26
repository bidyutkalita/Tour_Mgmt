using System;
using Xunit;
using TourManagement.Domain.Exceptions;

namespace TourManagement.Web.Tests.Domain;

/// <summary>
/// Unit tests for domain exceptions: NotFoundException, ValidationException, DuplicateEntityException.
/// </summary>
public class DomainExceptionsTests
{
    // ─── NotFoundException ───────────────────────────────────────────────────

    [Fact]
    public void NotFoundException_Constructor_SetsMessage()
    {
        var ex = new NotFoundException("Tour", 42);

        Assert.Contains("Tour", ex.Message);
        Assert.Contains("42", ex.Message);
    }

    [Fact]
    public void NotFoundException_IsException()
    {
        var ex = new NotFoundException("Booking", 99);

        Assert.IsAssignableFrom<Exception>(ex);
    }

    [Fact]
    public void NotFoundException_WithStringKey_SetsMessage()
    {
        var ex = new NotFoundException("UserInfo", "user@example.com");

        Assert.Contains("UserInfo", ex.Message);
        Assert.Contains("user@example.com", ex.Message);
    }

    [Fact]
    public void NotFoundException_CanBeThrown()
    {
        Exception? caught = null;
        try { throw new NotFoundException("Tour", 1); }
        catch (Exception ex) { caught = ex; }

        Assert.IsType<NotFoundException>(caught);
    }

    [Fact]
    public void NotFoundException_CanBeCaught()
    {
        NotFoundException? caught = null;
        try
        {
            throw new NotFoundException("Tour", 5);
        }
        catch (NotFoundException ex)
        {
            caught = ex;
        }

        Assert.NotNull(caught);
        Assert.Contains("Tour", caught.Message);
    }

    // ─── ValidationException ─────────────────────────────────────────────────

    [Fact]
    public void ValidationException_Constructor_SetsMessage()
    {
        var ex = new TourManagement.Domain.Exceptions.ValidationException("Field is required.");

        Assert.Equal("Field is required.", ex.Message);
    }

    [Fact]
    public void ValidationException_IsException()
    {
        var ex = new TourManagement.Domain.Exceptions.ValidationException("Error");

        Assert.IsAssignableFrom<Exception>(ex);
    }

    [Fact]
    public void ValidationException_CanBeThrown()
    {
        Exception? caught = null;
        try { throw new TourManagement.Domain.Exceptions.ValidationException("Invalid data"); }
        catch (Exception ex) { caught = ex; }

        Assert.IsType<TourManagement.Domain.Exceptions.ValidationException>(caught);
    }

    [Fact]
    public void ValidationException_CanBeCaught()
    {
        TourManagement.Domain.Exceptions.ValidationException? caught = null;
        try
        {
            throw new TourManagement.Domain.Exceptions.ValidationException("Bad input");
        }
        catch (TourManagement.Domain.Exceptions.ValidationException ex)
        {
            caught = ex;
        }

        Assert.NotNull(caught);
        Assert.Equal("Bad input", caught.Message);
    }

    // ─── DuplicateEntityException ────────────────────────────────────────────

    [Fact]
    public void DuplicateEntityException_Constructor_SetsMessage()
    {
        var ex = new DuplicateEntityException("UserInfo", "user@example.com");

        Assert.Contains("UserInfo", ex.Message);
        Assert.Contains("user@example.com", ex.Message);
    }

    [Fact]
    public void DuplicateEntityException_IsException()
    {
        var ex = new DuplicateEntityException("Tour", 1);

        Assert.IsAssignableFrom<Exception>(ex);
    }

    [Fact]
    public void DuplicateEntityException_CanBeThrown()
    {
        Exception? caught = null;
        try { throw new DuplicateEntityException("UserInfo", "dup@example.com"); }
        catch (Exception ex) { caught = ex; }

        Assert.IsType<DuplicateEntityException>(caught);
    }

    [Fact]
    public void DuplicateEntityException_CanBeCaught()
    {
        DuplicateEntityException? caught = null;
        try
        {
            throw new DuplicateEntityException("UserInfo", "dup@example.com");
        }
        catch (DuplicateEntityException ex)
        {
            caught = ex;
        }

        Assert.NotNull(caught);
        Assert.Contains("dup@example.com", caught.Message);
    }

    [Fact]
    public void DuplicateEntityException_WithIntKey_SetsMessage()
    {
        var ex = new DuplicateEntityException("Tour", 7);

        Assert.Contains("Tour", ex.Message);
        Assert.Contains("7", ex.Message);
    }
}
