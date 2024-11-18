
using System.Security.Claims;
using API.DTOs;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers

{
    [Authorize]
  public class UsersController(IUserRepository userRepostiory, IMapper mapper) : BaseApiController
  {
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
      var users = await userRepostiory.GetMemberAsync();

      return Ok(users);
    }


    // [HttpGet("{id:int}")] // /api/users/3
    // public async Task<ActionResult<AppUser>> GetUsers(int id)
    // {
    //   var user = await userRepostiory.GetUserByIdAsync(id);
    //   if (user == null) return NotFound();
    //   return user;
    // }

    [HttpGet("{username}")] // /api/users/username
    public async Task<ActionResult<MemberDto>> GetUsers(string username)
    {
      var user = await userRepostiory.GetMemberAsync(username);

      if (user == null) return NotFound();

      return Ok(user);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
      var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      
      if (username == null) return BadRequest("No username found in token");

      var user = await userRepostiory.GetUserByUsernameAsync(username);

      if (user == null) return BadRequest("Could not found user");

      mapper.Map(memberUpdateDto,user);

      if(await userRepostiory.SaveAllAsync())return NoContent(); 

      return BadRequest("Failed to update the user");
    }

  }
}
