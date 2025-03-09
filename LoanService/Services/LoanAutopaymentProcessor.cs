using LoanService.Database;
using LoanService.Exceptions;
using LoanService.Integrations;
using LoanService.Models.General;
using LoanService.Models.Loan;
using Microsoft.EntityFrameworkCore;

namespace LoanService.Services;

public class LoanAutopaymentProcessor(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<LoanAutopaymentProcessor> logger,
    IRabbitMqTransactionRequestProducer producer
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Loan Payment Processor launched");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    
                    var duePayments = await dbContext.LoanPayments
                        .Where(p => p.PaymentTime <= DateTime.UtcNow && p.Status != PaymentStatus.Payed)
                        .ToListAsync(stoppingToken);

                    foreach (var payment in duePayments)
                    {
                        try
                        {
                            var loan = await dbContext.Loans
                                .Include(l => l.Payments)
                                .FirstOrDefaultAsync(l => l.Payments.Any(p => p.Id == payment.Id));

                            if (loan == null)
                            {
                                throw new ResourceNotFoundException(
                                    $"Payment {payment.Id} should be payed, but it does not have a corresponding loan, so it cannot be payed");
                            }
                            
                            logger.LogInformation($"Taking away {payment.Amount} money from user {loan.UserId} to pay payment {payment.Id} for loan {loan.Id}");
                            await producer.SendTransactionRequestMessage(new TransactionRequest
                            {
                                LoanId = loan.Id,
                                PaymentId = payment.Id,
                                Amount = payment.Amount,
                                Type = TransactionType.LoanPayment,
                                UserId = loan.UserId
                            });
                        }
                        catch (Exception ex)
                        {
                            logger.LogError($"Error during processing payment {payment.Id}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in LoanPaymentProcessor: {ex.Message}");
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}