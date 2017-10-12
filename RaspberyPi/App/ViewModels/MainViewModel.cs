////////////////////////////////////////////////////////
// Copyright (c) 2017 Sameer Khandekar                //
// License: MIT License.                              //
////////////////////////////////////////////////////////
using System;

using App.Commands;

using MvvmAtom;


namespace App.ViewModels
{
    /// <summary>
    /// View model for the main page
    /// </summary>
    public class MainViewModel : AtomViewModelBase
    { 
        /// <summary>
        /// Constructor create commands
        /// </summary>
        public MainViewModel()
        {
            CheckDoor = new CheckDoorCommand(this);
            SetupHub = new SetupHubCommand(this);
        }

        /// <summary>
        /// DeviceName property
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
        /// Device IP property
        /// </summary>
        private string _deviceIP = "10.11.98.43";
        public string DeviceIP
        {
            get
            {
                return _deviceIP;
            }

            set
            {
                if (_deviceIP != value)
                {
                    _deviceIP = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Status of the door
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
        /// Last checked time
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
        public AtomCommandBase CheckDoor { get; set; }

        /// <summary>
        /// Command to setup hub
        /// </summary>
        public AtomCommandBase SetupHub { get; set; }
    }
}
