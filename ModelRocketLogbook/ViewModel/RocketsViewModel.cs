using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ModelRocketLogbook.Service;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ModelRocketLogbook.ViewModel
{
    public class RocketsViewModel : ViewModelBase
    {
        private readonly DataManager _dataManager;

        private ObservableCollection<RocketDetailViewModel> _rockets =
            new ObservableCollection<RocketDetailViewModel>();

        private ObservableCollection<string> _sortOptions =
            new ObservableCollection<string>(new string[] { "Sort by: Active", "Sort by: Name", "Sort by: Motor Mount" });

        private RocketDetailViewModel _selectedRocket;

        private int _sortSelectedIndex = 0;

        private RelayCommand _addNewRocket;

        public RocketsViewModel(
            DataManager dataManager)
        {
            _dataManager = dataManager;

            _dataManager.OnRocketCollectionChanged += HandleRocketCollectionChanged;

            HandleRocketCollectionChanged();
        }

        public event Action<Guid> OnFlightSelected;

        private void HandleRocketCollectionChanged()
        {

            Rockets = _dataManager.GetRockets()
                                  .Select(r => new RocketDetailViewModel(_dataManager, r.Id))
                                  .OrderBy(r => r.Inactive)
                                  .ToObservableCollection();

            for (int i = 0; i < Rockets.Count(); i++)
            {
                Rockets[i].OnFlightSelected += HandleChildFlightSelected;
            }

            if (Rockets.Count() > 0)
            {
                SelectedRocket = Rockets[0];
            }
        }

        private void HandleChildFlightSelected(
            Guid id)
        {
            OnFlightSelected?.Invoke(id);
        }

        public RelayCommand AddNewRocket => _addNewRocket ?? (_addNewRocket = new RelayCommand(() =>
        {
            var id = _dataManager.CreateRocket();

            SelectedRocket = Rockets.First(r => r.Id.Equals(id));
        }));

        public ObservableCollection<RocketDetailViewModel> Rockets
        {
            get => _rockets;
            set => Set(() => Rockets, ref _rockets, value);
        }

        public ObservableCollection<string> SortOptions
        {
            get => _sortOptions;
            set => Set(() => SortOptions, ref _sortOptions, value);
        }

        public RocketDetailViewModel SelectedRocket
        {
            get => _selectedRocket;
            set => Set(() => SelectedRocket, ref _selectedRocket, value);
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

                        Rockets = new ObservableCollection<RocketDetailViewModel>(Rockets.OrderBy(r => r.Inactive));
                        break;

                    case 1:

                        Rockets = new ObservableCollection<RocketDetailViewModel>(Rockets.OrderBy(r => r.Name));
                        break;

                    case 2:

                        Rockets = new ObservableCollection<RocketDetailViewModel>(Rockets.OrderBy(r => r.EnumMount));
                        break;
                }
            }
        }
    }
}