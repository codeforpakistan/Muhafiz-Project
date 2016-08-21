using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Web.Routing;

namespace WebApplication.Controllers
{
    public class AdminController : Controller
    {
        
        static List<string> textboxitems = new List<string>();
        static string dateval = "";
        static WebApplication.Models.cPerson person = new WebApplication.Models.cPerson();
        public List<Models.cPerson> RequestsAll { get; set; }
       
        Models.Admin user = new Models.Admin();
        // GET: Admin
        [HttpGet]
        public ActionResult Index(int ?page )       //Registered Requests View
        {


            Response.AddHeader("Refresh", "5");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            if (Session != null)
            {

                if (Session["AdminUser"] == null)
                {

                    return RedirectToAction("AdminLogin", "Home");
                }
                else {

                   
                    RequestsAll = user.GetAllRequests();
                    Session["PanicAlertsCount"] =  user.GetPanicAlertCount();
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return View(RequestsAll.ToPagedList(pageNumber, pageSize));

                }
            }
            else {



                return RedirectToAction("AdminLogin", "Home");


            }


        }
        [HttpGet]
        public ActionResult RegisteredUsers(int ?page) {
            Response.AddHeader("Refresh", "5");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            if (Session != null)
            {

                if (Session["AdminUser"] == null)
                {

                    return RedirectToAction("AdminLogin", "Home");
                }
                else {


                    RequestsAll = user.GetAllRegisteredUsers();
                    Session["PanicAlertsCount"] = user.GetPanicAlertCount();
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return View(RequestsAll.ToPagedList(pageNumber, pageSize));

                }
            }
            else {



                return RedirectToAction("AdminLogin", "Home");


            }


           
        }

        [HttpGet]
        public ActionResult PanicAlerts(int? page)
        {
            Response.AddHeader("Refresh", "10");
           
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            if (Session != null)
            {

                if (Session["AdminUser"] == null)
                {

                    return RedirectToAction("AdminLogin", "Home");
                }
                else {

                    List<Models.PanicAlertViewModel> PanicAlertList = new List<Models.PanicAlertViewModel>();
                    PanicAlertList = user.GetPanicAlerts();
                    //  Session["PanicAlertsCount"] = (long)PanicAlertList.Count();
                    Session["PanicAlertsCount"] = user.GetPanicAlertCount();
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return View(PanicAlertList.ToPagedList(pageNumber, pageSize));

                }
            }
            else {



                return RedirectToAction("AdminLogin", "Home");


            }
        }
        [HttpGet]
        public ActionResult ProcessAlert(int? Attackid, int ? Userid)
        {
            WebApplication.Models.PanicAlertViewModel viewModel = new WebApplication.Models.PanicAlertViewModel();

            if (Attackid == null)
            {
                return HttpNotFound();
            }
           viewModel= user.GetAlertDetails(Attackid, Userid);
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);


            // return View(per);

        }
        [HttpGet]
        public ActionResult InfoUpdateRequests(int? page) {


            Response.AddHeader("Refresh", "5");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            if (Session != null)
            {

                if (Session["AdminUser"] == null)
                {

                    return RedirectToAction("AdminLogin", "Home");
                }
                else {


                    List<Models.ProfileSettingViewModel> obj = user.GetAllUpdateInfoRequests();
                      Session["PanicAlertsCount"] =  user.GetPanicAlertCount();
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    return View(obj.ToPagedList(pageNumber, pageSize));

                }
            }
            else {



                return RedirectToAction("AdminLogin", "Home");


            }



        }


      
        public ActionResult Approve1(int? id, string param)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Models.cPerson per = user.GetUser(id, 0);
            if (per == null)
            {
                return HttpNotFound();
            }
            return ApproveConfirmed(id, param);


