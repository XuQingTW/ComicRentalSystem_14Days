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
        public UserRole Role { get; set; }

        public User(string username, string passwordHash, UserRole role)
        {
            // Consider adding validation or default values if necessary
            Username = username;
            PasswordHash = passwordHash;
            Role = role;
        }
    }
}
