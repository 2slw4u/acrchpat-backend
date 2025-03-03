using LoanService.Database;
using LoanService.Database.TableModels;
using LoanService.Models.Rate;
using LoanService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LoanService.Services.Logic;

public class RateService : IRateService
{
    private readonly AppDbContext _dbContext;

    public RateService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Guid> CreateRate(RateCreateModel model)
    {
        var rate = new Rate
        {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.UtcNow,
            Name = model.Name,
            RateValue = model.Rate
        };
        
        await _dbContext.Rates.AddAsync(rate);
        await _dbContext.SaveChangesAsync();
        
        return rate.Id;
    }

    public async Task<List<RateDto>> RateList()
    {
        return await _dbContext.Rates
            .Select(rate => new RateDto
            {
                Id = rate.Id,
                Name = rate.Name,
                Rate = rate.RateValue
            }).ToListAsync();
    }
}