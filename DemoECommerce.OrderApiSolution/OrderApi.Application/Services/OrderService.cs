using System.Net.Http.Json;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Converstions;
using OrderApi.Application.Interface;
using Polly;
using Polly.Registry;

namespace OrderApi.Application.Services;

public class OrderService(
    HttpClient httpClient,
    ResiliencePipelineProvider<string> resiliencePipelineProvider,
    IOrder orderRepository) : IOrderService
{
    public async Task<ProductDTO?> GetProduct(int productId)
    {
        var getProduct = await httpClient.GetAsync($"api/products/{productId}");
        if (!getProduct.IsSuccessStatusCode)
        {
            return null;
        }

        return await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
    }

    public async Task<AppUserDTO?> GetUser(int userId)
    {
        var getUser = await httpClient.GetAsync($"api/users/{userId}");
        if (!getUser.IsSuccessStatusCode)
        {
            return null;
        }

        return await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
    }

    public async Task<IEnumerable<OrderDTO>?> GetOrdersByClientId(int clientId)
    {
      var orders = await orderRepository.GetOrdersAsync(o => o.ClientId == clientId);
      if (!orders.Any())
      {
          return null;
      }

      return orders.ToDto();
    }

    public async Task<OrderDetailDTO?> GetOrderDetail(int orderId)
    {
        var order = await orderRepository.FindByIdAsync(orderId);
        if (order is null || order.Id <= 0)
        {
            return null;
        }

        var retryPipeline = resiliencePipelineProvider.GetPipeline("my-retry-pipeline");


        var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

        var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

        return new OrderDetailDTO(
            order.Id,
            productDTO!.Id,
            appUserDTO!.Id,
            appUserDTO.Email,
            appUserDTO.PhoneNumber,
            productDTO.Name,
            order.PurchaseQuantity,
            productDTO.Price,
            productDTO.Quantity * order.PurchaseQuantity,
            order.OrderDate
        );
    }
}