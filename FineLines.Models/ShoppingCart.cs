using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FineLines.Models
{
    public class ShoppingCart
    {
        public Product Product { get; set; }
        [Range(1,1000, ErrorMessage = "Value must be between 1 and 1000")]
        public int Count { get; set; }
    }
}
