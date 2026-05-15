using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;
using Triapka.Domain.Entities;

namespace Triapka.Api.Controllers;

[Authorize]
public class CartController(
    ICartService cartService,
    IOrderService orderService,
    ILogger<CartController> logger,
    UserManager<ApplicationUser> userManager) : Controller
{
    private readonly ICartService _cartService = cartService;
    private readonly IOrderService _orderService = orderService;
    private readonly ILogger<CartController> _logger = logger;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        CartDto dto = await _cartService.GetCartAsync();
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId)
    {
        await _cartService.AddToCartAsync(productId);
        _logger.LogInformation("Product {ProductId} was added to the cart", productId);
        return Json(new { success = true, message = "Товар додано в кошик" });
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int itemId)
    {
        await _cartService.RemoveFromCartAsync(itemId);
        _logger.LogInformation("Item {ItemId} was removed from the cart", itemId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int itemId, int newQuantity)
    {
        await _cartService.UpdateQuantityAsync(itemId, newQuantity);
        _logger.LogInformation("Item {ItemId} quantity updated to {NewQuantity}", itemId, newQuantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Auth");
        }

        var cart = await _cartService.GetCartAsync();
        ViewBag.Cart = cart;

        var user = await _userManager.FindByIdAsync(userId);

        var model = new CheckoutDto();

        if (user != null)
        {
            model.Phone = user.PhoneNumber ?? string.Empty;
            model.ShippingAddress = user.Address ?? string.Empty;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutDto dto)
    {
        CartDto cart = await _cartService.GetCartAsync();
        if (!ModelState.IsValid)
        {
            ViewBag.Cart = cart;
            return View(dto);
        }

        try
        {
            var result = await _orderService.CreateOrderAsync(dto);
            _logger.LogInformation("Order {OrderId} was created successfully", result.OrderId);
            return View("CheckoutSuccess", result);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Cart = cart;
            return View(dto);
        }
    }
}