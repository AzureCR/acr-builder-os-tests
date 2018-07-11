using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.InteropServices;
using System.Text;

namespace aspnetmvcapp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            ViewData["HOSTNAME"] = Environment.GetEnvironmentVariable("COMPUTERNAME") ??
                                                       Environment.GetEnvironmentVariable("HOSTNAME");
            ViewData["OSARCHITECTURE"] = RuntimeInformation.OSArchitecture;
            ViewData["OSDESCRIPTION"] = RuntimeInformation.OSDescription;
            ViewData["PROCESSARCHITECTURE"] = RuntimeInformation.ProcessArchitecture;
            ViewData["PROCESSOR_REVISION"] = Environment.GetEnvironmentVariable("PROCESSOR_REVISION");
            ViewData["FRAMEWORKDESCRIPTION"] = RuntimeInformation.FrameworkDescription;
            ViewData["ASPNETCOREPACKAGEVERSION"] = Environment.GetEnvironmentVariable("ASPNETCORE_PKG_VERSION");
            ViewData["ASPNETCORE_ENVIRONMENT"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            StringBuilder envVars = new StringBuilder();
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
                envVars.Append(string.Format("<strong>{0}</strong>:{1}<br \\>", de.Key, de.Value));

            ViewData["ENV_VARS"] = envVars.ToString();
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}