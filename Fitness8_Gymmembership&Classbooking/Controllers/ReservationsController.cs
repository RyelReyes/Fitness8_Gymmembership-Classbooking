using GymApi.Dtos;
using GymApi.Models;
using GymApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GymApi.Controllers;
[ApiController]
[Route("api/v1/reservations")]
[Authorize]
public class ReservationsController(IReservationService s) : ApiControllerBase
{
    [HttpGet] public Task<IActionResult> All() => Execute(async () => Ok(await s.GetAsync(User)));
    [HttpGet("{id:int}")] public Task<IActionResult> One(int id) => Execute(async () => Ok(await s.GetOneAsync(id, User)));
    [HttpPost][Authorize(Roles = Roles.Member)] public Task<IActionResult> Create(ReservationCreateDto d) => Execute(async () => Created("", await s.CreateAsync(d.ClassId != 0 ? d.ClassId : (d.ClassScheduleId != 0 ? d.ClassScheduleId : d.GymClassId), User)));
    [HttpPatch("{id:int}/status")] public Task<IActionResult> Status(int id, ReservationStatusDto d) => Execute(async () => Ok(await s.ChangeAsync(id, d.Status, User)));
}
