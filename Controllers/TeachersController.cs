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
    public class TeachersController : Controller
    {
        private schoolContext db = new schoolContext();


        /*public ActionResult Index()
        {
            return View(db.Teachers.ToList());
        }*/
        // GET: Teachers
        public ActionResult Index()
        {
            var results = db.Teachers.ToList();
            var list = JsonConvert.SerializeObject(results,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            return Content(list, "application/json");
        }

        // GET: Teachers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // GET: Teachers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Name,BDay,salary")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                if (teacher != null)
                {
                    db.Teachers.Add(teacher);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Name,BDay,salary")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                teacher.num_subjects = 0;
                db.Entry(teacher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Teacher teacher = db.Teachers.Find(id);
            db.Teachers.Remove(teacher);
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
