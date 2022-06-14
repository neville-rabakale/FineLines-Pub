﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FineLines.Models.ViewModels
{
    public class ProductVM
    {
        public Product product { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CoverTypeList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> PackagingList { get; set; }
    }
}
