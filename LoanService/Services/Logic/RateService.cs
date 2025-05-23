﻿using LoanService.Database;
using LoanService.Database.TableModels;
using LoanService.Exceptions;
using LoanService.Models.Rate;
using LoanService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LoanService.Services.Logic;

public class RateService(AppDbContext dbContext) : IRateService
{
    
    public async Task<Guid> CreateRate(RateCreateModel model, string? idempotencyKey)
    {
        Request? requestEntity = null;
        if (idempotencyKey != null && idempotencyKey != "")
        {
            requestEntity = dbContext.Requests.Where(x => x.IdempotencyKey == idempotencyKey).FirstOrDefault();
        }
        if (requestEntity != null)
        {
            throw new RequestIsAlreadyProcessing("Запрос находится в обработке или уже был обработан");
        }
        else if (idempotencyKey != null && idempotencyKey != "")
        {
            var newRequestEntity = new Request
            {
                Id = Guid.NewGuid(),
                IdempotencyKey = idempotencyKey,
                OperationName = "CreateRate"
            };
            dbContext.Requests.Add(newRequestEntity);
            await dbContext.SaveChangesAsync();
        }

        var existingRate = dbContext.Rates
            .FirstOrDefault(r => r.Name == model.Name);

        if (existingRate != null)
        {
            throw new BadRequestException($"Rate named {model.Name} already exists");
        }
        
        var rate = new Rate
        {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.UtcNow,
            Name = model.Name,
            RateValue = model.YearlyRate
        };
        
        await dbContext.Rates.AddAsync(rate);
        await dbContext.SaveChangesAsync();
        
        return rate.Id;
    }

    public async Task<List<RateDto>> RateList()
    {
        return await dbContext.Rates
            .Select(rate => new RateDto
            {
                Id = rate.Id,
                Name = rate.Name,
                YearlyRate = rate.RateValue
            }).ToListAsync();
    }
}