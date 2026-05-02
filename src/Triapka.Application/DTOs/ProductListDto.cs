using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triapka.Application.DTOs;
public class ProductListDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public string MainImageUrl { get; set; } = null!;
}
