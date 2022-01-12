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
    public class SubjectsController : Controller
    {
        private schoolContext db = new schoolContext();

        /*// GET: Subjects
        public ActionResult Index()
        {
            var subjects = db.Subjects.Include(s => s.Course).Include(s => s.Teacher);
            return View(subjects.ToList());
        }*/
        // GET: Subjects
        public ActionResult Index()
        {
            var results = db.Subjects.ToList();
            var list = JsonConvert.SerializeObject(results,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            return Content(list, "application/json");
        }

        // GET: Subjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = db.Subjects.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // GET: Subjects/Create
        public ActionResult Create()
        {
            ViewBag.Course_id = new SelectList(db.Courses, "id", "Name");
            ViewBag.Teacher_id = new SelectList(db.Teachers, "id", "Name");
            return View();
        }

        // POST: Subjects/Create
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Subject_id,Name,Course_id,Teacher_id")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                
                if(subject != null)
                    {
                    Course crs = db.Courses.Find(subject.Course_id);
                    if (crs != null) {
                        crs.Subjects += 1;

                        //If this teacher is already in this Course_id?? it will not count to the courses teachers
                        string query = "select * from Subjects where Teacher_id='" + subject.Teacher_id + "' and Course_id='" + subject.Course_id + "';";
                        IEnumerable<Subject> data = db.Database.SqlQuery<Subject>(query);
                        if (data.Count() == 0) {
                            crs.Teachers += 1;
                        }

                        db.Entry(crs).State = EntityState.Modified;
                        db.SaveChanges();

                        string tch_name = db.Teachers.Find(subject.Teacher_id).Name;
                        subject.TeacherName = tch_name;
                        subject.Students = 0;
                        subject.avg = 0;

                        db.Subjects.Add(subject);
                        db.SaveChanges();

                        db.Teachers.Find(subject.Teacher_id).num_subjects += 1;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }

            ViewBag.Course_id = new SelectList(db.Courses, "id", "Name", subject.Course_id);
            ViewBag.Teacher_id = new SelectList(db.Teachers, "id", "Name", subject.Teacher_id);
            return View(subject);
        }


        // GET: Subjects/Student/5
        public ActionResult Student(int? id)
        {
            //Course where student is enrolled
            string query = "select * from Subjects where Course_id='" + id + "'";
            IEnumerable<Subject> data = db.Database.SqlQuery<Subject>(query);

            Console.WriteLine(query);

            //var results = db.Courses.ToList();
            var results = data.ToList();
            var list = JsonConvert.SerializeObject(results,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            return Content(list, "application/json");
        }

            // GET: Subjects/Edit/5
            public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = db.Subjects.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            ViewBag.Course_id = new SelectList(db.Courses, "id", "Name", subject.Course_id);
            ViewBag.Teacher_id = new SelectList(db.Teachers, "id", "Name", subject.Teacher_id);
            return View(subject);
        }

        // POST: Subjects/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Name,Course_id,Teacher_id")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subject).State = EntityState.Modified;
                db.SaveChanges();
                string tch_name = db.Teachers.Find(subject.Teacher_id).Name;
                subject.TeacherName = tch_name;

                int num_subj;
                //IList<Client> clientList = from a in _dbFeed.Client.Include("Auto") select a;
                IList<Course> coursesList = db.Courses.ToList(); ;
                //foreach (Course course in db.Courses)//Thread problem
                foreach ( Course course in coursesList)
                {
                    num_subj = db.Subjects.Where(x => x.Course_id == course.id).Count();
                    db.Courses.Find(course.id).Subjects = num_subj;
                    db.SaveChanges();

                    int num_tch;
                    num_tch = db.Subjects.Where(x => x.Course_id == course.id).GroupBy(x => x.Teacher_id).Count();
                    db.Courses.Find(course.id).Teachers = num_tch;
                    db.SaveChanges();

                }

                IList<Subject> subjectsLists = db.Subjects.ToList();
                foreach (Subject subject_1 in subjectsLists)
                {
                    int num_subs = 0;
                    IList<Teacher> teachersLists = db.Teachers.ToList();
                    foreach (Teacher teacher_1 in teachersLists)
                    {
                        if (db.Subjects.Where(c => c.Subject_id == subject_1.Subject_id).Where(b => b.Teacher_id == teacher_1.id).Count() > 0)
                        {
                            num_subs = db.Subjects.Where(c => c.Subject_id == subject_1.Subject_id).Where(b => b.Teacher_id == teacher_1.id).Count();
                            db.Teachers.Find(teacher_1.id).num_subjects = num_subs;
                            db.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            ViewBag.Course_id = new SelectList(db.Courses, "id", "Name", subject.Course_id);
            ViewBag.Teacher_id = new SelectList(db.Teachers, "id", "Name", subject.Teacher_id);
            return View(subject);
        }

        // GET: Subjects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = db.Subjects.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Subject subject = db.Subjects.Find(id);
            db.Subjects.Remove(subject);
            db.SaveChanges();

            int num_subj;
            //IList<Client> clientList = from a in _dbFeed.Client.Include("Auto") select a;
            IList<Course> coursesList = db.Courses.ToList(); ;
            //foreach (Course course in db.Courses)//Thread problem
            foreach (Course course in coursesList)
            {
                num_subj = db.Subjects.Where(x => x.Course_id == course.id).Count();
                db.Courses.Find(course.id).Subjects = num_subj;
                db.SaveChanges();

                int num_tch;
                num_tch = db.Subjects.Where(x => x.Course_id == course.id).GroupBy(x => x.Teacher_id).Count();
                db.Courses.Find(course.id).Teachers = num_tch;
                db.SaveChanges();

            }

            db.Teachers.Find(subject.Teacher_id).num_subjects += 1;
            db.SaveChanges();

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
