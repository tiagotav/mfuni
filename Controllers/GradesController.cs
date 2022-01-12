using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mfuni.Models;
using Newtonsoft.Json;

namespace mfuni.Controllers
{
    public class GradesController : Controller
    {
        private schoolContext db = new schoolContext();

        // GET: Grades
        /*public ActionResult Index()
        {
            return View(db.Grades.ToList());
        }*/
        public ActionResult Index()
        {
            var results = db.Grades.ToList();
            var list = JsonConvert.SerializeObject(results,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            return Content(list, "application/json");
            //return View(db.Grades.ToList());
        }

        // GET: Grades/StdGrades/5
        public ActionResult StdGrades(int? id)
        {
            var results = db.Grades.Where(x => x.Student_id == id).ToList();
            var list = JsonConvert.SerializeObject(results,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            return Content(list, "application/json");
            //return View(db.Grades.ToList());
        }

        // GET: Grades/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grade grade = db.Grades.Find(id);
            if (grade == null)
            {
                return HttpNotFound();
            }
            return View(grade);
        }

        // GET: Grades/Create
        public ActionResult Create()
        {
            ViewBag.Student_id = new SelectList(db.Students, "Student_id", "Name");
            ViewBag.Subject_id = new SelectList(db.Subjects, "Subject_id", "Name");
            return View();
        }

        // POST: Grades/Create
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Subject_id,val,Student_id,Subject_id,Subject_Name,avg")] Grade grade)
        {
            if (ModelState.IsValid)
            {
                if (grade != null)
                {
                    grade.Subject_Name = db.Subjects.Find(grade.Subject_id).Name;
                    db.Grades.Add(grade);
                    db.SaveChanges();

                    medias(grade);
                }
                return RedirectToAction("Index");
            }

            ViewBag.Student_id = new SelectList(db.Students, "Student_id", "Name", grade.Student_id);
            ViewBag.Subject_id = new SelectList(db.Subjects, "Subject_id", "Name", grade.Subject_id);
            return View(grade);
        }

        public void medias(Grade grade)
        {

            //IList<Course> gradesList = db.Grades.Where(db.Subjects.Course_id);
            //Join Grade com Subjects//Subject.id//id Subjects
            IList<Course> coursesList = db.Courses.ToList();
            //foreach (Course course in db.Courses)//Thread problem
            foreach (Course course in coursesList)
            {
                if (db.Grades.Join(db.Subjects, grades => grades.Subject_id, subjects => subjects.Subject_id, (grades, subjects) => new { gradeid = grades.id, gradeval = grades.val, courseid = subjects.Course_id }).Where(course_uni => course_uni.courseid == course.id).Count() > 0)
                {
                    //Nao esta a fazer o que eu estou a pretender mas o join da valores certos usarei o proximo foreach
                    double avg = db.Grades.Join(db.Subjects, grades => grades.Subject_id, subjects => subjects.Subject_id, (grades, subjects) => new { gradeid = grades.id, gradeval = grades.val, courseid = subjects.Course_id }).Where(course_uni => course_uni.courseid == course.id).Average(v => grade.val);
                    int count = db.Grades.Join(db.Subjects, grades => grades.Subject_id, subjects => subjects.Subject_id, (grades, subjects) => new { gradeid = grades.id, gradeval = grades.val, courseid = subjects.Course_id }).Where(course_uni => course_uni.courseid == course.id).Count();
                    double temp = 0;
                    foreach (var query in db.Grades.Join(db.Subjects, grades => grades.Subject_id, subjects => subjects.Subject_id, (grades, subjects) => new { gradeid = grades.id, gradeval = grades.val, courseid = subjects.Course_id }).Where(course_uni => course_uni.courseid == course.id).ToList())
                    {
                        temp += query.gradeval;
                    }
                    avg = temp / count;
                    db.Courses.Find(course.id).avg = (float)avg; //Media curso
                    db.SaveChanges();
                }
                IList<Subject> subjectsList = db.Subjects.Where(x => x.Course_id == course.id).ToList();
                foreach (Subject subject in subjectsList)
                {
                    //Pode nao estar a fazer bem o metodo average faco qq coisa errada
                    /*if(db.Grades.Where(x => x.Subject_id == subject.Subject_id).Count() > 0) {
                        double avg = db.Grades.Where(x => x.Subject_id == subject.Subject_id).Average(v => v.val);
                        db.Subjects.Find(subject.Subject_id).avg = avg;
                        db.SaveChanges();
                    }*/

                    int count = db.Grades.Where(x => x.Subject_id == subject.Subject_id).Count();
                    double temp = 0;
                    foreach (var query in db.Grades.Where(x => x.Subject_id == subject.Subject_id).ToList())
                    {
                        temp += query.val;
                    }
                    double avg = temp / count;
                    double avg_1 = 0;
                    if (count > 0)
                    {
                        db.Subjects.Find(subject.Subject_id).avg = avg;
                        db.SaveChanges();
                    }
                }
            }
            double temp1 = 0;
            int count1 = db.Grades.Where(x => x.Student_id == grade.Student_id).Count();
            IList<Grade> grades_students_List = db.Grades.Where(x => x.Student_id == grade.Student_id).ToList();
            foreach (Grade query in grades_students_List)
            {
                temp1 += query.val;
            }
            double avg_2 = temp1 / count1;
            foreach (Grade query in grades_students_List)
            {
                db.Grades.Find(query.id).avg = avg_2;
                db.SaveChanges();
            }
            db.Students.Find(grade.Student_id).avg = avg_2;
            db.SaveChanges();

        }

        // GET: Grades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grade grade = db.Grades.Find(id);
            if (grade == null)
            {
                return HttpNotFound();
            }
            ViewBag.Student_id = new SelectList(db.Students, "Student_id", "Name", grade.Student_id);
            ViewBag.Subject_id = new SelectList(db.Subjects, "Subject_id", "Name", grade.Subject_id);
            return View(grade);
        }

        // POST: Grades/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,val,Student_id,Subject_id,Subject_Name,avg")] Grade grade)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grade).State = EntityState.Modified;
                db.SaveChanges();

                medias(grade);

                return RedirectToAction("Index");
            }
            ViewBag.Student_id = new SelectList(db.Students, "Student_id", "Name", grade.Student_id);
            ViewBag.Subject_id = new SelectList(db.Subjects, "Subject_id", "Name", grade.Subject_id);
            return View(grade);
        }

        // GET: Grades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grade grade = db.Grades.Find(id);
            if (grade == null)
            {
                return HttpNotFound();
            }
            return View(grade);
        }

        // POST: Grades/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Grade grade = db.Grades.Find(id);
            db.Grades.Remove(grade);
            db.SaveChanges();

            medias(grade);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