            // return View(per);

        }
        public void sendsms(string requestUriString, string recieptMobileno)
        {

          //string apiUrl = "http://api.bizsms.pk/api-send-branded-sms.aspx?username=d-sales-ay@bizsms.pk&pass=d3mosal3s45786**&text="+requestUriString+"&masking=Demo&destinationnum="+recieptMobileno+"&language=English";
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
      [HttpPost]
        public ActionResult ApproveConfirmed(int? id, string param)
        {
            var outputTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime now = TimeZoneInfo.ConvertTime(DateTime.Now, outputTimeZone);
                
            string Subject="Registration Approval";
            string Body = "Welcome to Muhafiz! Your request has been approved. You may now log in to your Muhafiz account.";
            
            if (user.ApproveUser(id,param))
            {
                Models.cPerson per = user.GetUser(id, 0);
                string SMSBody = "Your request has been approved. You may now log in to your Muhafiz account. Date and time: " + now.ToString(); 
                string recieptMobileno = per.mobilde;
                sendsms(SMSBody, recieptMobileno);
                Models.Admin.SendEmail(param, Subject, Body);
                return RedirectToAction("Index","Admin");

            }
            else {
                

                return RedirectToAction("Index", "Admin");

            }
          //  return View();
            
            
        }
       
        public ActionResult Reject(int? id,int param)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Models.cPerson per = user.GetUser(id,param);
            if (per == null)
            {
                return HttpNotFound();
            }
            return RejectConfirmed(id);


            // return View(per);

        }
        [HttpPost]
        public ActionResult RejectConfirmed(int? id)
        {
            if (user.RejectUser(id))
            {

                return RedirectToAction("Index", "Admin");

            }
            else {

                return RedirectToAction("Index", "Admin");

            }
            //  return View();


        }
        [HttpGet]
        public ActionResult GetAllThreathRequests(int? page, string returnmessage) {
           


            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            if (Session != null)
            {

                if (Session["AdminUser"] == null)
                {

                    return RedirectToAction("AdminLogin", "Home");
                }
                else {

                    Response.AddHeader("Refresh", "5");
                    List<WebApplication.Models.ThreatReportViewModel> returnedThreaReq = new List<WebApplication.Models.ThreatReportViewModel>();
                    returnedThreaReq = user.GetThreatRequests();
                    Session["PanicAlertsCount"] = user.GetPanicAlertCount();
                    int pageSize = 5;
                    int pageNumber = (page ?? 1);
                    ViewBag.messageerror = returnmessage;
                    return View(returnedThreaReq.ToPagedList(pageNumber, pageSize));

                }
            }
            else {


               
                return RedirectToAction("AdminLogin", "Home");


            }

        }

        [HttpGet]
        public ActionResult Discard(int?id, int ?param) {
            if (id == null)
            {
                return HttpNotFound();
            }
            else {
                return DiscardConfirmed(id);
            }

        }
        [HttpPost]
        public ActionResult DiscardConfirmed(int ?id) {

            if (user.DiscardthreatReq(id))
            {

                return RedirectToAction("GetAllThreathRequests", "Admin");

            }
            else {

                return RedirectToAction("GetAllThreathRequests", "Admin");

            }
        }


        [HttpGet]
        public ActionResult UpdateInfo(int? id, int? param, string returnmessage)
        {
           
         
            //==========
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            if (Session != null)
            {

                if (Session["AdminUser"] == null)
                {

                    return RedirectToAction("AdminLogin", "Home");
                }
                else {

                    if (id == null)
                    {
                        return HttpNotFound();
                    }
                    person = user.GetUser(id, param);

                    if (person == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.message = returnmessage;
                    return View(person);

                }
            }
            else {



                return RedirectToAction("AdminLogin", "Home");


            }

        }
        public ActionResult UpdateInfo(WebApplication.Models.cPerson userobj)
        {

            if ((userobj == null))
            {
                return HttpNotFound();
            }

            else
            {
                ModelState.Remove("password");
                ModelState.Remove("Confirmpassword");
                ModelState.Remove("workat");

                var errors = ModelState
                           .Where(x => x.Value.Errors.Count > 0)
                           .Select(x => new { x.Key, x.Value.Errors })
                           .ToArray();
                if (ModelState.IsValid)
                {

                    if (user.UpdatUserInfo(userobj))
                    {
                        string Subject = "Profile Updated ";
                        string Body = "Your profile settings have successfully been updated. Go to Profile Settings to view the changes.";
                        Models.Admin.SendEmail(userobj.email, Subject, Body);
                        return RedirectToAction("InfoUpdateRequests");
                    }
                    else {
                        var message = "Error while updating the profile, try later.";

                        return RedirectToAction("InfoUpdateRequests", new {returnmessage = message });

                    }
                }
                  else {
                       return View(userobj);


                    }
      
            }

        }
        public WebApplication.Models.ThreatReportViewModel Initialize(Models.ThreatReportViewModel ModelData)
        {
          //  Models.ThreatReportViewModel ModelData = new Models.ThreatReportViewModel();
            ModelData.AllCheckBoxes = new List<Models.CheckBoxLists>()
            {
                    new Models.CheckBoxLists { Id ="police",Name ="police"},
                    new Models.CheckBoxLists { Id ="organization",Name ="organization"},
                    new Models.CheckBoxLists { Id ="colleagues",Name ="colleagues"},
                    new Models.CheckBoxLists { Id ="family",Name ="family"},
                    new Models.CheckBoxLists { Id ="friends",Name ="friends"},
                    new Models.CheckBoxLists { Id ="None",Name ="None" }

            };
            ModelData.UserSelectedCheckBoxes = new List<Models.CheckBoxLists>()
            {




            };
            ModelData.SelectedCheckBoxes = ModelData.UserSelectedCheckBoxes.Select(x => x.Id).ToArray();

            return ModelData;

        }
        public ActionResult PostDateToCont(string DateValue) {

            dateval = DateValue;
            return Content("It works" + DateValue);
        }
        public WebApplication.Models.ThreatReportViewModel InitializeWhoToNotify(Models.ThreatReportViewModel ModelData)
        {
            //  Models.ThreatReportViewModel ModelData = new Models.ThreatReportViewModel();
            ModelData.AllCheckBoxesNotify = new List<Models.CheckBoxLists>()
            {
                    new Models.CheckBoxLists { Id ="police",Name ="police"},
                    new Models.CheckBoxLists { Id ="goverment",Name ="goverment"},
                    new Models.CheckBoxLists { Id ="local Unions",Name ="local Unions"},
                    new Models.CheckBoxLists { Id ="International bodies",Name ="International bodies"}
                  
            };
            ModelData.UserSelectedCheckBoxesNotify = new List<Models.CheckBoxLists>()
            {




            };
            ModelData.SelectedCheckBoxesNotify = ModelData.UserSelectedCheckBoxesNotify.Select(x => x.Id).ToArray();

            return ModelData;

        }


        [HttpGet]
        public ActionResult Process(int? id, int? param)
        {
            
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            if (Session != null)
            {

                if (Session["AdminUser"] == null)
                {

                    return RedirectToAction("AdminLogin", "Home");
                }
                else {

                    WebApplication.Models.ThreatReportViewModel viewModel = new WebApplication.Models.ThreatReportViewModel();

                    if (id == null)
                    {
                        return HttpNotFound();
                    }


                    viewModel = user.GetThreatRequest(id, param);
                    person = user.GetUser(param, 0);
                    viewModel.Users = person;
                    viewModel = Initialize(viewModel);
                    viewModel = InitializeWhoToNotify(viewModel);
                    if (viewModel.Threat == null)
                    {
                        return HttpNotFound();
                    }

                    return View(viewModel);

                }
            }
            else {



                return RedirectToAction("AdminLogin", "Home");


            }



        }


        [HttpPost]
        public ActionResult Process(WebApplication.Models.ThreatReportViewModel viewModel)
        {

            /*Gettig values form text boxex*/
                //=================================Email Body Creation fo sending email=====================================//
                long parentid = viewModel.Threat.ThreatReport_Id;
                string level = viewModel.Threat.Level_Of_Threat;
                string threatdetail = dateval;
                string sourceofthreat = viewModel.ThreatDetails.Source;
                string details = viewModel.ThreatDetails.Details;
                Boolean witness = viewModel.ThreatDetails.Witness;
                string witnessvalues = "No";
            if (witness == true)
            {
                witnessvalues = "Yes";

            }
           
               
                
                string Reason = viewModel.ThreatDetails.Any_Personal_Reason;
                string nameofsuspects = "<b>Name of Suspects are as follows: </b> <br />";
                
                for (int i = 0; i < textboxitems.Count(); i++)
                {
                    int val = i + 1;
                    nameofsuspects += val.ToString() + ". " + textboxitems[i] + "<br />";



                }
             
            
                nameofsuspects += "<br />";




                string whounotifyto = "<b>To whom you notified:</b> <br/>";
              
    
                if (viewModel.SelectedCheckBoxes != null)
                  {

                foreach (var item in viewModel.SelectedCheckBoxes)
                {

                    whounotifyto += ". " + item.ToString() + "<br/>";}
                 }
                whounotifyto += "<br/>";
                string towhommuhafiznotify = "<b>To whom Muhafiz notify:</b> <br/>";
                int initiall = 0;
                if (viewModel.SelectedCheckBoxesNotify!=null) {
                foreach (var item in viewModel.SelectedCheckBoxesNotify)
                {
                    int val = initiall + 1;
                    towhommuhafiznotify += ". " + item.ToString() + "<br/>";


                }
                }
                towhommuhafiznotify += "<br/>";
                string response = viewModel.ThreatDetails.Response;
                viewModel.Users = person;
             // =============================================End===============================================//
     //       if (ModelState.IsValid)
     //       {
                string ConfirmCode = user.InsertReportDetails(viewModel, textboxitems, dateval);
                if (!String.IsNullOrEmpty(ConfirmCode))
                {


                    string url = Url.Action("ConfirmationDetails", "Home",
                    new RouteValueDictionary(new { id = ConfirmCode }),
                    //url param
                    HttpContext.Request.Url.Scheme,
                    HttpContext.Request.Url.Host);
                    string Messagebody = "Dear " + person.UserName + "<br/><br/>" + "You have reported a threat using Muhafiz. Please review the details of the threat and then click the link below to verify that these details are accurate, to the best of your knowledge:<br/><br/>" + "<b>Parent threat number</b>: " + parentid.ToString() + "<br /><br />" + "<b>Level of threat</b>: " + level + "<br /><br />" + "<b>Date of threat:</b> " + threatdetail + "<br/><br />" + "<b>Source of threat:</b> " + sourceofthreat + "<br/><br />" + "<b>Details of threat:</b> " + details + "<br/><br />" + "<b>Witnesses of threat:</b> " + witnessvalues
                        + "<br/><br />" + "<b>Any personal reason:</b> " + Reason + "<br/><br />" + nameofsuspects + whounotifyto + "<b>Response of Authorities:</b> " + response + "<br/><br />" + towhommuhafiznotify + "<b>Link:</b> <br/>I hereby verify that the aforementioned details are correct and accurate, to the best of my knowledge at this time" + "<br/>" + url;
                    Models.Admin.SendEmail(person.email, "ConfirmationDetails", Messagebody);
                }

                return RedirectToAction("GetAllThreathRequests");

       //     }


             //    var message = "Error occurred while submitting credentials try again. ";
           //      return RedirectToAction("GetAllThreathRequests" , new { returnmessage = message });






        }

        public ActionResult test(string TextBoxVal)
        {

            textboxitems.Add(TextBoxVal);
            return Content("It works"+ TextBoxVal);
        }

      
        public ActionResult currentpasswordVerfication(string passwordValue)
        {

            // textboxitems.Add(TextBoxVal);
            string matched = "";
            byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(passwordValue);
            byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
            string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            if (hashedString.Equals(person.password))
            {
                matched = "true";
            }
            else {
                matched = "false";

            }

            return Content(matched);

        }
        [HttpGet]
        public ActionResult ThreatDetailConfirmation(int ? page)
        {

            Response.AddHeader("Refresh", "10");
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            

            List<Models.ThreatReportViewModel> modellist = user.GetConfirmedThreatDetail();
            if (modellist != null)
            {
                return View(modellist.ToPagedList(pageNumber, pageSize));
            }
            else {


                return HttpNotFound();

            }
        }

        [HttpGet]

        public ActionResult Confirm(int? id, int? param)
        {

            if (id == null)
            {

                return HttpNotFound();

            }
            else {



                return ConfirmDetails( id, param);

            }




        }



        [HttpPost]
        public ActionResult ConfirmDetails(int ? id, int ? param) {

            if (user.ConfirmDetailsByAdmin(id))
            {


                ViewBag.Displaymessages = "Details Confirmed";
                return RedirectToAction("ThreatDetailConfirmation", "Admin");

            }
            else {

                ViewBag.Displaymessages = "Error while confirming";
                return RedirectToAction("ThreatDetailConfirmation", "Admin");

            }




        }
        
        public ActionResult ThreatSearchIndex(string searchString, int? page) {

            List<Models.ThreatReportViewModel> SearchIndexList = user.GetAllThreatHistories();
            if (!String.IsNullOrEmpty(searchString))
            {
                SearchIndexList = user.GetAllThreatHistoriesByUserName(searchString);
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(SearchIndexList.ToPagedList(pageNumber, pageSize));

        }
        [HttpGet]
        public ActionResult ThreatDetails(string id) {

              Models.ThreatReportViewModel Detailmodel  = user.GetUserThreatDetails(Int32.Parse(id));
            if (Detailmodel != null)
            {
                return View(Detailmodel);

            }
            else {



                return RedirectToAction("ThreatSearchIndex", "Admin");
            }

            

        }



    }
}