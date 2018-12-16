using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritabilityCalculator
{
    public class TreeDrawData
    {
        public string Title { get; set; }
        public Branch Root { get; set; }
        public List<TraitValue> observed { get; set; }
        public double ModelVariance { get; set; }
        public double TotalVariance { get; set; }
        public double Liklihood { get; set; }
    }

    class TreeDraw
    {
        private TreeDrawData data;
        private string localPath = string.Empty;
        private double heritability;

        public TreeDraw(TreeDrawData treeData)
        {
            localPath = Environment.CurrentDirectory;
            data = treeData;
            heritability = treeData.TotalVariance / treeData.ModelVariance;
        }

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
            sb.AppendLine("<script src=" + localPath + "\\Graphics\\d3.v3.js></script>"); 
            sb.AppendLine("<link rel='stylesheet' href=" + localPath + "\\Graphics\\bootstrap.min.css></script>");
            sb.AppendLine("<h6 class='col-sm-12 title'>" + data.Title.ToUpper() + "</h6>");
            sb.AppendLine("<div id='tree'></div>");
            sb.AppendLine("<h3 class='title'>Observed Traits</h3>");
            sb.AppendLine("<div id='observed'></div>");
            sb.AppendLine("<h3 class='title' style='padding-top: 120px;'>Results</h3>");
            sb.AppendLine("<div id='resualts' class='col-sm-12 res'>");
            sb.AppendLine("<h4 class='col-sm-3'>Total Variance: " + data.ModelVariance + "</h4>");
            sb.AppendLine("<h4 class='col-sm-3'>Model Variance: " + data.TotalVariance + "</h4>");
            sb.AppendLine("<h4 class='col-sm-3'>Liklihood: " + data.Liklihood + "</h4>");
            sb.AppendLine("<h4 class='col-sm-3'>Heritability: " + heritability + "</h4>");
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("<script>");
            sb.AppendLine("var treeData = [");
            AddTreeData(data.Root, sb);
            sb.AppendLine("];");
            sb.AppendLine("var observedData = [");
            for (int i = 0; i < data.observed.Count; i++)
            {
                sb.AppendLine("{");
                sb.AppendLine("'name': '" + data.observed[i].value + "'");
                if (i < data.observed.Count-1)
                    sb.AppendLine(",'children': [");
            }
            for (int i = 0; i < data.observed.Count-1; i++)
            {
                sb.AppendLine("}]");
            }
            sb.AppendLine("}];");
            string s = File.ReadAllText(localPath + "\\Graphics\\treeViewController.js");
            sb.AppendLine(s);
            sb.AppendLine("</script>");
            if (!Directory.Exists(Path.Combine(localPath, "TreeResualts")))
                Directory.CreateDirectory(Path.Combine(localPath, "TreeResualts"));
            File.WriteAllText(Path.Combine(localPath, "TreeResualts", data.Title.Replace(" ", "") + ".html"), sb.ToString());
        }

        public void Open()
        {
            try
            {
                System.Diagnostics.Process.Start(@"chrome.exe", Path.Combine(localPath, "TreeResualts", data.Title.Replace(" ", "") + ".html"));
            }
            catch { }
        }

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
