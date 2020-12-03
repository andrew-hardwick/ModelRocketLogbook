using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ModelRocketLogbook.Model;
using ModelRocketLogbook.Service;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ModelRocketLogbook.ViewModel
{
    public class RocketDetailViewModel : ViewModelBase
    {
        #region Private Variables

        private readonly DataManager _dataManager;
        private readonly Guid _rocketId;

        private bool _active = true;

        private string _name = string.Empty;
        private MotorMount _mount = MotorMount.None;
        private Rocket _rocket;

        private ObservableCollection<string> _mountOptions =
            ((MotorMount[])Enum.GetValues(typeof(MotorMount))).Select(v => v.ToFormattedString()).ToObservableCollection();

        private ObservableCollection<FlightDetailViewModel> _flights =
            new ObservableCollection<FlightDetailViewModel>();

        private FlightDetailViewModel _selectedFlight;

        private int _selectedMountIndex = 0;

        private RelayCommand _selectFlight;
        private RelayCommand _addNewFlight;
        private RelayCommand _save;

        private bool _dirtyState = false;

        #endregion Private Variables

        #region Events

        public event Action<Guid> OnFlightSelected;

        #endregion Events

        #region Constructor

        public RocketDetailViewModel(
            DataManager dataManager,
            Guid rocketId)
        {
            _dataManager = dataManager;

            _dataManager.OnRocketChanged += HandleRocketChanged;
            _dataManager.OnFlightChanged += HandleFlightChanged;

            _rocketId = rocketId;

            SetValuesFromRocket();
        }

        #endregion Constructor

        #region Private Methods

        private void HandleRocketChanged(
            Guid id)
        {
            if (_rocketId.Equals(id))
            {
                SetValuesFromRocket();
            }
        }

        private void HandleFlightChanged(
            Guid id)
        {
            if (_rocket.Flights.Any(f => f.Id.Equals(id)))
            {
                SetValuesFromRocket();
            }
        }

        private void SetValuesFromRocket()
        {
            _rocket = _dataManager.GetRocket(_rocketId);

            Name = _rocket.Name;
            _mount = _rocket.Mount;

            SelectedMountIndex = _mountOptions.IndexOf(_mount.ToFormattedString());

            RaisePropertyChanged(() => Mount);

            Active = _rocket.Active;
            Flights = new ObservableCollection<FlightDetailViewModel>(_rocket.Flights.Select(f => new FlightDetailViewModel(_dataManager, f.Id)));
            DirtyState = false;

            if (_flights.Count() > 0)
            {
                _selectedFlight = _flights[0];
            }

            RaiseNonsetPropertiesChanged();
        }

        private void RaiseNonsetPropertiesChanged()
        {
            RaisePropertyChanged(() => Inactive);
            RaisePropertyChanged(() => AverageDryWeight);
            RaisePropertyChanged(() => AverageFlightWeight);
            RaisePropertyChanged(() => TotalLifetimeImpulse);
        }

        #endregion Private Methods

        #region Commands

        public RelayCommand SelectFlight => _selectFlight ?? (_selectFlight = new RelayCommand(() =>
        {
            OnFlightSelected?.Invoke(SelectedFlight.FlightId);
        }));

        public RelayCommand AddNewFlight => _addNewFlight ?? (_addNewFlight = new RelayCommand(() =>
        {
            var flight = _dataManager.CreateFlight(ref _rocket);

            DirtyState = true;

            OnFlightSelected?.Invoke(flight);
        }));

        public RelayCommand Save => _save ?? (_save = new RelayCommand(() =>
        {
            DirtyState = false;

            _dataManager.SaveRocket(
                _rocketId,
                Name,
                Active,
                EnumMount);
        }));

        #endregion Commands

        #region Public Properties

        public Guid Id => _rocketId;

        public bool Active
        {
            get => _active;
            set
            {
                DirtyState = true;

                Set(() => Active, ref _active, value);

                RaiseNonsetPropertiesChanged();
            }
        }

        public bool Inactive => !Active;

        public string Name
        {
            get => _name;
            set
            {
                DirtyState = true;
                Set(() => Name, ref _name, value);
            }
        }

        public ObservableCollection<FlightDetailViewModel> Flights
        {
            get => _flights;
            set => Set(() => Flights, ref _flights, value);
        }

        public string Mount
        {
            get => _mount.ToFormattedString();
            set
            {
                _mount = value.ToMotorMount();

                RaisePropertyChanged(() => Mount);
            }
        }

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

        public MotorMount EnumMount => _mount;

        public double AverageDryWeight => _rocket.AverageDryWeight;

        public double AverageFlightWeight => _rocket.AverageFlightWeight;

        public double TotalLifetimeImpulse => _rocket.TotalLifetimeImpulse;

        public FlightDetailViewModel SelectedFlight
        {
            get => _selectedFlight;
            set => Set(() => SelectedFlight, ref _selectedFlight, value);
        }

        public bool DirtyState
        {
            get => _dirtyState;
            set => Set(() => DirtyState, ref _dirtyState, value);
        }

        #endregion Public Properties
    }
}