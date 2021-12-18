using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogProject.Models;
using BlogProject.Models.ViewModels;
using System.Diagnostics;

namespace BlogProject.Controllers
{
    public class TeacherController : Controller
    {
        // GET: Teacher
        public ActionResult Index()
        {
            return View();
        }

        //GET: /Teacher/Error
        ///<summary>
        ///This window is for showing teacher errors
        ///</summary>
        public ActionResult Error()
        {
            return View();
        }

        //GET: /Teacher/List
        public ActionResult List(string SearchKey = null)
        {
            try
            {
                //Try to get a list of teachers.
                IEnumerable<Teacher> Teachers = TeacherDataController.ListTeachers(SearchKey);
                return View(Teachers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Debug.WriteLine(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        //GET: /Author/Ajax_List
        public ActionResult Ajax_List()
        {
            return View();
        }


        //GET: /Teacher/Show{id}
        [HttpGet]
        [Route("Teacher/Show/{id}")]
        public ActionResult Show(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            Teacher NewTeacher = controller.FindTeacher(id);
        }
        ;ost
        //GET: /Teacher/DeleteConfirm/{id}
        public ActionResult DeleteConfirm(int id)
        {
            //get information about teacher to confirm delete
            TeacherDataController controller = new TeacherDataController();
            Teacher NewTeacher   = controller.FindTeacher(id);
            return View(NewTeacher);
        }

        //POST: Teacher/Delete{id}
        public ActionResult Delete(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            controller.DeleteTeacher(id);

            return RedirectToAction("List");

        }

        //GET: /Teacher/New
        [HttpGet]
        [Route("Teacher/New")]
        public ActionResult New()
        {
            return View();
        }

        //POST: /Teacher/Create
        [HttpPost]
        public ActionResult Create(string TeacherFName, string TeacherLName, string EmployeeNumber)
        {
            //I want to create a teacher
            Debug.WriteLine("you are trying to make a teacher with " + TeacherFName + " and " + TeacherLName + " and " + EmployeeNumber);

            //Use C# Server Side Validation to ensure that there is no missing information when a teacher is added (such as a teacher name)

            Teacher NewTeacher = new Teacher();

            //Validate First name
            if (!string.IsNullOrEmpty(TeacherFName)) {
                NewTeacher.TeacherFname = TeacherFname;
            } else
            {
                ModelState.AddModelError("FName", "First Name is not valid");
            }

            if (!string.IsNullOrEmpty(TeacherLName))
            {
                NewTeacher.TeacherLname = TeacherLName;
            } else
            {
                ModelState.AddModelError("LName", "Last name is not valid");
            }

            if (!string.IsNullOrEmpty(EmployeeNumber))
            {
                string numberRegex = @"\bT\d{3}$";
                Regex re = new Regex(numberRegex);

                if (!re.IsMatch(EmployeeNumber))
                {
                    ModelState.AddModelError("Number", "Employee number does not match the Regex");
                } else
                {
                    NewTeacher.EmployeeNumber = EmployeeNumber;
                }
                
            }

            TeacherDataController controller = new TeacherDataController();
            controller.AddTeacher(NewTeacher);

            //go back to list of teachers
            return RedirectToAction("List");
            
        }
    }
}