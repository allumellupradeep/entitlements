using Microsoft.AspNetCore.Mvc;
using Domain;

namespace RbacV2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RbacController : ControllerBase
    {
        private readonly ProcessingServices _processingServices;

        public RbacController(ProcessingServices processingServices)
        {
            _processingServices = processingServices;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            var result = await _processingServices.TestConnection("RETURN 'Neo4j Connected' AS message");
            return Ok(result);
        }

        [HttpPost("authorize")]
        public async Task<IActionResult> Authorize(
           [FromHeader(Name = "subject")] string subject,
           [FromHeader(Name = "permissionName")] string permissionName,
           [FromHeader(Name = "username")] string username)
        {
            // Basic validation
            if (string.IsNullOrEmpty(subject) ||
                string.IsNullOrEmpty(permissionName) ||
                string.IsNullOrEmpty(username))
            {
                return BadRequest("Missing required headers");
            }

            var (isAuthorized, isActiveUser) = await _processingServices.Authorize(subject, permissionName, username);
            if (isAuthorized && isActiveUser)
            {
                return Ok(new
                {
                    subject,
                    permissionName,
                    username,
                    isAuthorized, 
                    isActiveUser
                });
            }
            return StatusCode(403, new
            {
                status = "DENY",
                message = !isAuthorized ? "Access denied" : "User is not active"
            });
        }
    }
}
