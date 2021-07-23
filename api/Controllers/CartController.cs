using Microsoft.AspNetCore.Mvc;
using Supermarket.DTO;
using Supermarket.Helpers;
using Supermarket.Services;
using System.Threading.Tasks;

namespace Supermarket.Controllers
{
    public class CartController : ApiControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpPost]
        public async Task<ActionResult> AddAsync([FromBody] CartDto model)
        {
            return StatusCodeResult(await _service.AddAsync(model));
        }
        [HttpPost]
        public async Task<ActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            return StatusCodeResult(await _service.AddToCart(request));
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAsync([FromBody] CartDto model)
        {
            return StatusCodeResult(await _service.UpdateAsync(model));
        }
        [HttpPut]
        public async Task<ActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            return StatusCodeResult(await _service.UpdateQuantity(request));
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            return StatusCodeResult(await _service.DeleteAsync(id));
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteCart([FromQuery]DeleteCartRequest request)
        {
            return StatusCodeResult(await _service.DeleteCart(request));
        }
        [HttpDelete]
        public async Task<ActionResult> ClearCart()
        {
            return StatusCodeResult(await _service.ClearCart());
        }
        [HttpGet]
        public async Task<ActionResult> CartTotal()
        {
            return Ok(await _service.CartTotal());
        }
        [HttpGet]
        public async Task<ActionResult> GetByIdAsync(int id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }
        [HttpGet]
        public async Task<ActionResult> GetProductsInCart(string langId)
        {
            return Ok(await _service.GetProductsInCart(langId));
        }
        [HttpGet]
        public async Task<ActionResult> GetWithPaginationsAsync(PaginationParams paramater)
        {
            return Ok(await _service.GetWithPaginationsAsync(paramater));
        }

    }
}
