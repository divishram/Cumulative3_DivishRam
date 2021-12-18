using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BlogProject.Models;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Web.Http.Cors;

namespace BlogProject.Controllers
{
    public class TeacherDataController : ApiController
    {
        //Connection to MySQL Database
        private SchoolDbContext School = new SchoolDbContext();

        ///summary
        ///Makes a request to SchoolDB data and returns the teacher's info (e.g. First name, Last name)
        ///</summary>
        ///<param name="SearchKey">The value in the key-value pair dictionary. Looks for the matching key</param>
        ///<example>GET api/TeacherData/ListTeachers
        ///<returns>
        ///List of first name and last names of teachers in the SchoolDB
        ///<example>GET api/TeacherData/ListTeachers
        ///

        [HttpGet]
        [Route("api/TeacherData/ListTeachers/{SearchKey?}")]
        public IEnumerable<string> ListTeachers(string SearchKey = null)    
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            try
            {

                //Open the connection between the web server and database
                Conn.Open();

                //Establish a new command (query) for our database
                MySqlCommand cmd = Conn.CreateCommand();

                //SQL QUERY
                cmd.CommandText = "Select * from Teachers where lower(teacherfname) like lower(@key) or lower(teacherlname) like lower(@key) or lower(concat(teacherfname, ' ', teacherlname)) like lower(@key) or lower(salary) like lower(@key) or lower(hiredate) like lower(@key)";
                cmd.Parameters.AddWithValue("@key", "%" + SearchKey + "%");
                cmd.Prepare();

                //Gather Result Set of Query into a variable
                MySqlDataReader ResultSet = cmd.ExecuteReader();

                //Create an empty list of Teacher
                List<String> Teachers = new List<Teachers> { };

                //Loop Through Each Row the Result Set
                while (ResultSet.Read())
                {
                    //Access Column information by the DB column name as an index'
                    int TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                    string TeacherFname = ResultSet["teacherfname"].ToString();
                    string TeacherLname = ResultSet["teacherlname"].ToString();
                    string EmployeeNumber = ResultSet["employeenumber"].ToString();
                    DateTime Hire = Convert.ToDateTime(ResultSet["hiredate"].ToString());
                    Decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                    Teacher NewTeacher = new Teacher();

                    NewTeacher.TeacherId = TeacherId;
                    NewTeacher.TeacherFname = TeacherFname;
                    NewTeacher.TeacherLname = TeacherLname;
                    NewTeacher.EmployeeNumber = EmployeeNumber;
                    NewTeacher.TeacherHireDate = Hire;
                    NewTeacher.TeacherSalary = Salary;

                    Teachers.Add(NewTeacher);
                }
            }

            catch(MySqlException ex)
            {
                //Catches issues with mysql
                Debug.WriteLine(ex);
                throw new ApplicationException("Issue was a database issue.", ex);
            }
            catch(Exception ex)
            {
                //Catches generic issues
                Debug.Write(ex);
                throw new ApplicationException("There was a server issue.", ex);
            }

            finally 
            {
                //Close Connection to MySql DB
                Conn.Close();
            }
            
            //Shows list of teachers
            return Teachers;
        }

        ///<summary>
        ///Find teacher with specified id
        ///</summary>
        ///<param name="id">The id of the teacher within the teacherid column in the SQL file</param>
        ///<example>
        ///GET: api/TeacherData/FindTeacher/1
        ///</example>
        ///<returns>
        ///1 Alexander Bennett T378 2016-08-05 00:00:00 55.30
        ///</returns>
        ///<example>
        ///GET: api/TeacherData/FindTeacher/2
        ///<returns>
        ///2 Caitlin Cummings T381 2014-06-10 00:00:00	62.77
        ///</returns>
        ///<example>
        ///GET: api/TeacherData/FindTeacher/3
        ///</example>
        ///<returns>
        ///3 Linda Chan T382 2015-08-22 00:00:00 60.22
        ///</returns>

