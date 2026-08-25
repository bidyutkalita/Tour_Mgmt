namespace Tour_Management.Models
{
    /// <summary>
    /// Tour entity model for EF Core.
    /// </summary>
    public class Tour
    {
        public int TourId { get; set; }
        public string TourName { get; set; }
        public string Place { get; set; }
        public string Days { get; set; }
        public string Price { get; set; }
        public string Locations { get; set; }
        public string TourInfo { get; set; }
        public string Pic { get; set; }
    }

    /// <summary>
    /// UserInfo entity model for EF Core.
    /// </summary>
    public class UserInfo
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }
        public string Dob { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }

    /// <summary>
    /// Booking entity model for EF Core.
    /// </summary>
    public class Booking
    {
        public int TourId { get; set; }
        public string TourName { get; set; }
        public string Place { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
    }
}
