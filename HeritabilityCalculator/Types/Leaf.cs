using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritabilityCalculator
{
    public class Leaf : Branch
    {
        public string Name { get; set; }
        public object TraitValue { get; set; }
    }
}
