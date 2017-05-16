using System.Collections.Generic;

namespace Tdmts.Obd2.BL
{
    public class Formula
    {

        public delegate double FormulaDelegate(int a, int b, int c, int d);

        public static double Blank(int a, int b, int c, int d)
        {
            return 0;
        }

        public static double Speed(int a, int b, int c, int d)
        {
            return a;
        }

        public static double Rpm(int a, int b, int c, int d)
        {
            return (256 * a + b) / 4;
        }
    }
}
