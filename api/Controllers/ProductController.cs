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
    public class ProductController : ApiControllerBase
    {
        private readonly DataContext _context;
        private readonly IProductService _service;
        public readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuaration;

        public ProductController(
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

        [HttpPost("{id}")]
        public async Task<ActionResult> UpdateStatus(int id)
        {
            return Ok(await _service.UpdateStatus(id));
        }
        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            return Ok(await _service.GetAllAsync());
        }
        [HttpGet]
        public async Task<IActionResult> GetProductsForAdmin([FromQuery] FilterRequest request)
        {
            return Ok(await _service.GetProductsForAdmin(request));
        }
        [HttpPost]
        public async Task<ActionResult> AddAsync([FromBody] ProductDto model)
        {
            return StatusCodeResult(await _service.AddAsync(model));
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Created([FromForm] Product entity)
        {
            if (ModelState.IsValid)
            {
                IFormFile file = Request.Form.Files["UploadedFile"];
                //var name = Request.Form["UploadedFileName"];
                //var URL = Request.Form["UploadedFileURL"];
                //var WorkBy = Request.Form["UploadedFileWorkBy"];
                if (file != null)
                {
                    if (!Directory.Exists(_environment.WebRootPath + "\\image\\"))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + "\\image\\");
                    }
                    using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "\\image\\" + file.FileName))
                    {
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        entity.VietnameseName = entity.VietnameseName;
                        entity.EnglishName = entity.EnglishName;
                        entity.ChineseName = entity.ChineseName;
                        entity.Description = entity.Description;
                        entity.OriginalPrice = entity.OriginalPrice;
                        entity.CreatedBy = entity.CreatedBy;
                        entity.StoreId = entity.StoreId;
                        entity.KindId = entity.KindId;
                        entity.Avatar = $"image/{file.FileName}";

                    }
                }
                else
                {
                    entity.VietnameseName = entity.VietnameseName;
                    entity.EnglishName = entity.EnglishName;
                    entity.ChineseName = entity.ChineseName;
                    entity.Description = entity.Description;
                    entity.OriginalPrice = entity.OriginalPrice;
                    entity.CreatedBy = entity.CreatedBy;
                    entity.StoreId = entity.StoreId;
                    entity.KindId = entity.KindId;

                }
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return Ok(entity);

            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }
            return Ok(entity);
        }
        [HttpPost]
        public async Task<IActionResult> Updated([FromForm] Product entity)
        {

            if (ModelState.IsValid)
            {
                IFormFile file = Request.Form.Files["UploadedFile"];
                
                //var item = await _context.Projects.FindAsync(entity.ID);
                if (file != null)
                {
                    if (!Directory.Exists(_environment.WebRootPath + "\\image\\"))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + "\\image\\");
                    }
                    using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "\\image\\" + file.FileName))
                    {
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        entity.VietnameseName = entity.VietnameseName;
                        entity.EnglishName = entity.EnglishName;
                        entity.ChineseName = entity.ChineseName;
                        entity.Description = entity.Description;
                        entity.OriginalPrice = entity.OriginalPrice;
                        entity.CreatedBy = entity.CreatedBy;
                        entity.StoreId = entity.StoreId;
                        entity.KindId = entity.KindId;
                        entity.Avatar = $"image/{file.FileName}";
                        //return "\\image\\" + file.FileName;

                    }
                }
                else
                {
                    entity.VietnameseName = entity.VietnameseName;
                    entity.EnglishName = entity.EnglishName;
                    entity.ChineseName = entity.ChineseName;
                    entity.Description = entity.Description;
                    entity.OriginalPrice = entity.OriginalPrice;
                    entity.CreatedBy = entity.CreatedBy;
                    entity.StoreId = entity.StoreId;
                    entity.KindId = entity.KindId;
                    entity.Avatar = entity.Avatar;
                }
                _context.Update(entity);
                await _context.SaveChangesAsync();
                return Ok(entity);

            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }
            return Ok(entity);

        }
        [HttpPut]
        public async Task<ActionResult> UpdateAsync([FromBody] ProductDto model)
        {
            return StatusCodeResult(await _service.UpdateAsync(model));
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            return StatusCodeResult(await _service.DeleteAsync(id));
        }

        [HttpGet]
        public async Task<ActionResult> GetByIdAsync(int id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpGet]
        public async Task<ActionResult> GetWithPaginationsAsync(PaginationParams paramater)
        {
            return Ok(await _service.GetWithPaginationsAsync(paramater));
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarRequest request)
        {
            return Ok(await _service.UploadAvatar(request));
        }
        [HttpGet]
        public async Task<IActionResult> GetProductsForConsumer([FromQuery] FilterRequest request)
        {
            return Ok(await _service.GetProductsForConsumer(request));
        }
    }
}
