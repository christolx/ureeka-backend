using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ureeka_backend.Models;

public class PaymentResults
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string OrderId { get; set; } = string.Empty;
    public string StatusCode { get; set; } = string.Empty;
    public string TransactionStatus { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public DateTime RecordedAt { get; set; }
}