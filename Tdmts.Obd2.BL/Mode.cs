using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tdmts.Obd2.BL
{
    public class Mode
    {
        private int id;

        List<ParameterID> parameterIDs = new List<ParameterID>();

        public Mode(int id)
        {
            Id = id;
        }

        public bool IsSupported(int pid)
        {
            foreach(ParameterID parameterId in parameterIDs)
            {
                if(parameterId.Pid == pid)
                {
                    if(parameterId.Supported == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public bool IsSupported(string description)
        {
            foreach (ParameterID parameterId in parameterIDs)
            {
                if (parameterId.Description.Equals(description, StringComparison.OrdinalIgnoreCase))
                {
                    if (parameterId.Supported == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public ParameterID Get(int pid)
        {
            try
            {
                return parameterIDs.Where(x => x.Pid == pid).First();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ParameterID Get(string description)
        {
            try
            {
                return parameterIDs.Where(x => x.Description.Equals(description, StringComparison.OrdinalIgnoreCase)).First();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public List<ParameterID> ParameterIDs
        {
            get
            {
                return parameterIDs;
            }

            set
            {
                parameterIDs = value;
            }
        }
    }
}
