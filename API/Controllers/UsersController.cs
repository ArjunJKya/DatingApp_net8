
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers

{
  [Authorize]
  public class UsersController( IUserRepository userRepostiory,IMapper mapper) : BaseApiController
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

  }
}
