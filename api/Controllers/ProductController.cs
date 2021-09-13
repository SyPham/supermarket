using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetUtility;
using OfficeOpenXml;
using Supermarket.Data;
using Supermarket.DTO;
using Supermarket.Helpers;
using Supermarket.Models;
using Supermarket.Services;
using System;
using System.Collections.Generic;
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

        [HttpPost]
        public async Task<ActionResult> Import([FromForm] IFormFile file2)
        {
            IFormFile file = Request.Form.Files["UploadedFile"];
            object createdBy = Request.Form["CreatedBy"];
            var datasList = new List<ProductDto>();
            var storeId = Request.Form["StoreId"].ToInt();
            //var datasList2 = new List<UploadDataVM2>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if ((file != null) && (file.Length > 0) && !string.IsNullOrEmpty(file.FileName))
            {
                string fileName = file.FileName;
                int userid = createdBy.ToInt();
                using (var package = new ExcelPackage(file.OpenReadStream()))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;

                    for (int rowIterator = 3; rowIterator <= noOfRow; rowIterator++)
                    {
                        datasList.Add(new ProductDto()
                        {
                            EnglishName = workSheet.Cells[rowIterator, 2].Value.ToSafetyString(),
                            ChineseName = workSheet.Cells[rowIterator, 3].Value.ToSafetyString(),
                            VietnameseName = workSheet.Cells[rowIterator, 4].Value.ToSafetyString(),
                            Description = workSheet.Cells[rowIterator, 5].Value.ToSafetyString(),
                            OriginalPrice = workSheet.Cells[rowIterator, 6].Value.ToDecimal(),
                            Store = workSheet.Cells[rowIterator, 7].Value.ToSafetyString(),
                            Kind = workSheet.Cells[rowIterator, 8].Value.ToSafetyString(),
                            Status = workSheet.Cells[rowIterator, 9].Value.ToBool(),
                        });
                    }
                }
                datasList.ForEach(item =>
                {
                    item.CreatedBy = userid;
                });
                await _service.ImportExcel(datasList);
                return Ok();
            }
            else
            {
                return StatusCode(500);
            }

        }
        [HttpGet]
        public async Task<IActionResult> ExcelExport()
        {
            string filename = "Productfile.xlsx";
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/excelTemplate", filename);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/octet-stream"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
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
                        entity.Status = true;
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
                    entity.Status = true;
                    entity.Avatar = $"image/default.png";

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
                        entity.Status = entity.Status;
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
                    entity.Status = entity.Status;
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
