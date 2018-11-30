using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using System;
using System.Collections.Generic;

namespace HeritabilityCalculator
{
    public class Branch
    {
        public double Length { get; set; }
        public List<Branch> SubBranches { get; set; } = new List<Branch>();

        //public void CalculateVM(object Main)
        //{
        //    if (!(Main is HeritabilityCalculator))
        //        return;

        //    HeritabilityCalculator MainForm = Main as HeritabilityCalculator;
        //    // Create the transition matrix A
        //    double[,] transition = MainForm.UserInputData.DistanceMatrix;
        //    //{
        //    //    { 0.7, 0.3 },
        //    //    { 0.4, 0.6 }
        //    //};

        //    // Create the emission matrix B
        //    double[,] emission = MainForm.UserInputData.EmissionMatrix;
        //    //{
        //    //    { 0.1, 0.4, 0.5 },
        //    //    { 0.6, 0.3, 0.1 }
        //    //};

        //    // Create the initial probabilities pi

        //    //double[] initial = MainForm.UserInputData.
        //        /*{ 0.6, 0.4 };*/

        //    // Create a new hidden Markov model
        //    var model = new HiddenMarkovModel(transition, emission, initial);

        //    // After that, one could, for example, query the probability
        //    // of a sequence occurring. We will consider the sequence
        //    int[] sequence = new int[] { 0, 1, 2 };

        //    // And now we will evaluate its likelihood
        //    double logLikelihood = model.LogLikelihood(sequence);
            

        //}

        

    }
}
