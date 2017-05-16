using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tdmts.Obd2.BL
{
    public class ParameterID
    {
        private int pid;
        private int databytesReturned;

        private string description;

        private bool supported;

        private double minValue;
        private double maxValue;
        private string unit;

        private List<byte> machineValue;
        private string humanValue;
        private Formula.FormulaDelegate formula;
        
        public ParameterID(int pid, int databytesReturned, string description, double minValue, double maxValue, string unit, Formula.FormulaDelegate formula)
        {
            Pid = pid;
            DatabytesReturned = databytesReturned;
            Description = description;
            MinValue = minValue;
            MaxValue = maxValue;
            Unit = unit;
            Formula = formula;
            Supported = true;
        }

        public double Calculate()
        {
            double result = 0;

            if(Machinevalue.Count == 0)
            {
                result = Formula(0, 0, 0, 0);
            }
            else if(Machinevalue.Count == 1)
            {
                result = Formula(Machinevalue[0], 0, 0, 0);
            }
            else if(Machinevalue.Count == 2)
            {
                result = Formula(Machinevalue[0], Machinevalue[1], 0, 0);
            }
            else if(Machinevalue.Count == 3)
            {
                result = Formula(Machinevalue[0], Machinevalue[1], Machinevalue[2], 0);
            }
            else if (Machinevalue.Count == 4)
            {
                result = Formula(Machinevalue[0], Machinevalue[1], Machinevalue[2], Machinevalue[3]);
            }
            else
            {
                //Do nothing
            }

            HumanValue = result.ToString();

            return result;
        }


        public int Pid
        {
            get
            {
                return pid;
            }

            set
            {
                pid = value;
            }
        }

        public int DatabytesReturned
        {
            get
            {
                return databytesReturned;
            }

            set
            {
                databytesReturned = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        public bool Supported
        {
            get
            {
                return supported;
            }

            set
            {
                supported = value;
            }
        }

        public double MinValue
        {
            get
            {
                return minValue;
            }

            set
            {
                minValue = value;
            }
        }

        public double MaxValue
        {
            get
            {
                return maxValue;
            }

            set
            {
                maxValue = value;
            }
        }

        public string Unit
        {
            get
            {
                return unit;
            }

            set
            {
                unit = value;
            }
        }

        public List<byte> Machinevalue
        {
            get
            {
                return machineValue;
            }

            set
            {
                machineValue = value;
            }
        }

        public string HumanValue
        {
            get
            {
                return humanValue;
            }

            set
            {
                humanValue = value;
            }
        }

        public Formula.FormulaDelegate Formula
        {
            get
            {
                return formula;
            }

            set
            {
                formula = value;
            }
        }
    }
}
