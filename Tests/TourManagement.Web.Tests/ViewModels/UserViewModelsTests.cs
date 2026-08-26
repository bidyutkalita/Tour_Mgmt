using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Tests.ViewModels;

/// <summary>
/// Unit tests for UserViewModels: LoginViewModel, RegisterViewModel,
/// UserUpdateViewModel, UserProfileViewModel, UserListViewModel.
/// </summary>
public class UserViewModelsTests
{
    // ─── LoginViewModel ──────────────────────────────────────────────────────

    [Fact]
    public void LoginViewModel_DefaultValues_AreCorrect()
    {
        var vm = new LoginViewModel();

        Assert.Equal(string.Empty, vm.Email);
        Assert.Equal(string.Empty, vm.Password);
    }

    [Fact]
    public void LoginViewModel_ValidModel_PassesValidation()
    {
        var vm = new LoginViewModel
        {
            Email = "user@example.com",
            Password = "password123"
        };

        var results = ValidateModel(vm);

        Assert.Empty(results);
    }

    [Fact]
    public void LoginViewModel_EmptyEmail_FailsValidation()
    {
        var vm = new LoginViewModel { Email = "", Password = "pass" };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void LoginViewModel_InvalidEmailFormat_FailsValidation()
    {
        var vm = new LoginViewModel { Email = "not-an-email", Password = "pass" };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void LoginViewModel_EmptyPassword_FailsValidation()
    {
        var vm = new LoginViewModel { Email = "user@example.com", Password = "" };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("Password"));
    }

    // ─── RegisterViewModel ───────────────────────────────────────────────────

    [Fact]
    public void RegisterViewModel_DefaultValues_AreCorrect()
    {
        var vm = new RegisterViewModel();

        Assert.Equal(string.Empty, vm.Email);
        Assert.Equal(string.Empty, vm.FirstName);
        Assert.Equal(string.Empty, vm.LastName);
        Assert.Equal(string.Empty, vm.Gender);
        Assert.Equal(string.Empty, vm.Password);
        Assert.Equal(default(DateTime), vm.Dob);
        Assert.Equal(string.Empty, vm.Street);
        Assert.Equal(string.Empty, vm.City);
        Assert.Equal(string.Empty, vm.State);
    }

    [Fact]
    public void RegisterViewModel_ValidModel_PassesValidation()
    {
        var vm = new RegisterViewModel
        {
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            Dob = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "Springfield",
            State = "IL"
        };

        var results = ValidateModel(vm);

        Assert.Empty(results);
    }

    [Fact]
    public void RegisterViewModel_EmptyFirstName_FailsValidation()
    {
        var vm = BuildValidRegisterViewModel();
        vm.FirstName = "";

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("FirstName"));
    }

