using LivestockManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestockManagementSystem.Services
{
    public static class DatabaseIntilizer
    {
        public static void Initialize()
        {
            using var db = new ApplicationDbContext();
            db.Database.Migrate();
        }
    }
}
