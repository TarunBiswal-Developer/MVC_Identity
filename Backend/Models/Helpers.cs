using Backend.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;

namespace Backend
{
    public class Constants
    {
        public static decimal ExcavationDepthCap = 1.5M;
    }
    public class SessionSection
    {
        public static TimeSpan TimeOut
        {
            get
            {
                return ((SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState")).Timeout;
            }
        }
    }
    public class Helpers
    {
        

        public static string BaseUrl
        {
            get
            {
                return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath;
            }
        }
        public static string VirtualDirectory
        {
            get
            {
                return HttpContext.Current.Request.ApplicationPath + "/";
            }
        }
        public static string ApplicationName
        {
            get
            {
                return ConfigurationManager.AppSettings["ApplicationName"];
            }
        }
        public static string ApplicationShortName
        {
            get
            {
                return ConfigurationManager.AppSettings["ApplicationShortName"];
            }
        }
        public static string ApplicationVersion
        {
            get
            {
                return ConfigurationManager.AppSettings["ApplicationVersion"];
            }
        }
        public static string ApplicationEnvironment
        {
            get
            {
                return ConfigurationManager.AppSettings["ApplicationEnvironment"];
            }
        }
        public static string AesKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AesKey"];
            }
        }
        public static bool ConcurrentLogin
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["ConcurrentLogin"]);
            }
        }
        public static bool TriggerEmail
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["TriggerEmail"]);
            }
        }
        public static string EmailServiceProtocol
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailServiceProtocol"];
            }
        }
  
        public static bool WriteExceptionsToEventLog
        {
            get
            {
                return Convert.ToBoolean(WebConfigurationManager.AppSettings["WriteExceptionsToEventLog"]);
            }
        }
        public static void WriteToErrorLog(Exception e, string actionName)
        {
            //sLogFormat used to create log files format :
            // dd/mm/yyyy hh:mm:ss AM/PM ==> Log Message
            string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ----> ";
            //this variable used to create log filename format "
            //for example filename : ErrorLogYYYYMMDD
            string sErrorTime = DateTime.Now.ToString("ddMMyyyy");
            string message = "An exception occurred communicating with the data source." + "\n\n";
            message += "Action: " + actionName + "\n\n";
            message += "Exception: " + e.ToString() + "\n\n";
            //Log Folder Path
            string LogPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/logs";
            bool dirExists = Directory.Exists(LogPath);
            if (!dirExists)
                Directory.CreateDirectory(LogPath);
            StreamWriter sw = new StreamWriter(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/logs/ErrorLog" + sErrorTime + ".txt", true);
            sw.WriteLine(sLogFormat + message);
            sw.Flush();
            sw.Close();
        }
        public static void WriteToEventLog(string EventTag, string EventLog)
        {
            //sLogFormat used to create log files format :
            // dd/mm/yyyy hh:mm:ss AM/PM ==> Log Message
            string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ----> ";
            //this variable used to create log filename format "
            //for example filename : ErrorLogYYYYMMDD
            string sErrorTime = DateTime.Now.ToString("ddMMyyyy");
            string message = "Tag: " + EventTag + "\n\n";
            message += "Log: " + EventLog + "\n\n";
            //Log Folder Path
            string LogPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/logs";
            bool dirExists = Directory.Exists(LogPath);
            if (!dirExists)
                Directory.CreateDirectory(LogPath);
            StreamWriter sw = new StreamWriter(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/logs/EventLog" + sErrorTime + ".txt", true);
            sw.WriteLine(sLogFormat + message);
            sw.Flush();
            sw.Close();
        }

        public static string GenerateRandomPassword(int length)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            char[] chars = new char[length];
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }
        
        public static string escapeSpecialChars(string s)
        {
            string res = s;
            if (!string.IsNullOrEmpty(res))
            {
                // replace entities with literal values
                res = res.Replace("&", "amp;");
                res = res.Replace("#", "hash;");
                res = res.Replace("+", "plus;");
                res = res.Replace(",", "comma;");
                res = res.Replace("&amp;", "&");
            }
            return res;
        }
      

        public static string RenderViewToString(ControllerContext context, string viewName, object model = null)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");
            ViewDataDictionary viewData = new ViewDataDictionary(model);
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                ViewContext viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        public static string GenerateRandomNumbers(int length)
        {
            string allowedChars = "0123456789";
            char[] chars = new char[length];
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }
     

    }
    public static class HtmlExtensions
    {
        public static string IsActive(this HtmlHelper html, string controllers = "", string actions = "", string cssClass = "active")
        {
            ViewContext viewContext = html.ViewContext;
            bool isChildAction = viewContext.Controller.ControllerContext.IsChildAction;
            if (isChildAction)
                viewContext = html.ViewContext.ParentActionViewContext;
            System.Web.Routing.RouteValueDictionary routeValues = viewContext.RouteData.Values;
            string currentAction = routeValues["action"].ToString();
            string currentController = routeValues["controller"].ToString();
            if (string.IsNullOrEmpty(actions))
                actions = currentAction;
            if (string.IsNullOrEmpty(controllers))
                controllers = currentController;
            string[] acceptedActions = actions.Trim().Split(',').Distinct().ToArray();
            string[] acceptedControllers = controllers.Trim().Split(',').Distinct().ToArray();
            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ? cssClass : string.Empty;
        }
    }
    public static class ExceptionExtensions
    {
        public static string GetFullMessage(this Exception ex)
        {
            return ex.InnerException == null
                 ? ex.Message
                 : ex.Message + " --> " + ex.InnerException.GetFullMessage();
        }
    }
    
    public class LogViewModel
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public DateTime? FileCreatedOn { get; set; }
    }
}