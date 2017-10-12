////////////////////////////////////////////////////////
// Copyright (c) 2017 Sameer Khandekar                //
// License: MIT License.                              //
////////////////////////////////////////////////////////
using System;
using System.Threading.Tasks;

namespace ESP82266ClientLib
{
    /// <summary>
    /// Interface defining the signature of the method
    /// </summary>
    interface IESP8266Client
    {
        Task<DoorStatus> GetDoorStatus(string esp8266Name);
    }
}
