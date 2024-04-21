using eUseControl.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eUseControl.BusinessLogic.DBModel
{
  public class SessionContext : DbContext
  {
    public SessionContext() : base("name=sportLandia")
    {
    }

    public virtual DbSet<Session> Sessions { get; set; }
  }
}
