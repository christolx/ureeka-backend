using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ureeka_backend.Data;
using ureeka_backend.Models;

namespace ureeka_backend.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DonationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<DonationsController> _logger;

    public DonationsController(
        AppDbContext context,
        ILogger<DonationsController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    [HttpPost("record-payment")]
    public async Task<IActionResult> RecordPayment([FromBody] PaymentResults paymentData)
    {
        try
        {
            _logger.LogInformation($"Recording payment for order {paymentData.OrderId}");
            
            var paymentResult = new PaymentResults
            {
                OrderId = paymentData.OrderId,
                StatusCode = paymentData.StatusCode,
                TransactionStatus = paymentData.TransactionStatus,
                UserEmail = paymentData.UserEmail,
                IsSuccessful = paymentData.StatusCode == "200" && paymentData.TransactionStatus == "settlement",
                RecordedAt = DateTime.UtcNow
            };
            
            _context.PaymentResults.Add(paymentResult);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Successfully recorded payment for order {paymentData.OrderId} from {paymentData.UserEmail}");

            return Ok(new { success = true, message = "Payment recorded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording payment");
            return StatusCode(500, new { success = false, message = "Failed to record payment" });
        }
    }
}