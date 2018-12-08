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
        public TraitValue TraitValue { get; set; } = new TraitValue();
    }
}
