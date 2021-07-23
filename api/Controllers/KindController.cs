﻿using Microsoft.AspNetCore.Mvc;
using Supermarket.DTO;
using Supermarket.Helpers;
using Supermarket.Services;
using System.Threading.Tasks;

namespace Supermarket.Controllers
{
    public class KindController : ApiControllerBase
    {
        private readonly IKindService _service;

        public KindController(IKindService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAllByStore(int id)
        {
            return Ok(await _service.GetAllByStore(id));
        }
        [HttpGet("{id}/{lang}")]
        public async Task<ActionResult> GetAllByStoreLang(int id, string lang)
        {
            return Ok(await _service.GetAllByStoreLang(id , lang));
        }

        [HttpPost]
        public async Task<ActionResult> AddAsync([FromBody] KindDto model)
        {
            return StatusCodeResult(await _service.AddAsync(model));
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAsync([FromBody] KindDto model)
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

    }
}
