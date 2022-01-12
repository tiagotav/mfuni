using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mfuni.Models
{
    public class Grade
    {
        [Key]
        public int id { get; set; }
        public double val { get; set; }
        [ForeignKey("Student")]
        public int Student_id { get; set; }
        //Para ser foreign key nome da table students tem queser id_student e aqui tambem*/
        public Student Student { get; set; }
        [ForeignKey("Subject")]
        public int Subject_id { get; set; }
        public Subject Subject { get; set; }
        public string Subject_Name { get; set; }
        public double avg { get; set; }
    }
}