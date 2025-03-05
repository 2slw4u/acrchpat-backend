using LoanService.Models.Rate;

namespace LoanService.Services.Interfaces;

public interface IRateService
{
    Task<Guid> CreateRate(RateCreateModel model);

    Task<List<RateDto>> RateList();
}