namespace Ordering.Domain.ValueObjects
{
    public record Payment
    {
        public string? CardName { get; } = default!;
        public string CardNumber { get; } = default!;
        public string Expiration { get; } = default!;
        public string CVV { get; } = default!;
        public int PaymentMethod { get; set; }

        protected Payment()
        {
        }

        private Payment(string cardName, string cardNumber, string expiration, string cvv, int paymentMethod)
        {
            CardName = cardName;
            CardNumber = cardNumber;
            Expiration = expiration;
            CVV = cvv;
            PaymentMethod = paymentMethod;
        }

        public static Payment Of(string cardName, string cardNumber, string expiration, string cvv, int paymentMethod)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cardName, nameof(cardName));
            ArgumentException.ThrowIfNullOrWhiteSpace(cardNumber, nameof(cardNumber));
            ArgumentException.ThrowIfNullOrWhiteSpace(cvv, nameof(cvv));
            // Valida se é menor que 3 ou maior que 4
            ArgumentOutOfRangeException.ThrowIfLessThan(cvv.Length, 3, nameof(cvv));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(cvv.Length, 4, nameof(cvv));

            return new Payment(cardName, cardNumber, expiration, cvv, paymentMethod);
        }
    }
}
