using Accord.Statistics.Models.Markov;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace HeritabilityCalculator
{
    public class ModelVariance : Variance
    {

        public ModelVariance(UserInput userinput) : base(userinput)
        {
        }

        public override void Calculate(object Main)
        {
            if (!(Main is HeritabilityCalculator))
                return;
            HeritabilityCalculator form = Main as HeritabilityCalculator;
            if (!userData.Validate())
                return;
            
            double[,] Q = CalculateExp(userData.EmissionMatrix);

     
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
                int z = 0;
                for (int i = 0; i < answer.GetLength(0); i++)
                {
                    for (int j = 0; j < answer.GetLength(0); j++)
                    {
                        if (answer[i, j] == oldanswer[i, j])
                            z++;
                    }
                }
                if (z == x.Length)
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
                counter++;

            }
            return answer;
        }

    

    private void print_matrix (double[,] mat)
        {
            int row_count = mat.GetLength(0), col_count = mat.GetLength(1);

            for (int i = 0; i < row_count; i++)
            {     // calc: Q*t
                for (int j = 0; j < col_count; j++)
                    Console.Out.Write("[" + mat[i, j] + "] ");
                Console.Out.WriteLine();
            }
        }
        /// <summary>
        /// vm calc step 1: generate random root value
        /// returns a random
        /// </summary>

        private TraitValue getRandomRootValue(String[] trait_values)
        {
            TraitValue tv = new TraitValue();   // to be returned
            int n = trait_values.Length, rand_index = 0, max = 0;

            int[] random_indexes = new int[n];
            for (int i = 0; i < n; i++)
                random_indexes[i] = 0;

            Random rand = new Random();
            for (int i = 0; i < 1000; i++)      // generate 1000 random values in the range of: 0 to n-1
                random_indexes[rand.Next(n)]++;

            for (int i = 0; i < n; i++) {       // pick the highest generated one
                if (random_indexes[i] > max)
                {
                    max = random_indexes[i];
                    rand_index = i;
                }
            }

            tv.value = trait_values[rand_index];             // set as root value
            tv.Name = "root's random trait value";
            return tv;
        }

        private void update_log (String status, Branch b, HeritabilityCalculator f )
        {

            string msg = "VM CALC: ";
            if (status == "success")
            {
                msg += "random root node ("+ b.trait_value.value + ") has been generated succussfuly";
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
