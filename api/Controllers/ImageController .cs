using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Supermarket.Data;
using Supermarket.DTO;
using Supermarket.Helpers;
using Supermarket.Models;
using Supermarket.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IProductService _service;
        public readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuaration;

        public ImageController(
            DataContext context,
            IProductService service,
            IWebHostEnvironment environment,
            IConfiguration configuration
            )
        {
            _context = context;
            _service = service;
            _configuaration = configuration;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            return Ok(await _service.GetAllAsync());
        }
        // GET api/files/sample.png
        [HttpGet("{fileName}")]
        public IActionResult Get(string fileName)
        {
            //string path = _environment.WebRootPath + "/image/" + fileName;
            //byte[] b = System.IO.File.ReadAllBytes(path);
            //return "data:image/png;base64," + Convert.ToBase64String(b);
            var path = _environment.WebRootPath + "/image/" + fileName;
            var imageFileStream = System.IO.File.OpenRead(path);
            return File(imageFileStream, "image/jpeg");
        }
       
    }
}
