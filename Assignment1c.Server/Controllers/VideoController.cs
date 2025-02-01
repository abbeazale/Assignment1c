using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace Assignment1c.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Makes the base route /api/video
    public class VideoController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public VideoController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload")] // Full endpoint: POST /api/video/upload
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            Console.WriteLine("running");
            if (file == null || file.Length == 0)
            {
                Console.WriteLine("No file received."); // Debugging
                return BadRequest("No file uploaded.");
            }

            Console.WriteLine($"Received file: {file.FileName}, Size: {file.Length} bytes"); // Debugging


            if (!file.ContentType.StartsWith("video/"))
                return BadRequest("Invalid file type at video/");

            try
            {
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");

                // Create directory if it doesn't exist
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return accessible URL
                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                return Ok(new { fileUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.ToString()}"); // Log full error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}