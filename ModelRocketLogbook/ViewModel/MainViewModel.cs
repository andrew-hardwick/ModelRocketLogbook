using GalaSoft.MvvmLight;
using System;

namespace ModelRocketLogbook.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private int _selectedTabIndex = 0;

        public MainViewModel(
            RocketsViewModel rocketsViewModel,
            MotorsViewModel motorsViewModel,
            FlightsViewModel flightsViewmodel)
        {
            RocketsViewModel = rocketsViewModel;
            MotorsViewModel = motorsViewModel;
            FlightsViewModel = flightsViewmodel;

            rocketsViewModel.OnFlightSelected += HandleFlightSelected;
        }

        /// <summary>
        /// This method handles an action from elsewhere that a flight has
        /// been selected and we should show the flights view
        /// </summary>
        /// <param name="obj"></param>
        private void HandleFlightSelected(Guid obj)
        {
            SelectedTabIndex = 2;
        }

        public  RocketsViewModel RocketsViewModel { get; private set; }

        public  MotorsViewModel MotorsViewModel { get; private set; }

        public  FlightsViewModel FlightsViewModel { get; private set; }

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => Set(() => SelectedTabIndex, ref _selectedTabIndex, value);
        }
    }
}