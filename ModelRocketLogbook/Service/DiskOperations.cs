using ModelRocketLogbook.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModelRocketLogbook.Service
{
    /*************************************************
     * For now this entire class is debug values
     *************************************************/

    public class DiskOperations
    {
        private string _baseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ModelRocketLogbook");

        public HashSet<Motor> Motors { get; private set; } = new HashSet<Motor>();
        public HashSet<Rocket> Rockets { get; private set; } = new HashSet<Rocket>();

        public DiskOperations()
        {
            Directory.CreateDirectory(_baseFolder);

            var motorsPath = Path.Combine(_baseFolder, "motors");
            Directory.CreateDirectory(motorsPath);

            var rocketsPath = Path.Combine(_baseFolder, "rockets");
            Directory.CreateDirectory(rocketsPath);

            LoadMotors();
            LoadRockets();
        }

        public void SaveMotor(
            Motor motor)
        {
            Motors.Add(motor);

            var motorsPath = Path.Combine(_baseFolder, "motors");

            var filename = Path.Combine(motorsPath, motor.Id.ToString());

            File.WriteAllText(filename, JsonConvert.SerializeObject(motor));
        }

        public void SaveRocket(
            Rocket rocket)
        {
            Rockets.Add(rocket);

            var rocketsPath = Path.Combine(_baseFolder, "rockets");

            var filename = Path.Combine(rocketsPath, rocket.Id.ToString());

            File.WriteAllText(filename, JsonConvert.SerializeObject(rocket));
        }

        private void LoadMotors()
        {
            var motorsPath = Path.Combine(_baseFolder, "motors");

            var files = Directory.EnumerateFiles(motorsPath);

            foreach (var file in files)
            {
                try
                {
                    var motor = JsonConvert.DeserializeObject<Motor>(File.ReadAllText(file));

                    Motors.Add(motor);
                }
                catch
                {
                    //do nothing
                }
            }
        }

        private void LoadRockets()
        {
            var rocketsPath = Path.Combine(_baseFolder, "rockets");

            var files = Directory.EnumerateFiles(rocketsPath);

            foreach (var file in files)
            {
                try
                {
                    var rocket = JsonConvert.DeserializeObject<Rocket>(File.ReadAllText(file));

                    for (int i = 0; i < rocket.Flights.Count(); i++)
                    {
                        rocket.Flights[i].Motor = Motors.FirstOrDefault(m => m.Id.Equals(rocket.Flights[i].MotorId));
                        rocket.Flights[i].RocketId = rocket.Id;
                    }

                    Rockets.Add(rocket);
                }
                catch
                {
                    //do nothing
                }
            }
        }
    }
}