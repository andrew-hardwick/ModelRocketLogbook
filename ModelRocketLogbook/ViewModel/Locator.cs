using CommonServiceLocator;
using ModelRocketLogbook.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;

namespace ModelRocketLogbook.ViewModel
{
public    class Locator
    {
        public Locator()
        {
            var container = new UnityContainer();

            container.RegisterType<MainViewModel>(new ContainerControlledLifetimeManager());
            container.RegisterType<RocketsViewModel>(new ContainerControlledLifetimeManager());
            container.RegisterType<MotorsViewModel>(new ContainerControlledLifetimeManager());
            container.RegisterType<FlightsViewModel>(new ContainerControlledLifetimeManager());

            container.RegisterType<DataManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<DiskOperations>(new ContainerControlledLifetimeManager());

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
    }
}
