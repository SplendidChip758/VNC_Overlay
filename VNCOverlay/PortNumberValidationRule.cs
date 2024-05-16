﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VNCOverlay
{
    public class PortNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string pattern = @"^(\d+)(,\d+)*$"; // Regex pattern for comma-separated integers
            if (value == null || !Regex.IsMatch(value.ToString(), pattern))
            {
                return new ValidationResult(false, "Invalid format. Please enter numbers separated by commas.");
            }

            return ValidationResult.ValidResult;
        }
    }
}
