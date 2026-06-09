using Microsoft.AspNetCore.Mvc;
using ArslanTeklifSistemi.Services;

namespace ArslanTeklifSistemi.Controllers
{
    [ApiController]
    [Route("api/offer")]
    public class OfferApiController : ControllerBase
    {
        private readonly WooCommerceService _wooCommerceService;

        public OfferApiController(WooCommerceService wooCommerceService)
        {
            _wooCommerceService = wooCommerceService;
        }

        [HttpGet("product/{sku}")]
        public async Task<IActionResult> GetProduct(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                return BadRequest(new { message = "Stok kodu boş olamaz." });
            }

            var product = await _wooCommerceService.GetProductBySkuAsync(sku.Trim());

            if (product == null)
            {
                return NotFound(new { message = "Ürün sitemizde bulunamadı." });
            }

            return Ok(product);
        }

        [HttpGet("rates")]
        public async Task<IActionResult> GetRates()
        {
            var rates = await _wooCommerceService.GetLiveExchangeRatesAsync();
            return Ok(rates);
        }
    }
}