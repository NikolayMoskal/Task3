using System.Collections.Generic;

namespace Task3.Billings
{
    public class TariffPlan
    {
        private readonly List<double> _discounts;
        private readonly List<int> _discountSeconds;
        private readonly double _priceForMinute;
        private int _freeSeconds;
        public string Name { get; }
        
        public TariffPlan(string name)
        {
            Name = name;
            _priceForMinute = 0.07;
            _freeSeconds = 30 * 60;
            _discounts = new List<double> {0.1, 0.05};
            _discountSeconds = new List<int> {20 * 60, 5 * 60};
        }

        public double Calculate(int seconds)
        {
            double result = 0;

            // снятие бесплатных минут с тарифа
            if (_freeSeconds > 0)
            {
                if (_freeSeconds >= seconds)
                {
                    _freeSeconds -= seconds;
                    return 0;
                }

                seconds -= _freeSeconds;

                if (seconds == 0) return result;
            }

            // как закончились бесплатные, то поочередное снятие скидочных минут с тарифа
            for (var index = 0; index < _discountSeconds.Count; index++)
            {
                if (_discountSeconds[index] > 0)
                {
                    if (_discountSeconds[index] >= seconds)
                    {
                        _discountSeconds[index] -= seconds;
                        result += seconds * _priceForMinute / 60 * _discounts[index];
                        return result;
                    }

                    seconds -= _discountSeconds[index];
                    result += _discountSeconds[index] * _priceForMinute / 60 * _discounts[index];

                    if (seconds == 0)
                    {
                        return result;
                    }
                }
            }

            // а как закончилось все, считаем по обычному тарифу
            result += seconds * _priceForMinute / 60;

            return result;
        }
    }
}