using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bio.IO.Newick;
using Newtonsoft.Json;

namespace HeritabilityCalculator
{
    /// <summary>
    /// Main form for the project
    /// </summary>
    public partial class HeritabilityCalculator : Form
    {
        #region Fields

        /// <summary>
        /// Main tree from user input
        /// </summary>
        public Tree MainTree;
        /// <summary>
        /// Number of trees to be simulated
        /// </summary>
        public const int numOftrees = 20;
        /// <summary>
        /// Number of t0 iterations
        /// </summary>
        public const int t0Itr = 10;
        /// <summary>
        /// Number of variance partitions
        /// </summary>
        public const int numOfPartitions = 10;

        /// <summary>
        /// File dialog for user input
        /// </summary>
        public OpenFileDialog fileDialog;
        /// <summary>
        /// Root for main tree
        /// </summary>
        public Branch MainBranch;
        /// <summary>
        /// Newick parser tree
        /// </summary>
        public Bio.Phylogenetics.Tree tree;
        /// <summary>
        /// If tree is a valid newick tree
        /// </summary>
        public bool TreeValid = false;
        /// <summary>
        /// If user unput is valid
        /// </summary>
        public bool UserInputValid = false;
        /// <summary>
        /// Contains user input data
        /// </summary>
        public UserInput UserInputData;
        /// <summary>
        /// Contains total variance data
        /// </summary>
        public TotalVariance VT;
        /// <summary>
        /// Contains model variance data
        /// </summary>
        public ModelVariance VM;
        /// <summary>
        /// Contains all branches depths
        /// </summary>
        public List<double> depths = new List<double>();

        #endregion Fields

        /// <summary>
        /// Initialize a new instance of HeritabilityCalculator
        /// </summary>
        public HeritabilityCalculator()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            WriteToLog("Insert Data", MessageType.Important);
        }

        /// <summary>
        /// Message type for log coloring
        /// </summary>
        public enum MessageType
        {
            /// <summary>
            /// Info message type
            /// </summary>
            Info,
            /// <summary>
            /// Error message type
            /// </summary>
            Error,
            /// <summary>
            /// Success message type
            /// </summary>
            Success,
            /// <summary>
            /// Important message type
            /// </summary>
            Important
        }

