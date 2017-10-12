////////////////////////////////////////////////////////
// Copyright (c) 2017 Sameer Khandekar                //
// License: MIT License.                              //
////////////////////////////////////////////////////////
using System;

using App.ViewModels;
using ESP82266ClientLib;
using MvvmAtom;

namespace App.Commands
{
    /// <summary>
    /// Command to check the door
    /// </summary>
    public class CheckDoorCommand : AtomCommandBase
    {
        /// <summary>
        /// Constructor will initialize the client
        /// </summary>
        /// <param name="viewModel"></param>
        public CheckDoorCommand(AtomViewModelBase viewModel) 
            : base(viewModel)
        {
            Init();
        }

        /// <summary>
        /// When device IP gets changed, this will en/disble the command
        /// </summary>
        /// <param name="propName"></param>
        public override void EvaluateCanExecuteChanged(string propName)
        {
            if (string.Equals(nameof(MainViewModel.DeviceIP), propName))
            {
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Can execute, if there is a string in the Device IP field
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override bool CanExecute(object parameter)
        {
            return !string.IsNullOrWhiteSpace(mainView.DeviceIP);
        }

        /// <summary>
        /// Obtains status from ESP8266 and updates
        /// Status and last checked in the view model
        /// </summary>
        /// <param name="parameter"></param>
        public async override void Execute(object parameter)
        {
            var ret = await _client.GetDoorStatus(mainView.DeviceIP);

            mainView.Status = ret.ToString();
            mainView.LastChecked = DateTime.Now.ToString();
        }

        // instantiate the client
        private void Init()
        {
            _client = new ESP8266Client();
        }

        ESP8266Client _client;

        MainViewModel mainView => (MainViewModel)base.ViewModel;
    }
}
