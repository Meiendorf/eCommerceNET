using System.Linq.Expressions;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Response;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interface;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;

namespace OrderApi.Infrastructure.Repositories;

public class OrderRepository(OrderDbContext context) : IOrder
{
    public async Task<GenericResponse> CreateAsync(Order entity)
    {
        try
        {
            var createdOrder = context.Orders.Add(entity).Entity;
            await context.SaveChangesAsync();

            return createdOrder.Id > 0
                ? new GenericResponse(true, $"Order with ID {entity.Id} has been created")
                : new GenericResponse(false, "Order creation failed");
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            return new GenericResponse(false, "Error occurred while adding a new order");
        }
    }

    public async Task<GenericResponse> UpdateAsync(Order entity)
    {
        try
        {
            var existingOrder = await FindByIdAsync(entity.Id);
            if (existingOrder is null)
            {
                return new GenericResponse(false, $"Order with ID {entity.Id} does not exist");
            }

            context.Entry(existingOrder).State = EntityState.Detached;
            context.Orders.Update(entity);
            await context.SaveChangesAsync();

            return new GenericResponse(true, $"Order with ID {entity.Id} has been updated");
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            return new GenericResponse(false, "Error occurred while updating the order");
        }
    }

    public async Task<GenericResponse> DeleteAsync(Order entity)
    {
        try
        {
            var existingOrder = await FindByIdAsync(entity.Id);
            if (existingOrder is null)
            {
                return new GenericResponse(false, $"Order with ID {entity.Id} does not exist");
            }

            context.Orders.Remove(existingOrder);
            await context.SaveChangesAsync();

            return new GenericResponse(true, $"Order with ID {entity.Id} has been deleted");
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            return new GenericResponse(false, "Error occurred while deleting the order");
        }
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        try
        {
            return await context.Orders.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            throw new Exception("Error occurred while retrieving orders");
        }
    }

    public async Task<Order?> FindByIdAsync(int id)
    {
        try
        {
            return await context.Orders.FindAsync(id);
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            throw new Exception("Error occurred while retrieving order");
        }
    }

    public async Task<Order?> GetByAsync(Expression<Func<Order, bool>> predicate)
    {
        return await context.Orders.Where(predicate).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
    {
        try
        {
            return await context.Orders.Where(predicate).ToListAsync();
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            throw new Exception("Error occurred while retrieving filtered orders");
        }
    }
}