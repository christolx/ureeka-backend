namespace ureeka_backend.Models
{
    public class TransactionRequest
    {
        public string OrderId { get; set; } = string.Empty;
        public int Amount { get; set; }
    }

    public class MidtransRequest
    {
        public TransactionDetail TransactionDetails { get; set; } = new TransactionDetail();
        public CreditCard CreditCard { get; set; } = new CreditCard();
        public CustomerDetails CustomerDetails { get; set; } = new CustomerDetails();
    }

    public class TransactionDetail
    {
        public string OrderId { get; set; } = string.Empty;
        public int GrossAmount { get; set; }
    }

    public class CreditCard
    {
        public bool Secure { get; set; }
    }

    public class CustomerDetails
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class MidtransResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
    }
}