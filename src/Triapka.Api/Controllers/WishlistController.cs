using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;

namespace Triapka.Api.Controllers;

[Authorize]
public class WishlistController(
    IWishlistService wishlistService,
    ILogger<WishlistController> logger) : Controller
{
    private readonly IWishlistService _wishlistService = wishlistService;
    private readonly ILogger<WishlistController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        IEnumerable<WishlistItemDto> dto = await _wishlistService.GetWishlistAsync();
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId)
    {
        await _wishlistService.AddToWishlistAsync(productId);
        _logger.LogInformation("Product {ProductId} was added to the wishlist", productId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int itemId)
    {
        await _wishlistService.RemoveFromWishlistAsync(itemId);
        _logger.LogInformation("Item {ItemId} was removed from the wishlist", itemId);
        return RedirectToAction(nameof(Index));
    }
}