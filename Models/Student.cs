using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mfuni.Models
{
    public class Student
    {
        [Key]
        public int Student_id { get; set; }
        public string Name { get; set; }
        public DateTime BDay { get; set; }
        public int RNumber { get; set; }
        public int Course_id { get; set; }
        public string Course_Name { get; set; }
        public Course Course { get; set; }
        public double avg { get; set; }
    }
}