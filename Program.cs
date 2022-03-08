// problems , dt 3 March 2022. 1) query with store procedure call, 2, for loop email again and again run done, 3, console app running with windows start up or as automatic service done
//subject dynamic store procedure se description column ka aye , court type aye , reminder date aye , case id , case no , start date etc , yani total case ki information aye.


using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Globalization;

namespace Send_Email_Console_CS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Getting Connection ...");

            var datasource = @"N500034MPA\SQLEXPRESS";//your server
            var database = "TaxDB"; //your database name
            var username = "sa"; //username of server to connect
            var password = "@mir1978t"; //password

            //your connection string 
            string connString = @"Data Source=" + datasource + ";Initial Catalog="
                        + database + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;

            //create instanace of database connection
            SqlConnection conn = new SqlConnection(connString);


            try
            {
                Console.WriteLine("Openning Connection ...");

                //open connection
                conn.Open();

                Console.WriteLine("Connection successful!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            // using (SqlCommand command = new SqlCommand("SELECT * FROM cases", conn))
            // using (SqlCommand command = new SqlCommand("SELECT ReminderEmailDate FROM courts where courttype = 'TOLP' and reminderemaildate is not null", conn)) //is jaga per store procedure call kerwa lo jo sab ke sab reminder dates pick ker le courttype IN (TOLP,XYZ,ABC)

            SqlCommand command = conn.CreateCommand(); //connect through store procedure.
            //command.CommandText = "reminder_date";
            command.CommandText = "court_cases";
            command.CommandType = System.Data.CommandType.StoredProcedure;

            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {

                            //var reminder_date = reader.GetValue(i);
                            //var data = reader.GetName(9);
                            // var data1 = reader.GetValue(9);
                            var caseNo = reader.GetValue(21);
                            var courttype = reader.GetValue(1);
                            var courttypedesc = courttype.ToString();
                            var desc = reader.GetValue(23);
                            var descstring = desc.ToString();
                            var myString = descstring.Replace("\r\n", string.Empty);
                            var yourstring = descstring.Replace(System.Environment.NewLine, string.Empty);
                            var reminderdate_columnName = reader.GetName(9);
                            var reminder_date = reader.GetValue(9);
                            var reminderdateF = reminder_date.ToString(); //is ke under bi de saktey hain or dot laga ker bi reference ker saktey hain
                            var reminder_dateformat = DateTime.Parse(reminderdateF);
                            DateTime currentdate = DateTime.Today;
                           



                            if (reminder_dateformat == currentdate) //date ko date ke saat hi match kerwa lo , complier samj jata hai , string /date khabi bi match nai ho ga ..."" yeah lag jata hai string main

                            {

                                //Console.Read(); //mtlb idher khery ho jo ..... command screen pe stop reho or break statment.

                                //Console.WriteLine("Enter To Address:");
                                string to = "amir.shahzad1978@gmail.com";




                                // Console.WriteLine("Enter Subject:");
                                // string subject = Console.ReadLine().Trim();

                                //string subject = "Console Reminder Email From System Subject";

                                var subject = myString;

                                //Console.WriteLine("Enter Body:");
                                //string body = Console.ReadLine().Trim();

                                string body = courttypedesc;

                                using (MailMessage mm = new MailMessage(ConfigurationManager.AppSettings["FromEmail"], to)) //system gets to email and from email from appconfig file
                                    try
                                    {
                                        mm.Subject = subject;
                                        mm.Body = body;
                                        mm.IsBodyHtml = false;
                                        SmtpClient smtp = new SmtpClient();
                                        smtp.Host = ConfigurationManager.AppSettings["Host"]; //get app.config host value
                                        smtp.EnableSsl = true;
                                        NetworkCredential NetworkCred = new NetworkCredential(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]); //gets username pass
                                        smtp.UseDefaultCredentials = true;
                                        smtp.Credentials = NetworkCred;
                                        smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]); //get port  , Gathers all Information SMTP, FROM , TO PORT ETC 
                                        Console.WriteLine("Sending Email......");
                                        smtp.Send(mm); // using send property 
                                        Console.WriteLine("Email Sent.");
                                        System.Threading.Thread.Sleep(3000);
                                        //Environment.Exit(0); // to exit from loop , or exit from envirnoment.
                                    }
                                    catch (Exception e)
                                    {

                                        Console.WriteLine("Error: " + e.Message);
                                    }

                            }

                            else
                            {
                                Console.WriteLine("Date Not Matched");

                            }
                        }


                    }


                }
            }





        }


    }
}



//DateTime dt1 = DateTime.Parse("03/05/2022"); // is ke under apna string variable bi rekh saktey hain  , date ke under DateTime dt1 = DateTime.Parse(xyz);
//exampl
//    var reminderdateF = reminder_date.ToString(); //is ke under bi de saktey hain or dot laga ker bi reference ker saktey hain
//var reminder_dateformat = DateTime.Parse(reminderdateF);


//DateTime dt2 = DateTime.Now;
//DateTime dt3 = DateTime.Today; // for no time and AM, PM

//if (dt1.Date == dt2.Date)
//{
//    Console.WriteLine("It's a macth with date format"); //It's a later date
//}
//else
//{
//    Console.WriteLine("It's not matched date");
//}