using Accord.Statistics.Models.Markov;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using System.Security.Cryptography;
using Accord.Statistics.Filters;

namespace HeritabilityCalculator
{

    public class ModelVarianceData
    {
        public double Variance { get; set; }
        public double Likelihood { get; set; }
    }

    public class ModelVariance : Variance
    {

        private Branch Root;
        private Tree tree;

        public ModelVariance(UserInput userinput, Branch root, Tree mainTree) : base(userinput)
        {
            Root = root;
            tree = mainTree;
        }

        public override void Calculate(object Main)
        {
            if (!(Main is HeritabilityCalculator))
                return;
            HeritabilityCalculator form = Main as HeritabilityCalculator;
            if (!userData.Validate())
                return;

            // Alg Start

            List<ModelVarianceData> data = new List<ModelVarianceData>();

            for (int i = 1; i <= 100; i++)
            {
                tree.currentPosition = 0;
                Branch currentTree = tree.Parse();
                SimulateTree(currentTree, null);
                List<Branch> curLeavs = new List<Branch>();
                GetCurrentLeavs(currentTree, curLeavs);
                List<TraitValue> currentTraitValues = GetCurObservedTraitValues(curLeavs, i);
                form.WriteToLog("Ittration " + i + ":", HeritabilityCalculator.MessageType.Info);
                foreach (TraitValue t in currentTraitValues)
                    form.WriteToLog(t.value, HeritabilityCalculator.MessageType.Info);
                Dictionary<string,int> numOfInstances = GetNumOfInstances(currentTraitValues.ToArray());
                if (numOfInstances == null)
                    continue;
                Dictionary<string,double> Pc = GetPC(numOfInstances);
                if (Pc == null)
                    continue;
                ModelVarianceData d = new ModelVarianceData();
                d.Variance = GetVariance(Pc);
                //d.Likelihood = GetLikelihood(Pc, currentTraitValues); // ToDo: implement
            }
        }

        private double GetLikelihood(Dictionary<string, double> Pc, List<TraitValue> currentTraitValues)
        {
            List<string> current = new List<string>();
            foreach (TraitValue trait in currentTraitValues)
            {
                current.Add(trait.value);
            }

            var codebook = new Codification("CurrentPc", current.ToArray());
            int[] sequence = codebook.Transform(current.ToArray());
            // Create the initial probabilities pi
            List<double> initial = new List<double>();
            foreach(double p in Pc.Values)
            {
                initial.Add(p);
            }

            // Create a new hidden Markov model
            var hmm = new HiddenMarkovModel(userData.DistanceMatrix, userData.EmissionMatrix, initial.ToArray());

            // After that, one could, for example, query the probability
            // of a sequence occurring. We will consider the sequence
            //int[] sequence = new int[] { 0, 1, 2 };

            // And now we will evaluate its likelihood
            double logLikelihood = hmm.LogLikelihood(sequence);

            return logLikelihood;
        }

        // Return a random integer between a min and max value.
        private double GetRandom()
        {
            RNGCryptoServiceProvider Rand = new RNGCryptoServiceProvider();
            int max = 100;
            int min = 0;
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                // Get four random bytes.
                byte[] four_bytes = new byte[4];
                Rand.GetBytes(four_bytes);
                // Convert that into an uint.
                scale = BitConverter.ToUInt32(four_bytes, 0);
            }

            // Add min to the scaled difference between max and min.
            int num = (int)(min + (max - min) *
                (scale / (double)uint.MaxValue));

            return (double)num / 100;
        }

        private List<TraitValue> GetCurObservedTraitValues(List<Branch> currentLeavs, double t0) 
        {
            List<TraitValue> traitvalues = new List<TraitValue>();
            foreach (Branch leave in currentLeavs)
            {
                TraitValue t = new TraitValue();
                t.value = GetNodeTrait(leave.TraitValue.value, t0);
                traitvalues.Add(t);
            }
            return traitvalues;
        }

        private void GetCurrentLeavs(Branch fatherNode, List<Branch> observed)
        {
            if (fatherNode.SubBranches.Count == 0)
            {
                observed.Add(fatherNode);
                return;
            }

            foreach (Branch child in fatherNode.SubBranches)
            {
                GetCurrentLeavs(child, observed);
            }
        }

        private void SimulateTree(Branch fatherNode, string fatherTrait)
        {
            if (fatherNode.Length == 0)
            {
                fatherNode.TraitValue.value = GetRoot();
            }
            else
            {
                string trait = GetNodeTrait(fatherTrait, fatherNode.Length);
                fatherNode.TraitValue.value = trait;
            }

            foreach (Branch child in fatherNode.SubBranches)
            {
                SimulateTree(child, fatherNode.TraitValue.value);
            }
        }

