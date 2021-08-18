using System.Web.Mvc;
using LMS.UI.MVC.Models; //added this using statement so I can access Models
using System; //had to add for the try/catch for Exception
using System.Configuration; //added this so I can update mailmessage to ConfigurationManager instead of our pers info
using System.Net; //added this for client credentials for email
using System.Net.Mail; //added this so I can set up email (mailmessage)
using reCAPTCHA.MVC;

namespace LMS.UI.MVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CaptchaValidator(PrivateKey = "6LfxmQUcAAAAAJbJdlG7IGFgC8BgeG6VwTd6uYhO", ErrorMessage = "Invalid input CAPTCHA", RequiredMessage = "Please verify you are not a robot (beep boop)")]
        public ActionResult Contact(ContactViewModel cvm, bool captchaValid)
        {
            if (!ModelState.IsValid)
            {
                return View(cvm);
            }

            if (ModelState.IsValid && captchaValid)
            {

            
            string message = $"You have received an email from {cvm.Name} with a subject of {cvm.Subject}. Please respond to {cvm.Email} with your response to the following message: <br/> {cvm.Message}";

            MailMessage mm = new MailMessage(ConfigurationManager.AppSettings["EmailUser"].ToString(), ConfigurationManager.AppSettings["EmailTo"].ToString(), cvm.Subject, message);

            mm.IsBodyHtml = true;
            mm.Priority = MailPriority.High;
            mm.ReplyToList.Add(cvm.Email);

            SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["EmailClient"].ToString());
            //client.Port = 8889; - for gmail

            client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EmailUser"].ToString(), ConfigurationManager.AppSettings["EmailPass"].ToString());

            try
            {
                client.Send(mm);
            }
            catch (Exception ex)
            {
                ViewBag.CustomerMessage = $"Ope. Something went wrong and we couldn't process your request. Please try again later. Error Message: </br> {ex.StackTrace}";
                return View(cvm);
            }

            return View("EmailConfirmation", cvm);
            }

            return View(cvm);
        }//end contact cvm

        //[HttpPost]
        //[CaptchaValidator(PrivateKey = "6LfxmQUcAAAAAJbJdlG7IGFgC8BgeG6VwTd6uYhO", ErrorMessage = "Invalid input CAPTCHA", RequiredMessage = "Please verify you are not a robot (beep boop)")]
        //public ActionResult Index(ContactViewModel contact, bool captchaValid)
        //{
        //    if (ModelState.IsValid && captchaValid)
        //    {
        //        return View("reCaptchaPassed");
        //    }
        //    return View(contact);
        //}
    }//end class
}//end namespace
