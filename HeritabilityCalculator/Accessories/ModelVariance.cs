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

            /* VM ALGORITHM */

            // 1. Create tree root node with a RANDOM phenotypic value.

            Branch root = new Branch();

            root.trait_value = getRandomRootValue(userData.Traits);

            if (!(root.trait_value == null))
                update_log("success", root, form);
            else
                update_log("failure", root, form);

            // 2. Calculate the transition probabilitis matrix: exp_mat = e ^ Q * t

            // CALCULATION ERROR
            int t = 7;
            double[,] exp_mat = calc_exponential(userData.EmissionMatrix,t);
            print_matrix(exp_mat);

            // 3. Using the transition probabilitis matrix (exp_mat), generate phenotypic values for sub branches

            // ......
            // ......


        }

        private double[,] calc_exponential (double[,] Q, int t)
        {
            int row_count = Q.GetLength(0), col_count = Q.GetLength(1);
            double[,] result = new double[row_count,col_count];

            for (int i = 0; i < row_count; i++)         // Q*t
                for (int j = 0; j < col_count; j++)
                    result[i,j] = Q[i, j]*t;
            Console.Out.WriteLine("*t: ");
            print_matrix(result);

            //return Matrix.Exp(result);
            return Matrix.Exp(result);

        }

        // TO TEST
        private double[,] matrix_addition (double [,] A, double [,] B)
        {
            int m, n, i, j;

            m = A.GetLength(0);
            n = B.GetLength(1);

            if (!(m == B.GetLength(0)))
                return null;
            if (!(n == A.GetLength(1)))
                return null;

            double[,] C = new double[m, n];

            for (i = 0; i < m; i++)
                for (j = 0; j < n; j++)
                    C[i, j] = A[i, j] + B[i, j];

            return C;

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
