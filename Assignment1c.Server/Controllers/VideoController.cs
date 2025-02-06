using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace Assignment1c.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public VideoController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            Console.WriteLine("running");
            if (file == null || file.Length == 0)
            {
                Console.WriteLine("No file received.");
                return BadRequest("No file uploaded.");
            }

            Console.WriteLine($"Received file: {file.FileName}, Size: {file.Length} bytes"); // Debugging

            //makes sure that its a video being uploaded
            if (!file.ContentType.StartsWith("video/"))
                return BadRequest("Invalid file type at video/");

            //try catch for saving the video
            try
            {
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                //use guid to name the file 
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                //save the file to local directiory 
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                //return the location of the file
                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                return Ok(new { fileUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.ToString()}"); 
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}