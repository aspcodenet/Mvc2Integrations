using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mvc2Integrations.ViewModels
{
    public class ShowCurrencyConverterViewModel
    {
        [Required]
        [DisplayName("From currency")]
        public string FromCurrency { get; set; }

        [Required]
        [DisplayName("To currency")]
        public string ToCurrency { get; set; }

        public List<SelectListItem> AllCurrencies { get; set; }
        [Required]
        [DisplayName("Amount")]
        public int Belopp { get; set; } = 1;

        public decimal Result { get; set; } = 0;
    }
}