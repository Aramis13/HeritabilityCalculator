using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritabilityCalculator
{
     public class UserInput
    {
        public double[,] DistanceMatrix;
        public double[,] EmissionMatrix;
        public TraitValue[] ObservedTraits;
        public string[] Traits;
        public int N
        {
            get
            {
                if (ObservedTraits != null)
                    return ObservedTraits.Length;
                else
                    return 0;
            }
        }

        public bool Validate()
        {
            return DistanceMatrix != null && EmissionMatrix != null && ObservedTraits != null
                && Traits != null;
        }
    }

    public class TraitValue
    {
        public string Name = string.Empty;
        public string value;
    }
}
