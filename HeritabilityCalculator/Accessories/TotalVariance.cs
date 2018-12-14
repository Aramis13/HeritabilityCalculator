using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritabilityCalculator
{
    public class TotalVariance : Variance
    {
       
        #region Fields
        public double VT_Final_Result { get; private set; }

        #endregion Fields

        public TotalVariance(UserInput userinput) : base(userinput)
        {
           
        }

        public override void Calculate(object Main)
        {
            if (!(Main is HeritabilityCalculator))
                return;
            HeritabilityCalculator form = Main as HeritabilityCalculator;
            if (!userData.Validate())
                return;
            Dictionary<string,int> numOfInstances = GetNumOfInstances(userData.ObservedTraits);
            if (numOfInstances == null)
                return;
            Dictionary<string,double> Pc = GetPC(numOfInstances);
            if (Pc == null)
                return;
            VT_Final_Result = GetVariance(Pc);
            RaiseFinished(form, new FinishedEventArgs("Total Variance: " + VT_Final_Result));
        }

    }
}
