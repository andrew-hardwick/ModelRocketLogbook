using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ModelRocketLogbook.Service;
using System.Collections.ObjectModel;
using System.Linq;

namespace ModelRocketLogbook.ViewModel
{
    public class MotorsViewModel : ViewModelBase
    {
        #region Private Variables

        private readonly DataManager _dataManager;

        private ObservableCollection<MotorDetailViewModel> _motors =
            new ObservableCollection<MotorDetailViewModel>();

        private MotorDetailViewModel _selectedMotor;

        private ObservableCollection<string> _sortOptions =
            new ObservableCollection<string>(new string[] { "Sort by: Manufacturer", "Sort by: Name", "Sort by: Motor Mount" });

        private int _sortSelectedIndex = 0;

        private RelayCommand _addNewMotor;

        #endregion Private Variables

        #region Constructor

        public MotorsViewModel(
            DataManager dataManager)
        {
            _dataManager = dataManager;

            _dataManager.OnMotorCollectionChanged += HandleMotorCollectionChanged;

            HandleMotorCollectionChanged();
        }

        #endregion Constructor

        #region Commands

        public RelayCommand AddNewMotor => _addNewMotor ?? (_addNewMotor = new RelayCommand(() =>
        {
            var id = _dataManager.CreateMotor();

            SelectedMotor = Motors.First(m => m.Id.Equals(id));
        }));

        #endregion Commands

        #region Private Methods

        private void HandleMotorCollectionChanged()
        {
            Motors = _dataManager.GetMotorIds()
                                  .Select(id => new MotorDetailViewModel(_dataManager, id))
                                  .OrderBy(m => m.Name)
                                  .ToObservableCollection();

            SelectedMotor = _motors[0];
        }

        #endregion Private Methods

        #region Public Properties

        public ObservableCollection<MotorDetailViewModel> Motors
        {
            get => _motors;
            set => Set(() => Motors, ref _motors, value);
        }

        public MotorDetailViewModel SelectedMotor
        {
            get => _selectedMotor;
            set => Set(() => SelectedMotor, ref _selectedMotor, value);
        }

        public ObservableCollection<string> SortOptions
        {
            get => _sortOptions;
            set => Set(() => SortOptions, ref _sortOptions, value);
        }

        public int SortSelectedIndex
        {
            get => _sortSelectedIndex;
            set
            {
                switch (value)
                {
                    case 0:
                    default:

                        Motors = Motors.OrderBy(m => m.Manufacturer).ToObservableCollection();
                        break;

                    case 1:

                        Motors = Motors.OrderBy(m => m.Name).ToObservableCollection();
                        break;

                    case 2:

                        Motors = Motors.OrderBy(m => m.EnumMount).ToObservableCollection();
                        break;
                }

                Set(() => SortSelectedIndex, ref _sortSelectedIndex, value);
            }
        }

        #endregion Public Properties
    }
}