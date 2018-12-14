using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritabilityCalculator
{
    class TreeDraw
    {
        private string title = string.Empty;
        private Branch Root;
        private string localPath = string.Empty;

        public TreeDraw(string header, Branch root)
        {
            title = header;
            Root = root;
            localPath = Environment.CurrentDirectory;
        }

        public void Create()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='en'>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset='utf - 8'>");
            sb.AppendLine("<title>" + title + "</title>");
            sb.AppendLine("<style>");
            sb.AppendLine(".node {cursor: pointer;}");
            sb.AppendLine(".node circle {fill: #fff;stroke: steelblue;stroke - width: 3px;}");
            sb.AppendLine(".node text {font: 12px sans - serif;}");
            sb.AppendLine(".link {fill: none;stroke: #ccc;stroke - width: 2px;}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");          
            sb.AppendLine("<script src=" + localPath + "\\Graphics\\d3.v3.js></script>");
            sb.AppendLine("</body>");
            sb.AppendLine("<script>");
            sb.AppendLine("var treeData = [");
            AddTreeData(Root, sb);
            sb.AppendLine("];");
            string s = File.ReadAllText(localPath + "\\Graphics\\treeViewController.js");
            sb.AppendLine(s);
            sb.AppendLine("</script>");
            if (!Directory.Exists(Path.Combine(localPath, "TreeResualts")))
                Directory.CreateDirectory(Path.Combine(localPath, "TreeResualts"));
            File.WriteAllText(Path.Combine(localPath, title + ".html"), sb.ToString());
        }

        public void Open()
        {
            try
            {
                System.Diagnostics.Process.Start(@"chrome.exe", Path.Combine(localPath, title + ".html"));
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
