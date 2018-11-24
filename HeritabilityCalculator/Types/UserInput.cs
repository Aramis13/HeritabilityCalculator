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
        public TraitValue[] TraitValues;
        //public Dictionary<string, object> TraitValues;
    }

    public class TraitValue
    {
        public string Name;
        public object value;
    }
}
