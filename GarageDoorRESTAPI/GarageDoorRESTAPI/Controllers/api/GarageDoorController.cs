////////////////////////////////////////////////////////
// Copyright (c) 2017 Sameer Khandekar                //
// License: MIT License.                              //
////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.AspNet.SignalR;

using GarageDoorRESTAPI.Attributes;
using GarageDoorRESTAPI.Hub;
using GarageDoorRESTAPI.Storage;

namespace GarageDoorRESTAPI.Controllers.api
{
    /// <summary>
    /// Controller to retrieve the status of the door
    /// </summary>
    public class GarageDoorController : ApiController
    {

        /// <summary>
        /// Get to retrieve the status of the given door
        /// </summary>
        /// <param name="doorName">Name of the door</param>
        /// <returns>Status</returns>
        // http://localhost:53058/api/GarageDoor/1
        [CustomKeyAuthorize]
        public string Get(string doorName)
        {
            // create instance of the utility
            var util = new GarageDoorHubUtil();
            // get the status of the door
            var task = Task.Run<string>(() => util.GetDoorStatus(doorName));
            task.Wait();
            return task.Result;
        }
    }
}