        [HttpGet]
        [Route("api/TeacherData/FindTeacher/{id}")]
        public string FindTeacher(int id)
        {
            Teacher NewTeacher = new Teacher();

            try 
            {
               
                //Connect to the database
                MySqlConnection Conn = School.AccessDatabase();

                //Open connection between database and server
                Conn.Open();

                //Write and query for the DB
                MySqlCommand Cmd = Conn.CreateCommand();

                //Variable with the query that will be used in DB
                string query = "SELECT * FROM teachers WHERE teacherid =" + id;

                //SQL Query
                Cmd.CommandText = query;

                //Rather results of query into variable
                MySqlDataReader ResultSet = Cmd.ExecuteReader();

                //While loop to iterate through the teacher names
                while (ResultSet.Read())
                {
                    //Variables to gather data from database
                    int id = (int)ResultSet["teacherid"];
                    string TeacherFname = ResultSet["teacherfname"].ToString();
                    string TeacherLname = ResultSet["teacherlname"].ToString();
                    string EmployeeNumber = ResultSet["employeenumber"].ToString();
                    DateTime HireDate = (DateTime)ResultSet["hiredate"];
                    decimal Salary = (decimal)ResultSet["salary"];

                    //Variable to store converted teacher data
                    Teacher NewTeacher = new Teacher();

                    //Storing the data into the variable
                    NewTeacher.TeacherId = id;
                    NewTeacher.TeacherFname = TeacherFname;
                    NewTeacher.TeacherLname = TeacherLname;
                    NewTeacher.EmployeeNumber = EmployeeNumber;
                    NewTeacher.HireDate = HireDate;
                    NewTeacher.Salary = Salary;
           
                }
                //checking for model validity after pulling from the database
                if (!NewTeacher.IsValid()) throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            catch(HttpResponseException ex)
            {
                Debug.WriteLine(ex);
                throw new ApplicationException("That teacher was not found.", ex);
            }

            catch (MySqlException ex)
            {
                //Catches issues with mysql
                Debug.WriteLine(ex);
                throw new ApplicationException("There was a server issue.", ex);

            }

            finally
            {
                //Close the server DB connection
                Conn.Close();

            }

            //return teacher data
            return NewTeacher;
        }
        ///<summary>
        ///Adds a new Teacher into the system
        ///</summary>
        ///<param name="NewTeacher"> Teacher Object</param>
        ///<example>POST api/TeacherData/AddTeacher</example>
        [HttpPost]
        [EnableCors(origins: "*", methods: "*", headers: "*")]

        public void AddTeacher(Teacher NewTeacher)
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            try
            {
                //Open the connection between the web server and database
                Conn.Open();

                //Establish a new comman (query) for our database
                MySqlCommand cmd = Conn.CreateCommand();

                //SQL QUERY
                string query = "insert into teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) values (@TeacherFname, @TeacherLname, @EmployeeNumber, current_date(), @Salary)";
            
                //Run the query
                cmd.CommandText = query;
            
                cmd.Parameters.AddWithValue("@TeacherFname", NewTeacher.TeacherFname);
                cmd.Parameters.AddWithValue("@TeacherLname", NewTeacher.TeacherLname);
                cmd.Parameters.AddWithValue("@EmployeeNumber", NewTeacher.EmployeeNumber);
                cmd.Parameters.AddWithValue("@TeacherHireDate", NewTeacher.HireDate);
                cmd.Parameters.AddWithValue("@TeacherSalary", NewTeacher.Salary);

                cmd.Prepare();

                //DML Operations
                cmd.ExecuteNonQuery();

                //Close connection between MySQL Database and the WebServer
                Conn.Close();
                }

            catch (MySqlException ex)
            {
                //Catches issues with MySQL
                Debug.WriteLine(ex);
                throw new ApplicationException("Issue was a database issue.", ex);
            }

            catch (Exception ex)
            {
                //Catches generic issues
                Debug.Write(ex);
                throw new ApplicationException("There was a server issue.", ex);
            }

            finally
            {
                //Close the connection between MySQL database and Webserver
                Conn.Close();
            }





        }

