using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritabilityCalculator
{
    public class FinishedEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public FinishedEventArgs(string msg)
        {
            Message = msg;
        }
    }

    public abstract class Variance
    {
        #region Fields

        public UserInput userData;
        public event EventHandler<FinishedEventArgs> Finished;

        #endregion Fields

        public Variance(UserInput userinput)
        {
            userData = userinput;
            Finished += Variance_Finished;
        }

        /// <summary>
        /// Write finished message to log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Variance_Finished(object sender, FinishedEventArgs e)
        {
            if (!(sender is HeritabilityCalculator))
                return;
            HeritabilityCalculator form = sender as HeritabilityCalculator;
            form.WriteToLog(e.Message, HeritabilityCalculator.MessageType.Important);
        }

        protected virtual void RaiseFinished(object sender, FinishedEventArgs e)
        {
            if (Finished != null)
                Finished(sender, e);
        }

        /// <summary>
        /// Calculate distribution of each phenotypic value
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,double> GetPC(Dictionary<string,int> NumOfInstances)
        {
            if (!userData.Validate())
                return null;
            Dictionary<string, double> Pc = new Dictionary<string, double>();
            foreach (string key in NumOfInstances.Keys)
            {
                double val = NumOfInstances[key] / (double)userData.N;
                Pc.Add(key, val);
            }
            return Pc;
        }

        public double GetVariance(Dictionary<string, double> Pc)
        {
            List<double> Vis = new List<double>();
            for (int i = 0; i < Pc.Count; i++)
            {
                double V = 0;
                for (int j = 0; j < userData.Traits.Length; j++)
                {
                    V += GetFerchetDistance(userData.DistanceMatrix[i, j], Pc.ElementAt(i).Value);
                }
                Vis.Add(V);
            }
            return Vis.Min();
        }

        /// <summary>
        /// Calcualte how many instances of each phenotype are exist
        /// </summary>
        /// <param name="traitValues"></param>
        /// <returns></returns>
        public Dictionary<string,int> GetNumOfInstances(TraitValue[] traitValues)
        {
            if (traitValues == null)
                return null;
            Dictionary<string, int> NumOfInstances = new Dictionary<string, int>();
            foreach (TraitValue data in traitValues)
            {
                if (NumOfInstances.ContainsKey(data.value))
                    NumOfInstances[data.value]++;
                else
                    NumOfInstances.Add(data.value, 1);
            }
            return NumOfInstances;
        }

        /// <summary>
        /// Gets variance using ferchet distance
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="Pc"></param>
        /// <returns></returns>
        public double GetFerchetDistance(double distance, double Pc)
        {
            return Math.Pow(distance, 2) * Pc;
        }

        public virtual void Calculate(object Main)
        {
            return;
        }
    }
}
