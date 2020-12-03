using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelRocketLogbook.Model
{
    public class Motor
    {
        public string Name { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Propellant { get; set; } = string.Empty;
        public int DefaultDelay { get; set; } = 0;
        public double MaxThrust { get; set; } = 0;
        public double AverageThrust { get; set; } = 0;
        public double TotalImpulse { get; set; } = 0;
        public MotorMount Mount { get; set; } = MotorMount.None;

        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
