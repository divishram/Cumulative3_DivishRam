using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Diagnostics

namespace BlogProject.Models
{
    public class Teacher
    {
        //The following fields define a teacher
        public int TeacherId;
        public string TeacherFname;
        public string TeacherLname;
        public string EmployeeNumber;
        public DateTime HireDate;
        public decimal Salary;

        //Server-side validation logic
        public bool IsValid()
        {
            bool valid = true;

            if (TeacherFname == null || TeacherLname == null || Salary == null || EmployeeNumber == null || Salary == null)
            {
                //check to see if important fields are filled.
                valid = false;
            }

            else
            {
                //validate fields to make sure they meet server constraints

                //teacher fist name cannot be too short or too long
                if (TeacherFname.Length < || TeacherFname > 255) valid = false;

                //teacher last name cannot be too short or too long
                if (TeacherLname.Length < 2 || TeacherLname.Length > 255) valid = false;

                //Employee id is 3-digits so check if it must be at least 100, no more than 999
                if (EmployeeNumber < 100 || EmployeeNumber > 999) valid = false;
            }
            Debug.WriteLine("The model validity is: " + valid);
            return valid;
        }
        //Parameter-less constructore function
        //necessary for ajax request to automatically bind from the [FromBody] attribute
        public Teacher() { }

    }
}
