using eUseControl.Domain.Entities.Product;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eUseControl.BusinessLogic.DBModel
{
    class ReviewContext : DbContext
    {
        public DbSet<Review> Reviews { get; set; }

        public ReviewContext() : base("name=sportLandia")
        {
        }
    }
}
