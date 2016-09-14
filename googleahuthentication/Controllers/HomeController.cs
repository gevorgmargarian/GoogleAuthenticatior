using Google.Authenticator;
using googleahuthentication.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace googleahuthentication.Controllers
{
    public class HomeController : Controller
    {
        private const string key = "qaz123!@@)(*";
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel login)
        {
            string message = "";
            bool status = false;

            if (login.UserName == "admin" && login.Password == "password")
            {
                status = true;
                message = "2FA Verification";
                Session["username"] = login.UserName;

                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                string userUniqueKey = login.UserName + key;//it is a demo ,key  will be any encrypt value
                Session["userUniqueKey"] = userUniqueKey;
                var setupinfo = tfa.GenerateSetupCode("Dotnet Awesome", login.UserName, userUniqueKey, 300, 300);
                ViewBag.BarcodeImageUrl = setupinfo.QrCodeSetupImageUrl;
                ViewBag.SetupCode = setupinfo.ManualEntryKey;
            }
            else
            {
                message = "Invalid Credintial";
            }
            ViewBag.Massage = message;
            ViewBag.Status = status;
            return View();
        }
        public ActionResult MyProfile()
        {
            if (Session["Username"] == null || Session["Isvalid2FA"] == null || !(bool)Session["Isvalid2FA"])
            {
                return RedirectToAction("Login");
            }
            ViewBag.Message = "Welcome" +"    "+ Session["Username"].ToString().ToUpper();
            return View();
        }
        public ActionResult Verify2FA()
        {
            var token = Request["passcode"];
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            string UserUniqueKey = Session["userUniqueKey"].ToString();
            bool isvalid = tfa.ValidateTwoFactorPIN(UserUniqueKey, token);
            if (isvalid)
            {
                Session["isvalid2FA"] = true;
                return RedirectToAction("MyProfile","Home");
            }
            return RedirectToAction("Login","Home");
        }
    }
}