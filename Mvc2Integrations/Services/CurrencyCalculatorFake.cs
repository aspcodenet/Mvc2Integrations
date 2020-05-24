using System.Collections.Generic;

namespace Mvc2Integrations.Services
{
    public class CurrencyCalculatorFake : ICurrencyCalculator
    {
        public List<string> GetCurrencies()
        {
            return new List<string>
            {
                "SGD", "MYR", "EUR", "USD", "AUD", "JPY", "CNH", "HKD", "CAD", "INR", "DKK", "GBP", "RUB", "NZD", "MXN", "IDR", "TWD", "THB", "VND"
            };
        }

        public decimal ConvertCurrency(string @from, string to, decimal belopp)
        {
            return belopp*10;
        }
    }
}