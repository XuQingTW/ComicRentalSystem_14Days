namespace ComicRentalSystem_14Days.Models
{
    public enum UserRole
    {
        Member,
        Admin
    }

    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEndDate { get; set; }

        public User(string username, string passwordHash, UserRole role)
        {
            Username = username;
            PasswordHash = passwordHash;
            Role = role;
            FailedLoginAttempts = 0;
            LockoutEndDate = null;
        }
    }
}
