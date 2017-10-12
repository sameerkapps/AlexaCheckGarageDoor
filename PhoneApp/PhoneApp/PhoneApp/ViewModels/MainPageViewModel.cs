////////////////////////////////////////////////////////
// Copyright (c) 2017 Sameer Khandekar                //
// License: MIT License.                              //
////////////////////////////////////////////////////////
using System;

using MvvmAtom;
using PhoneApp.Commands;

namespace PhoneApp.ViewModels
{
    /// <summary>
    /// View model for the web page
    /// </summary>
    public class MainPageViewModel : AtomViewModelBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MainPageViewModel()
        {
            CheckDoorCommand = new CheckDoorCommand(this);
        }

        /// <summary>
        /// Name of the device
        /// </summary>
        private string _deviceName = "GarageDoor1";
        public string DeviceName
        {
            get
            {
                return _deviceName;
            }

            set
            {
                if (_deviceName != value)
                {
                    _deviceName = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Status returned by the REST API
        /// </summary>
        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status != value)
                {
                    _status = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Last check time
        /// </summary>
        private string _lastChked;
        public string LastChecked
        {
            get
            {
                return _lastChked;
            }
            set
            {
                if (_lastChked != value)
                {
                    _lastChked = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Command to check door
        /// </summary>
        public AtomCommandBase CheckDoorCommand { get; set; }
    }
}
