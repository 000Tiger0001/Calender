public class AdminService
{
    public async Task<Admin?> AdminExists(Admin? admin)
    {
        List<Admin> admins = await AccesJson.ReadJson<Admin>();

        return admins.FirstOrDefault(a => a.Username == admin?.Username && a.Password == admin.Password && a.Email == admin.Email);
    }

    //Save admin to json
    public async Task<bool> SaveAdmin(Admin admin)
    {
        if (AdminExists(admin) is not null) return true;

        await AccesJson.WriteJson(admin);
        return false;
    }

    public async Task<bool> UpdateAdmin(Admin admin)
    {
        List<Admin> admins = await AccesJson.ReadJson<Admin>();

        int index = admins.FindIndex(a => a.Id == admin.Id);

        if (index < 0) return false;

        admins[index] = admin;
        AccesJson.WriteJsonList(admins);

        return true;
    }

    public async Task<bool> DeleteAdmin(Guid Id)
    {
        List<Admin> admins = await AccesJson.ReadJson<Admin>();
        int index = admins.FindIndex(a => a.Id == Id);

        if (index < 0) return false;

        admins.RemoveAt(index);

        AccesJson.WriteJsonList(admins);

        return true;
    }

    public async Task<Admin> GetAdmin(Guid Id)
    {
        List<Admin> admins = await AccesJson.ReadJson<Admin>();
        int index = admins.FindIndex(a => a.Id == Id);
        if (index < 0) return null!;
        return admins[index];
    }

    public async Task<Admin[]> GetManyAdmins(Guid[] ids)
    {
        List<Admin> specificAdmins = [];
        List<Admin> allAdmins = await AccesJson.ReadJson<Admin>();
        foreach (Guid id in ids)
        {
            Admin admin = allAdmins.Find(a => a.Id == id)!;
            if (admin is not null) specificAdmins.Add(admin);
        }
        return specificAdmins.ToArray();
    }

    public async Task<Admin[]> GetAllAdmins()
    {
        List<Admin> allAdmins = await AccesJson.ReadJson<Admin>();
        return allAdmins.ToArray();
    }
}