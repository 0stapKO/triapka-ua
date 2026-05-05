namespace Triapka.Application.Interfaces;

using Triapka.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Triapka.Application.DTOs;

public interface IWishlistService
{
    Task<IEnumerable<WishlistItemDto>> GetWishlistAsync();

    Task<WishlistItemDto> AddToWishlistAsync(int productId);

    Task RemoveFromWishlistAsync(int wishlistItemId);
}