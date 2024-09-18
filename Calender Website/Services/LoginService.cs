using System.Text.Json;

public class LoginService
{

    //Does this admin account exist in the data?
    public Admin AdminExists(Admin admin) => MemoryDB.Admins.FirstOrDefault(a => a.Username == admin.Username && a.Password == admin.Password)!;


    //Is there any admin logged into this pc as of this moment?
    public bool IsRegistered() => MemoryDB.Admins.Any(a => a.LoggedIn);

    //Get the current admin account active on this pc
    public Admin GetCurrentAdmin() => MemoryDB.Admins.First(a => a.LoggedIn);

    //Save admin to json
    // test message
    public async Task<bool> SaveAdmin(Admin admin)
    {
        string path = $"Data/Admins.json";
        List<Admin> admins = JsonSerializer.Deserialize<List<Admin>>(await File.ReadAllTextAsync(path))!;
        foreach (Admin posibleSameAdmin in admins)
        {
            if (posibleSameAdmin.Email == admin.Email && posibleSameAdmin.Username == admin.Username && posibleSameAdmin.Password == admin.Password) return false;
        }
        admins.Add(admin);
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(admins));
        return true;
    }
}