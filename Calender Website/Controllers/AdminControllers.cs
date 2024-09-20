using Microsoft.AspNetCore.Mvc;

[Route("Calender-Website")]
public class AdminControllers : Controller
{
    readonly AdminService AS;

    public AdminControllers(AdminService adminService)
    {
        AS = adminService;
    }

    [HttpDelete("delete-admin")]
    public async Task<IActionResult> DeleteAdmin([FromQuery] Guid Id)
    {
        bool doesExist = await AS.DeleteAdmin(Id);
        if (!doesExist) return BadRequest("Admin does not exist");
        return Ok("Admin is deleted");
    }

    [HttpGet("get-admin")]
    public async Task<IActionResult> GetAdmins([FromQuery] Guid Id)
    {
        Admin admin = await AS.GetAdmin(Id);
        if (admin == null) return BadRequest("Admin does not exist");
        return Ok(admin);
    }

    /*[HttpGet("get-many-admins")]
    public async Task<IActionResult> GetManyAdmins([FromQuery] Guid[] ids)
    {
        Admin[] admins = await AS.GetManyAdmins(ids);
        if (admins.Length <= 0) return BadRequest("There were no admins with one of these ids");
        return Ok(admins);
    }*/

    [HttpGet("get-all-admins")]
    public async Task<IActionResult> GetAllAdmins()
    {
        Admin[] admins = await AS.GetAllAdmins();
        if (admins.Length <= 0) return BadRequest("There are no admins available at the moment. ");
        return Ok(admins);
    }
}