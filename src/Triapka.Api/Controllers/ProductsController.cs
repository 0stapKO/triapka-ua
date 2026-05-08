using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Triapka.Application.Interfaces;

namespace Triapka.Api.Controllers;

[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index(string? search)
    {
        IEnumerable<Triapka.Application.DTOs.ProductListDto> products;

        if (string.IsNullOrWhiteSpace(search))
        {
            products = await _productService.GetAllProductsAsync();
        }
        else
        {
            _logger.LogInformation("Користувач шукає товар: {SearchQuery}", search);
            products = await _productService.SearchProductsByNameAsync(search);
        }

        return View(products);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product is null)
        {
            _logger.LogWarning("Товар з ID {Id} не знайдено", id);
            return NotFound();
        }

        return View(product);
    }
}