        private string GetNodeTrait(string fatherTrait, double distance)
        {
            string trait = null;
            int index = -1;
            for (int i = 0; i < userData.Traits.Length; i++)
            {
                if (userData.Traits[i] == fatherTrait)
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                //Random rnd = new Random();
                //double P = rnd.Next(100);
                //P /= 100;
                double P = GetRandom();
                double[,] Mt = Elementwise.Multiply(userData.EmissionMatrix, distance);  // Calc matrix *t0 example
                double[,] Q = CalculateExp(Mt); // Calc the exp matrix using taylor expansion
                double[] probs = Q.GetRow(index);
                Dictionary<string, double> traitQs = new Dictionary<string, double>(0);
                for (int j = 0; j < userData.Traits.Length; j++)
                {
                    traitQs.Add(userData.Traits[j], probs[j]);
                }
                var sortedProbs = traitQs.ToList();
                sortedProbs.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
                
                if (P < sortedProbs.ElementAt(0).Value)
                {
                    trait = sortedProbs.ElementAt(0).Key;
                }
                else if (P > sortedProbs.ElementAt(sortedProbs.Count - 1).Value)
                {
                    trait = sortedProbs.ElementAt(sortedProbs.Count - 1).Key;
                }
                else
                {
                    for (int i = 0; i < userData.Traits.Length-1; i++)
                    {
                        if (sortedProbs.ElementAt(i).Value <= P && P < sortedProbs.ElementAt(i+1).Value)
                        {
                            trait = userData.Traits[i+1];
                            break;
                        }
                    }
                }
            }
            return trait;
        }

        private string GetRoot()
        {
            string root = null;
            Random rnd = new Random();
            double P = rnd.Next(100);
            P /= 100;

            double traitprob = (double)1 / userData.Traits.Length;
            for (int i = 0; i < userData.Traits.Length; i++)
            {
                if ((traitprob * i) <= P && P < (traitprob * (i + 1)))
                {
                    root = userData.Traits[i];
                    break;
                }
            }
            return root;
        }

        /// <summary>
        /// Calculate exp(x) using Taylor Expansion
        /// </summary>
        /// <param name="x">Exponent for Euler's Number</param>
        /// <returns>Returns exp(x)</returns>
        private double[,] CalculateExp(double[,] x)
        {
            // Holds and returns final answer
            double[,] answer = new double[x.GetLength(0), x.GetLength(1)];

            // Holds previous answer and is used to stop Taylor Expansion
            double[,] oldanswer = new double[x.GetLength(0), x.GetLength(1)];

            // Summation index variable
            double k = 0;

            // Refine the solution by adding more terms to the Taylor Expansion.
            // Stop when the answer doesn't change.
            while (true)
            {
                double[,] temp = new double[x.GetLength(0), x.GetLength(1)];
                temp = GetMatrixPower(x, k);
                double facturial = Factorial(k);
                temp = Elementwise.Divide(temp, facturial);
                answer = Elementwise.Add(answer, temp);
                //int z = 0;
                //for (int i = 0; i < answer.GetLength(0); i++)
                //{
                //    for (int j = 0; j < answer.GetLength(0); j++)
                //    {
                //        if (answer[i, j] == oldanswer[i, j])
                //            z++;
                //    }
                //}
                //if (z == x.Length)
                if (k > 120)
                {
                    break;
                }
                else
                {
                    oldanswer = answer;
                    k++;
                }
            }
            return answer;
        }

        private double[,] GetMatrixPower(double[,] x, double power)
        {

            double[,] temp = (double[,])x.Clone();

            if (power == 0)
            {
                for (int i = 0; i < x.GetLength(0); i++)
                {
                    for (int j = 0; j < x.GetLength(1); j++)
                    {
                        if (i == j)
                            temp[i, j] = 1;
                        else
                            temp[i, j] = 0;
                    }
                }
            }
            else
            {
                int l = 0;
                while (l < power - 1)
                {
                    double[,] res = new double[x.GetLength(0), x.GetLength(1)];
                    int size = 0;
                    while (size < x.Length)
                    {
                        for (int i = 0; i < x.GetLength(0); i++)
                        {
                            for (int j = 0; j < x.GetLength(1); j++)
                            {
                                double[] row = temp.GetRow(i);
                                double[] col = x.GetColumn(j);
                                double val = 0;
                                for (int k = 0; k < row.Length; k++)
                                    val += row[k] * col[k];
                                res[i, j] = val;
                                size++;
                            }
                        }
                        temp = res;
                    }
                    l++;
                }
            }
            return temp;
        }

        /// <summary>
        /// Calculate the factorial for x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double Factorial(double x)
        {
            double answer = 1;
            double counter = 1;

            while (counter <= x)
            {
                answer = answer * counter;
                if (double.IsInfinity(answer) || double.IsNaN(answer))
                    return double.MaxValue;
                counter++;

            }
            return answer;
        }

        private void print_matrix(double[,] mat)
        {
            int row_count = mat.GetLength(0), col_count = mat.GetLength(1);

            for (int i = 0; i < row_count; i++)
            {     // calc: Q*t
                for (int j = 0; j < col_count; j++)
                    Console.Out.Write("[" + mat[i, j] + "] ");
                Console.Out.WriteLine();
            }
        }
      

        private void update_log(string status, Branch b, HeritabilityCalculator f)
        {

            string msg = "VM CALC: ";
            if (status == "success")
            {
                msg += "random root node (" + b.TraitValue.value + ") has been generated succussfuly";
                RaiseFinished(f, new FinishedEventArgs(msg));
            }
            else
            {
                msg += "FAILED @ ModelVariance.cs , Calculate())";
                RaiseFinished(f, new FinishedEventArgs(msg));
            }
        }

    }
}
