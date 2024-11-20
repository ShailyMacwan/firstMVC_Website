using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;

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

            /* ----------------------------------------------------------------------------------------------------*/

            List<int> years = new List<int>();
            List<int> salesData = new List<int>();
            List<int> revenueData = new List<int>();

            string connectionstring = "Data Source=SHAILY_MACWAN;Initial Catalog=WebsiteData; Integrated Security=True;Connect Timeout=30";
            string query = "Select Year, sum(Sales) AS totalSales, sum(Revenue) AS totalRevenue FROM SalesData GROUP BY Year ORDER BY Year";

            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) 
                {
                    years.Add(Convert.ToInt32(reader["year"]));
                    salesData.Add(Convert.ToInt32(reader["totalSales"]));
                    revenueData.Add(Convert.ToInt32(reader["totalRevenue"]));
                }

                con.Close();
            }
            
            ViewBag.years= years;
            ViewBag.salesData = salesData;
            ViewBag.revenueData = revenueData;



            /* ----------------------------------------------------------------------------------------------------*/




            /* --------------------------------------------------------------*/

            List<string> months = new List<string>
{
    "January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
};
            List<int> salesData1 = new List<int>();

            // Initialize sales data with 0
            foreach (var _ in months) salesData1.Add(0);

            // SQL Query to fetch monthly sales data
            string query1 = @"
    SELECT Month, SUM(Sales) AS TotalSales 
    FROM SalesData 
    GROUP BY Month
    ORDER BY CASE 
        WHEN Month = 'January' THEN 1
        WHEN Month = 'February' THEN 2
        WHEN Month = 'March' THEN 3
        WHEN Month = 'April' THEN 4
        WHEN Month = 'May' THEN 5
        WHEN Month = 'June' THEN 6
        WHEN Month = 'July' THEN 7
        WHEN Month = 'August' THEN 8
        WHEN Month = 'September' THEN 9
        WHEN Month = 'October' THEN 10
        WHEN Month = 'November' THEN 11
        WHEN Month = 'December' THEN 12
    END;
";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand(query1, con);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string month = reader["Month"].ToString();
                        int totalSales = Convert.ToInt32(reader["TotalSales"]);

                        int index = months.IndexOf(month);
                        if (index >= 0)
                        {
                            salesData1[index] = totalSales;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error or handle it
                Console.WriteLine("Error: " + ex.Message);
            }

            ViewBag.Months = months;
            ViewBag.SalesData1 = salesData1;

            return View();


        }
    }
}