﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treefort.Entity.Validation
{
    public class RequireUtcDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Get Value of the property
            var date = (DateTime)value;

            if (date.Kind != DateTimeKind.Utc)
            {
                return new ValidationResult("Date is not of UTC kind");
            }

            return null;
        }
    }
}
