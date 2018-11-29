using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Statistics.Models.Markov.Learning;
using Newtonsoft.Json;

namespace HeritabilityCalculator
{
    public partial class HeritabilityCalculator : Form
    {
        #region Examples
        // TraitValues = new Dictionary<string, object>();     // phenotypic trait values of Leaves (current observed)
        // TraitValues.Add("A", "red");
        // TraitValues.Add("B", "green");
        // TraitValues.Add("C", "blue");
        // TraitValues.Add("D", "red");
        // TraitValues.Add("E", "red");
        // TraitValues.Add("F", "red");
        // TraitValues.Add("G", "green");
        // TraitValues.Add("H", "green");

        // /*

        //   ** ORDER IN THIS EXAMPLE: RED -> GREEN -> BLUE

        //         RED   GREEN   BLUE
        //  RED    0     0.2     0.7
        //  GREEN  0.2   0       0.5
        //  BLUE   0.7   0.5     0

        //  */

        // order = new string[] { "red", "green", "blue" };        // order of matrix
        // DistanceMatrix = new double[N, N] { {0,0.2,0.7}, {0.2,0,0.5}, {0.7,0.5,0} };    // D matrix
        // TotalVariance VT = new TotalVariance(TraitValues, DistanceMatrix, order);   // calc VT

        // Console.WriteLine("Total Variance is: " + VT.getVT());  // print VT

        //// var tree = new Tree("(((A:0.1,(B:0.2,C:0.3):0.4):0.5,((D:0.6,E:0.7):0.8,(F:0.9,G:1):1.1):1.2):1.3)").Parse();
        #endregion Examples

        #region Fields

        public const int N = 3;   // square matrix dimension (# of different phenotypes)
        public Tree MainTree;
        public string TraitName;
    
        
        public string[] order;      // order of phenotypic values as presented in the distances matrix
        public OpenFileDialog fileDialog;
        public Branch MainBranch;
        public bool TreeValid = false;
        public bool UserInputValid = false;
        public UserInput UserInputData;

        #endregion Fields


        public HeritabilityCalculator()
        {
            InitializeComponent();
            WriteToLog("Insert Data", MessageType.Important);
        }

        public enum MessageType
        {
            Info,
            Error,
            Success,
            Important
        }

        public string GetFileString(string filter, out string filePath)
        {
            fileDialog = new OpenFileDialog();
            fileDialog.Filter = filter;
            string res = null;
            filePath = null;
            DialogResult result = fileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                filePath = fileDialog.FileName;
                try
                {
                    res = File.ReadAllText(filePath);
                }
                catch
                {
                    WriteToLog("Failed to read file: " + filePath, MessageType.Error);
                }
            }
            return res;
        }

        private void TreeBrowse_Click(object sender, EventArgs e)
        {
            string path;
            string newickTree = GetFileString("Newick Tree (*.txt)|*.txt", out path);
            string msg = string.Empty;
            MessageType type = MessageType.Info;
            if (newickTree != null)
            {
                try
                {
                    MainTree = new Tree(newickTree);
                    if (!MainTree.ValidateTree(out msg))
                    {
                        TreeValid = false;
                        WriteToLog(msg, MessageType.Error);
                        throw new Exception();
                    }
                    MainBranch = MainTree.Parse();
                    TreeValid = true;
                    msg = "Tree uploaded successfully";
                    type = MessageType.Success;
                    TreePathText.Text = path;
                }
                catch
                {
                    msg = "Failed to parse input" + Environment.NewLine;
                    msg += "Validate that your input is in Newick format.";
                    type = MessageType.Error;
                    TreeValid = false;
                }
            }
            else
                TreeValid = false;
            UpdateStartButton();
            WriteToLog(msg, type);
        }

        private void InputBrowse_Click(object sender, EventArgs e)
        {
            string path;
            string json = GetFileString("Input Data (*.json)|*.json", out path);
            string msg = string.Empty;
            MessageType type = MessageType.Info;
            if (json != null)
            {
                try
                {
                    UserInputData = JsonConvert.DeserializeObject<UserInput>(json);
                    if (UserInputData.DistanceMatrix == null || UserInputData.EmissionMatrix == null
                        || UserInputData.TraitValues == null)
                    {
                        UserInputValid = false;
                        throw new Exception();
                    }
                       
                    UserInputValid = true;
                    msg = "User input uploaded successfully";
                    type = MessageType.Success;
                    UserInputText.Text = path;
                }
                catch
                {
                    msg = "Failed to parse input" + Environment.NewLine;
                    msg += "Validate that your input is in the correct format.";
                    type = MessageType.Error;
                    UserInputValid = false;
                }
            }
            else
                UserInputValid = false;
            UpdateStartButton();
            WriteToLog(msg, type);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {

        }

        public void UpdateStartButton()
        {
            if (UserInputData == null || UserInputData.DistanceMatrix == null || UserInputData.EmissionMatrix == null
                || UserInputData.TraitValues == null)
                UserInputValid = false;
            StartButton.Enabled = TreeValid && UserInputValid;
        }

        public void WriteToLog(string msg, MessageType type)
        {
            switch (type)
            {
                case MessageType.Info: Log.SelectionColor = Color.Gray; break;
                case MessageType.Error: Log.SelectionColor = Color.Red; break;
                case MessageType.Success: Log.SelectionColor = Color.Green; break;
                case MessageType.Important: Log.SelectionColor = Color.Blue; break;
            }

            Log.AppendText(msg + Environment.NewLine);
            Log.SelectionStart = Log.Text.Length;
            Log.ScrollToCaret();
        }


    }
}
