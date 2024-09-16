public class Admin
{
    public int Id { get; set; }
    public string Username { get; }
    public string Password { get; set; }
    public string Email { get; }
    public bool LoggedIn { get; set; }

    public Admin(int id, string username, string password, string email)
    {
        Id = id;
        Username = username;
        Password = password;
        Email = email;
    }
}