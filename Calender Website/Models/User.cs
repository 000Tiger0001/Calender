public class User : IUser
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "None";
    public string LastName { get; set; } = "None";
    public string Email { get; set; } = "None";
    public string Password { get; set; } = "None";
    public int RecurringDays { get; set; }

    public User() { }

    public User(string firstName, string lastName, string email, string password, int recurringDays)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        RecurringDays = recurringDays;
    }
}