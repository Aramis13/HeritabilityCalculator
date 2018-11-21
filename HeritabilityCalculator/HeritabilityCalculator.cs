using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeritabilityCalculator
{
    public partial class HeritabilityCalculator : Form
    {

        #region Fields

        public const int N = 3;   // square matrix dimension (# of different phenotypes)
        public Tree MainTree;
        public string TraitName;
        public Dictionary<string, object> TraitValues;
        public double[,] DistanceMatrix;
        public double[,] EmissionMatrix;
        public string[] order;      // order of phenotypic values as presented in the distances matrix

        #endregion Fields


        public HeritabilityCalculator()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TraitValues = new Dictionary<string, object>();     // phenotypic trait values of Leaves (current observed)
            TraitValues.Add("A", "red");
            TraitValues.Add("B", "green");
            TraitValues.Add("C", "blue");
            TraitValues.Add("D", "red");
            TraitValues.Add("E", "red");
            TraitValues.Add("F", "red");
            TraitValues.Add("G", "green");
            TraitValues.Add("H", "green");

            /*
              
              ** ORDER IN THIS EXAMPLE: RED -> GREEN -> BLUE
              
                    RED   GREEN   BLUE
             RED    0     0.2     0.7
             GREEN  0.2   0       0.5
             BLUE   0.7   0.5     0
                
             */

            order = new string[] { "red", "green", "blue" };        // order of matrix
            DistanceMatrix = new double[N, N] { {0,0.2,0.7}, {0.2,0,0.5}, {0.7,0.5,0} };    // D matrix
            TotalVariance VT = new TotalVariance(TraitValues, DistanceMatrix, order);   // calc VT

            Console.WriteLine("Total Variance is: " + VT.getVT());  // print VT

           // var tree = new Tree("(((A:0.1,(B:0.2,C:0.3):0.4):0.5,((D:0.6,E:0.7):0.8,(F:0.9,G:1):1.1):1.2):1.3)").Parse();
        }
    }
}
