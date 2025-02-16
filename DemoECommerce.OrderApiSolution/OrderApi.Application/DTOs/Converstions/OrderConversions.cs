using OrderApi.Domain.Entities;

namespace OrderApi.Application.DTOs.Converstions;

public static class OrderConversions
{
    public static Order ToEntity(this OrderDTO orderDto) => new Order()
    {
        Id = orderDto.Id,
        ProductId = orderDto.ProductId,
        ClientId = orderDto.ClientId,
        OrderDate = orderDto.OrderDate,
        PurchaseQuantity = orderDto.PurchaseQuantity
    };

    public static OrderDTO ToDto(this Order order) => new OrderDTO(
        order.Id,
        order.ProductId,
        order.ClientId,
        order.PurchaseQuantity,
        order.OrderDate
    );

    public static IEnumerable<OrderDTO> ToDto(this IEnumerable<Order> orders) => orders
        .Select(o => new OrderDTO(o.Id, o.ProductId, o.ClientId, o.PurchaseQuantity, o.OrderDate)).ToList();
}