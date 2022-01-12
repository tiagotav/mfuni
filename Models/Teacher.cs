using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace mfuni.Models
{
    public class Teacher
    {
        [Key]
        public int id { get; set; }
        public string Name { get; set; }
        public DateTime BDay { get; set; }
        public float salary { get; set; }
        public ICollection<Subject> Subjects { get; set; }
        public int num_subjects { get; set; }
    }
}