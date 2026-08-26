using System;
using Xunit;
using TourManagement.Application.DTOs;

namespace TourManagement.Web.Tests.Application;

/// <summary>
/// Unit tests for Application DTOs: TourDto, TourCreateDto, TourUpdateDto,
/// UserDto, UserCreateDto, UserUpdateDto, BookingDto, BookingCreateDto, BookingUpdateDto.
/// </summary>
public class ApplicationDtosTests
{
    // ─── TourDto ─────────────────────────────────────────────────────────────

    [Fact]
    public void TourDto_DefaultValues_AreCorrect()
    {
        var dto = new TourDto();

        Assert.Equal(0, dto.TourId);
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0m, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.Pic);
    }

    [Fact]
    public void TourDto_SetProperties_RetainsValues()
    {
        var dto = new TourDto
        {
            TourId = 3,
            TourName = "Kerala Tour",
            Place = "Kerala",
            Days = 6,
            Price = 20000m,
            Locations = "Backwaters",
            TourInfo = "God's own country",
            Pic = "kerala.jpg"
        };

        Assert.Equal(3, dto.TourId);
        Assert.Equal("Kerala Tour", dto.TourName);
        Assert.Equal("Kerala", dto.Place);
        Assert.Equal(6, dto.Days);
        Assert.Equal(20000m, dto.Price);
        Assert.Equal("Backwaters", dto.Locations);
        Assert.Equal("God's own country", dto.TourInfo);
        Assert.Equal("kerala.jpg", dto.Pic);
    }

    // ─── TourCreateDto ───────────────────────────────────────────────────────

    [Fact]
    public void TourCreateDto_DefaultValues_AreCorrect()
    {
        var dto = new TourCreateDto();

        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0m, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.Pic);
    }

    [Fact]
    public void TourCreateDto_SetProperties_RetainsValues()
    {
        var dto = new TourCreateDto
        {
            TourName = "Rajasthan Tour",
            Place = "Rajasthan",
            Days = 8,
            Price = 30000m,
            Locations = "Jaipur, Jodhpur",
            TourInfo = "Land of kings",
            Pic = "raj.jpg"
        };

        Assert.Equal("Rajasthan Tour", dto.TourName);
        Assert.Equal("Rajasthan", dto.Place);
        Assert.Equal(8, dto.Days);
        Assert.Equal(30000m, dto.Price);
        Assert.Equal("Jaipur, Jodhpur", dto.Locations);
        Assert.Equal("Land of kings", dto.TourInfo);
        Assert.Equal("raj.jpg", dto.Pic);
    }

    // ─── TourUpdateDto ───────────────────────────────────────────────────────

    [Fact]
    public void TourUpdateDto_DefaultValues_AreCorrect()
    {
        var dto = new TourUpdateDto();

        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0m, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.Pic);
    }

    [Fact]
    public void TourUpdateDto_SetProperties_RetainsValues()
    {
        var dto = new TourUpdateDto
        {
            TourName = "Updated Tour",
            Place = "Updated Place",
            Days = 10,
            Price = 50000m,
            Locations = "Updated Locations",
            TourInfo = "Updated Info",
            Pic = "updated.jpg"
        };

        Assert.Equal("Updated Tour", dto.TourName);
        Assert.Equal("Updated Place", dto.Place);
        Assert.Equal(10, dto.Days);
        Assert.Equal(50000m, dto.Price);
        Assert.Equal("Updated Locations", dto.Locations);
        Assert.Equal("Updated Info", dto.TourInfo);
        Assert.Equal("updated.jpg", dto.Pic);
    }

    // ─── UserDto ─────────────────────────────────────────────────────────────

    [Fact]
    public void UserDto_DefaultValues_AreCorrect()
    {
        var dto = new UserDto();

        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Equal(string.Empty, dto.Gender);
        Assert.Equal(default(DateTime), dto.Dob);
        Assert.Equal(string.Empty, dto.Street);
        Assert.Equal(string.Empty, dto.City);
        Assert.Equal(string.Empty, dto.State);
    }

    [Fact]
    public void UserDto_SetProperties_RetainsValues()
    {
        var dob = new DateTime(1985, 7, 20);
        var dto = new UserDto
        {
            Email = "alice@example.com",
            FirstName = "Alice",
            LastName = "Smith",
            Gender = "Female",
            Dob = dob,
            Street = "10 Elm St",
            City = "Denver",
            State = "CO"
        };

        Assert.Equal("alice@example.com", dto.Email);
        Assert.Equal("Alice", dto.FirstName);
        Assert.Equal("Smith", dto.LastName);
        Assert.Equal("Female", dto.Gender);
        Assert.Equal(dob, dto.Dob);
        Assert.Equal("10 Elm St", dto.Street);
        Assert.Equal("Denver", dto.City);
        Assert.Equal("CO", dto.State);
    }

    // ─── UserCreateDto ───────────────────────────────────────────────────────

    [Fact]
    public void UserCreateDto_DefaultValues_AreCorrect()
    {
        var dto = new UserCreateDto();

        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Equal(string.Empty, dto.Gender);
        Assert.Equal(string.Empty, dto.Password);
        Assert.Equal(default(DateTime), dto.Dob);
        Assert.Equal(string.Empty, dto.Street);
        Assert.Equal(string.Empty, dto.City);
        Assert.Equal(string.Empty, dto.State);
    }

    [Fact]
    public void UserCreateDto_SetProperties_RetainsValues()
    {
        var dob = new DateTime(1995, 2, 14);
        var dto = new UserCreateDto
        {
            Email = "bob@example.com",
            FirstName = "Bob",
            LastName = "Jones",
            Gender = "Male",
            Password = "securepass",
            Dob = dob,
            Street = "5 Oak Ave",
            City = "Austin",
            State = "TX"
        };

        Assert.Equal("bob@example.com", dto.Email);
        Assert.Equal("Bob", dto.FirstName);
        Assert.Equal("Jones", dto.LastName);
        Assert.Equal("Male", dto.Gender);
        Assert.Equal("securepass", dto.Password);
        Assert.Equal(dob, dto.Dob);
        Assert.Equal("5 Oak Ave", dto.Street);
        Assert.Equal("Austin", dto.City);
        Assert.Equal("TX", dto.State);
    }

    // ─── UserUpdateDto ───────────────────────────────────────────────────────

    [Fact]
    public void UserUpdateDto_DefaultValues_AreCorrect()
    {
        var dto = new UserUpdateDto();

        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Equal(string.Empty, dto.Gender);
        Assert.Null(dto.Password);
        Assert.Equal(default(DateTime), dto.Dob);
        Assert.Equal(string.Empty, dto.Street);
        Assert.Equal(string.Empty, dto.City);
        Assert.Equal(string.Empty, dto.State);
    }

    [Fact]
    public void UserUpdateDto_NullPassword_IsAllowed()
    {
        var dto = new UserUpdateDto { Password = null };

        Assert.Null(dto.Password);
    }

    [Fact]
    public void UserUpdateDto_SetProperties_RetainsValues()
    {
        var dob = new DateTime(1988, 11, 30);
        var dto = new UserUpdateDto
        {
            FirstName = "Carol",
            LastName = "White",
            Gender = "Female",
            Password = "newpass123",
            Dob = dob,
            Street = "99 Pine Rd",
            City = "Seattle",
            State = "WA"
        };

        Assert.Equal("Carol", dto.FirstName);
        Assert.Equal("White", dto.LastName);
        Assert.Equal("Female", dto.Gender);
        Assert.Equal("newpass123", dto.Password);
        Assert.Equal(dob, dto.Dob);
        Assert.Equal("99 Pine Rd", dto.Street);
        Assert.Equal("Seattle", dto.City);
        Assert.Equal("WA", dto.State);
    }

    // ─── BookingDto ──────────────────────────────────────────────────────────

    [Fact]
    public void BookingDto_DefaultValues_AreCorrect()
    {
        var dto = new BookingDto();

        Assert.Equal(0, dto.BookingId);
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(default(DateTime), dto.BookingDate);
    }

    [Fact]
    public void BookingDto_SetProperties_RetainsValues()
    {
        var date = new DateTime(2024, 8, 10);
        var dto = new BookingDto
        {
            BookingId = 7,
            TourName = "Manali Tour",
            Place = "Manali",
            Email = "traveler@example.com",
            FirstName = "Dave",
            BookingDate = date
        };

        Assert.Equal(7, dto.BookingId);
        Assert.Equal("Manali Tour", dto.TourName);
        Assert.Equal("Manali", dto.Place);
        Assert.Equal("traveler@example.com", dto.Email);
        Assert.Equal("Dave", dto.FirstName);
        Assert.Equal(date, dto.BookingDate);
    }

    // ─── BookingCreateDto ────────────────────────────────────────────────────

    [Fact]
    public void BookingCreateDto_DefaultValues_AreCorrect()
    {
        var dto = new BookingCreateDto();

        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
    }

    [Fact]
    public void BookingCreateDto_SetProperties_RetainsValues()
    {
        var dto = new BookingCreateDto
        {
            TourName = "Ooty Tour",
            Place = "Ooty",
            Email = "eve@example.com",
            FirstName = "Eve"
        };

        Assert.Equal("Ooty Tour", dto.TourName);
        Assert.Equal("Ooty", dto.Place);
        Assert.Equal("eve@example.com", dto.Email);
        Assert.Equal("Eve", dto.FirstName);
    }

    // ─── BookingUpdateDto ────────────────────────────────────────────────────

    [Fact]
    public void BookingUpdateDto_DefaultValues_AreCorrect()
    {
        var dto = new BookingUpdateDto();

        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
    }

    [Fact]
    public void BookingUpdateDto_SetProperties_RetainsValues()
    {
        var dto = new BookingUpdateDto
        {
            TourName = "Updated Tour",
            Place = "Updated Place",
            Email = "updated@example.com",
            FirstName = "Updated"
        };

        Assert.Equal("Updated Tour", dto.TourName);
        Assert.Equal("Updated Place", dto.Place);
        Assert.Equal("updated@example.com", dto.Email);
        Assert.Equal("Updated", dto.FirstName);
    }
}