    [Fact]
    public void RegisterViewModel_PasswordTooShort_FailsValidation()
    {
        var vm = BuildValidRegisterViewModel();
        vm.Password = "abc"; // less than 6 chars

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("Password"));
    }

    [Fact]
    public void RegisterViewModel_EmailExceedsMaxLength_FailsValidation()
    {
        var vm = BuildValidRegisterViewModel();
        vm.Email = new string('a', 45) + "@b.com"; // > 50 chars

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void RegisterViewModel_EmptyCity_FailsValidation()
    {
        var vm = BuildValidRegisterViewModel();
        vm.City = "";

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("City"));
    }

    // ─── UserUpdateViewModel ─────────────────────────────────────────────────

    [Fact]
    public void UserUpdateViewModel_DefaultValues_AreCorrect()
    {
        var vm = new UserUpdateViewModel();

        Assert.Equal(string.Empty, vm.FirstName);
        Assert.Equal(string.Empty, vm.LastName);
        Assert.Equal(string.Empty, vm.Gender);
        Assert.Null(vm.Password);
        Assert.Equal(default(DateTime), vm.Dob);
        Assert.Equal(string.Empty, vm.Street);
        Assert.Equal(string.Empty, vm.City);
        Assert.Equal(string.Empty, vm.State);
    }

    [Fact]
    public void UserUpdateViewModel_ValidModel_PassesValidation()
    {
        var vm = new UserUpdateViewModel
        {
            FirstName = "Jane",
            LastName = "Doe",
            Gender = "Female",
            Dob = new DateTime(1992, 5, 15),
            Street = "456 Oak Ave",
            City = "Chicago",
            State = "IL"
        };

        var results = ValidateModel(vm);

        Assert.Empty(results);
    }

    [Fact]
    public void UserUpdateViewModel_NullPassword_PassesValidation()
    {
        // Password is optional in update
        var vm = new UserUpdateViewModel
        {
            FirstName = "Jane",
            LastName = "Doe",
            Gender = "Female",
            Password = null,
            Dob = new DateTime(1992, 5, 15),
            Street = "456 Oak Ave",
            City = "Chicago",
            State = "IL"
        };

        var results = ValidateModel(vm);

        Assert.Empty(results);
    }

    [Fact]
    public void UserUpdateViewModel_PasswordTooShort_FailsValidation()
    {
        var vm = new UserUpdateViewModel
        {
            FirstName = "Jane",
            LastName = "Doe",
            Gender = "Female",
            Password = "abc", // too short
            Dob = new DateTime(1992, 5, 15),
            Street = "456 Oak Ave",
            City = "Chicago",
            State = "IL"
        };

        var results = ValidateModel(vm);

        Assert.Contains(results, r => r.MemberNames.Contains("Password"));
    }

    // ─── UserProfileViewModel ────────────────────────────────────────────────

    [Fact]
    public void UserProfileViewModel_DefaultValues_AreCorrect()
    {
        var vm = new UserProfileViewModel();

        Assert.Equal(string.Empty, vm.Email);
        Assert.Equal(string.Empty, vm.FirstName);
        Assert.Equal(string.Empty, vm.LastName);
        Assert.Equal(string.Empty, vm.Gender);
        Assert.Equal(default(DateTime), vm.Dob);
        Assert.Equal(string.Empty, vm.Street);
        Assert.Equal(string.Empty, vm.City);
        Assert.Equal(string.Empty, vm.State);
    }

    [Fact]
    public void UserProfileViewModel_FullName_CombinesFirstAndLastName()
    {
        var vm = new UserProfileViewModel
        {
            FirstName = "John",
            LastName = "Doe"
        };

        Assert.Equal("John Doe", vm.FullName);
    }

    [Fact]
    public void UserProfileViewModel_FullName_WithEmptyNames_ReturnsSpace()
    {
        var vm = new UserProfileViewModel
        {
            FirstName = "",
            LastName = ""
        };

        Assert.Equal(" ", vm.FullName);
    }

    [Fact]
    public void UserProfileViewModel_SetAllProperties_RetainsValues()
    {
        var dob = new DateTime(1990, 6, 15);
        var vm = new UserProfileViewModel
        {
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Smith",
            Gender = "Male",
            Dob = dob,
            Street = "789 Pine Rd",
            City = "Boston",
            State = "MA"
        };

        Assert.Equal("john@example.com", vm.Email);
        Assert.Equal("John", vm.FirstName);
        Assert.Equal("Smith", vm.LastName);
        Assert.Equal("Male", vm.Gender);
        Assert.Equal(dob, vm.Dob);
        Assert.Equal("789 Pine Rd", vm.Street);
        Assert.Equal("Boston", vm.City);
        Assert.Equal("MA", vm.State);
        Assert.Equal("John Smith", vm.FullName);
    }

    // ─── UserListViewModel ───────────────────────────────────────────────────

    [Fact]
    public void UserListViewModel_DefaultValues_AreCorrect()
    {
        var vm = new UserListViewModel();

        Assert.NotNull(vm.Users);
        Assert.Empty(vm.Users);
        Assert.Null(vm.SearchTerm);
    }

    [Fact]
    public void UserListViewModel_SetUsers_RetainsValues()
    {
        var users = new List<UserProfileViewModel>
        {
            new UserProfileViewModel { Email = "a@a.com", FirstName = "Alice", LastName = "A" },
            new UserProfileViewModel { Email = "b@b.com", FirstName = "Bob", LastName = "B" }
        };

        var vm = new UserListViewModel
        {
            Users = users,
            SearchTerm = "Alice"
        };

        Assert.Equal(2, vm.Users.Count());
        Assert.Equal("Alice", vm.SearchTerm);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static RegisterViewModel BuildValidRegisterViewModel() =>
        new RegisterViewModel
        {
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            Dob = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "Springfield",
            State = "IL"
        };

    private static IList<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var ctx = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, ctx, results, true);
        return results;
    }
}
