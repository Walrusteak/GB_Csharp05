using System;
using System.Collections.Generic;
using System.Linq;

namespace Homework01
{
    public class ValuesHolder
    {
        private List<WeatherForecast> _list;

        public List<WeatherForecast> Values => _list;

        public ValuesHolder()
        {
            _list = new List<WeatherForecast>();
        }

        public void Add(WeatherForecast weatherForecast) => _list.Add(weatherForecast);
    }
}
