////////////////////////////////////////////////////////
// Copyright (c) 2017 Sameer Khandekar                //
// License: MIT License.                              //
////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using MvvmAtom;

using App.ViewModels;
using ESP82266ClientLib;

namespace App.Commands
{
    /// <summary>
    /// Command to set up the hub and regiser the ESP8266
    /// </summary>
    public class SetupHubCommand :  AtomCommandBase
    {
        public SetupHubCommand(AtomViewModelBase viewModel)
            :base(viewModel)
        {
        }

        /// <summary>
        /// En/disable the command based on the values in IP and name
        /// </summary>
        /// <param name="propName"></param>
        public override void EvaluateCanExecuteChanged(string propName)
        {
            if (nameof(MainViewModel.DeviceIP).Equals(propName) ||
                nameof(MainViewModel.DeviceName).Equals(propName))
            {
                RaiseCanExecuteChanged();
            }
        }

        // Can execute only if both IP and name has values
        public override bool CanExecute(object parameter)
        {
            return !string.IsNullOrWhiteSpace(mainView.DeviceName) &&
                   !string.IsNullOrWhiteSpace(mainView.DeviceIP);
        }

        /// <summary>
        /// Sets up the hub
        /// </summary>
        /// <param name="parameter"></param>
        public override void Execute(object parameter)
        {
            SetupHub(HubBaseAddress);
        }

        #region private methods
        /// <summary>
        /// Sets up Signal R hub
        /// </summary>
        private void SetupHub(string cloudBaseAddr)
        {
            var nameDictionary = new Dictionary<string, string>();
            nameDictionary["DoorName"] = mainView.DeviceName;
            // set the hub connection
            var hubConnection = new HubConnection(cloudBaseAddr, nameDictionary);
            // create the proxy
            _hubProxy = hubConnection.CreateHubProxy("GarageDoorHub");
            // lisen to CheckGarageDoor
            _hubProxy.On<string, string>("CheckGarageDoor", (id, doorName) => ReportDoorStatus(id, doorName));
            // start listening
            hubConnection.Start(new LongPollingTransport());
        }

        /// <summary>
        /// Server asks to check the door
        /// </summary>
        /// <param name="id">id of the session</param>
        /// <param name="on"></param>
        private async void ReportDoorStatus(string id, string doorName)
        {
            // create the client
            ESP8266Client client = new ESP8266Client();
            // obtain the status from ESP8266
            // it is unknown in case of exception
            DoorStatus status = DoorStatus.Unknown;
            try
            {
                status = await client.GetDoorStatus(mainView.DeviceIP);
            }
            catch (Exception)
            {
            }

            // build the parameters for the method that will be invoked on the hub
            var arr = new string[] { id, mainView.DeviceName, status.ToString() };
            // call the method on the hub
            await _hubProxy.Invoke("SetDoorStatus", arr);
        }

        #endregion

        private IHubProxy _hubProxy;

        MainViewModel mainView => (MainViewModel)base.ViewModel;

        private const string HubBaseAddress = "http://localhost:53058/"; // Change to YOUR azurewebsite, when deployed
    }
}
