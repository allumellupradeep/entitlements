using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
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

            var isAuthorized = await _processingServices.Authorize(subject, permissionName, username);
            if (isAuthorized)
            {
                return Ok(new
                {
                    subject,
                    permissionName,
                    username,
                    isAuthorized
                });
            }
            return StatusCode(403, new
            {
                status = "DENY",
                message = "Access denied"
            });
        }
    }
}
