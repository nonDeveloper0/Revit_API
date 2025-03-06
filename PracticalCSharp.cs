using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivNonDev.Util
{
    public static class UnitConverter
    {
        public const double FEET_TO_METER_CONVERSION_FACTOR = 0.3048;
        public const double METER_TO_FEET_CONVERSION_FACTOR = 1 / 0.3048;
        public const double RADIAN_TO_DEGREE_CONVERSION_FACTOR = 180 / Math.PI;
        public const double DEGREE_TO_RADIAN_CONVERSION_FACTOR = Math.PI / 180;

        public static double ConvertFeetToMeter(double feet)
        {
            return feet * FEET_TO_METER_CONVERSION_FACTOR; 
        }
        public static double ConvertMeterToFeet(double meter)
        {
            return meter * METER_TO_FEET_CONVERSION_FACTOR;
        }
        public static double ConvertRadianToDegree(double radian)
        {
            return radian * RADIAN_TO_DEGREE_CONVERSION_FACTOR;
        }
        public static double ConvertDegreeToRadian(double degree)
        {
            return degree * DEGREE_TO_RADIAN_CONVERSION_FACTOR;
        }
    }
}
