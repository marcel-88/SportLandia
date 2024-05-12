using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eUseControl.Domain.Entities.Product
{
    public class ReviewFormModel
    {
        public int ProductId { get; set; }
        public string Username { get; set; } 
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

}
