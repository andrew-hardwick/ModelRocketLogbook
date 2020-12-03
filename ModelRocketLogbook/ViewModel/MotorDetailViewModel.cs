using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ModelRocketLogbook.Model;
using ModelRocketLogbook.Service;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ModelRocketLogbook.ViewModel
{
    public class MotorDetailViewModel : ViewModelBase
    {
        #region Private Members

        private readonly DataManager _dataManager;
        private readonly Guid _motorId;

        private string _name;
        private string _manufacturer;
        private string _propellant;
        private MotorMount _mount = MotorMount.None;

        private ObservableCollection<string> _mountOptions =
            ((MotorMount[])Enum.GetValues(typeof(MotorMount)))
                               .Select(v => v.ToFormattedString())
                               .ToObservableCollection();

        private int _selectedMountIndex = 0;
        private int _defaultDelay = 0;

        private bool _dirtyState = false;

        private double _maxThrust;
        private double _averageThrust;
        private double _totalImpulse;

        private RelayCommand _save;

        #endregion Private Members

        #region Constructor

        public MotorDetailViewModel(
            DataManager dataManager,
            Guid id)
        {
            _dataManager = dataManager;
            _motorId = id;

            SetValuesFromDataManager();
        }

        #endregion Constructor

        #region Private Methods

        private void SetValuesFromDataManager()
        {
            var motor = _dataManager.GetMotor(_motorId);

            Name = motor.Name;
            Manufacturer = motor.Manufacturer;
            Propellant = motor.Propellant;
            _mount = motor.Mount;
            DefaultDelay = motor.DefaultDelay;
            MaxThrust = motor.MaxThrust;
            AverageThrust = motor.AverageThrust;
            TotalImpulse = motor.TotalImpulse;

            DirtyState = false;

            RaiseNonsetPropertiesChanged();
        }

        private void RaiseNonsetPropertiesChanged()
        {
            RaisePropertyChanged(() => Mount);
        }

        #endregion Private Methods

        #region Commands

        public RelayCommand Save => _save ?? (_save = new RelayCommand(() =>
        {
            DirtyState = false;

            _dataManager.SaveMotor(
                _motorId,
                Name,
                Manufacturer,
                Propellant,
                EnumMount,
                DefaultDelay,
                MaxThrust,
                AverageThrust,
                TotalImpulse);
        }));

        #endregion Commands

        #region public Properties

        public Guid Id => _motorId;

        public string Name
        {
            get => _name;
            set
            {
                DirtyState = true;

                Set(() => Name, ref _name, value);
            }
        }

        public string Manufacturer
        {
            get => _manufacturer;
            set
            {
                DirtyState = true;

                Set(() => Manufacturer, ref _manufacturer, value);
            }
        }

        public string Propellant
        {
            get => _propellant;
            set
            {
                DirtyState = true;

                Set(() => Propellant, ref _propellant, value);
            }
        }

        public string Mount
        {
            get => _mount.ToFormattedString();
            set
            {
                _mount = value.ToMotorMount();

                DirtyState = true;

                RaisePropertyChanged(() => Mount);
            }
        }

        public MotorMount EnumMount => _mount;

        public ObservableCollection<string> MountOptions
        {
            get => _mountOptions;
            set => Set(() => MountOptions, ref _mountOptions, value);
        }

        public int SelectedMountIndex
        {
            get => _selectedMountIndex;
            set
            {
                DirtyState = true;
                Set(() => SelectedMountIndex, ref _selectedMountIndex, value);
                Mount = MountOptions[value];
            }
        }

        public int DefaultDelay
        {
            get => _defaultDelay;
            set
            {
                DirtyState = true;
                Set(() => DefaultDelay, ref _defaultDelay, value);
            }
        }

        public bool DirtyState
        {
            get => _dirtyState;
            set => Set(() => DirtyState, ref _dirtyState, value);
        }

        public double MaxThrust
        {
            get => _maxThrust;
            set
            {
                DirtyState = true;
                Set(() => MaxThrust, ref _maxThrust, value);
            }
        }

        public double AverageThrust
        {
            get => _averageThrust;
            set
            {
                DirtyState = true;
                Set(() => AverageThrust, ref _averageThrust, value);
            }
        }

        public double TotalImpulse
        {
            get => _totalImpulse;
            set
            {
                DirtyState = true;
                Set(() => TotalImpulse, ref _totalImpulse, value);
            }
        }

        #endregion public Properties
    }
}