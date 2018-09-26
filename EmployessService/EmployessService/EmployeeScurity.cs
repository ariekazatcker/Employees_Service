using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EmployessService
{
    /// <summary>
    /// Providing employees security services
    /// </summary>
    public class EmployeeScurity
    {
        static string s_user;
        static string s_password;

        static EmployeeScurity()
        {
            //Retrieving Employees Service credential data
            var applicationSettings = ConfigurationManager.GetSection("ApplicationSettings") as NameValueCollection;
            //applicationSettings might be null in case of unit-test
            if (applicationSettings != null)
            {
                s_user = applicationSettings["User"] != null ? applicationSettings["User"] : "Admin";
                s_password = applicationSettings["Password"] != null ? applicationSettings["Password"] : "Dem0";
            }
            else
            {
                s_user = "Admin";
                s_password = "Dem0";
            }
        }

        /// <summary>
        /// Authenticating logged in user, verifying that match Employee Service credentials.  
        /// </summary>
        /// <param name="user">Logged in user</param>
        /// <param name="password">Logged in password</param>
        /// <returns>'true' if logged in credentials match Employees Service credentials and 'false' otherwise</returns>
        public static bool AuthenticateLogin(string user, string password)
        {
            return s_user.Equals(user) && s_password.Equals(password);
        }

        /// <summary>
        /// Authenticating logged in user name
        /// </summary>
        /// <param name="username">Logged in user</param>
        /// <returns>'true' if logged in user match Employees Service user and 'false' otherwise</returns>
        public static bool AuthenticateUser(string username)
        {
            return s_user.Equals(username);
        }
    }
}