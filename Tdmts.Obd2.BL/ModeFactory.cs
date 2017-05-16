using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tdmts.Obd2.BL
{
    public class ModeFactory
    {
        public static Mode createMode1()
        {
            try
            {
                Mode mode1 = new Mode(1);

                ParameterID m01pid00 = new ParameterID(0x00, 4, "Reset", 0, 65535, "", Formula.Blank);
                ParameterID m01pid0C = new ParameterID(0x0C, 2, "Engine RPM", 0, 16383.75, "rpm", Formula.Rpm);
                ParameterID m01pid0D = new ParameterID(0x0D, 1, "Vehicle speed", 0, 255, "km/h", Formula.Speed);

                mode1.ParameterIDs.Add(m01pid00);
                mode1.ParameterIDs.Add(m01pid0C);
                mode1.ParameterIDs.Add(m01pid0D);

                return mode1;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
