using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net;
using System.IO;
using System.Text;

namespace WebApplication.Controllers
{
    public class UserController : Controller
    {
        Models.cPerson LogedInUsers = new Models.cPerson();
        private readonly static string reservedCharacters = "!*'();:@&=+$,/?%#[]";

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
       
        public static string UrlEncode(string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            var sb = new StringBuilder();

            foreach (char @char in value)
            {
                if (reservedCharacters.IndexOf(@char) == -1)
                    sb.Append(@char);
                else
                    sb.AppendFormat("%{0:X2}", (int)@char);
            }
            return sb.ToString();
        }

        public void sendsms(string requestUriString, string recieptMobileno)
        {

            string apiUrl = "http://bsms.ufone.com/bsms_app5/sendapi-0.3.jsp?id=03359646370&message="+requestUriString+"&shortcode=B-SMS&lang=English&password=321321&mobilenum="+recieptMobileno;
          
            Uri address = new Uri(apiUrl);
           
         
             //code to execute url at background
             // Create the web request

            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

            // Set type to POST
            request.Method = "GET";
            request.ContentType = "text/xml";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());

                // Console application output
                var strOutputXml = reader.ReadToEnd();
            }

        }



      

        // GET: User
        // Request Pending Status Displays after Registration Request
        public ActionResult Index() 
        {



            return View();
        }

        [HttpGet]
        public ActionResult ReportHistory() {




            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            if (Session != null)
            {

                if (Session["User"] == null)
                {

                    return RedirectToAction("Login", "Home");
                }
                else {


                    List<Models.ThreatReportViewModel> ReportHist = new List<Models.ThreatReportViewModel>();
                    LogedInUsers = (WebApplication.Models.cPerson)Session["User"];

                    ReportHist = LogedInUsers.GetAllReportHistory(LogedInUsers.Registration_Id);
                    int? page = 1;
                    int pageSize = 5;
                    int pageNumber = (page ?? 1);
                    return View(ReportHist.ToPagedList(pageNumber, pageSize));

                }
            }
            else {



                return RedirectToAction("Login", "Home");


            }



        }


        [HttpGet]
        public ActionResult ReportaThreat(string returnmessage) {
         

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            if (Session != null)
            {

                if (Session["User"] == null)
                {

                    return RedirectToAction("Login", "Home");
                }
                else {

                    Models.ThreatReportViewModel modelobj = new Models.ThreatReportViewModel();
                    modelobj.Users = (WebApplication.Models.cPerson)Session["User"];
                    ViewBag.message = returnmessage;
                    return View(modelobj);

                }
            }
            else {



                    return RedirectToAction("Login", "Home");


            }

        }

        [HttpPost, ActionName("ReportThreat")]
        public ActionResult ReportThreat(Models.ThreatReportViewModel modelobj) {

            var outputTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime now = TimeZoneInfo.ConvertTime(DateTime.Now, outputTimeZone);
            modelobj.Users = (WebApplication.Models.cPerson)Session["User"];
            if (LogedInUsers.reportathreat(modelobj)) {

                string SmSbody = "Threat Report from : \n" + modelobj.Users.UserName + ", Registration Id:" + modelobj.Users.Registration_Id + ", Mobileno:" + modelobj.Users.mobilde + ",Organizationname " + modelobj.Users.orgname + ", DateandTime: " +now;

                sendsms(SmSbody, "923425063376");


                var message = "Your Threat Report has been sent Wait for call.";

                return RedirectToAction("Login", "Home", new { returnmessage = message });

            }
            else {


                var message = "Error in reporting your Threat try again.";

                return RedirectToAction("ReportaThreat", "User", new { returnmessage = message });

            }

        }

      // the view displayed to LogedIn User after LogedIn Successfully 
        [HttpGet]
        public ActionResult LoginedHome(string returnmessage) {
         
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            if (Session != null)
            {

                if (Session["User"] == null)
                {

                    return RedirectToAction("Login", "Home");
                }
                else {

                    var LogedInUser = (WebApplication.Models.cPerson)Session["User"];
                    ViewBag.UserName = LogedInUser.email;
                    ViewBag.message = returnmessage;
                    return View();

                }
            }
            else {



                return RedirectToAction("Login", "Home");


            }
        }
        [HttpPost, ActionName("PanicAlertRequest")]
        public ActionResult PanicAlertRequest() {

            LogedInUsers = (WebApplication.Models.cPerson)Session["User"];
            if (LogedInUsers.PanicAlertRequest(LogedInUsers))
            {

                int randno = RandomNumber(1, 10000);
                string smsbody = "$$@@##" + randno;
                sendsms(UrlEncode(smsbody), "923425063376");

                var message = "Your Panic Alert has been sent Wait for call from admin.";

                return RedirectToAction("LoginedHome", "User", new { returnmessage = message });

            }
            else {
                var message = "Error in sending Panic Alert Try again.";
                return RedirectToAction("LoginedHome", "User", new { returnmessage = message });


            }



        }
        //ProfileSetting Request
        [HttpGet]
        public ActionResult ProfileSettingRequest(string returnmessage) {

           

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            if (Session != null)
            {

                if (Session["User"] == null)
                {

                    return RedirectToAction("Login", "Home");
                }
                else {

                    LogedInUsers = (WebApplication.Models.cPerson)Session["User"];

                    ViewBag.message = returnmessage;
                    return View(LogedInUsers);

                }
            }
            else {



                return RedirectToAction("Login", "Home");


            }





        }
        [HttpPost, ActionName("ProfileSetting")]
        public ActionResult ProfileSetting() {
            
            LogedInUsers = (WebApplication.Models.cPerson)Session["User"];
            if (LogedInUsers.UpdateProfileRequest(LogedInUsers))
            {

                var message = "Your Request for Profile Update has been sent Wait for call from admin.";

                return RedirectToAction("Login", "User", new { returnmessage = message });

            }
            else {

                return HttpNotFound();


            }




        }



    }
}