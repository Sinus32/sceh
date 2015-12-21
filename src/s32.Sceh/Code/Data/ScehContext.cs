using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace s32.Sceh.Code.Data
{
    public class ScehContext : DbContext
    {
        public ScehContext()
            : base("name=ScehDbConnection")
        {
        }

        public DbSet<SteamAppInfo> SteamAppInfos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}