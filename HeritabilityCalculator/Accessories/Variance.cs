using System;
using System.Collections.Generic;
using System.Linq;

namespace HeritabilityCalculator
{
    /// <summary>
    /// Data for finished calculation event
    /// </summary>
    public class FinishedEventArgs : EventArgs
    {
        /// <summary>
        /// Message to be displayed in log
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// Flag to update progress bar
        /// </summary>
        public bool UpdateProgress { get; private set; }
        /// <summary>
        /// Create instance of finished event args
        /// </summary>
        /// <param name="msg">Message to be displayed in log</param>
        /// <param name="updateProgress">Flag to update progress bar</param>
        public FinishedEventArgs(string msg, bool updateProgress)
        {
            Message = msg;
            UpdateProgress = updateProgress;
        }
    }

    /// <summary>
    /// Contains variance data
    /// </summary>
    public abstract class Variance
    {
        #region Fields

        /// <summary>
        /// User input data
        /// </summary>
        public UserInput userData;
        /// <summary>
        /// Finished calculation event handler
        /// </summary>
        public event EventHandler<FinishedEventArgs> Finished;
        /// <summary>
        /// Iterations counter
        /// </summary>
        private int itr = 0;

        #endregion Fields

        /// <summary>
        /// Create new instance of variance
        /// </summary>
        /// <param name="userinput">User input data</param>
        public Variance(UserInput userinput)
        {
            userData = userinput;
            Finished += Variance_Finished;
        }

        /// <summary>
        /// Write finished message to log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Data for finished calculation event</param>
        private void Variance_Finished(object sender, FinishedEventArgs e)
        {
            if (!(sender is HeritabilityCalculator))
                return;
            HeritabilityCalculator form = sender as HeritabilityCalculator;
            form.WriteToLog(e.Message + (itr++), HeritabilityCalculator.MessageType.Info);
            if (e.UpdateProgress)
                form.UpdateProgress();
        }

        /// <summary>
        /// Raise finished event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Data for finished calculation event</param>
        protected virtual void RaiseFinished(object sender, FinishedEventArgs e)
        {
            Finished?.Invoke(sender, e);
        }

        /// <summary>
        /// Calculate distribution of each phenotypic value
        /// </summary>
        /// <returns>Distribution of each phenotypic value</returns>
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

        /// <summary>
        /// Calculate variance for all traits
        /// </summary>
        /// <param name="Pc">Distribution of each phenotypic value</param>
        /// <returns>Minimum variance</returns>
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
        /// <param name="traitValues">List of all observed trait values</param>
        /// <returns>How many instances of each phenotype are exist</returns>
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
        /// Calculate variance using ferchet distance
        /// </summary>
        /// <param name="distance">Ferchet distance</param>
        /// <param name="Pc">Distribution of each phenotypic value</param>
        /// <returns>Variance</returns>
        public double GetFerchetDistance(double distance, double Pc)
        {
            return Math.Pow(distance, 2) * Pc;
        }

        /// <summary>
        /// Calculate variance
        /// </summary>
        /// <param name="Main">Main form</param>
        public virtual void Calculate(object Main)
        {
            return;
        }
    }
}
