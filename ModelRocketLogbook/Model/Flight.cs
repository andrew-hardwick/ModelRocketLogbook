using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRocketLogbook.Model
{
    public class Flight
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public FlightResult FlightResult { get; set; } = FlightResult.Nominal;
        public DateTime DateOfFlight { get; set; } = DateTime.Now;
        public Guid MotorId { get; set; }
        public double DryWeight { get; set; } = 0.0;
        public double FlightWeight { get; set; } = 0.0;
        public int AdjustedDelay { get; set; }
        public double Apogee { get; set; }
        public string Notes { get; set; }

        [JsonIgnore]
        public Motor Motor { get; set; } = new Motor();
        [JsonIgnore]
        public Guid RocketId { get; set; } = Guid.NewGuid();
    }
}
