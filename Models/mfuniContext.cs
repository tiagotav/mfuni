using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;

namespace mfuni.Models
{
    public class schoolContext : DbContext
    {
        public schoolContext() : base()
        {

        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        public System.Data.Entity.DbSet<mfuni.Models.Teacher> Teachers { get; set; }

        public System.Data.Entity.DbSet<mfuni.Models.Student> Students { get; set; }

        public System.Data.Entity.DbSet<mfuni.Models.Grade> Grades { get; set; }
    }
}