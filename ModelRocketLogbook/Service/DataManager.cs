using ModelRocketLogbook.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelRocketLogbook.Service
{
    public class DataManager
    {
        private readonly DiskOperations _diskOperations;

        private HashSet<Rocket> _rockets = new HashSet<Rocket>();
        private HashSet<Motor> _motors = new HashSet<Motor>();
        private HashSet<Flight> _flights = new HashSet<Flight>();

        public event Action<Guid> OnRocketChanged;

        public event Action<Guid> OnMotorChanged;

        public event Action OnRocketCollectionChanged;

        public event Action OnMotorCollectionChanged;

        public event Action OnFlightCollectionChanged;

        public DataManager(
            DiskOperations diskOperations)
        {
            _diskOperations = diskOperations;

            _rockets = _diskOperations.Rockets;

            _motors = _diskOperations.Motors;

            ResetFlightSet();
        }

        private void ResetFlightSet()
        {
            _flights = _rockets.SelectMany(r => r.Flights).ToHashSet();

            OnFlightCollectionChanged?.Invoke();
        }

        public Guid CreateRocket()
        {
            var rocket = new Rocket();

            _rockets.Add(rocket);

            OnRocketCollectionChanged?.Invoke();

            return rocket.Id;
        }

        public Guid CreateMotor()
        {
            var motor = new Motor();

            _motors.Add(motor);

            OnMotorCollectionChanged?.Invoke();

            return motor.Id;
        }

        public Guid CreateFlight(
            ref Rocket rocket)
        {
            var flight = new Flight();

            flight.RocketId = rocket.Id;

            var currentFlights = rocket.Flights.ToList();

            currentFlights.Add(flight);

            rocket.Flights = currentFlights;

            _flights.Add(flight);

            OnRocketChanged?.Invoke(rocket.Id);
            OnFlightCollectionChanged?.Invoke();

            return flight.Id;
        }

        internal void SaveRocket(
            Guid id,
            string name,
            bool active,
            MotorMount mount)
        {
            var flights = _rockets.First(r => r.Id.Equals(id)).Flights;

            _rockets = _rockets.Where(r => !r.Id.Equals(id)).ToHashSet();

            var rocket = new Rocket
            {
                Id = id,
                Active = active,
                Name = name,
                Mount = mount,
                Flights = flights
            };

            _diskOperations.SaveRocket(rocket);

            _rockets.Add(rocket);

            OnRocketChanged?.Invoke(id);
        }

        private void SaveRocket(
            Rocket rocket)
        {
            _rockets = _rockets.Where(r => !r.Id.Equals(rocket.Id)).ToHashSet();

            _diskOperations.SaveRocket(rocket);

            _rockets.Add(rocket);

            OnRocketChanged?.Invoke(rocket.Id);
        }

        internal void SaveFlight(
            Guid id,
            FlightResult flightResult,
            DateTime dateOfFlight,
            Motor selectedMotor,
            int adjustedDelay,
            double dryWeight,
            double flightWeight,
            double apogee,
            string notes)
        {
            var rocket = _rockets.First(r => r.Id.Equals(_flights.First(f => f.Id.Equals(id)).RocketId));

            _flights = rocket.Flights.Where(f => !f.Id.Equals(id)).ToHashSet();

            var flight = new Flight
            {
                Id = id,
                FlightResult = flightResult,
                DateOfFlight = dateOfFlight,
                Motor = selectedMotor,
                MotorId = selectedMotor.Id,
                AdjustedDelay = adjustedDelay,
                DryWeight = dryWeight,
                FlightWeight = flightWeight,
                Apogee = apogee,
                Notes = notes,
                RocketId = rocket.Id
            };

            _flights.Add(flight);

            rocket.Flights = _flights.ToList();

            SaveRocket(rocket);

            ResetFlightSet();
        }

        internal void SaveMotor(
            Guid id,
            string name,
            string manufacturer,
            string propellant,
            MotorMount mount,
            int defaultDelay,
            double maxThrust,
            double averageThrust,
            double totalImpulse)
        {
            _motors = _motors.Where(m => !m.Id.Equals(id)).ToHashSet();

            var motor = new Motor
            {
                Id = id,
                Name = name,
                Manufacturer = manufacturer,
                Propellant = propellant,
                Mount = mount,
                DefaultDelay = defaultDelay,
                MaxThrust = maxThrust,
                AverageThrust = averageThrust,
                TotalImpulse = totalImpulse
            };

            _diskOperations.SaveMotor(motor);

            _motors.Add(motor);

            OnMotorChanged?.Invoke(id);
        }

        //TODO: There should be a generic(non repetitive) way to do this
        public Rocket GetRocket(Guid rocketId) =>
            _rockets.First(r => r.Id.Equals(rocketId));

        public IEnumerable<Rocket> GetRockets() =>
            _rockets;

        public IEnumerable<Guid> GetRocketIds() =>
            _rockets.Select(r => r.Id);

        public Flight GetFlight(Guid flightId) =>
            _flights.First(f => f.Id.Equals(flightId));

        public IEnumerable<Flight> GetFlights() =>
            _flights;

        public IEnumerable<Guid> GetFlightIds() =>
            _flights.Select(f => f.Id);

        public Motor GetMotor(Guid motorId) =>
            _motors.First(m => m.Id.Equals(motorId));

        public IEnumerable<Motor> GetMotors() =>
            _motors;

        public IEnumerable<Guid> GetMotorIds() =>
            _motors.Select(m => m.Id);
    }
}