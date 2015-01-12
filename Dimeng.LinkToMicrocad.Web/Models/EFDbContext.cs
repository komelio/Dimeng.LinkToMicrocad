using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Dimeng.LinkToMicrocad.Web.Models
{
    public class EFDbContext : DbContext
    {
        public DbSet<MVProduct> Products { get; set; }
    }
}