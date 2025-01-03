using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagement_Domain.DTOs;
using StudentManagement_Domain.Interface;
namespace StudentManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {

        private readonly IRoleRepository _repo;

        public RolesController(IRoleRepository roleService)
        {
            _repo = roleService;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _repo.GetAllRolesAsync();
            return Ok(roles);
        }

        // GET: api/Roles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var role = await _repo.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound(new { message = "Role not found" });
            }
            return Ok(role);
        }

        // POST: api/Roles
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest(new { message = "Role name is required" });
            }

            var result = await _repo.CreateRoleAsync(roleName);
            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetRoleById), new { id = roleName }, roleName);
            }

            return BadRequest(result.Errors);
        }

        // PUT: api/Roles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] string newRoleName)
        {
            if (string.IsNullOrEmpty(newRoleName))
            {
                return BadRequest(new { message = "New role name is required" });
            }

            var result = await _repo.UpdateRoleAsync(id, newRoleName);
            if (result.Succeeded)
            {
                return Ok(new { message = "Role updated successfully" });
            }

            return BadRequest(result.Errors);
        }

        // DELETE: api/Roles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var result = await _repo.DeleteRoleAsync(id);
            if (result.Succeeded)
            {
                return Ok(new { message = "Role deleted successfully" });
            }

            return NotFound(new { message = "Role not found" });
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRoleToUser([FromQuery] string userEmail, [FromQuery] string roleName)
        {
            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(roleName))
            {
                return BadRequest(new { message = "UserId and RoleName are required" });
            }

            var result = await _repo.AssignRoleToUserAsync(userEmail, roleName);
            if (result.Succeeded)
            {
                return Ok(new { message = "Role assigned successfully" });
            }

            return BadRequest(result.Errors);
        }
    }
}
