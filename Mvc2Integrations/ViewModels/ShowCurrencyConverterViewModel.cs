using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mvc2Integrations.ViewModels
{
    public class MustBeUnevenAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            int v = Convert.ToInt32(value);
            return v % 2 != 0;
        }
    }

    public class NotSameAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _otherPropertyName;

        public NotSameAttribute(string otherPropertyName)
        {
            _otherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_otherPropertyName);
            if (property == null)
                throw new ArgumentException("Property with this name not found");
            var comparisonValue = property.GetValue(validationContext.ObjectInstance);
            
            if(value.Equals(comparisonValue))
                return new ValidationResult(ErrorMessage);
            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-notequalto",
                ErrorMessage);
            context.Attributes.Add("data-val-notequalto-other", _otherPropertyName);
        }
    }

    public class ShowCurrencyConverterViewModel
    {
        [Required]
        [DisplayName("From currency")]
        public string FromCurrency { get; set; }

        [NotSame("FromCurrency", ErrorMessage = "Kan inte vara samma som from")]
        //[Compare("FromCurrency")]
        [Required]
        [DisplayName("To currency")]
        public string ToCurrency { get; set; }

        public List<SelectListItem> AllCurrencies { get; set; }
        [Required]
        [DisplayName("Amount")]
        public int Belopp { get; set; } = 1;

        public decimal Result { get; set; } = 0;


        [MustBeUneven(ErrorMessage = "Din dumsnut bla bla")]
        public int Tal { get; set; } = 0;

    }
}