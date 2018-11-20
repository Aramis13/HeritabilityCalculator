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
        public HeritabilityCalculator()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Branch b = new Branch();
          
            var tree = new Parser("(((A:0.1,(B:0.2,C:0.3):0.4):0.5,((D:0.6,E:0.7):0.8,(F:0.9,G:1):1.1):1.2):1.3)").ParseTree();
        }
    }
}
