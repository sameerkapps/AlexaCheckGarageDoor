////////////////////////////////////////////////////////
// Copyright (c) 2017 Sameer Khandekar                //
// License: MIT License.                              //
////////////////////////////////////////////////////////
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SignalRHub = Microsoft.AspNet.SignalR;

using Microsoft.AspNet.SignalR;

using GarageDoorRESTAPI.Storage;

namespace GarageDoorRESTAPI.Hub
{
    /// <summary>
    /// Hub for the garage door
    /// </summary>
    public class GarageDoorHub : SignalRHub.Hub
    {
        /// <summary>
        /// When connected, save the connection id of the door
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            try
            {
                var deviceName = Context.QueryString["DoorName"];
                if (!string.IsNullOrEmpty(deviceName))
                {
                    Groups.Add(Context.ConnectionId, deviceName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return base.OnConnected();
        }

        /// <summary>
        /// This is called by the client passing unique key that was given to it by the server
        /// And the value of the door status. This is compiled and added to the queue
        /// </summary>
        /// <param name="id">unique id of the query</param>
        /// <param name="doorName">Name of the door</param>
        /// <param name="val">value of the status</param>
        /// <returns></returns>
        public async Task SetDoorStatus(string id, string doorName, string val)
        {
            Debug.WriteLine($"Val is {val}");
            // add to queue
            var queUtil = new QueueUtil();
            await queUtil.AddToQueue(id + val);
        }

        // dictionary stores map of door name and connection id
        ConcurrentDictionary<string, TaskCompletionSource<string>> _doorToTask = new ConcurrentDictionary<string, TaskCompletionSource<string>>();

        // To improve
        // disconnect, reconnect are not handled.
    }
}