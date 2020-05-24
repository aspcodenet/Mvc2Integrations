using System.Collections.Generic;

namespace Mvc2Integrations.Services
{
    public interface ICurrencyCalculator
    {
        public List<string> GetCurrencies();
        public decimal ConvertCurrency(string from, string to, decimal belopp);
    }
}