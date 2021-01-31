using System;

namespace AOPSamples
{
    public static class SimpleLib
    {
        public static string CurrentDate => $"The time now is {DateTime.Now} (output of DateTime.Now)";
    }
}
