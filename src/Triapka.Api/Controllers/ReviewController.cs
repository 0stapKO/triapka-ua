using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Triapka.Application.DTOs;
using Triapka.Application.Interfaces;
using Triapka.Application.Services;

namespace Triapka.Api.Controllers;

[Authorize]
public class ReviewsController : Controller
{
    private readonly IReviewService reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        this.reviewService = reviewService;
    }

    [HttpPost]
    public async Task<IActionResult> Add(CreateReview dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Auth");
        }

        dto.UserId = userId;

        if (ModelState.IsValid)
        {
            await reviewService.AddReviewAsync(dto);
        }

        return RedirectToAction("Details", "Products", new { id = dto.ProductId });
    }
}