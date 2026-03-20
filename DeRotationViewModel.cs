using NINA.Core.Interfaces;
using NINA.Equipment.Interfaces.ViewModel;
using NINA.Profile.Interfaces;
using NINA.WPF.Base.ViewModel;
using System;
using System.ComponentModel.Composition;

namespace AltAzDeRotator
{
    [Export(typeof(IDockableVM))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DeRotationViewModel : DockableVM
    {
        [ImportingConstructor]
        public DeRotationViewModel(IProfileService profileService) : base(profileService)
        {
        }

        public new string Id => "AltAz_DeRotator_Status_Window";
        public new string Title => "Alt-Az De-Rotator";
        public new bool IsTool => true;

        private double _altitude;
        public double Altitude
        {
            get => _altitude;
            set { _altitude = value; RaisePropertyChanged(); }
        }

        private double _azimuth;
        public double Azimuth
        {
            get => _azimuth;
            set { _azimuth = value; RaisePropertyChanged(); }
        }

        private double _rotationRate;
        public double RotationRate
        {
            get => _rotationRate;
            set { _rotationRate = value; RaisePropertyChanged(); }
        }

        private double _targetPosition;
        public double TargetPosition
        {
            get => _targetPosition;
            set { _targetPosition = value; RaisePropertyChanged(); }
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; RaisePropertyChanged(); }
        }

        private bool _isDeRotationEnabled = true;
        public bool IsDeRotationEnabled
        {
            get => _isDeRotationEnabled;
            set { _isDeRotationEnabled = value; RaisePropertyChanged(); }
        }

        private string _status = "Stopped";
        public string Status
        {
            get => _status;
            set { _status = value; RaisePropertyChanged(); }
        }
    }
}
