////////////////////////////////////////////////////////
// Copyright (c) 2017 Sameer Khandekar                //
// License: MIT License.                              //
////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace GarageDoorRESTAPI.Attributes
{
    public class CustomKeyAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Static constructor to read values from config and assign them
        /// </summary>
        static CustomKeyAuthorizeAttribute()
        {
            GarageDoorSecurityKey = ConfigurationManager.AppSettings.Get(nameof(GarageDoorSecurityKey));
        }

        /// <summary>
        /// Checks, if the correct key is passed
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var currHeaders = HttpContext.Current.Request.Headers;
            if (currHeaders.AllKeys.Any((key) => string.Equals(key, nameof(GarageDoorSecurityKey), StringComparison.InvariantCultureIgnoreCase)))
            {
                return string.Equals(currHeaders[nameof(GarageDoorSecurityKey)],
                                     GarageDoorSecurityKey,
                                     StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        private static readonly string GarageDoorSecurityKey;
    }
}