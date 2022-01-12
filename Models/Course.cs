using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace mfuni.Models
{
    public class Course
    {
        [Key]
        public int id { get; set; }
        public string Name { get; set; }
        public int Subjects { get; set; }
        public int Teachers { get; set; }
        public int Students { get; set; }
        public double avg { get; set; }
    }
}