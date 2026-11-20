using GymApi.Models;
using GymApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GymApi.Controllers;
[ApiController]
[Route("api/v1/reports")]
[Authorize(Roles = Roles.Admin)]
public class ReportsController(IReportService s) : ApiControllerBase
{
    [HttpGet("overdue-memberships")] public Task<IActionResult> Overdue() => Execute(async () => Ok(await s.OverdueAsync()));
    [HttpGet("attendance")] public Task<IActionResult> Attendance() => Execute(async () => Ok(await s.AttendanceAsync()));
}
