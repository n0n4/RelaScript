using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript
{
    public interface IRandomProvider
    {
        void SetSeed(int val);
        double RandomDouble(double min, double max);
        int RandomInt(int min, int max);
    }
}
