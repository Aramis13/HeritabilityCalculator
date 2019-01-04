using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using System;
using System.Collections.Generic;

namespace HeritabilityCalculator
{
    /// <summary>
    /// Contains tree node data
    /// </summary>
    public class Branch
    {
        /// <summary>
        /// Edge length
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// All node sub branches
        /// </summary>
        public List<Branch> SubBranches { get; set; } = new List<Branch>();
        /// <summary>
        /// Node trait value
        /// </summary>
        public TraitValue TraitValue { get; set; } = new TraitValue();
    }
}
