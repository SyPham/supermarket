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

        [HttpGet("{langId}/{teamId}")]
        public async Task<IActionResult> ReportBuyPersion(string langId, int teamId)
        {
            var bin = await _service.ReportBuyPersion(langId, teamId);
            return File(bin, "application/octet-stream", "ReportBuyPersion.xlsx");
        }
        [HttpGet("{langId}/{teamId}")]
        public async Task<IActionResult> ReportBuyItem(string langId, int teamId)
        {
            var bin = await _service.ReportBuyItem(langId, teamId);
            return File(bin, "application/octet-stream", "ReportBuyItem.xlsx");
        }
        [HttpPut]
        public async Task<IActionResult> TransferBuyList(List<AddToBuyListDto> model)
        {
            var status = await _service.Transfer(model);
            return Ok(status);
        }
        [HttpPut]
        public async Task<IActionResult> CancelBuyingList(List<AddToBuyListDto> model)
        {
            var status = await _service.CancelBuyList(model);
            return Ok(status);
        }
        [HttpPut]
        public async Task<IActionResult> CancelPendingList(List<AddToBuyListDto> model)
        {
            var status = await _service.CancelPendingList(model);
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
        public async Task<ActionResult> GetProductsForCartStatus(string langId)
        {
            return Ok(await _service.GetProductsForCartStatus(langId));
        }

        [HttpGet("{langId}/{teamId}")]
        public async Task<ActionResult> GetProductsInOrderPendingByAdmin(string langId , int teamId)
        {
            return Ok(await _service.GetProductsInOrderPendingByAdmin(langId, teamId));
        }
        [HttpGet("{langId}/{startDate}/{endDate}")]
        public async Task<ActionResult> GetUserDelevery(string langId , DateTime startDate, DateTime endDate)
        {
            return Ok(await _service.GetUserDelevery(langId, startDate, endDate));
        }
        [HttpGet("{langId}")]
        public async Task<ActionResult> GetBuyingBuyPerson(string langId)
        {
            return Ok(await _service.GetBuyingBuyPerson(langId));
        }
        [HttpGet("{langId}/{teamId}")]
        public async Task<ActionResult> GetProductsInOrderBuyingByAdmin(string langId ,int teamId)
        {
            return Ok(await _service.GetProductsInOrderBuyingByAdmin(langId, teamId));
        }
        [HttpGet("{langId}/{teamId}/{startDate}/{endDate}")]
        public async Task<ActionResult> GetProductsInOrderCompleteByAdmin(string langId, int teamId, DateTime startDate, DateTime endDate)
        {
            return Ok(await _service.GetProductsInOrderCompleteByAdmin(langId, teamId, startDate, endDate));
        }
        [HttpGet]
        public async Task<ActionResult> GetWithPaginationsAsync(PaginationParams paramater)
        {
            return Ok(await _service.GetWithPaginationsAsync(paramater));
        }

        [HttpGet]
        public async Task<ActionResult> GetProductsForCartStatusByCompleteStatus(string langId)
        {
            return Ok(await _service.GetProductsForCartStatusByCompleteStatus(langId));
        }

        [HttpGet]
        public async Task<ActionResult> GetProductsForCartStatusByBuyingAndPenidngStatus(string langId)
        {
            return Ok(await _service.GetProductsForCartStatusByBuyingAndPenidngStatus(langId));
        }


  [HttpPut]
        public async Task<ActionResult> UpdateQuantity([FromBody] UpdateQuantityOrderRequest request)
        {
            return StatusCodeResult(await _service.UpdateQuantity(request));
        }
    
        [HttpDelete]
        public async Task<ActionResult> DeleteCart([FromQuery]DeleteCartOrderRequest request)
        {
            return StatusCodeResult(await _service.DeleteCart(request));
        }
        [HttpDelete]
        public async Task<ActionResult> ClearCart()
        {
            return StatusCodeResult(await _service.ClearCart());
        }
          [HttpGet]
        public async Task<ActionResult> GetProductsInCart(string langId)
        {
            return Ok(await _service.GetProductsInCart(langId));
        }
    }
}
