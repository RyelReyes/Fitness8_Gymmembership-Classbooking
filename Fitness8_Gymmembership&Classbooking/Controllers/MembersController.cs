using GymApi.Dtos;
using GymApi.Models;
using GymApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GymApi.Controllers;
[ApiController]
[Route("api/v1/members")]
[Authorize]
public class MembersController(IMemberService service) : ApiControllerBase
{
    [HttpGet][Authorize(Roles = Roles.AdminHelpdeskOrMember)] public Task<IActionResult> All() => Execute(async () => User.IsInRole(Roles.Member) ? Forbid() : Ok(await service.GetAllAsync()));
    [HttpGet("me")][Authorize(Roles = Roles.Member)] public Task<IActionResult> Me() => Execute(async () => { var id = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value); var all = await service.GetAllAsync(); return Ok(all.FirstOrDefault(x => x.Username == User.Identity!.Name)); });
    [HttpGet("{id:int}")][Authorize(Roles = Roles.AdminOrHelpdesk)] public Task<IActionResult> One(int id) => Execute(async () => Ok(await service.GetAsync(id)));
    [HttpPost][Authorize(Roles = Roles.AdminOrHelpdesk)] public Task<IActionResult> Create(MemberCreateDto d) => Execute(async () => BadRequest(new { error = "Use public registration or the member registration form." }));
    [HttpPut("{id:int}")][Authorize(Roles = Roles.AdminOrHelpdesk)] public Task<IActionResult> Update(int id, MemberUpdateDto d) => Execute(async () => Ok(await service.UpdateAsync(id, d)));
    [HttpPatch("{id:int}/renew")][Authorize(Roles = Roles.AdminOrHelpdesk)] public Task<IActionResult> Renew(int id, MemberRenewDto d) => Execute(async () => Ok(await service.RenewAsync(id, d.PlanName)));
    [HttpDelete("{id:int}")][Authorize(Roles = Roles.AdminOrHelpdesk)] public Task<IActionResult> Delete(int id) => Execute(async () => { await service.DeactivateAsync(id); return NoContent(); });
}
