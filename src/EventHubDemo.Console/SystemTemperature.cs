using System;

namespace EventHubDemo.Console
{
    public class SystemTemperature
    {
        private readonly Random rnd = new Random();

        public double GetTemperature()
        {
            return rnd.NextDouble()*20;
        }
    }
}