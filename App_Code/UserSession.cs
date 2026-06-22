using System;
using System.Web;

namespace SACHIF_SIMS_Website
{
    public class UserSession
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }

        public static UserSession CurrentUser
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session["User"] != null)
                {
                    return (UserSession)HttpContext.Current.Session["User"];
                }
                return null;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Session["User"] = value;
                }
            }
        }

        public static bool IsLoggedIn()
        {
            return CurrentUser != null;
        }

        public static bool IsAdmin()
        {
            return CurrentUser != null && CurrentUser.Role == "admin";
        }

        public static bool IsSocialWorker()
        {
            return CurrentUser != null && CurrentUser.Role == "socialworker";
        }

        public static bool IsResponder()
        {
            return CurrentUser != null && CurrentUser.Role == "responder";
        }

        public static bool HasRole(string role)
        {
            return CurrentUser != null && CurrentUser.Role == role;
        }

        public static void Logout()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
            }
        }
    }
}