using Microsoft.EntityFrameworkCore;
using ProductProvider.Infrastructure.Contexts;
using ProductProvider.Infrastructure.Entities;
using ProductProvider.Infrastructure.Models;

namespace ProductProvider.Infrastructure.Services;

public class ReviewService(IDbContextFactory<DataContext> context)
{
    private readonly IDbContextFactory<DataContext> _context = context;


    public async Task<ReviewEntity> CreateReviewAsync(ReviewModel reviewModel)
    {
        try
        {
            await using var context = _context.CreateDbContext();

            if (reviewModel.Text != null && reviewModel.Stars != null && reviewModel.ProductId != Guid.Empty)
            {
                var reviewEntity = new ReviewEntity
                {
                    Stars = reviewModel.Stars,
                    Text = reviewModel.Text,
                    ProductId = reviewModel.ProductId,
                };
                context.Reviews.Add(reviewEntity);
                await context.SaveChangesAsync();
                await context.DisposeAsync();
                Console.WriteLine("Review successfully created");
                return reviewEntity;
            }
            return null!;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }
    }

    public async Task<IEnumerable<ReviewEntity>> GetAllReviewsAsync()
    {
        try
        {
            await using var context = _context.CreateDbContext();

            var reviewEntities = await context.Reviews.Include(x => x.Product).ToListAsync();
            if (reviewEntities.Count == 0)
            {
                return Enumerable.Empty<ReviewEntity>();
            }
            else
            {
               
                return reviewEntities;
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }
    }
    public async Task<IEnumerable<ReviewModel>> GetReviewsByProductId(Guid productId)
    {
        try
        {
            await using var context = _context.CreateDbContext();

            var reviewEntities = await context.Reviews
              .Where(r => r.ProductId == productId)
              .ToListAsync();
            if (reviewEntities.Count == 0)
            {
                return Enumerable.Empty<ReviewModel>();
            }
            else
            {
                var reviews = reviewEntities.Select(r => new ReviewModel
                {
                    Id = r.Id,
                    Stars = r.Stars,
                    Text = r.Text,
                    ProductId = r.ProductId,
                }).ToList();

                return reviews;
            }


        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return Enumerable.Empty<ReviewModel>();
        }
    }

    public async Task<ReviewModel> GetReviewById(Guid id)
    {
        try
        {
            await using var context = _context.CreateDbContext();

            var reviewEntity = await context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if(reviewEntity != null)
            {
                var reviewModel = new ReviewModel
                {
                    Id = reviewEntity.Id,
                    Stars = reviewEntity.Stars,
                    Text = reviewEntity.Text,
                    ProductId = reviewEntity.ProductId,
                };
                return reviewModel;
            }
            else
            {
                return null!;
            }   
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }
    }

  
    public async Task<ReviewModel> UpdateReviewAsync(ReviewUpdateRequest request)
    {
        try
        {
            await using var context = _context.CreateDbContext();

            var reviewEntity = await context.Reviews.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (reviewEntity == null)
            {
                return null!;
            }
            else
            {
                reviewEntity.Stars = request.Stars;
                reviewEntity.Text = request.Text;
                await context.SaveChangesAsync();
                await context.DisposeAsync();
                var review = new ReviewModel
                {
                    Stars = reviewEntity.Stars,
                    Text = reviewEntity.Text,
                };
                Console.WriteLine("ReviewEntity was successfully updated");
                return review;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null!;
        }

    }
    public async Task<bool> DeleteReviewAsync(Guid id)
    {
        try
        {
            await using var context = _context.CreateDbContext();
            var reviewEntity = await context.Reviews.FirstOrDefaultAsync(i => i.Id == id);
            if (reviewEntity == null) return false;

            context.Reviews.Remove(reviewEntity);
            await context.SaveChangesAsync();
            Console.WriteLine("ReviewEntity successfully removed");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }

}



