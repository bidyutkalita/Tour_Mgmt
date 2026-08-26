using System;
using System.Collections.Generic;
using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Web.Tests.Domain;

/// <summary>
/// Unit tests for domain entities: Tour, Booking, UserInfo.
/// </summary>
public class DomainEntitiesTests
{
    // ─── Tour ────────────────────────────────────────────────────────────────

    [Fact]
    public void Tour_DefaultValues_AreCorrect()
    {
        var tour = new Tour();

        Assert.Equal(0, tour.TourId);
        Assert.Equal(string.Empty, tour.TourName);
        Assert.Equal(string.Empty, tour.Place);
        Assert.Equal(0, tour.Days);
        Assert.Equal(0m, tour.Price);
        Assert.Equal(string.Empty, tour.Locations);
        Assert.Equal(string.Empty, tour.TourInfo);
        Assert.Null(tour.Pic);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_SetProperties_RetainsValues()
    {
        var tour = new Tour
        {
            TourId = 1,
            TourName = "Goa Tour",
            Place = "Goa",
            Days = 5,
            Price = 15000m,
            Locations = "Baga, Calangute",
            TourInfo = "Beautiful beaches",
            Pic = "goa.jpg"
        };

        Assert.Equal(1, tour.TourId);
        Assert.Equal("Goa Tour", tour.TourName);
        Assert.Equal("Goa", tour.Place);
        Assert.Equal(5, tour.Days);
        Assert.Equal(15000m, tour.Price);
        Assert.Equal("Baga, Calangute", tour.Locations);
        Assert.Equal("Beautiful beaches", tour.TourInfo);
        Assert.Equal("goa.jpg", tour.Pic);
    }

    [Fact]
    public void Tour_Bookings_CanBePopulated()
    {
        var tour = new Tour();
        tour.Bookings.Add(new Booking { BookingId = 1, TourName = "Goa Tour" });

        Assert.Single(tour.Bookings);
    }

    [Fact]
    public void Tour_Pic_CanBeNull()
    {
        var tour = new Tour { Pic = null };

        Assert.Null(tour.Pic);
    }

    // ─── Booking ─────────────────────────────────────────────────────────────

    [Fact]
    public void Booking_DefaultValues_AreCorrect()
    {
        var booking = new Booking();

        Assert.Equal(0, booking.BookingId);
        Assert.Equal(string.Empty, booking.TourName);
        Assert.Equal(string.Empty, booking.Place);
        Assert.Equal(string.Empty, booking.Email);
        Assert.Equal(string.Empty, booking.FirstName);
        Assert.Null(booking.User);
    }

    [Fact]
    public void Booking_BookingDate_DefaultsToUtcNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var booking = new Booking();
        var after = DateTime.UtcNow.AddSeconds(1);

        Assert.True(booking.BookingDate >= before && booking.BookingDate <= after);
    }

    [Fact]
    public void Booking_SetProperties_RetainsValues()
    {
        var date = new DateTime(2024, 6, 15, 10, 0, 0, DateTimeKind.Utc);
        var booking = new Booking
        {
            BookingId = 5,
            TourName = "Kashmir Tour",
            Place = "Kashmir",
            Email = "user@example.com",
            FirstName = "Alice",
            BookingDate = date
        };

        Assert.Equal(5, booking.BookingId);
        Assert.Equal("Kashmir Tour", booking.TourName);
        Assert.Equal("Kashmir", booking.Place);
        Assert.Equal("user@example.com", booking.Email);
        Assert.Equal("Alice", booking.FirstName);
        Assert.Equal(date, booking.BookingDate);
    }

    [Fact]
    public void Booking_User_CanBeAssigned()
    {
        var user = new UserInfo { Email = "user@example.com", FirstName = "Alice" };
        var booking = new Booking { User = user };

        Assert.NotNull(booking.User);
        Assert.Equal("user@example.com", booking.User.Email);
    }

    // ─── UserInfo ────────────────────────────────────────────────────────────

    [Fact]
    public void UserInfo_DefaultValues_AreCorrect()
    {
        var user = new UserInfo();

        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal(string.Empty, user.LastName);
        Assert.Equal(string.Empty, user.Gender);
        Assert.Equal(string.Empty, user.Password);
        Assert.Equal(default(DateTime), user.Dob);
        Assert.Equal(string.Empty, user.Street);
        Assert.Equal(string.Empty, user.City);
        Assert.Equal(string.Empty, user.State);
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void UserInfo_SetProperties_RetainsValues()
    {
        var dob = new DateTime(1990, 3, 20);
        var user = new UserInfo
        {
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "hashedpassword",
            Dob = dob,
            Street = "123 Main St",
            City = "Springfield",
            State = "IL"
        };

        Assert.Equal("john@example.com", user.Email);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("Male", user.Gender);
        Assert.Equal("hashedpassword", user.Password);
        Assert.Equal(dob, user.Dob);
        Assert.Equal("123 Main St", user.Street);
        Assert.Equal("Springfield", user.City);
        Assert.Equal("IL", user.State);
    }

    [Fact]
    public void UserInfo_Bookings_CanBePopulated()
    {
        var user = new UserInfo();
        user.Bookings.Add(new Booking { BookingId = 1 });
        user.Bookings.Add(new Booking { BookingId = 2 });

        Assert.Equal(2, user.Bookings.Count);
    }
}
