using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators;
using ureeka_backend.Models;

namespace ureeka_backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class MidtransController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MidtransController> _logger;

        public MidtransController(IConfiguration configuration, ILogger<MidtransController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("generate-snap-token")]
        public async Task<IActionResult> GenerateSnapToken([FromBody] TransactionRequest transactionRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(transactionRequest.OrderId) || transactionRequest.Amount <= 0)
                {
                    return BadRequest(new { error = "OrderId dan Amount harus valid" });
                }

                string serverKey = _configuration["Midtrans:ServerKey"];
                
                var midtransRequestObj = new MidtransRequest
                {
                    TransactionDetails = new TransactionDetail
                    {
                        OrderId = transactionRequest.OrderId,
                        GrossAmount = transactionRequest.Amount
                    },
                    CreditCard = new CreditCard
                    {
                        Secure = true
                    },
                    CustomerDetails = new CustomerDetails
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "johndoe@example.com",
                        Phone = "08123456789"
                    }
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                };
                string jsonRequest = JsonSerializer.Serialize(midtransRequestObj, jsonOptions);
                
                var clientOptions = new RestClientOptions("https://app.sandbox.midtrans.com")
                {
                    Authenticator = new HttpBasicAuthenticator(serverKey, "")
                };
                var client = new RestClient(clientOptions);

                var restRequest = new RestRequest("snap/v1/transactions", Method.Post);
                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddStringBody(jsonRequest, DataFormat.Json);

                var response = await client.ExecuteAsync(restRequest);

                if (response.IsSuccessful)
                {
                    var responseObj = JsonSerializer.Deserialize<MidtransResponse>(
                        response.Content,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        }
                    );

                    return Ok(new { snapToken = responseObj.Token });
                }
                else
                {
                    _logger.LogError($"Midtrans API error: {response.Content}");
                    return StatusCode(500, new { error = "Gagal membuat transaksi", details = response.Content });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating snap token");
                return StatusCode(500, new { error = ex.Message });
            }
        }
        
        [AllowAnonymous]
        [HttpPost("notification")]
        public async Task<IActionResult> HandleNotification()
        {
            try
            {
                using var reader = new System.IO.StreamReader(Request.Body, Encoding.UTF8);
                var body = await reader.ReadToEndAsync();

                _logger.LogInformation($"Received notification: {body}");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling notification");
                return StatusCode(500, new { error = ex.Message });
            }
        }
        
        [HttpGet("status/{orderId}")]
        public async Task<IActionResult> CheckStatus(string orderId)
        {
            try
            {
                string serverKey = _configuration["Midtrans:ServerKey"];
                
                var options = new RestClientOptions("https://api.sandbox.midtrans.com")
                {
                    Authenticator = new HttpBasicAuthenticator(serverKey, "")
                };
                var client = new RestClient(options);

                var request = new RestRequest($"v2/{orderId}/status", Method.Get);
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    var responseData = JsonSerializer.Deserialize<object>(
                        response.Content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    return Ok(responseData);
                }
                else
                {
                    _logger.LogError($"Midtrans status API error: {response.Content}");
                    return StatusCode(500, new { error = "Gagal mendapatkan status transaksi" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking transaction status");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}