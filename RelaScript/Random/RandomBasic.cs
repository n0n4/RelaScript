using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript
{
    public class RandomBasic : IRandomProvider
    {
        public Random Random = null;

        public RandomBasic()
        {
            Random = new Random();
        }

        public RandomBasic(int seed)
        {
            Random = new Random(seed);
        }

        public void SetSeed(int seed)
        {
            Random = new Random(seed);
        }

        public double RandomDouble(double min, double max)
        {
            return Random.NextDouble() * (max - min) + min;
        }

        public int RandomInt(int min, int max)
        {
            return Random.Next(min, max + 1);
        }
    }
}
