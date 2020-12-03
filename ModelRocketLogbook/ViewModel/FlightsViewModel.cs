using GalaSoft.MvvmLight;
using ModelRocketLogbook.Service;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ModelRocketLogbook.ViewModel
{
    public class FlightsViewModel : ViewModelBase
    {
        private readonly DataManager _dataManager;

        private ObservableCollection<FlightDetailViewModel> _flights =
            new ObservableCollection<FlightDetailViewModel>();

        private ObservableCollection<string> _sortOptions =
            new ObservableCollection<string>(new string[] { "Sort by: Result", "Sort by: Rocket", "Sort by: Motor" });

        private FlightDetailViewModel _selectedFlight;

        private int _sortSelectedIndex = 0;

        public FlightsViewModel(
            DataManager dataManager,
            RocketsViewModel rocketsViewModel)
        {
            _dataManager = dataManager;

            _dataManager.OnFlightCollectionChanged += HandleFlightCollectionChanged;

            rocketsViewModel.OnFlightSelected += HandleFlightSelected;

            HandleFlightCollectionChanged();
        }

        #region Private Methods

        private void HandleFlightSelected(Guid id)
        {
            SelectedFlight = Flights.First(f => f.FlightId == id);
        }

        private void HandleFlightCollectionChanged()
        {
            Flights = _dataManager.GetFlightIds()
                                  .Select(i => new FlightDetailViewModel(_dataManager, i))
                                  .OrderBy(f => f.FlightResult)
                                  .ToObservableCollection();

            if (Flights.Count() > 0)
            {
                SelectedFlight = Flights[0];
            }
        }

        private void HandleFlightChanged(Guid obj)
        {
            //todo
        }

        #endregion Private Methods

        #region Public Properties

        public ObservableCollection<FlightDetailViewModel> Flights
        {
            get => _flights;
            set => Set(() => Flights, ref _flights, value);
        }

        public ObservableCollection<string> SortOptions
        {
            get => _sortOptions;
            set => Set(() => SortOptions, ref _sortOptions, value);
        }

        public FlightDetailViewModel SelectedFlight
        {
            get => _selectedFlight;
            set => Set(() => SelectedFlight, ref _selectedFlight, value);
        }

        public int SortSelectedIndex
        {
            get => _sortSelectedIndex;
            set
            {
                Set(() => SortSelectedIndex, ref _sortSelectedIndex, value);

                switch (value)
                {
                    case 0:
                    default:

                        Flights = Flights.OrderBy(f => f.FlightResult).ToObservableCollection();
                        break;

                    case 1:

                        Flights = Flights.OrderBy(f => f.RocketName).ToObservableCollection();
                        break;

                    case 2:

                        Flights = Flights.OrderBy(f => f.MotorName).ToObservableCollection();
                        break;
                }
            }
        }

        #endregion Public Properties
    }
}