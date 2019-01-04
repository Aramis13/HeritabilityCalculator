using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritabilityCalculator
{
    /// <summary>
    /// Containcs leaf node data
    /// </summary>
    public class Leaf : Branch
    {
        /// <summary>
        /// Node name
        /// </summary>
        public string Name { get; set; }
    }
}
