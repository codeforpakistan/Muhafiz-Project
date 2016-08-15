using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {   
        public const string LogOnSession = "UserSession";
        private static string organizationName = "";




        // GET: Home

        [HttpGet]
        public ActionResult AdminLogin(string returnmessage)
        {
            ViewBag.message = returnmessage;
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(Models.Admin user)
        {
            


            if (ModelState.IsValid)
            {

                var adminuser = user.IsValidUser(user.UserName, user.Password);
                if (adminuser!=null)
                {
                    Session["AdminUser"] = adminuser;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    
                    var message = "invalid email or password!!";
                    return RedirectToAction("AdminLogin", "Home", new { returnmessage = message });
                }
            }
            return RedirectToAction("AdminLogin", "Home");
        }
        [HttpGet]
        public ActionResult Login(string returnmessage)
        {
            // Session["User"] = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            ViewBag.message = returnmessage;
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.cPerson user)
        {
          

            //  if (ModelState.IsValid)
            // {
             Response.Cache.SetCacheability(HttpCacheability.NoCache);
            var User= user.IsValidUser(user.email, user.password);
                if (User!=null)
                {

                    Session["User"] = User;
                    Session["UserReportHistory"] = user.GetUserAllReportCount(User.Registration_Id);
                        if (User.RegistrationStatus.Equals("Pending"))
                            {
                                var message = "Your Registration is not approved by Admin";
                                return RedirectToAction("Login", "Home", new { returnmessage = message });
                            }
                        else {
                            return RedirectToAction("LoginedHome", "User"); }
                }
                else
                {
                    var message = "invalid email or password ";
                    return RedirectToAction("Login", "Home",new { returnmessage = message });
                 

                }
          //  }
           // return RedirectToAction("Login", "Home");

        }
        [HttpGet]
        public ActionResult Register(string returnmessage) {            
            

            ViewBag.message = returnmessage;
            return View();

        }
        [HttpPost, ActionName("Register")]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Models.cPerson user) {
            var errors = ModelState
                            .Where(x => x.Value.Errors.Count > 0)
                            .Select(x => new { x.Key, x.Value.Errors })
                            .ToArray();
            if (ModelState.IsValid)
            {

                if (user.IsEmailExist(user.email)) {

                    var message = "Account with that email already Exist";
                    return RedirectToAction("Register", "Home", new { returnmessage = message });

                }
                else if(user.IsPhoneExist(user.mobilde)){

                    var message = "Account with that Mobile No already Exist";
                    return RedirectToAction("Register", "Home", new { returnmessage = message });

                }

                else {
                    string subject = "Registration Pending";
                    string msgbody = "Thank you for signing up.You will be notified when your registered request has been approved.";
                    user.RegistrationRequest(user, organizationName);
                    if (Models.Admin.SendEmail(user.email, subject, msgbody)) {
                        return RedirectToAction("Index", "User");
                    }
                    else {
                        var message = "Error in sending mail";
                        return RedirectToAction("Register", "Home", new { returnmessage = message });
                    }
                    
                }
            }
            else {

                return View(user);
            }
        }
       

        [HttpGet]
        public ActionResult Logout() {
         
            Session.Remove("User");
          
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            
            return RedirectToAction("Login", "Home");

        }
        [HttpGet]
        public ActionResult AdminLogout() {

            Session.Remove("AdminUser");
            Session.Remove("PanicAlertsCount");
            return RedirectToAction("AdminLogin", "Home");


        }

        
        public ActionResult OrganizationText(string TextBoxVal) {


            organizationName = TextBoxVal;
            return Content("It works"+ TextBoxVal);
        }

        [HttpGet]
        public ActionResult ConfirmationDetails(string id)
        {
            string Display = "";
            string activationCode = !string.IsNullOrEmpty(id) ? id : Guid.Empty.ToString();
            if (!string.IsNullOrEmpty(activationCode)) {
                Models.ThreatDetails comfirmdetail = new Models.ThreatDetails();
                if (comfirmdetail.ConfirmDetailsOfThreat(activationCode))
                {

                    Display = "Details of your threat report have been confirmed.";


                }
                else {
                    Display = "Details of your threat report have already been Confirmed";


                }

            }

            ViewBag.Display = Display;
            return View();

        }

    }
}