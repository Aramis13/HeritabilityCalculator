using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritabilityCalculator
{
    public class Branch
    {
        public double Length { get; set; }
        public List<Branch> SubBranches { get; set; } = new List<Branch>();
    }
}
