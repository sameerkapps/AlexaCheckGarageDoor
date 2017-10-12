////////////////////////////////////////////////////////
// Copyright (c) 2017 Sameer Khandekar                //
// License: MIT License.                              //
////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ESP82266ClientLib
{
    /// <summary>
    /// The client that actually makes http calls
    /// </summary>
    public class ESP8266Client : IESP8266Client
    {
        /// <summary>
        /// Given the ip, this makes GET request and returns the value
        /// </summary>
        /// <param name="esp8266IP">IP address of the esp8266</param>
        /// <returns></returns>
        public async Task<DoorStatus> GetDoorStatus(string esp8266IP)
        {
            // http client to make the call
            using (HttpClient client = new HttpClient())
            {
                // build the URI, http is fine on the internal Wi-Fi with firewall and password protection
                // around it
                UriBuilder uriBuilder = new UriBuilder("http", esp8266IP);
                // here is the path
                uriBuilder.Path = "/IsOpen";
                try
                {
                    // send the request and wait for the response
                    var responseMessage = await client.GetAsync(uriBuilder.ToString());
                    // if the call is sucessfull
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        // read the response as string and
                        // convert it to coresponding enum and return it
                        var resp = await responseMessage.Content.ReadAsStringAsync();
                        if (resp.Equals("0"))
                        {
                            return DoorStatus.Closed;
                        }
                        else if(resp.Equals("1"))
                        {
                            return DoorStatus.Open;
                        }
                    }

                    // if the call is not sucessfull, the door staus is unknown
                    return DoorStatus.Unknown;
                }
                catch (Exception ex)
                {
                    return DoorStatus.Unknown;
                }
            }
        }
    }
}
