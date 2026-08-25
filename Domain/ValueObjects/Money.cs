namespace Domain.ValueObjects
{
    public sealed class Money
    {
        public decimal Amount { get; }

        private Money(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be less than 0.");
            this.Amount = amount;
        }

        public static Money Of(decimal amount) => new(amount);
        public override bool Equals(object? obj)
        {
            return obj is Money m && Amount == m.Amount;
        }
        public override int GetHashCode() => Amount.GetHashCode();
    }
}