        /// <summary>
        /// Open help file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "F1")
            {
                System.Diagnostics.Process.Start(Path.Combine(Environment.CurrentDirectory, "Help" ,"Help.pdf"));
            }
        }

        /// <summary>
        /// Open a file dialog for file input
        /// </summary>
        /// <param name="filter">What type of files to enable selection</param>
        /// <param name="filePath">The selected file path</param>
        /// <returns>The contants of the selected file</returns>
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

        /// <summary>
        /// Load the newick tree file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeBrowse_Click(object sender, EventArgs e)
        {
            string newickTree = GetFileString("Newick Tree (*.txt)|*.txt", out string path);
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
        int numOfLeevs = 0;
        /// <summary>
        /// Create the main tree from the user input file
        /// </summary>
        /// <param name="node">A node in the tree</param>
        /// <param name="branch">The local structre for the tree</param>
        /// <param name="edge">Length of nodes edge</param>
        /// <param name="max">The maximum length of the nodes vertebrae</param>
        public void CreateMainTree(Bio.Phylogenetics.Node node, Branch branch, double edge, double max)
        {
            max += edge;
            if (node.IsLeaf)
            {
                depths.Add(max);
                numOfLeevs++;
            }
            branch.Length = edge;
            branch.SubBranches = new List<Branch>();
            for (int i = 0; i < node.Children.Count; i++)
            {
                branch.SubBranches.Add(new Branch());
                CreateMainTree(node.Children.Keys.ElementAt(i), branch.SubBranches.ElementAt(i), node.Edges.ElementAt(i).Distance, max);
            }
        }

        /// <summary>
        /// Load the user input file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputBrowse_Click(object sender, EventArgs e)
        {
            string json = GetFileString("Input Data (*.json)|*.json", out string path);
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
                catch (Exception exp)
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

        /// <summary>
        /// Start calculation of the main algorithm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartButton_Click(object sender, EventArgs e)
        {
            ProgressLabel.Text = "Progress: 0%";
            ProgressLabel.Visible = true;
            BrowseTreeLabel.Visible = false;
            TreePathText.Visible = false;
            UserInputText.Visible = false;
            TreeBrowse.Visible = false;
            InputBrowse.Visible = false;
            StartButton.Visible = false;
            BrowseInputlabel.Visible = false;
            Log.Clear();
            WriteToLog("Starting Calculation...", MessageType.Important);

            CalculateProgressBar.Maximum = numOftrees;
            CalculateProgressBar.Step = 1;
            CalculateProgressBar.Value = 0;
            CalculateProgressBar.Visible = true;
  
            VT = new TotalVariance(UserInputData);
            List<Task> tasks = new List<Task>
            {
                Task.Factory.StartNew(() => VT.Calculate(this))
            };

            VM = new ModelVariance(UserInputData, tree, t0Itr)
            {
                deltaT = depths.Max() / 40
            };
            tasks.Add(Task.Factory.StartNew(() => Parallel.For(0, numOftrees, i => VM.Calculate(this))));
            await Task.WhenAll(tasks);

            WriteToLog("Processing Results...", MessageType.Important);
            double partition = GetPartitionsDelta(VM.Results);
            int[,] baskets = GetBaskets(VM.Results, partition);

            TreePathText.Visible = true;
            UserInputText.Visible = true;
            BrowseTreeLabel.Visible = true;
            TreeBrowse.Visible = true;
            InputBrowse.Visible = true;
            BrowseInputlabel.Visible = true;
            CalculateProgressBar.Visible = false;
            ProgressLabel.Visible = false;

            StartButton.Visible = true;

            int vtPartition = GetPartition(partition, VT.VT_Final_Result);
            int bestItr = GetMaxT0(baskets, vtPartition);

            int[] bestItrArr = new int[numOfPartitions];
            for (int i = 0; i < numOfPartitions; i++)
            {
                bestItrArr[i] = baskets[bestItr, i];
            }
            double[] liklihoods = GetLiklihoods(baskets, vtPartition);
            double[] vmResualts = GetVM(VM.Results, bestItr);
            double liklihood = bestItrArr[vtPartition] / (double)numOftrees;
            double[] X2 = GetX2(liklihoods, bestItr);
            Branch bestTree = VM.Results.ElementAt(0).GeneratedTree;
            List<TraitValue> traitValues = VM.Results.ElementAt(0).Data.ElementAt(bestItr).ObservedTraits;

            TreeDrawData data = new TreeDrawData()
            {
                Liklihood = liklihood,
                Observed = traitValues,
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
            TreeDraw treeDraw = new TreeDraw(data);
            treeDraw.Create();
            Task task = Task.Factory.StartNew((Action)(() =>
            {
                WriteToLog("Algorithem finished successfully" + Environment.NewLine + "Opening results view...", MessageType.Success);
                Thread.Sleep(4000);
            }));
            await task;
            treeDraw.Open();
        }

        /// <summary>
        /// Calculate X2 values for best iteration 
        /// </summary>
        /// <param name="liklihoods">Liklihoods for every t0 iteration in the VT partition</param>
        /// <param name="bestItr">Best iteration index</param>
        /// <returns>X2 values for best iteration</returns>
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

        /// <summary>
        /// Calculate liklihoods for every t0 iteration in the VT partition
        /// </summary>
        /// <param name="baskets">All calculated variances distributed in to partitions</param>
        /// <param name="vtPartition">VT partition index</param>
        /// <returns>Liklihoods for every t0 iteration in the VT partition</returns>
        public double[] GetLiklihoods(int[,] baskets, int vtPartition)
        {
            double[] res = new double[t0Itr];
            for (int i = 0; i < t0Itr; i++)
            {
                res[i] = baskets[i, vtPartition] / (double)numOftrees;
            }
            return res;
        }

        /// <summary>
        /// Calculate VM for best to iteration, min VM and max VM
        /// </summary>
        /// <param name="Resaults">Results data structure that holds all the algorithm data</param>
        /// <param name="bestItr">Best iteration index</param>
        /// <returns>VM for best to iteration, min VM and max VM</returns>
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

        /// <summary>
        /// Calculate the best t0 iteration
        /// </summary>
        /// <param name="baskets">All calculated variances distributed in to partitions</param>
        /// <param name="vtPartition">VT calculated partition</param>
        /// <returns>Best t0 iteration index</returns>
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

        /// <summary>
        /// Distribute all variance results in to partitions for every t0 iteration
        /// </summary>
        /// <param name="Resaults">Results data structure that holds all the algorithm data</param>
        /// <param name="partitionSize">VT calculated partition</param>
        /// <returns>Variances distributed in to partitions</returns>
        public int[,] GetBaskets(ConcurrentBag<ModelVarianceContainer> Resaults, double partitionSize)
        {
            int[,] res = new int[t0Itr, numOfPartitions];
            string s = string.Empty;
            for(int i = 0; i < t0Itr; i++)
            {
                for(int j = 0; j < numOftrees; j++)
                {
                    int p = GetPartition(partitionSize, Resaults.ElementAt(j).Data.ElementAt(i).Variance);
                    res[i, p]++;
                }
            }
            return res;
        }

        /// <summary>
        /// Calculate partition slot
        /// </summary>
        /// <param name="partitionSize">Size of one partition</param>
        /// <param name="variance">Variance to put in slot</param>
        /// <returns>Calculated variance slot</returns>
        public int GetPartition(double partitionSize, double variance)
        {
            for (int i = 0; i < numOfPartitions-1; i++)
            {
                if (partitionSize * i < variance && variance <= partitionSize * (i + 1))
                {
                    return i;
                }
            }
            return numOfPartitions - 1;
        }

        /// <summary>
        /// Calculate variance slot size
        /// </summary>
        /// <param name="Resaults">Results data structure that holds all the algorithm data</param>
        /// <returns>Variance slot size</returns>
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

        /// <summary>
        /// Update start button disabled state
        /// </summary>
        public void UpdateStartButton()
        {

            if (UserInputData == null || !UserInputData.Validate())
                UserInputValid = false;
            StartButton.Enabled = TreeValid && UserInputValid;
        }

        /// <summary>
        /// Handle log writing for the project
        /// </summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="type">Message type</param>
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

        /// <summary>
        /// Update calculations progress bar
        /// </summary>
        public void UpdateProgress()
        {
            if (CalculateProgressBar.InvokeRequired)
            {
                CalculateProgressBar.BeginInvoke((Action)(() =>
                {
                    CalculateProgressBar.Value++;
                }));
            }
            else
            {
                CalculateProgressBar.Value++;
            }
            if (ProgressLabel.InvokeRequired)
            {
                ProgressLabel.BeginInvoke((Action)(() =>
                {
                    ProgressLabel.Text = "Progress: " + ((double)CalculateProgressBar.Value / numOftrees) * 100 + "%";
                }));
            }
            else
            {
                ProgressLabel.Text = "Progress: " + ((double)CalculateProgressBar.Value / numOftrees) * 100 + "%";
            }
        }
    }
}
