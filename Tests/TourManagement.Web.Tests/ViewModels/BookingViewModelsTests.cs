using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Tests.ViewModels;

/// <summary>
/// Unit tests for BookingViewModel, BookingFormViewModel, and BookingListViewModel.
/// </summary>
public class BookingViewModelsTests
{
    // ─── BookingViewModel ────────────────────────────────────────────────────

    [Fact]
    public void BookingViewModel_DefaultValues_AreCorrect()
    {
        var vm = new BookingViewModel();

        Assert.Equal(0, vm.BookingId);
        Assert.Equal(string.Empty, vm.TourName);
        Assert.Equal(string.Empty, vm.Place);
        Assert.Equal(string.Empty, vm.Email);
        Assert.Equal(string.Empty, vm.FirstName);
        Assert.Equal(default(DateTime), vm.BookingDate);
    }

    [Fact]
    public void BookingViewModel_SetProperties_RetainsValues()
    {
        var date = new DateTime(2024, 6, 15);
        var vm = new BookingViewModel
        {
            BookingId = 10,
            TourName = "Goa Tour",
            Place = "Goa",
            Email = "user@example.com",
            FirstName = "John",
            BookingDate = date
        };

        Assert.Equal(10, vm.BookingId);
        Assert.Equal("Goa Tour", vm.TourName);
        Assert.Equal("Goa", vm.Place);
        Assert.Equal("user@example.com", vm.Email);
        Assert.Equal("John", vm.FirstName);
        Assert.Equal(date, vm.BookingDate);
    }

    // ─── BookingFormViewModel ────────────────────────────────────────────────

    [Fact]
    public void BookingFormViewModel_DefaultValues_AreCorrect()
    {
        var vm = new BookingFormViewModel();

        Assert.Equal(0, vm.BookingId);
        Assert.Equal(string.Empty, vm.TourName);
        Assert.Equal(string.Empty, vm.Place);
        Assert.Equal(string.Empty, vm.Email);
        Assert.Equal(string.Empty, vm.FirstName);
    }

    [Fact]
    public void BookingFormViewModel_ValidModel_PassesValidation()
    {
        var vm = new BookingFormViewModel
        {
            TourName = "Goa Tour",
            Place = "Goa",
            Email = "user@example.com",
            FirstName = "John"
        };

        var results = ValidateModel(vm);

        Assert.Empty(results);
    }

    [Fact]
    public void BookingFormViewModel_EmptyTourName_FailsValidation()
    {
        var vm = new BookingFormViewModel
        {
            TourName = "",
            Place = "Goa",
            Email = "user@example.com",
            FirstName = "John"
        };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
    }

    [Fact]
    public void BookingFormViewModel_InvalidEmail_FailsValidation()
    {
        var vm = new BookingFormViewModel
        {
            TourName = "Goa Tour",
            Place = "Goa",
            Email = "not-an-email",
            FirstName = "John"
        };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void BookingFormViewModel_EmptyEmail_FailsValidation()
    {
        var vm = new BookingFormViewModel
        {
            TourName = "Goa Tour",
            Place = "Goa",
            Email = "",
            FirstName = "John"
        };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void BookingFormViewModel_EmptyFirstName_FailsValidation()
    {
        var vm = new BookingFormViewModel
        {
            TourName = "Goa Tour",
            Place = "Goa",
            Email = "user@example.com",
            FirstName = ""
        };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("FirstName"));
    }

    [Fact]
    public void BookingFormViewModel_TourNameExceedsMaxLength_FailsValidation()
    {
        var vm = new BookingFormViewModel
        {
            TourName = new string('T', 51), // exceeds 50
            Place = "Goa",
            Email = "user@example.com",
            FirstName = "John"
        };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
    }

    [Fact]
    public void BookingFormViewModel_PlaceExceedsMaxLength_FailsValidation()
    {
        var vm = new BookingFormViewModel
        {
            TourName = "Goa Tour",
            Place = new string('P', 51), // exceeds 50
            Email = "user@example.com",
            FirstName = "John"
        };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("Place"));
    }

    [Fact]
    public void BookingFormViewModel_FirstNameExceedsMaxLength_FailsValidation()
    {
        var vm = new BookingFormViewModel
        {
            TourName = "Goa Tour",
            Place = "Goa",
            Email = "user@example.com",
            FirstName = new string('F', 51) // exceeds 50
        };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("FirstName"));
    }

    // ─── BookingListViewModel ────────────────────────────────────────────────

    [Fact]
    public void BookingListViewModel_DefaultValues_AreCorrect()
    {
        var vm = new BookingListViewModel();

        Assert.NotNull(vm.Bookings);
        Assert.Empty(vm.Bookings);
        Assert.Null(vm.SearchTerm);
        Assert.False(vm.IsAdminView);
    }

    [Fact]
    public void BookingListViewModel_SetProperties_RetainsValues()
    {
        var bookings = new List<BookingViewModel>
        {
            new BookingViewModel { BookingId = 1, TourName = "Tour A" },
            new BookingViewModel { BookingId = 2, TourName = "Tour B" }
        };

        var vm = new BookingListViewModel
        {
            Bookings = bookings,
            SearchTerm = "Tour",
            IsAdminView = true
        };

        Assert.Equal(2, vm.Bookings.Count());
        Assert.Equal("Tour", vm.SearchTerm);
        Assert.True(vm.IsAdminView);
    }

    [Fact]
    public void BookingListViewModel_IsAdminView_CanBeSetToTrue()
    {
        var vm = new BookingListViewModel { IsAdminView = true };

        Assert.True(vm.IsAdminView);
    }

    [Fact]
    public void BookingListViewModel_IsAdminView_CanBeSetToFalse()
    {
        var vm = new BookingListViewModel { IsAdminView = false };

        Assert.False(vm.IsAdminView);
    }

    // ─── Helper ──────────────────────────────────────────────────────────────

    private static IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var ctx = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, ctx, results, true);
        return results;
    }
}
