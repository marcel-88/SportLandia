﻿using eUseControl.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eUseControl.BusinessLogic.DBModel
{
    class UserContext : DbContext
    {
        public UserContext() :
            base("name=sportLandia") // connectionstring name define in your web.config
        {
        }

        public virtual DbSet<ULoginData> Users { get; set; }
    }
}
