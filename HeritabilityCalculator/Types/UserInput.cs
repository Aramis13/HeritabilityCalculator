namespace HeritabilityCalculator

{
    /// <summary>
    /// Contains all User data input
    /// </summary>
    public class UserInput
    {
        /// <summary>
        /// Contains ferchet distances
        /// </summary>
        public double[,] DistanceMatrix;
        /// <summary>
        /// Contains transition values
        /// </summary>
        public double[,] EmissionMatrix;
        /// <summary>
        /// Contains Observed traits
        /// </summary>
        public TraitValue[] ObservedTraits;
        /// <summary>
        /// Traits in intrist
        /// </summary>
        public string[] Traits;
        /// <summary>
        /// NUmber of observed traits
        /// </summary>
        public int N
        {
            get
            {
                if (ObservedTraits != null)
                    return ObservedTraits.Length;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Validates the user input
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            return DistanceMatrix != null && EmissionMatrix != null && ObservedTraits != null
                && Traits != null;
        }
    }

    /// <summary>
    /// Contains trait value data
    /// </summary>
    public class TraitValue
    {
        /// <summary>
        /// Trait name
        /// </summary>
        public string Name = string.Empty;
        /// <summary>
        /// Trait value
        /// </summary>
        public string value;
    }
}
