using Microsoft.AspNetCore.Mvc;
using Supermarket.DTO;
using Supermarket.Helpers;
using Supermarket.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Supermarket.Controllers
{
    public class OrderController : ApiControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }
        [HttpPut]
        public async Task<IActionResult> TransferBuyList(List<AddToBuyListDto> model)
        {
            var status = await _service.Transfer(model);
            return Ok(status);
        }
        [HttpPut]
        public async Task<IActionResult> TransferComplete(List<AddToCompleteListDto> model)
        {
            var status = await _service.TransferComplete(model);
            return Ok(status);
        }
        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpPost]
        public async Task<ActionResult> AddAsync([FromBody] OrderDto model)
        {
            return StatusCodeResult(await _service.AddAsync(model));
        }
        [HttpPost]
        public async Task<ActionResult> PlaceOrder()
        {
            return StatusCodeResult(await _service.PlaceOrder());
        }
        [HttpPut]
        public async Task<ActionResult> UpdateAsync([FromBody] OrderDto model)
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
        public async Task<ActionResult> GetProductsInOrder(string langId)
        {
            return Ok(await _service.GetProductsInOrder(langId));
        }
        [HttpGet]
        public async Task<ActionResult> GetProductsInOrderByAdmin(string langId)
        {
            return Ok(await _service.GetProductsInOrderByAdmin(langId));
        }

        [HttpGet]
        public async Task<ActionResult> GetProductsInOrderPendingByAdmin(string langId)
        {
            return Ok(await _service.GetProductsInOrderPendingByAdmin(langId));
        }
        [HttpGet("{langId}/{startDate}/{endDate}")]
        public async Task<ActionResult> GetUserDelevery(string langId , DateTime startDate, DateTime endDate)
        {
            return Ok(await _service.GetUserDelevery(langId, startDate, endDate));
        }
        [HttpGet]
        public async Task<ActionResult> GetProductsInOrderBuyingByAdmin(string langId)
        {
            return Ok(await _service.GetProductsInOrderBuyingByAdmin(langId));
        }
        [HttpGet]
        public async Task<ActionResult> GetProductsInOrderCompleteByAdmin(string langId)
        {
            return Ok(await _service.GetProductsInOrderCompleteByAdmin(langId));
        }
        [HttpGet]
        public async Task<ActionResult> GetWithPaginationsAsync(PaginationParams paramater)
        {
            return Ok(await _service.GetWithPaginationsAsync(paramater));
        }

    }
}
