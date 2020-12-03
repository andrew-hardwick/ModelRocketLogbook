using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ModelRocketLogbook.Model;
using ModelRocketLogbook.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;

namespace ModelRocketLogbook.ViewModel
{
    public class FlightDetailViewModel : ViewModelBase
    {
        #region Private Variables

        private readonly DataManager _dataManager;
        private readonly Guid _flightId;

        private Flight _flight;

        private string _rocketName = string.Empty;
        private string _notes = string.Empty;

        private int _selectedMotorIndex = 0;
        private int _adjustedDelay = 0;

        private double _dryWeight = 0.0;
        private double _flightWeight = 0.0;
        private double _apogee = 0.0;

        private ObservableCollection<FlightResult> _resultOptions
            = new ObservableCollection<FlightResult>();

        private ObservableCollection<string> _motorOptions
            = new ObservableCollection<string>();

        private List<Guid> _motorIds;

        private FlightResult _flightResult = FlightResult.Nominal;

        private bool _dirtyState = false;

        private DateTime _dateOfFlight = DateTime.Now;

        private Motor _selectedMotor = null;

        private RelayCommand _save;

        #endregion Private Variables

        #region Constructor

        public FlightDetailViewModel(
            DataManager datamanager,
            Guid flightId)
        {
            _dataManager = datamanager;

            _dataManager.OnRocketChanged += HandleRocketChanged;

            _dataManager.OnMotorCollectionChanged += HandleMotorCollectionChanged;

            HandleMotorCollectionChanged();

            _flightId = flightId;

            SetValuesFromFlight();

            ResultOptions = ((FlightResult[])Enum.GetValues(typeof(FlightResult))).ToObservableCollection();
        }

        #endregion Constructor

        #region Commands

        public RelayCommand Save => _save ?? (_save = new RelayCommand(() =>
        {
            _dataManager.SaveFlight(
                _flightId,
                FlightResult,
                DateOfFlight,
                _selectedMotor,
                AdjustedDelay,
                DryWeight,
                FlightWeight,
                Apogee,
                Notes);

            _dirtyState = false;
        }));

        #endregion Commands

        #region Private Methods

        private void HandleRocketChanged(Guid rocketId)
        {
            if (_flight.RocketId.Equals(rocketId))
            {
                SetValuesFromFlight();
            }
        }

        private void HandleMotorCollectionChanged()
        {
            var motors = _dataManager.GetMotors();

            _motorIds = motors.Select(m => m.Id).ToList();

            MotorOptions = _dataManager.GetMotors().Select(m => $"{m.Manufacturer} {m.Name} ({m.Propellant})").ToObservableCollection();

            SelectCorrectMotorIndex();
        }

        private void SetValuesFromFlight()
        {
            _flight = _dataManager.GetFlight(_flightId);
            FlightResult = _flight.FlightResult;

            RocketName = _dataManager.GetRocket(_flight.RocketId).Name;
            DateOfFlight = _flight.DateOfFlight;
            _selectedMotor = _flight.Motor;
            AdjustedDelay = _flight.AdjustedDelay;
            DryWeight = _flight.DryWeight;
            FlightWeight = _flight.FlightWeight;
            Apogee = _flight.Apogee;
            Notes = _flight.Notes;

            SelectCorrectMotorIndex();

            DirtyState = false;
        }

        private void SelectCorrectMotorIndex()
        {
            if (_flight?.Motor != null)
            {
                SelectedMotorIndex = _motorIds.IndexOf(_flight.Motor.Id);
            }
        }

        private void RaiseNonSetPropertiesChanged()
        {
            RaisePropertyChanged(() => Nominal);
            RaisePropertyChanged(() => Mishap);
            RaisePropertyChanged(() => Catastrophe);
            RaisePropertyChanged(() => MotorName);
        }

        #endregion Private Methods

        #region Public Properties

        public string RocketName
        {
            get => _rocketName;
            set => Set(() => RocketName, ref _rocketName, value);
        }

        public string Notes
        {
            get => _notes;
            set
            {
                Set(() => Notes, ref _notes, value);
                DirtyState = true;
            }
        }

        public int SelectedMotorIndex
        {
            get => _selectedMotorIndex;
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                Set(() => SelectedMotorIndex, ref _selectedMotorIndex, value);
                _selectedMotor = _dataManager.GetMotor(_motorIds[SelectedMotorIndex]);
                RaiseNonSetPropertiesChanged();
                DirtyState = true;
            }
        }

        public int AdjustedDelay
        {
            get => _adjustedDelay;
            set { Set(() => AdjustedDelay, ref _adjustedDelay, value);
                DirtyState = true;
            }
        }

        public double DryWeight
        {
            get => _dryWeight;
            set
            {
                Set(() => DryWeight, ref _dryWeight, value);
                DirtyState = true;
            }
        }
        public double FlightWeight
        {
            get => _flightWeight;
            set
            {
                Set(() => FlightWeight, ref _flightWeight, value);
                DirtyState = true;
            }
        }
        public double Apogee
        {
            get => _apogee;
            set
            {
                Set(() => Apogee, ref _apogee, value);
                DirtyState = true;
            }
        }

        public ObservableCollection<FlightResult> ResultOptions
        {
            get => _resultOptions;
            set => Set(() => ResultOptions, ref _resultOptions, value);
        }

        public ObservableCollection<string> MotorOptions
        {
            get => _motorOptions;
            set => Set(() => MotorOptions, ref _motorOptions, value);
        }

        public FlightResult FlightResult
        {
            get => _flightResult;
            set
            {
                Set(() => FlightResult, ref _flightResult, value);

                RaiseNonSetPropertiesChanged();

                DirtyState = true;
            }
        }

        public bool DirtyState
        {
            get => _dirtyState;
            set => Set(() => DirtyState, ref _dirtyState, value);
        }

        public DateTime DateOfFlight
        {
            get => _dateOfFlight;
            set
            {
                Set(() => DateOfFlight, ref _dateOfFlight, value);
                DirtyState = true;
            }
        }

        public bool Nominal => FlightResult == FlightResult.Nominal;

        public bool Mishap => FlightResult == FlightResult.Mishap;

        public bool Catastrophe => FlightResult == FlightResult.Catastrophe;

        public string MotorName => _selectedMotor?.Name;

        public Motor Motor => _selectedMotor;

        public Guid FlightId => _flightId;

        #endregion Public Properties
    }
}