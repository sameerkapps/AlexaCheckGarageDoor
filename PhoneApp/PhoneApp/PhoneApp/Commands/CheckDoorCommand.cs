////////////////////////////////////////////////////////
// Copyright (c) 2017 Sameer Khandekar                //
// License: MIT License.                              //
////////////////////////////////////////////////////////
using MvvmAtom;
using PhoneApp.ViewModels;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PhoneApp.Commands
{
    public class CheckDoorCommand : AtomCommandBase
    {
        /// <summary>
        /// Constructor will initialize the client
        /// </summary>
        /// <param name="viewModel"></param>
        public CheckDoorCommand(AtomViewModelBase viewModel)
            : base(viewModel)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add(HeaderKeyName, HeaderKeyVal);
        }

        /// <summary>
        /// When device IP gets changed, this will en/disble the command
        /// </summary>
        /// <param name="propName"></param>
        public override void EvaluateCanExecuteChanged(string propName)
        {
            if (string.Equals(nameof(MainPageViewModel.DeviceName), propName))
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
            return !string.IsNullOrWhiteSpace(mainView.DeviceName);
        }

        /// <summary>
        /// Obtains status from ESP8266 and updates
        /// Status and last checked in the view model
        /// </summary>
        /// <param name="parameter"></param>
        public async override void Execute(object parameter)
        {
            UriBuilder uriBuilder = new UriBuilder(BaseAddr + mainView.DeviceName);

            try
            {
                Debug.WriteLine(uriBuilder.ToString());
                var responseMessage = await _client.GetAsync(uriBuilder.ToString());

                mainView.Status = await responseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                mainView.Status = "Error while checking";
            }

            mainView.LastChecked = DateTime.Now.ToString();
        }

        HttpClient _client = new HttpClient();

        MainPageViewModel mainView => (MainPageViewModel)base.ViewModel;

        private const string BaseAddr = "https://YOUR SITE.azurewebsites.net/api/GarageDoor?doorName=";
        private const string HeaderKeyName = "GarageDoorSecurityKey";
        private const string HeaderKeyVal = "YOUR KEY";
    }
}
