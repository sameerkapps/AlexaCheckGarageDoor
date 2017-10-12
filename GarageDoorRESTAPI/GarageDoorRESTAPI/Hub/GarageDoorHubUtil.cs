////////////////////////////////////////////////////////
// Copyright (c) 2017 Sameer Khandekar                //
// License: MIT License.                              //
////////////////////////////////////////////////////////
using System;
using System.Threading.Tasks;

using Microsoft.AspNet.SignalR;

using GarageDoorRESTAPI.Storage;
using System.Diagnostics;
using Microsoft.AspNet.SignalR.Hubs;

namespace GarageDoorRESTAPI.Hub
{
    /// <summary>
    /// Signal R allows server calling client method. But the client cannot return a value.
    /// This utility addresses the limitation.
    /// </summary>
    public class GarageDoorHubUtil
    {
        /// <summary>
        /// Method creates a GUID and invokes client method. It is expected that the client will call a method on the Signal R Hub.
        /// With the GUID and the return value.
        /// The hub method will compose a queue message and add it to the queue.
        /// This method will wait till a message with the unique id shows up in the queue
        /// </summary>
        /// <param name="doorName"></param>
        /// <returns></returns>
        public async Task<string> GetDoorStatus(string doorName)
        {
            // get the context
            var homeContext = GlobalHost.ConnectionManager.GetHubContext<GarageDoorHub>();
            Debug.WriteLine($"Pre-Garage door open");
            // create a guid
            var id = Guid.NewGuid().ToString();
            var queUtil = new QueueUtil();
            // ensure that the door name exists
            var grp = homeContext.Clients.Group(doorName);
            if (grp != null)
            {
                // Not sure why sending only to group does not work.
                // if you can find the solution, let me know.
                // commented code kept here for reference
                //homeContext.Clients.Group(doorName).CheckGarageDoor(id, doorName);

                //var args = new string[] { id, doorName };
                //await grp.Invoke("CheckGarageDoor", args);
                // grp.CheckGarageDoor(id, doorName);

                // send message to client to check garage door with the id
                homeContext.Clients.All.CheckGarageDoor(id, doorName);

                // wait for the message to show up
                var ret = await queUtil.WaitForMessage(id, 200 * 1000);

                // obtain value from the message and return it to the caller
                if (!string.IsNullOrEmpty(ret))
                {
                    var val = ret.Substring(id.Length);

                    return val;
                }
            }
            return "Unknown";
        }
    }
}