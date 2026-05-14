using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Triapka.Application.Interfaces;

namespace Triapka.Api.Controllers;

[Authorize(Roles = "Admin")]
public class ProductsController(
    IProductService productService,
    ILogger<ProductsController> logger) : Controller
{
    private readonly IProductService _productService = productService;
    private readonly ILogger<ProductsController> _logger = logger;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index(string? search, int? categoryId)
    {
        IEnumerable<Triapka.Application.DTOs.ProductListDto> products;

        if (categoryId.HasValue)
        {
            products = await _productService.GetProductsByCategoryAsync(categoryId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(search))
        {
            _logger.LogInformation("Користувач шукає товар: {SearchQuery}", search);
            products = await _productService.SearchProductsByNameAsync(search);
        }
        else
        {
            products = await _productService.GetAllProductsAsync();
        }

        return View(products);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);

        return View("Index", products);
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