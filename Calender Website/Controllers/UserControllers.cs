using Microsoft.AspNetCore.Mvc;

[Route("Calender-Website")]
public class UserController : Controller
{
    readonly UserService US;
    EventAttendanceService EAS;

    public UserController(UserService us, EventAttendanceService eas)
    {
        US = us;
        EAS = eas;
    }

    [HttpGet("get-user-names")]
    public async Task<IActionResult> GetUserNames([FromQuery] Guid eventId) => Ok(await EAS.AttendanceStrings(eventId));

    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers() => Ok(await US.GetAllUsers());

    [HttpGet("get-user-by-id")]
    public async Task<IActionResult> GetUserById([FromQuery] Guid Id)
    {
        User user = await US.GetUserById(Id);
        if (user is null) return BadRequest("User does not exist");
        return Ok(user);
    }

    [HttpGet("get-many-users-by-id")]
    public async Task<IActionResult> GetManyUsers([FromQuery] Guid[] Ids)
    {
        User[] foundUsers = await US.GetManyUsers(Ids);
        if (foundUsers.Length <= 0) return BadRequest("There are no users with any of these ids");
        return Ok(foundUsers);
    }

    [HttpPut("update-user")]
    public async Task<IActionResult> UpdateUser([FromBody] User user)
    {
        bool userUpdated = await US.UpdateUser(user);
        if (!userUpdated) return BadRequest("Couldn't find user");
        return Ok("User has been updated");
    }

    [HttpDelete("delete-user-by-id")]
    [LoggedInFilter]
    public async Task<IActionResult> DeleteUser([FromQuery] Guid Id)
    {
        bool userIsDeleted = await US.DeleteUserById(Id);
        if (!userIsDeleted) return BadRequest("User not found");
        return Ok("User is deleted");
    }

    [HttpDelete("delete-user")]
    [AdminFilter]
    public async Task<IActionResult> DeleteUser(User user)
    {
        bool userIsDeleted = await US.DeleteUserWithUser(user);
        if (!userIsDeleted) return BadRequest("User not found");
        return Ok("User is deleted");
    }

    [HttpDelete("delete-friend")]
    [LoggedInFilter]
    public async Task<IActionResult> DeleteFriend(Guid id)
    {
        string sessionIdString = HttpContext.Session.GetString("UserId")!;
        Guid sessionId = Guid.Parse(sessionIdString);

        User user = await UserAccess.Get(sessionId);
        if (user is null) return BadRequest("User not found");
        if (!user.Friends.Contains(id)) return BadRequest("Friend not found");

        User friend = await UserAccess.Get(id);

        friend.Friends.Remove(sessionId);
        user.Friends.Remove(id);
        await US.UpdateUser(user);
        await US.UpdateUser(friend);
        return Ok("Friend deleted");
    }


    [HttpGet("friends")]
    [LoggedInFilter]
    public async Task<IActionResult> GetAllFriends()
    {
        string sessionIdString = HttpContext.Session.GetString("UserId")!;
        Guid sessionId = Guid.Parse(sessionIdString);

        User user = await UserAccess.Get(sessionId);
        if (user is null) return BadRequest("User not found");
        return Ok(user.Friends);
    }

    [HttpGet("friend-requests")]
    [LoggedInFilter]
    public async Task<IActionResult> GetAllFriendRequests()
    {
        string sessionIdString = HttpContext.Session.GetString("UserId")!;
        Guid sessionId = Guid.Parse(sessionIdString);

        User user = await UserAccess.Get(sessionId);
        if (user is null) return BadRequest("User not found");
        return Ok(user.FriendRequests);
    }


    [HttpPost("manage-friend-request")]
    [LoggedInFilter]
    public async Task<IActionResult> ManageFriendRequest([FromQuery] Guid id, [FromQuery] bool approve)
    {
        User friend = await UserAccess.Get(id);

        if (friend is null) return BadRequest("User not found");

        string sessionIdString = HttpContext.Session.GetString("UserId")!;
        Guid sessionId = Guid.Parse(sessionIdString);

        User user = await UserAccess.Get(sessionId);
        if (user is null) return BadRequest("User not found");
        if (!user.FriendRequests.Contains(id)) return BadRequest("Friend Request not found");
        if (user.Friends.Contains(id)) return BadRequest("You are already friends");

        if (approve)
        {
            user.FriendRequests.Remove(id);
            user.Friends.Add(id);
            friend.Friends.Add(user.Id);
            await US.UpdateUser(user);
            await US.UpdateUser(friend);
            return Ok("Friend request was approved");
        }
        else
        {
            user.FriendRequests.Remove(id);
            await US.UpdateUser(user);
            return Ok("Friend request was denied");
        }
    }

    [HttpPost("send-friend-request")]
    [LoggedInFilter]
    public async Task<IActionResult> SendFriendRequest([FromQuery] Guid toId)
    {
        // check if the sender exists 
        string sessionIdString = HttpContext.Session.GetString("UserId")!;
        Guid sessionId = Guid.Parse(sessionIdString);

        User user = await UserAccess.Get(sessionId);
        if (user is null) return BadRequest("User not found");

        // make sure user doesn`t send a friend request to himself
        if (toId == user.Id) return BadRequest("You cannot send a friend request to yourself");

        // check if potential friend exists
        User friend = await UserAccess.Get(toId);
        if (friend is null) return BadRequest("Have not found your friend");

        if (friend.FriendRequests.Contains(user.Id)) return Ok("You have already sent a friend request");

        // add a friend request to the friend
        friend.FriendRequests.Add(user.Id);

        await US.UpdateUser(friend);
        return Ok("Friend request was sent");
    }

    [HttpGet("find-people")]
    [LoggedInFilter]
    public async Task<IActionResult> FindPeople([FromQuery] string str)
    {
        List<User> allUsers = await UserAccess.LoadAll();
        str = str.ToLower();
        List<User> foundUsers = allUsers
        .Where(u => u.FirstName.ToLower().Contains(str) ||
                    u.LastName.ToLower().Contains(str))
        .ToList();
        return Ok(foundUsers);
    }
}