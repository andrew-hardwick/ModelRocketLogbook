using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelRocketLogbook.Model
{
    public class Rocket
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public bool Active { get; set; } = true;
        public MotorMount Mount { get; set; } = MotorMount.None;
        public List<Flight> Flights { get; set; } = new List<Flight>();
        public double AverageDryWeight => CalculateAverageDryWeight();
        public double AverageFlightWeight => CalculateAverageFlightWeight();
        public double TotalLifetimeImpulse => CalculateTotalLifetimeImpulse();

        //TODO: These two function should be able to be combined with some sort of delegate to handle the specific value we're averaging.
        private double CalculateAverageDryWeight()
        {
            var flightCount = Flights.Count();

            if (flightCount > 0)
            {
                return Flights.Sum(f => f.DryWeight) / flightCount;
            }
            else
            {
                return 0d;
            }
        }

        private double CalculateAverageFlightWeight()
        {
            var flightCount = Flights.Count();

            if (flightCount > 0)
            {
                return Flights.Sum(f => f.FlightWeight) / flightCount;
            }
            else
            {
                return 0d;
            }
        }

        private double CalculateTotalLifetimeImpulse()
        {
            var flightCount = Flights.Where(f => f.Motor != null).Count();

            if (flightCount > 0)
            {
                return Flights.Where(f => f.Motor != null).Sum(f => f.Motor.TotalImpulse);
            }
            else
            {
                return 0;
            }
        }
    }
}