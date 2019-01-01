using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Statistics.Models.Markov.Learning;
using Bio.IO.Newick;
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

        public Tree MainTree;
        public string TraitName;
        public const int numOftrees = 100;
        public const int t0Itr = 10;
        public const int numOfPartitions = 10;


        public string[] order;      // order of phenotypic values as presented in the distances matrix
        public OpenFileDialog fileDialog;
        public Branch MainBranch;
        public Bio.Phylogenetics.Tree tree;
        public bool TreeValid = false;
        public bool UserInputValid = false;
        public UserInput UserInputData;
        public TotalVariance VT;
        public ModelVariance VM;
        
        #endregion Fields


        public HeritabilityCalculator()
        {
            InitializeComponent();
            WriteToLog("Insert Data", MessageType.Important);
            WriteToLog("Number of processors on the mechine: " + Environment.ProcessorCount, MessageType.Info);
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
                    //MainTree = new Tree(newickTree);
                    //if (!MainTree.ValidateTree(out msg))
                    //{
                    //    TreeValid = false;
                    //    WriteToLog(msg, MessageType.Error);
                    //    throw new Exception();
                    //}
                    //MainBranch = MainTree.Parse();
                    var parser = new NewickParser();
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(newickTree);
                    tree = parser.Parse(sb);
                    MainBranch = new Branch();
                    CreateMainTree(tree.Root, MainBranch, 0, 0);
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

        List<double> depths = new List<double>();
        public void CreateMainTree(Bio.Phylogenetics.Node node, Branch branch, double edge, double max)
        {
            max += edge;
            if (node.IsLeaf)
                depths.Add(max);
            branch.Length = edge;
            branch.SubBranches = new List<Branch>();
            for (int i = 0; i < node.Children.Count; i++)
            {
                branch.SubBranches.Add(new Branch());
                CreateMainTree(node.Children.Keys.ElementAt(i), branch.SubBranches.ElementAt(i), node.Edges.ElementAt(i).Distance, max);
            }
           
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
                    if (!UserInputData.Validate())
                    {
                        UserInputValid = false;
                        throw new Exception();
                    }

                    UserInputValid = true;
                    msg = "User input uploaded successfully";
                    type = MessageType.Success;
                    UserInputText.Text = path;
                }
                catch (Exception ex)
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

        private async void StartButton_Click(object sender, EventArgs e)
        {
            TreeBrowse.Enabled = false;
            InputBrowse.Enabled = false;
            StartButton.Enabled = false;
            Log.Clear();
            WriteToLog("Starting Calculation...", MessageType.Important);
            VT = new TotalVariance(UserInputData);
            List<Task> tasks = new List<Task>
            {
                Task.Factory.StartNew(() => VT.Calculate(this))
            };

            VM = new ModelVariance(UserInputData, tree, MainTree, t0Itr);
            VM.deltaT = depths.Max() / 40;
            tasks.Add(Task.Factory.StartNew(() => Parallel.For(0, numOftrees, i => VM.Calculate(this))));
            await Task.WhenAll(tasks);

            WriteToLog("Processing Results...", MessageType.Important);
            double partition = GetPartitionsDelta(VM.Resaults);
            int[,] baskets = GetBaskets(VM.Resaults, partition);

            TreeBrowse.Enabled = true;
            InputBrowse.Enabled = true;
            StartButton.Enabled = true;

            int vtPartition = GetPartition(partition, VT.VT_Final_Result);
            int bestItr = GetMaxT0(baskets, vtPartition);

            int[] bestItrArr = new int[numOfPartitions];
            for (int i = 0; i < numOfPartitions; i++)
            {
                bestItrArr[i] = baskets[bestItr, i];
            }
            double[] liklihoods = GetLiklihoods(baskets, vtPartition);
            double[] vmResualts = GetVM(VM.Resaults, bestItr);
            double liklihood = bestItrArr[vtPartition] / (double)numOftrees;
            double[] X2 = GetX2(liklihoods, bestItr);
            Branch bestTree = VM.Resaults.ElementAt(0).GeneratedTree;
            List<TraitValue> traitValues = VM.Resaults.ElementAt(0).Data.ElementAt(bestItr).ObservedTraits;

            TreeDrawData data = new TreeDrawData()
            {
                Liklihood = liklihood,
                observed = traitValues,
                Root = bestTree,
                Title = "Eye Color",
                ModelVariance = vmResualts,
                TotalVariance = VT.VT_Final_Result,
                NumOfPartitions = numOfPartitions,
                Partition = partition,
                BestItrRes = bestItrArr,
                BestItr = bestItr,
                X2 = X2,
                DeltaT = VM.deltaT,
                NumOfTrees = numOftrees
            };
            TreeDraw l = new TreeDraw(data);
            l.Create();
            l.Open();
        }

        public double[] GetX2(double[] liklihoods, int bestItr)
        {
            double[] res = new double[t0Itr];
            for (int i = 0; i < t0Itr; i++)
            {
                if (liklihoods[i] != 0)
                    res[i] = -2 * Math.Log(liklihoods[i] / liklihoods[bestItr]);
            }
            return res;
        }

        public double[] GetLiklihoods(int[,] baskets, int vtPartition)
        {
            double[] res = new double[t0Itr];
            for (int i = 0; i < t0Itr; i++)
            {
                res[i] = baskets[i, vtPartition] / (double)numOftrees;
            }
            return res;
        }

        public double[] GetVM(ConcurrentBag<ModelVarianceContainer> Resaults, int bestItr)
        {
            double sumMin = 0;
            double sum = 0;
            double sumMax = 0;
            for (int i = 0; i < numOftrees; i++)
            {
                if (bestItr != 0)
                    sumMin += Resaults.ElementAt(i).Data.ElementAt(bestItr - 1).Variance;
                else
                    sumMin += Resaults.ElementAt(i).Data.ElementAt(bestItr).Variance;
                sum += Resaults.ElementAt(i).Data.ElementAt(bestItr).Variance;
                sumMax += Resaults.ElementAt(i).Data.ElementAt(bestItr + 1).Variance;
            }
            sumMin /= numOftrees;
            sum /= numOftrees;
            sumMax /= numOftrees;
            return new double[] { sumMin, sum, sumMax };
        }

        public int GetMaxT0(int[,] baskets, int vtPartition)
        {
            int max = 0;
            int itr = 0;
            for (int i = 0; i< baskets.GetLength(0); i++)
            {
                if (max < baskets[i, vtPartition])
                {
                    max = baskets[i, vtPartition];
                    itr = i;
                }
            }
            return itr;
        }

        public int[,] GetBaskets(ConcurrentBag<ModelVarianceContainer> Resaults, double partition)
        {
            int[,] res = new int[t0Itr, numOfPartitions];
            string s = string.Empty;
            for(int i = 0; i < t0Itr; i++)
            {
                for(int j = 0; j < numOftrees; j++)
                {
                    //if (j < numOftrees-1)
                    //    s += Resaults.ElementAt(j).Data.ElementAt(i).Variance + ",";
                    //else
                    //    s += Resaults.ElementAt(j).Data.ElementAt(i).Variance + Environment.NewLine;
                    int p = GetPartition(partition, Resaults.ElementAt(j).Data.ElementAt(i).Variance);
                    res[i, p]++;
                }
            }
            //File.WriteAllText("C:\\Users\\Idan\\Desktop\\TestV.csv", s);
            return res;
        }

        public int GetPartition(double partition, double variance)
        {
            for (int i = 0; i < numOfPartitions-1; i++)
            {
                if (partition*i < variance && variance <= partition * (i + 1))
                {
                    return i;
                }
            }
            return numOfPartitions - 1;
        }

        public double GetPartitionsDelta(ConcurrentBag<ModelVarianceContainer> Resaults)
        {
            List<double> Vs = new List<double>();
            foreach(ModelVarianceContainer container in Resaults)
            {
                Vs.Add(container.Data.ElementAt(t0Itr).Variance);
            }
            double max = Vs.Max();
            return max / 10;
        }

        public void UpdateStartButton()
        {

            if (UserInputData == null || !UserInputData.Validate())
                UserInputValid = false;
            StartButton.Enabled = TreeValid && UserInputValid;
        }

        public void WriteToLog(string message, MessageType type)
        {
            string msg = DateTime.Now.ToLongTimeString() + ": " + message;
            if (Log.InvokeRequired)
            {
                Log.BeginInvoke((Action)(() =>
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
                }));
            }
            else
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
}
