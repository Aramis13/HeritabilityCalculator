using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HeritabilityCalculator
{
    /// <summary>
    /// Data required for final display
    /// </summary>
    public class TreeDrawData
    {
        /// <summary>
        /// File title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Root node
        /// </summary>
        public Branch Root { get; set; }
        /// <summary>
        /// Observed phenotypic trait values
        /// </summary>
        public List<TraitValue> Observed { get; set; }
        /// <summary>
        /// Model variance min, mid and max
        /// </summary>
        public double[] ModelVariance { get; set; }
        /// <summary>
        /// Total variance
        /// </summary>
        public double TotalVariance { get; set; }
        /// <summary>
        /// Maximum liklihood estimation
        /// </summary>
        public double Liklihood { get; set; }
        /// <summary>
        /// Partition size
        /// </summary>
        public double Partition { get; set; }
        /// <summary>
        /// Number of partitions
        /// </summary>
        public int NumOfPartitions { get; set; }
        /// <summary>
        /// X2 for every iteration
        /// </summary>
        public double[] X2 { get; set; }
        /// <summary>
        /// Best iteration index
        /// </summary>
        public double BestItr { get; set; }
        /// <summary>
        /// Results for best iteration index
        /// </summary>
        public int[] BestItrRes { get; set; }
        /// <summary>
        /// Deltat
        /// </summary>
        public double DeltaT { get; set; }
        /// <summary>
        /// Number of simulated trees
        /// </summary>
        public int NumOfTrees { set; get; }
    }

    /// <summary>
    /// Contains final results
    /// </summary>
    class TreeDraw
    {
        /// <summary>
        /// Tree draw data
        /// </summary>
        private TreeDrawData data;
        /// <summary>
        /// Local exe path
        /// </summary>
        private readonly string localPath = string.Empty;
        /// <summary>
        /// Minimum heritability value
        /// </summary>
        private readonly double heritabilityMin;
        /// <summary>
        /// Midium heritability value
        /// </summary>
        private readonly double heritability;
        /// <summary>
        /// Maximum heritability value
        /// </summary>
        private readonly double heritabilityMax;
        /// <summary>
        /// Tolerance for X2 statistics
        /// </summary>
        private const double tolerance = 0.7;

        /// <summary>
        /// Create instance of tree draw
        /// </summary>
        /// <param name="treeData">Data required for final display</param>
        public TreeDraw(TreeDrawData treeData)
        {
            localPath = Environment.CurrentDirectory;
            data = treeData;
            heritabilityMin = treeData.ModelVariance[0] / treeData.TotalVariance;
            heritability = treeData.ModelVariance[1] / treeData.TotalVariance;
            heritabilityMax = treeData.ModelVariance[2] / treeData.TotalVariance;
        }

        /// <summary>
        /// Create dynamic html file to display final results
        /// </summary>
        public void Create()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='en'>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset='utf - 8'>");
            sb.AppendLine("<title>" + data.Title + "</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body {background: linear-gradient(to right, #757F9A, #D7DDE8);}");
            sb.AppendLine(".svg{width:100%}");
            sb.AppendLine(".observed-traits {position: absolute; top: 450px;}");
            sb.AppendLine(".res {top: 70px; text-align: center; display: inline-flex; height: 100px;}");
            sb.AppendLine(".node {cursor: pointer;}");
            sb.AppendLine(".title {text-align: center; font-family: cursive; font-size: 50px;}");
            sb.AppendLine(".node circle {fill: #fff;stroke: steelblue;stroke - width: 3px;}");
            sb.AppendLine(".node text {font: 12px sans - serif;}");
            sb.AppendLine(".link {fill: none;stroke: #ccc;stroke - width: 2px;}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<script src=" + localPath + "\\Graphics\\jquery-3.3.1.min.js></script>");
            sb.AppendLine("<script src=" + localPath + "\\Graphics\\d3.v3.js></script>");
            sb.AppendLine("<script src=" + localPath + "\\Graphics\\mdb.js></script>");
            sb.AppendLine("<link rel='stylesheet' href=" + localPath + "\\Graphics\\bootstrap.min.css></script>");
            sb.AppendLine("<h6 class='col-sm-12 title'>" + data.Title.ToUpper() + "</h6>");
            sb.AppendLine("<div id='tree' class='col-sm-12'></div>");
            sb.AppendLine("<h3 class='title'>Observed Traits</h3>");
            sb.AppendLine("<div id='observed'></div>");
            sb.AppendLine("<h3 class='title' style='padding-top: 120px; padding-bottom: 25px;'>Results</h3>");
            sb.AppendLine("<div class='col-sm-6' style='float:left;'>");
            sb.AppendLine("<canvas id=\"barchart\" style=\"width: 500px; \"></canvas>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='col-sm-6' style='float:right;'>");
            sb.AppendLine("<canvas id=\"linechart\" style=\"width: 500px; \"></canvas>");
            sb.AppendLine("</div>");          
            sb.AppendLine("<div id='resualts' class='col-sm-12 res'>");
            sb.AppendLine("<h4 class='col-sm-4'>Total Variance: " + String.Format("{0:0.00}", data.TotalVariance) + "</h4>");
            sb.AppendLine("<h4 class='col-sm-4'>Model Variance: " + String.Format("{0:0.00}", data.ModelVariance[1]) + "</h4>");
            sb.AppendLine("<h4 class='col-sm-4'>Liklihood: " + data.Liklihood * 100 + "%</h4>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div id='heritability' class='col-sm-12 res'>");
            sb.AppendLine("<h4 class='col-sm-4'>Heritability (Min): " + String.Format("{0:0.00}", heritabilityMax) + "</h4>");
            sb.AppendLine("<h4 class='col-sm-4' style='font-weight: 900;'>Heritability: " + String.Format("{0:0.00}", heritability) + "</h4>");
            sb.AppendLine("<h4 class='col-sm-4'>Heritability (Max): " + String.Format("{0:0.00}", heritabilityMin) + "</h4>");
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("<script>");
            sb.AppendLine("var treeData = [");
            AddTreeData(data.Root, sb);
            sb.AppendLine("];");
            sb.AppendLine("var observedData = [");
            for (int i = 0; i < data.Observed.Count; i++)
            {
                sb.AppendLine("{");
                sb.AppendLine("'name': '" + data.Observed[i].value + "'");
                if (i < data.Observed.Count-1)
                    sb.AppendLine(",'children': [");
            }
            for (int i = 0; i < data.Observed.Count-1; i++)
            {
                sb.AppendLine("}]");
            }
            sb.AppendLine("}];");
            string s = File.ReadAllText(localPath + "\\Graphics\\treeViewController.js");
            sb.AppendLine(s);

            sb.AppendLine("var options = {");
            sb.AppendLine("scales: {");
            sb.AppendLine("yAxes: [{");
            sb.AppendLine("scaleLabel: {");
            sb.AppendLine("display: true,");
            sb.AppendLine("labelString: 'Percentage %'");
            sb.AppendLine("}");
            sb.AppendLine(" }],");
            sb.AppendLine("xAxes: [{");
            sb.AppendLine("scaleLabel: {");
            sb.AppendLine("display: true,");
            sb.AppendLine("labelString: 'Variance'");
            sb.AppendLine("}");
            sb.AppendLine(" }]");
            sb.AppendLine("}");
            sb.AppendLine("};");
            sb.AppendLine("var options1 = {");
            sb.AppendLine("scales: {");
            sb.AppendLine("yAxes: [{");
            sb.AppendLine("scaleLabel: {");
            sb.AppendLine("display: true,");
            sb.AppendLine("labelString: 'X²'");
            sb.AppendLine("}");
            sb.AppendLine(" }],");
            sb.AppendLine("xAxes: [{");
            sb.AppendLine("scaleLabel: {");
            sb.AppendLine("display: true,");
            sb.AppendLine("labelString: '∆t'");
            sb.AppendLine("}");
            sb.AppendLine(" }]");
            sb.AppendLine("}");
            sb.AppendLine("};");

            sb.AppendLine("var ctx = document.getElementById(\"barchart\").getContext('2d');");
            sb.AppendLine("var myChart = new Chart(ctx, {");
            sb.AppendLine("type: 'bar',");
            sb.AppendLine("data: {");
            sb.Append("labels: [");
            
            for (int i = 0; i < data.NumOfPartitions; i++)
            {
                if (i < data.NumOfPartitions-1)
                    sb.Append("'" + String.Format("{0:0.00}", data.Partition * i) + "-" + String.Format("{0:0.00}", data.Partition * (i+1)) + "',");
                else
                    sb.Append("'" + String.Format("{0:0.00}", data.Partition * i) + "-" + String.Format("{0:0.00}", data.Partition * (i + 1)) + "'");
            }
            sb.AppendLine("],");
            sb.AppendLine("datasets: [{");
            sb.AppendLine("label: 'Elongation " + String.Format("{0:0.00}", data.DeltaT * data.BestItr) + "',");
            sb.Append("data: [");
            for (int i = 0; i < data.NumOfPartitions; i++)
            {
                if (i < data.NumOfPartitions - 1)
                    sb.Append(String.Format("{0:0.00}", (data.BestItrRes[i] / (double)data.NumOfTrees) * 100) + ",");
                else
                    sb.Append(String.Format("{0:0.00}", (data.BestItrRes[i] / (double)data.NumOfTrees) * 100));
            }
            sb.AppendLine("],");
            sb.AppendLine("backgroundColor: [");
            sb.AppendLine("'rgba(255, 99, 132, 0.2)',");
            sb.AppendLine("'rgba(54, 162, 235, 0.2)',");
            sb.AppendLine("'rgba(255, 206, 86, 0.2)',");
            sb.AppendLine("'rgba(75, 192, 192, 0.2)',");
            sb.AppendLine("'rgba(153, 102, 255, 0.2)',");
            sb.AppendLine("'rgba(255, 159, 64, 0.2)'");
            sb.AppendLine("],");

            sb.AppendLine("borderColor: [");
            sb.AppendLine("'rgba(255,99,132,1)',");
            sb.AppendLine("'rgba(54, 162, 235, 1)',");
            sb.AppendLine("'rgba(255, 206, 86, 1)',");
            sb.AppendLine("'rgba(75, 192, 192, 1)',");
            sb.AppendLine("'rgba(153, 102, 255, 1)',");
            sb.AppendLine("'rgba(255, 159, 64, 1)'");
            sb.AppendLine("],");
            sb.AppendLine("borderWidth: 1");
            sb.AppendLine("}]");
            sb.AppendLine("},");
            sb.AppendLine("options: options");
            sb.AppendLine("});");

            sb.AppendLine("var ctx1 = document.getElementById(\"linechart\").getContext('2d');");
            sb.AppendLine("var myChart = new Chart(ctx1, {");
            sb.AppendLine("type: 'line',");
            sb.AppendLine("data: {");
            sb.Append("labels: [");

            for (int i = 0; i < data.X2.Length; i++)
            {
                if (i < data.X2.Length - 1)
                    sb.Append("'" + String.Format("{0:0.00}", data.DeltaT * i) + "',");
                else
                    sb.Append("'" + String.Format("{0:0.00}", data.DeltaT * i) + "'");
            }
            sb.AppendLine("],");
            sb.AppendLine("datasets: [{");
            sb.AppendLine("label: 'X² Distribution',");
            sb.Append("data: [");
            for (int i = 0; i < data.X2.Length; i++)
            {
                if (i < data.X2.Length - 1)
                    sb.Append(data.X2[i] + ",");
                else
                    sb.Append(data.X2[i]);
            }
            sb.AppendLine("],");
            sb.AppendLine("backgroundColor: [");
            sb.AppendLine("'rgba(255, 99, 132, 0.2)',");
            sb.AppendLine("'rgba(54, 162, 235, 0.2)',");
            sb.AppendLine("'rgba(255, 206, 86, 0.2)',");
            sb.AppendLine("'rgba(75, 192, 192, 0.2)',");
            sb.AppendLine("'rgba(153, 102, 255, 0.2)',");
            sb.AppendLine("'rgba(255, 159, 64, 0.2)'");
            sb.AppendLine("],");

            sb.AppendLine("borderColor: [");
            sb.AppendLine("'rgba(255,99,132,1)',");
            sb.AppendLine("'rgba(54, 162, 235, 1)',");
            sb.AppendLine("'rgba(255, 206, 86, 1)',");
            sb.AppendLine("'rgba(75, 192, 192, 1)',");
            sb.AppendLine("'rgba(153, 102, 255, 1)',");
            sb.AppendLine("'rgba(255, 159, 64, 1)'");
            sb.AppendLine("],");
            sb.AppendLine("borderWidth: 1");
            sb.AppendLine("},{");
            sb.Append("data: [");
            for (int i = 0; i < data.X2.Length; i++)
            {
                if (i < data.X2.Length - 1)
                    sb.Append(tolerance + ", ");
                else
                    sb.Append(tolerance);
            }
            sb.AppendLine("],");
            sb.AppendLine("label: 'ε Tolerance',");
            sb.AppendLine("borderColor: [");
            sb.AppendLine("'#6f42c1'");
            sb.AppendLine("],");
            sb.AppendLine("fill: false,");
            sb.AppendLine("borderWidth: 1");

            sb.AppendLine("}]");
            sb.AppendLine("},");
            sb.AppendLine("options: options1");
            sb.AppendLine("});");

            sb.AppendLine("</script>");
            if (!Directory.Exists(Path.Combine(localPath, "TreeResualts")))
                Directory.CreateDirectory(Path.Combine(localPath, "TreeResualts"));
            File.WriteAllText(Path.Combine(localPath, "TreeResualts", data.Title.Replace(" ", "") + ".html"), sb.ToString());
        }

        /// <summary>
        /// Open the created file in a chrome browser
        /// </summary>
        public void Open()
        {
            try
            {
                System.Diagnostics.Process.Start(@"chrome.exe", Path.Combine(localPath, "TreeResualts", data.Title.Replace(" ", "") + ".html"));
            }
            catch { }
        }

        /// <summary>
        /// Add all tree nodes to js tree object
        /// </summary>
        /// <param name="node">Tree node</param>
        /// <param name="sb">String for tree build</param>
        private void AddTreeData(Branch node, StringBuilder sb)
        {
            sb.AppendLine("{");
            sb.AppendLine("'name': '" + node.TraitValue.value + "'");
            if (node.SubBranches.Count > 0)
            {
                sb.AppendLine(",'children': [");
                for (int i = 0; i < node.SubBranches.Count; i++)
                {
                    AddTreeData(node.SubBranches[i], sb);
                    if (i < node.SubBranches.Count-1)
                        sb.AppendLine(",");
                }
                sb.AppendLine("]");
            }
            sb.Append("}");
        }
    }
}
