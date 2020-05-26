using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Polly;
using RestSharp;

namespace Mvc2Integrations.Services
{
    class CachedCurrencyCalculator : ICurrencyCalculator
    {
        private readonly ICurrencyCalculator _inner;
        private readonly IMemoryCache _cache;

        public CachedCurrencyCalculator(ICurrencyCalculator inner, IMemoryCache cache)
        {
            _inner = inner;
            _cache = cache;
        }
        public List<string> GetCurrencies()
        {
            var ret =  _cache.GetOrCreate("GetCurrencies", entry => _inner.GetCurrencies());
            if(!ret.Any())
                _cache.Remove("GetCurrencies");
            return ret;
        }

        public decimal ConvertCurrency(string @from, string to, decimal belopp)
        {
            return _inner.ConvertCurrency(from, to, belopp);
        }
    }

    class RetryCurrencyCalculator : ICurrencyCalculator
    {
        private readonly ICurrencyCalculator _inner;

        public RetryCurrencyCalculator(ICurrencyCalculator inner)
        {
            _inner = inner;
        }
        public List<string> GetCurrencies()
        {
            var result = new List<string>();

            Polly.Policy.HandleResult<List<string>>(r => r.Count() == 0)
                .Retry(3, onRetry: (result, i) => System.Threading.Thread.Sleep(i * 10000 + 5000)).Execute(() =>
                    {
                        result = _inner.GetCurrencies();
                        return result;
                    });
            return result;
        }

        public decimal ConvertCurrency(string @from, string to, decimal belopp)
        {
            var result = 0m;
            Polly.Policy.HandleResult<decimal>(r => r == 0)
                .Retry(3, onRetry: (result, i) => System.Threading.Thread.Sleep(i * 10000 + 5000)).Execute(() =>
                {
                    result = _inner.ConvertCurrency(from, to, belopp);
                    return result;
                });
            return result;
        }
    }


    class CurrencyCalculatorRapidApi : ICurrencyCalculator
    {
        public List<string> GetCurrencies()
        {
            var list = new List<string>();
            var client = new RestClient();
            var request = new RestRequest("https://currency-exchange.p.rapidapi.com/listquotes",
                Method.GET, DataFormat.Json);

            request.AddHeader("x-rapidapi-host", "currency-exchange.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "5e6066745bmshaf9bc51c1001802p19cb2bjsn7e247cd625f2");
            var response = client.Execute(request);       


            if (response.StatusCode == HttpStatusCode.OK)
            {
                list = JsonConvert.DeserializeObject<List<string>>(response.Content);
            }
            return list;
        }

        public decimal ConvertCurrency(string @from, string to, decimal belopp)
        {
            var b = $"{belopp:0.0}".Replace(",", ".");
        var client = new RestClient($"https://currency-exchange.p.rapidapi.com/exchange?q={b}&from={from}&to={to}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "currency-exchange.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "5e6066745bmshaf9bc51c1001802p19cb2bjsn7e247cd625f2");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Convert.ToDecimal(response.Content);
            }

            return 0;
        }
    }
}