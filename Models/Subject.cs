using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mfuni.Models
{
    public class Subject
    {
        //nota: suprimir um atributo de fazer campo na tabela//como se faz?
        [Key]
        public int Subject_id { get; set; }
        public string Name { get; set; }
        [ForeignKey("Course")]
        public int Course_id { get; set; }
        public Course Course { get; set; }
        [ForeignKey("Teacher")]
        public int Teacher_id { get; set; }
        public Teacher Teacher { get; set; }
        public string TeacherName { get; set; }
        public int Students { get; set; }
        public double avg { get; set; }
    }
}