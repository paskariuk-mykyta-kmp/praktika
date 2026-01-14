namespace praktika.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool IsAdmin { get; set; }

        public User(int id, string username, string passwordHash, bool isAdmin)
        {
            Id = id;
            Username = username;
            PasswordHash = passwordHash;
            IsAdmin = isAdmin;
        }

        public string ToCsv() => $"{Id};{Username};{PasswordHash};{IsAdmin}";

        public static User FromCsv(string line)
        {
            var p = line.Split(';');
            return new User(int.Parse(p[0]), p[1], p[2], bool.Parse(p[3]));
        }
    }
}