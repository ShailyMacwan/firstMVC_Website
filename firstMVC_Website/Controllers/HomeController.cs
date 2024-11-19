using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace firstMVC_Website.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        public ActionResult Index()
        {
            var name = Session["name"] as string;
            ViewBag.Name = name;
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Register(string name, string email, string contact, string password)
        {
            con.Open();

            SqlCommand cmd = new SqlCommand("insert into shaily (name, email, contact, password) values ('"+name+"','"+email+"', '"+contact+"', '"+password+"')", con);

            cmd.Parameters.AddWithValue("name", name);
            cmd.Parameters.AddWithValue("email", email);
            cmd.Parameters.AddWithValue("password", password);
            cmd.Parameters.AddWithValue("contact", contact);

            cmd.ExecuteNonQuery();

            con.Close();
            Console.WriteLine("Registered");
            //return View();
            return RedirectToAction("Login", "Home");
        }

        [HttpGet]
        public ActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {

       
            string query = "SELECT name from shaily WHERE email = @Email AND password = @Password";
            string connectiontring = "Data Source=SHAILY_MACWAN;Initial Catalog=WebsiteData; Integrated Security=True;Connect Timeout=30";
            try
            {
                using(SqlConnection con = new SqlConnection(connectiontring))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue ("@Password", password);
                    
                    

                        var endResult = cmd.ExecuteScalar();

                        if (endResult != null)
                        {

                            Session["name"] = endResult;

                            return RedirectToAction("Dashboard", "Home");
                        }
                        else
                        {
                          

                            ViewBag.ErrorMessage = "Enter correct credentials";

                            return View();
                        }
                    }

                };
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;

                return View();

            }
        }

        public ActionResult Dashboard()
        {
            var name = Session["name"] as string;

            if (string.IsNullOrEmpty(name))
            {
               
                return RedirectToAction("Login");
            }

           
            ViewBag.Name = name;

            return View();
        }
    }
}