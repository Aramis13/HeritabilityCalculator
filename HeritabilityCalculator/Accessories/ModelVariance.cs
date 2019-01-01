using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using System.IO;

namespace HeritabilityCalculator
{

    public class ModelVarianceContainer
    {
        public Branch GeneratedTree { get; set; }
        public List<ModelVarianceData> Data { get; set; }
    }

    public class ModelVarianceData
    {
        public double T0 { get; set; }
        public double Variance { get; set; }
        public List<TraitValue> ObservedTraits { get; set; } = new List<TraitValue>();
    }

    public class ModelVariance : Variance
    {
        //private Tree tree;
        private Bio.Phylogenetics.Tree tree;
        public ConcurrentBag<ModelVarianceContainer> Resaults = new ConcurrentBag<ModelVarianceContainer>();
        public double deltaT = 1;
        private int t0Itr;


        public ModelVariance(UserInput userinput, Bio.Phylogenetics.Tree root, Tree mainTree, int Itr) : base(userinput)
        {
            tree = root;
            //tree = mainTree;
            //deltaT = tree.MaxDepth / 40;
            t0Itr = Itr;
        }

  
        public override void Calculate(object Main)
        {
            if (!(Main is HeritabilityCalculator))
                return;
            HeritabilityCalculator form = Main as HeritabilityCalculator;
            if (!userData.Validate())
                return;

            // Alg Start
 
            Branch currentTree = new Branch();
            //Tree t = new Tree(tree.input);
            //currentTree = t.Parse();
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
            Resaults.Add(container);
            RaiseFinished(form, new FinishedEventArgs("Finished Itteration "));
        }

        /// <summary>
        /// Return a random integer between a min and max value.
        /// </summary>
        /// <returns></returns>
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
                TraitValue t = new TraitValue
                {
                    value = GetNodeTrait(leave.TraitValue.value, t0)
                };
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
                double P = GetRandom();
                double[,] Mt = Elementwise.Multiply(userData.EmissionMatrix, distance);  // Calc matrix *t0 example
                double[,] Q = CalculateExp(Mt); // Calc the exp matrix using taylor expansion
                string s = string.Empty;
                //for (int i = 0; i < Q.GetLength(0); i++)
                //{
                //    for (int j = 0; j < Q.GetLength(1); j++)
                //    {
                //        if (j < Q.GetLength(1) - 1)
                //            s += Q[i, j] + ",";
                //        else
                //            s += Q[i, j] + Environment.NewLine;
                //    }
                //}
                //File.WriteAllText("C:\\Users\\Idan\\Desktop\\Test2.csv", s);
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
    }
}
