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
    public class StudentsController : Controller
    {
        private schoolContext db = new schoolContext();

        /*// GET: Students
        public ActionResult Index()
        {
            return View(db.Students.ToList());
        }*/
        // GET: Students
        public ActionResult Index()
        {


            var results = db.Students.ToList();
            
            var list = JsonConvert.SerializeObject(results,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            return Content(list, "application/json");
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Student_id,Name,BDay,RNumber,Course_id")] Student student)
        {
            if (ModelState.IsValid)
            {
                if (student != null)
                {
                        Course crs = db.Courses.Find(student.Course_id);
                        
                    if (crs != null)
                    {
                        crs.Students += 1;
                        db.Entry(crs).State = EntityState.Modified;
                        db.SaveChanges();

                        student.Course_Name = crs.Name;
                        student.avg = 0;

                        db.Students.Add(student);
                        db.SaveChanges();

                        //int num_subj;
                        //IList<Client> clientList = from a in _dbFeed.Client.Include("Auto") select a;
                        IList<Subject> subjectsList = db.Subjects.Where(x => x.Course_id == student.Course_id).ToList();
                        //foreach (Course course in db.Courses)//Thread problem
                        foreach (Subject subject in subjectsList)
                        {
                            /*num_subj = db.Subjects.Where(x => x.Course_id == course.id).Count();
                            db.Courses.Find(course.id).Subjects = num_subj;
                            db.SaveChanges();

                            int num_tch;
                            num_tch = db.Subjects.Where(x => x.Course_id == course.id).GroupBy(x => x.Teacher_id).Count();
                            db.Courses.Find(course.id).Teachers = num_tch;
                            db.SaveChanges();*/

                            //subject.Students = crs.Students;
                            //Fazer entry por exemplo

                            db.Subjects.Find(subject.Subject_id).Students = crs.Students;
                            db.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Student_id,Name,BDay,RNumber,Course_id")] Student student)
        {
            if (ModelState.IsValid)
            {
                Course crs = db.Courses.Find(student.Course_id);
                if (crs != null)
                {
                    //student.Student_id = 4; 
                    db.Entry(student).State = EntityState.Modified;
                    db.SaveChanges();
                    int num_stds;
                    /* bastava no proprio id !? *//* Todos e mais seguro por enquanto por causa de trocar curso */
                    IList<Course> coursesList = db.Courses.ToList(); ;
                    foreach (Course course in coursesList)
                    {
                        num_stds = db.Students.Where(x => x.Course_id == course.id).Count();
                        db.Courses.Find(course.id).Students = num_stds;
                        db.SaveChanges();

                        IList<Subject> subjectsList = db.Subjects.Where(x => x.Course_id == course.id).ToList();
                        //foreach (Course course in db.Courses)//Thread problem
                        foreach (Subject subject in subjectsList)
                        {
                            db.Subjects.Find(subject.Subject_id).Students = course.Students;
                            db.SaveChanges();
                        }
                    }
                }
                /* */
                

                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course crs = db.Courses.Find(db.Students.Find(id).Course_id);
            crs.Students -= 1;
            db.Entry(crs).State = EntityState.Modified;
            db.SaveChanges();

            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();

            IList<Subject> subjectsList = db.Subjects.Where(x => x.Course_id == student.Course_id).ToList();
            //foreach (Course course in db.Courses)//Thread problem
            foreach (Subject subject in subjectsList)
            {   db.Subjects.Find(subject.Subject_id).Students = crs.Students;
                db.SaveChanges();
            }

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