        ///<summary>
        ///Deletes a teacher by its primary key (id)
        ///</summary>
        ///<param name="id">The id of the teacher within the teacherid column in the SQL file</param>

        [HttpPost]
        public void DeleteTeacher(int id)
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new comman (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            string query = "Delete FROM teachers WHERE teacherid=@id";
            
            //Enter string into command
            cmd.CommandText = query;

            cmd.Parameters.AddWithValue("@id", id);

            cmd.Prepare();

            //DML Operations
            cmd.ExecuteNonQuery();

            //Close connection between MySQL Database and the WebServer
            Conn.Close();

        }

        ///<summary>
        ///Updates a teacher on the MySQL Database. Non-deterministic
        ///</summary>
        ///<param name="TeacherInfo>An object with fields that map to the columns of the teacher's table.</param>
        ///<example>
        ///POST api/TeacherData/UpdateTeacher/1
        ///Form data/post data/ request body {
        ///   "TeacherFname" : "Alexander",
        ///   "TeacherLname: "Bennett",
        ///    "EmployeeNumber": "T378",
        ///    "TeacherHireDate: "12/17/21 11:00:00 PM",
        ///    "TeacherSalary": "55.30"
        ///   }
        ///</example>
        ///<example>
        ///POST api/TeacherData/UpdateTeacher/2
        //////Form data/post data/ request body {
        ///   "TeacherFname" : "Caitlin",
        ///   "TeacherLname: "Cummings",
        ///    "EmployeeNumber": "T381",
        ///    "TeacherHireDate: "12/17/21 11:10:00 PM",
        ///    "TeacherSalary": "62.77"
        ///   }
        ///</example>
        ///  ///<example>
        ///POST api/TeacherData/UpdateTeacher/3
        //////Form data/post data/ request body {
        ///   "TeacherFname" : "Linda",
        ///   "TeacherLname: "Chan",
        ///    "EmployeeNumber": "T382",
        ///    "TeacherHireDate: "12/17/21 11:20:00 PM",
        ///    "TeacherSalary": "60.22"
        ///   }
        ///</example>


        [HttpPost]
        [EnableCors(origins: "*", methods: "*", headers: "*")]
        public void UpdateTeacher(int id, [FromBody]Teacher TeacherInfo)
        {
            //Exit method if model fields are not included
            if (!TeacherInfo.IsValid()) throw new ApplicationException("Posted Data was not valid");
            try
            {
                //Open connection to web server and DB
                Conn.Open();

                //Establish a new query/command for our db
                MySqlCommand cmd = Conn.CreateCommand();

                //SQL query
                cmd.CommandText = "UPDATE teachers SET teacherfname=@TeacherFname, teacherlname=@TeacherLname, employeenumber=@EmployeeNumber, hiredate=current_date(), salary=@Salary WHERE teacherid=@TeacherId";
                cmd.Parameters.AddWithValue("@TeacherFname", TeacherInfo.TeacherFname);
                cmd.Parameters.AddWithValue("@TeacherLname", TeacherInfo.TeacherLname);
                cmd.Parameters.AddWithValue("@EmployeeNumber", TeacherInfo.EmployeeNumber);
                cmd.Parameters.AddWithValue("@TeacherHireDate", TeacherInfo.TeacherHireDate);
                cmd.Parameters.AddWithValue("@TeacherSalary", TeacherInfo.TeacherSalary);
                cmd.Parameters.AddWithValue("@TeacherId", id);

                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }

            catch(MySqlException ex)

            {
                //Catches issues with mysql
                Debug.WriteLine(ex);
                throw new ApplicationException("Issue was a DB issue.", ex);
            }

            catch (Exception ex)

            {
                //catches generic issues
                Debug.Write(ex);
                throw new ApplicationException("There was a server issue.", ex);
            }

            finally
            {
                //close connection between mysql db and webserver
                Conn.Close();
            }





            }
        }



    }
}
