using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Mvc2Integrations.Services
{
    class CurrencyCalculatorRapidApi : ICurrencyCalculator
    {
        public List<string> GetCurrencies()
        {
            throw new NotImplementedException();
        }

        public decimal ConvertCurrency(string @from, string to, decimal belopp)
        {
            throw new NotImplementedException();
        }
    }
}