using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Mvc2Integrations.Models;
using Mvc2Integrations.Services;
using Mvc2Integrations.ViewModels;

namespace Mvc2Integrations.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICurrencyCalculator _currentCalculator;

        public HomeController(ILogger<HomeController> logger, ICurrencyCalculator currentCalculator)
        {
            _logger = logger;
            _currentCalculator = currentCalculator;
        }

        public IActionResult Index()
        {
            var viewModel = new ShowCurrencyConverterViewModel();
            viewModel.AllCurrencies = GetAllCurrencies();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(ShowCurrencyConverterViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Result = _currentCalculator.ConvertCurrency(model.FromCurrency, model.ToCurrency, model.Belopp);
            }
            model.AllCurrencies = GetAllCurrencies();
            return View(model);
        }


        List<SelectListItem> GetAllCurrencies()
        {
            var list = _currentCalculator.GetCurrencies().Select(r => new SelectListItem
            {
                Text = r,
                Value = r
            }).ToList();
            list.Insert(0, new SelectListItem { Value = "", Text = "Select one" });
            return list;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
