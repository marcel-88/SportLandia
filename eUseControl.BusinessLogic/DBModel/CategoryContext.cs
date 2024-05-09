using eUseControl.Domain.Entities.Product;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eUseControl.BusinessLogic.DBModel
{
    class CategoryContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }

        public CategoryContext() : base("name=sportLandia")
        {
        }
    }
}
