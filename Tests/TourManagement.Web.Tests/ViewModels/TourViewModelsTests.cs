using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Tests.ViewModels;

/// <summary>
/// Unit tests for TourViewModel, TourFormViewModel, and TourListViewModel.
/// </summary>
public class TourViewModelsTests
{
    // ─── TourViewModel ───────────────────────────────────────────────────────

    [Fact]
    public void TourViewModel_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var vm = new TourViewModel();

        // Assert
        Assert.Equal(0, vm.TourId);
        Assert.Equal(string.Empty, vm.TourName);
        Assert.Equal(string.Empty, vm.Place);
        Assert.Equal(0, vm.Days);
        Assert.Equal(0m, vm.Price);
        Assert.Equal(string.Empty, vm.Locations);
        Assert.Equal(string.Empty, vm.TourInfo);
        Assert.Null(vm.Pic);
    }

    [Fact]
    public void TourViewModel_SetProperties_RetainsValues()
    {
        // Arrange & Act
        var vm = new TourViewModel
        {
            TourId = 5,
            TourName = "Goa Tour",
            Place = "Goa",
            Days = 7,
            Price = 15000m,
            Locations = "Baga, Calangute",
            TourInfo = "Beautiful beaches",
            Pic = "goa.jpg"
        };

        // Assert
        Assert.Equal(5, vm.TourId);
        Assert.Equal("Goa Tour", vm.TourName);
        Assert.Equal("Goa", vm.Place);
        Assert.Equal(7, vm.Days);
        Assert.Equal(15000m, vm.Price);
        Assert.Equal("Baga, Calangute", vm.Locations);
        Assert.Equal("Beautiful beaches", vm.TourInfo);
        Assert.Equal("goa.jpg", vm.Pic);
    }

    // ─── TourFormViewModel ───────────────────────────────────────────────────

    [Fact]
    public void TourFormViewModel_DefaultValues_AreCorrect()
    {
        var vm = new TourFormViewModel();

        Assert.Equal(0, vm.TourId);
        Assert.Equal(string.Empty, vm.TourName);
        Assert.Equal(string.Empty, vm.Place);
        Assert.Equal(0, vm.Days);
        Assert.Equal(0m, vm.Price);
        Assert.Equal(string.Empty, vm.Locations);
        Assert.Equal(string.Empty, vm.TourInfo);
        Assert.Null(vm.Pic);
        Assert.Null(vm.PicFile);
    }

    [Fact]
    public void TourFormViewModel_ValidModel_PassesValidation()
    {
        // Arrange
        var vm = new TourFormViewModel
        {
            TourName = "Goa Tour",
            Place = "Goa",
            Days = 5,
            Price = 15000m,
            Locations = "Baga Beach",
            TourInfo = "Enjoy the beaches"
        };

        // Act
        var results = ValidateModel(vm);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void TourFormViewModel_EmptyTourName_FailsValidation()
    {
        var vm = new TourFormViewModel
        {
            TourName = "",
            Place = "Goa",
            Days = 5,
            Price = 15000m,
            Locations = "Baga",
            TourInfo = "Info"
        };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
    }

    [Fact]
    public void TourFormViewModel_TourNameExceedsMaxLength_FailsValidation()
    {
        var vm = new TourFormViewModel
        {
            TourName = new string('A', 21), // exceeds 20
            Place = "Goa",
            Days = 5,
            Price = 15000m,
            Locations = "Baga",
            TourInfo = "Info"
        };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("TourName"));
    }

    [Fact]
    public void TourFormViewModel_DaysOutOfRange_FailsValidation()
    {
        var vm = new TourFormViewModel
        {
            TourName = "Tour",
            Place = "Goa",
            Days = 0, // below minimum of 1
            Price = 15000m,
            Locations = "Baga",
            TourInfo = "Info"
        };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("Days"));
    }

    [Fact]
    public void TourFormViewModel_DaysAtMaxBoundary_PassesValidation()
    {
        var vm = new TourFormViewModel
        {
            TourName = "Tour",
            Place = "Goa",
            Days = 99,
            Price = 15000m,
            Locations = "Baga",
            TourInfo = "Info"
        };

        var results = ValidateModel(vm);

        Assert.DoesNotContain(results, r => r.MemberNames.Contains("Days"));
    }

    [Fact]
    public void TourFormViewModel_PriceOutOfRange_FailsValidation()
    {
        var vm = new TourFormViewModel
        {
            TourName = "Tour",
            Place = "Goa",
            Days = 5,
            Price = 0m, // below minimum of 1
            Locations = "Baga",
            TourInfo = "Info"
        };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("Price"));
    }

    [Fact]
    public void TourFormViewModel_LocationsExceedsMaxLength_FailsValidation()
    {
        var vm = new TourFormViewModel
        {
            TourName = "Tour",
            Place = "Goa",
            Days = 5,
            Price = 15000m,
            Locations = new string('L', 101), // exceeds 100
            TourInfo = "Info"
        };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("Locations"));
    }

    [Fact]
    public void TourFormViewModel_TourInfoExceedsMaxLength_FailsValidation()
    {
        var vm = new TourFormViewModel
        {
            TourName = "Tour",
            Place = "Goa",
            Days = 5,
            Price = 15000m,
            Locations = "Baga",
            TourInfo = new string('I', 201) // exceeds 200
        };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("TourInfo"));
    }

    // ─── TourListViewModel ───────────────────────────────────────────────────

    [Fact]
    public void TourListViewModel_DefaultValues_AreCorrect()
    {
        var vm = new TourListViewModel();

        Assert.NotNull(vm.Tours);
        Assert.Empty(vm.Tours);
        Assert.Null(vm.SearchTerm);
    }

    [Fact]
    public void TourListViewModel_SetTours_RetainsValues()
    {
        var tours = new List<TourViewModel>
        {
            new TourViewModel { TourId = 1, TourName = "Tour A" },
            new TourViewModel { TourId = 2, TourName = "Tour B" }
        };

        var vm = new TourListViewModel
        {
            Tours = tours,
            SearchTerm = "Tour"
        };

        Assert.Equal(2, vm.Tours.Count());
        Assert.Equal("Tour", vm.SearchTerm);
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
