using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritabilityCalculator
{
    class TotalVariance
    {
        #region Fields
        public double n, VT_Final_Result;
        public Dictionary<string, int> NumOfInstances = new Dictionary<string, int>();
        public Dictionary<string, double> Pc = new Dictionary<string, double>();
        double[,] DistanceMatrix;
        public string[] order;
        #endregion Fields

        /*  CONSTRUCTOR */
        public TotalVariance(Dictionary<string, object> traitValues, double[,] DistanceMatrix, string[] order)
        {
            this.n = traitValues.Count;
            this.DistanceMatrix = DistanceMatrix;
            this.order = order;

            CalcInstances(traitValues);
            CalculatePC();
            CalcTotalVariance();
        }

        /* Calculate Total Variance */
        public bool CalcTotalVariance()
        {
            List<double> Vis = new List<double>();
            double temp_vi = 0;
            string current_phenotype;

            for (int i = 0; i < DistanceMatrix.GetLength(0); i++)
            {
                current_phenotype = order[i];
                for (int j = 0; j < DistanceMatrix.GetLength(1); j++)
                    temp_vi += DistanceMatrix[i, j] * DistanceMatrix[i, j];
                temp_vi *= Pc[current_phenotype];
                Vis.Add(temp_vi);
                temp_vi = 0;
            }
            this.VT_Final_Result = Vis.Min();
            return true;
        }

        /* Calcualte how many instances of each phenotype are exist    */
        public bool CalcInstances(Dictionary<string, object> traitValues)
        {
            foreach (string value in traitValues.Values)
            {
                if (NumOfInstances.ContainsKey(value))
                    NumOfInstances[value]++;
                else
                    NumOfInstances.Add(value, 1);
            }
            
            return true;
        } 

        /* Calculate distribution of each phenotypic value  */
        public bool CalculatePC()
        {
            foreach (string key in NumOfInstances.Keys)
            {
                double val = NumOfInstances[key] / n;
                Pc.Add(key, val);
            }
            return true;
        }

        public double GetVT()
        {
            return this.VT_Final_Result;
        }

    }
}
