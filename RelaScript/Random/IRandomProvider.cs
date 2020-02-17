using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript
{
    public interface IRandomProvider
    {
        double RandomDouble(double min, double max);
        int RandomInt(int min, int max);
    }
}
