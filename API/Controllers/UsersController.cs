
using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers

{
  [Authorize]
  public class UsersController(IUserRepository userRepostiory, IMapper mapper, IPhotoService photoService) : BaseApiController
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
      var user = await userRepostiory.GetUserByUsernameAsync(User.GetUserName());

      if (user == null) return BadRequest("Could not found user");

      mapper.Map(memberUpdateDto, user);

      if (await userRepostiory.SaveAllAsync()) return NoContent();

      return BadRequest("Failed to update the user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
      var user = await userRepostiory.GetUserByUsernameAsync(User.GetUserName());

      if (user == null) return BadRequest("Cannot update user");

      var result = await photoService.AddPhotoAsync(file);

      if (result.Error != null) return BadRequest(result.Error.Message);

      var photo = new Photo
      {
        Url = result.SecureUrl.AbsoluteUri,
        PublicId = result.PublicId
      };

      user.Photos.Add(photo);

      if (await userRepostiory.SaveAllAsync())
        return CreatedAtAction(nameof(GetUsers),
        new { username = user.UserName }, mapper.Map<PhotoDto>(photo));

      return BadRequest("Problem adding photo");

    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
      var user = await userRepostiory.GetUserByUsernameAsync(User.GetUserName());

      if (user == null) return BadRequest("Could not find user");

      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

      if (photo == null || photo.IsMain) return BadRequest("Cannot use this as main photo");

      var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
      if (currentMain != null) currentMain.IsMain = false;

      photo.IsMain = true;

      if (await userRepostiory.SaveAllAsync()) return NoContent();

      return BadRequest("Problem setting main photo");

    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
      var user = await userRepostiory.GetUserByUsernameAsync(User.GetUserName());

      if (user == null) return BadRequest("User not found");

      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

      if(photo == null || photo.IsMain)return BadRequest("This photo cannot be delete");

      if(photo.PublicId != null)
      {
        var result = await photoService.DeletePhotoAsync(photo.PublicId);
        if(result.Error != null)return BadRequest(result.Error.Message);
      }

      user.Photos.Remove(photo);
      if(await  userRepostiory.SaveAllAsync()) return Ok();
       return BadRequest("Problem deleting Api");
    }

  }


}
