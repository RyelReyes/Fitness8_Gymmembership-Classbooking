using GymApi.Dtos;
using GymApi.Models;
using GymApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GymApi.Controllers;
[ApiController]
[Route("api/v1/classes")]
[Authorize]
public class ClassesController(IClassService s) : ApiControllerBase
{
    [HttpGet] public Task<IActionResult> All() => Execute(async () => Ok(await s.GetAllAsync()));
    [HttpGet("instructors")] public Task<IActionResult> Instructors() => Execute(async () => Ok(await s.GetInstructorsAsync()));
    [HttpGet("{id:int}/roster")][Authorize(Roles = Roles.AdminHelpdeskOrMember + "," + Roles.Instructor)] public Task<IActionResult> Roster(int id) => Execute(async () => Ok(await s.RosterAsync(id)));
    [HttpPost][Authorize(Roles = Roles.AdminOrHelpdesk)] public Task<IActionResult> Create(GymClassInputDto d) => Execute(async () => Created("", await s.SaveAsync(null, d)));
    [HttpPut("{id:int}")][Authorize(Roles = Roles.AdminOrHelpdesk)] public Task<IActionResult> Update(int id, GymClassInputDto d) => Execute(async () => Ok(await s.SaveAsync(id, d)));
    [HttpDelete("{id:int}")][Authorize(Roles = Roles.AdminOrHelpdesk)] public Task<IActionResult> Delete(int id) => Execute(async () => { await s.CancelAsync(id); return NoContent(); });
}
