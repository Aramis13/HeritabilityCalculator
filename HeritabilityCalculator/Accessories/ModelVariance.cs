using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using System.Security.Cryptography;
using System.Collections.Concurrent;

namespace HeritabilityCalculator
{
    /// <summary>
    /// Contains all VM iterations data
    /// </summary>
    public class ModelVarianceContainer
    {
        /// <summary>
        /// Genrated tree
        /// </summary>
        public Branch GeneratedTree { get; set; }
        /// <summary>
        /// Generated tree data
        /// </summary>
        public List<ModelVarianceData> Data { get; set; }
    }

    /// <summary>
    /// Contains VM data
    /// </summary>
    public class ModelVarianceData
    {
        /// <summary>
        /// Elongation for current iteration
        /// </summary>
        public double T0 { get; set; }
        /// <summary>
        /// Variance for current iteration
        /// </summary>
        public double Variance { get; set; }
        /// <summary>
        /// Observed traits for current iteration
        /// </summary>
        public List<TraitValue> ObservedTraits { get; set; } = new List<TraitValue>();
    }

    /// <summary>
    /// Represents model variance
    /// </summary>
    public class ModelVariance : Variance
    {
        /// <summary>
        /// Newick parsed tree
        /// </summary>
        private Bio.Phylogenetics.Tree tree;
        /// <summary>
        /// Results array for all trees and iterations
        /// </summary>
        public ConcurrentBag<ModelVarianceContainer> Results = new ConcurrentBag<ModelVarianceContainer>();
        /// <summary>
        /// Deltat value
        /// </summary>
        public double deltaT = 1;
        /// <summary>
        /// NUmber of t0 iterations
        /// </summary>
        private readonly int t0Itr;

        /// <summary>
        /// Create new instance of model variance
        /// </summary>
        /// <param name="userinput">User input data</param>
        /// <param name="root">Root of the parsed newick tree</param>
        /// <param name="Itr">Number of t0 iteration</param>
        public ModelVariance(UserInput userinput, Bio.Phylogenetics.Tree root, int Itr) : base(userinput)
        {
            tree = root;
            t0Itr = Itr;
        }

        /// <summary>
        /// Main algorithm for model variance
        /// </summary>
        /// <param name="Main">Main form</param>
        public override void Calculate(object Main)
        {
            if (!(Main is HeritabilityCalculator))
                return;
            HeritabilityCalculator form = Main as HeritabilityCalculator;
            if (!userData.Validate())
                return;

            // Algorithm Start
            Branch currentTree = new Branch();
            form.CreateMainTree(tree.Root, currentTree, 0, 0);
            SimulateTree(currentTree, null);
            List<Branch> curLeavs = new List<Branch>();
            GetCurrentLeavs(currentTree, curLeavs);
            List<double> vmList = new List<double>();
            ModelVarianceContainer container = new ModelVarianceContainer
            {
                GeneratedTree = currentTree
            };
            List<ModelVarianceData> data = new List<ModelVarianceData>();
            for (int i = 0; i <= t0Itr; i++)
            {
                List<TraitValue> currentTraitValues = GetCurObservedTraitValues(curLeavs, i * deltaT);
                Dictionary<string,int> numOfInstances = GetNumOfInstances(currentTraitValues.ToArray());
                if (numOfInstances == null)
                    continue;
                Dictionary<string,double> Pc = GetPC(numOfInstances);
                if (Pc == null)
                    continue;
                data.Add(new ModelVarianceData() {
                    Variance = GetVariance(Pc),
                    ObservedTraits = currentTraitValues,
                    T0 = i * deltaT
                });
               
            }
            container.Data = data;
            Results.Add(container);
            RaiseFinished(form, new FinishedEventArgs("Finished Itteration ", true));
        }

        /// <summary>
        /// Return a random integer between a min and max value
        /// </summary>
        /// <returns>Crypto random number</returns>
        private double GetRandom()
        {
            RNGCryptoServiceProvider Rand = new RNGCryptoServiceProvider();
            int max = 100;
            int min = 0;
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                byte[] four_bytes = new byte[4];
                Rand.GetBytes(four_bytes);
                scale = BitConverter.ToUInt32(four_bytes, 0);
            }
            int num = (int)(min + (max - min) *
                (scale / (double)uint.MaxValue));
            return (double)num / 100;
        }

        /// <summary>
        /// Calculate observed traits
        /// </summary>
        /// <param name="currentLeavs">Current tree leavs</param>
        /// <param name="t0">Current to iteration</param>
        /// <returns>Current observed traits</returns>
        private List<TraitValue> GetCurObservedTraitValues(List<Branch> currentLeavs, double t0) 
        {
            List<TraitValue> traitvalues = new List<TraitValue>();
            foreach (Branch leave in currentLeavs)
            {
                TraitValue t = new TraitValue
                {
                    value = GetNodeTrait(leave.TraitValue.value, t0)
                };
                traitvalues.Add(t);
            }
            return traitvalues;
        }

        /// <summary>
        /// Generate observed traits from current tree
        /// </summary>
        /// <param name="fatherNode">Root for current tree</param>
        /// <param name="observed">Empty observed list</param>
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

        /// <summary>
        /// Simulate a trait for eich node in the tree
        /// </summary>
        /// <param name="fatherNode">Root for current tree</param>
        /// <param name="fatherTrait">Direct ancestor phenotypic trait of node</param>
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

        /// <summary>
        /// Simulate a phenotypic trait
        /// </summary>
        /// <param name="fatherTrait">Direct ancestor phenotypic trait of node</param>
        /// <param name="distance">Edge length</param>
        /// <returns></returns>
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
                double P = GetRandom();
                double[,] Mt = Elementwise.Multiply(userData.EmissionMatrix, distance);  // Calc matrix *t0 example
                double[,] Q = CalculateExp(Mt); // Calc the exp matrix using taylor expansion
                string s = string.Empty;
                double[] probs = Q.GetRow(index);
                Dictionary<string, double> traitQs = new Dictionary<string, double>(0);
                for (int j = 0; j < userData.Traits.Length; j++)
                {
                    traitQs.Add(userData.Traits[j], probs[j]);
                }
                var sortedProbs = traitQs.ToList();
                if (P < sortedProbs.ElementAt(0).Value)
                {
                    trait = sortedProbs.ElementAt(0).Key;
                }
                else
                {
                    double sum = sortedProbs.ElementAt(0).Value;
                    for (int i = 1; i < userData.Traits.Length - 1; i++)
                    {
                        sum += sortedProbs.ElementAt(i).Value;
                        if (P < sum)
                        {
                            trait = sortedProbs.ElementAt(i).Key;
                            break;
                        }
                    }
                }
                if (trait == null)
                {
                    trait = sortedProbs.ElementAt(userData.Traits.Length-1).Key;
                }
            }
            return trait;
        }

        /// <summary>
        /// Simulate the root phenotypic trait
        /// </summary>
        /// <returns>Root phenotypic trait</returns>
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

        /// <summary>
        /// Calculate matrix power
        /// </summary>
        /// <param name="x">A matrix</param>
        /// <param name="power">Power</param>
        /// <returns>Matrix power</returns>
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
        /// <param name="x">The number to factor</param>
        /// <returns>Factorial for x</returns>
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
    }
}
