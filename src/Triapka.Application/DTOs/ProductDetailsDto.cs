using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triapka.Application.DTOs;
public class ProductDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public List<string> ImageUrls { get; set; } = [];

    public string CategoryName { get; set; } = null!;

    public float Rate { get; set; }
}
