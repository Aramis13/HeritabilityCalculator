using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritabilityCalculator
{
    class TotalVariance
    {
        public double n;
        public Dictionary<string, int> NumOfInstances = new Dictionary<string, int>();
        public Dictionary<string, double> Pc = new Dictionary<string, double>();
        


        public TotalVariance(Dictionary<string, object> traitValues)
        {
            this.n = traitValues.Count;
            CalculateNumOfInstances(traitValues);
        }

        public bool CalculateNumOfInstances(Dictionary<string, object> traitValues)
        {
            foreach (string value in traitValues.Values)
            {
                if (NumOfInstances.ContainsKey(value))
                    NumOfInstances[value]++;
                else
                    NumOfInstances.Add(value, 1);
                
            }
            CalculatePC();
            return true;
        } 

        public bool CalculatePC()
        {

            foreach(string key in NumOfInstances.Keys)
            {
                double val = NumOfInstances[key] / n;
                Pc.Add(key, val);
                Console.WriteLine(key + ": " + val);
            }
            return true;
        }
    }
}
