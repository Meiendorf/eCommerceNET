using System.Linq.Expressions;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Response;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Infrastructure.Repositories;

public class ProductRepository(ProductDbContext context) : IProduct
{
    public async Task<GenericResponse> CreateAsync(Product entity)
    {
        try
        {
            var getProduct = await GetByAsync(p => p.Name == entity.Name);
            if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
            {
                return new GenericResponse(false, $"Product with name {entity.Name} already exists");
            }

            var currentEntity = context.Products.Add(entity).Entity;
            await context.SaveChangesAsync();

            if (currentEntity.Id > 0)
            {
                return new GenericResponse(true, $"Product with id {entity.Id} has been created");
            }
            else
            {
                return new GenericResponse(false, $"Product with name {entity.Name} does not exist");
            }
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            return new GenericResponse(false, "Error occurred while adding new product");
        }
    }

    public async Task<GenericResponse> UpdateAsync(Product entity)
    {
        try
        {
            var product = await FindByIdAsync(entity.Id);
            if (product is null)
            {
                return new GenericResponse(false, $"Product with id {entity.Id} does not exist");
            }
            
            context.Entry(product).State = EntityState.Detached;
            context.Products.Update(product);
            await context.SaveChangesAsync();
            
            return new GenericResponse(true, $"Product with id {entity.Id} has been updated");
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            return new GenericResponse(false, "Error occurred while updating product");
        }
    }

    public async Task<GenericResponse> DeleteAsync(Product entity)
    {
        try
        {
            var product = await FindByIdAsync(entity.Id);
            if (product is null)
            {
                return new GenericResponse(false, $"Product with id {entity.Id} does not exist");
            }

            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return new GenericResponse(true, $"Product with id {entity.Id} has been deleted");
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            return new GenericResponse(false, "Error occurred while deleting product");
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        try
        {
            return await context.Products.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            throw new Exception("Error occurred while getting all products");
        }
    }

    public async Task<Product?> FindByIdAsync(int id)
    {
        try
        {
            return await context.Products.FindAsync(id);
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            throw new Exception("Error occurred while getting product");
        }
    }

    public async Task<Product?> GetByAsync(Expression<Func<Product, bool>> predicate)
    {
        return await context.Products.Where(predicate).FirstOrDefaultAsync();
    }
}