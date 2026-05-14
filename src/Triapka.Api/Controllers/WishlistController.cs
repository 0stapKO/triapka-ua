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
        var (_, isNew) = await _wishlistService.AddToWishlistAsync(productId);
        if (isNew)
        {
            _logger.LogInformation("Product {ProductId} was added to the wishlist", productId);
            return Json(new { success = true, message = "Товар додано до вподобань" });
        }
        else
        {
            return Json(new { success = false, message = "Товар вже є у вподобаних" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int itemId)
    {
        await _wishlistService.RemoveFromWishlistAsync(itemId);
        _logger.LogInformation("Item {ItemId} was removed from the wishlist", itemId);
        return RedirectToAction(nameof(Index));
    }
}