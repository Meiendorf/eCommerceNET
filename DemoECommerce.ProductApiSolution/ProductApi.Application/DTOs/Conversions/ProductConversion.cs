using ProductApi.Domain.Entities;

namespace ProductApi.Application.DTOs.Conversions;

public static class ProductConversion
{
    public static Product ToEntity(this ProductDTO product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        Quantity = product.Quantity
    };

    public static (ProductDTO?, IEnumerable<ProductDTO>?) ToDTO(Product? product, IEnumerable<Product>? products)
    {
        if (product is not null)
        {
            return (new ProductDTO(
                product.Id,
                product.Name!,
                product.Price,
                product.Quantity), null);
        }

        if (products is not null)
        {
            return (null, products.Select(p =>
                new ProductDTO(p.Id, p.Name!, p.Price, p.Quantity)).ToList());
        }

        return (null, null);
    }
}