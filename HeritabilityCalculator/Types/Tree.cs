using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritabilityCalculator
{
    /// <summary>
    /// Parse a newick format string to c# class
    /// </summary>
    public class Tree
    {
        private int currentPosition;
        public string input;
        private List<double> depths = new List<double>();
        private double sum = 0;

        public double MaxDepth
        {
            get
            {
                return depths.Max();
            }
        }

        public Tree(string text)
        {
            input = new string(text.Where(c => !char.IsWhiteSpace(c)).ToArray());
            currentPosition = 0;
        }

        public bool ValidateTree(out string error)
        {
            bool success = true;
            error = "None";
            if (string.IsNullOrEmpty(input))
            {
                success = false;
                error = "There is no input in the system";
            }
            else
            {
                error = "Input is not in correct format";
                int index = 0;
                int right = 0;
                int left = 0;
                while (index < input.Length)
                {
                    if (input[index] == '(')
                        left++;
                    else if (input[index] == ')')
                        right++;
                    index++;
                }
                if (right == 0 || left == 0 || right != left)
                    success = false;
            }
            return success;
        }

        public Branch Parse()
        {
            return new Branch { SubBranches = ParseBranchSet() };
        }
        private List<Branch> ParseBranchSet()
        {
            double depth = 0;
            var ret = new List<Branch>();
            ret.Add(ParseBranch());
            while (PeekCharacter() == ',')
            {
                //double curmax = 
                currentPosition++; // ','
                ret.Add(ParseBranch());
            }
            return ret;
        }
        private Branch ParseBranch()
        {
            var tree = ParseSubTree();
            currentPosition++; // ':'
            tree.Length = ParseDouble();
            if (tree is Leaf)
            {
                depths.Add(sum);
                sum = 0;
            }
            else
            {
                sum += tree.Length;
            }
            //depth += tree.Length;
            return tree;
        }
        private Branch ParseSubTree()
        {
            if (char.IsLetter(PeekCharacter()))
            {
                return new Leaf { Name = ParseIdentifier() };
            }

            currentPosition++; // '('
            var branches = ParseBranchSet();
            currentPosition++; // ')'
            return new Branch { SubBranches = branches };
        }
        private string ParseIdentifier()
        {
            var identifer = "";
            char c;
            while ((c = PeekCharacter()) != 0 && (char.IsLetter(c) || c == '_'))
            {
                identifer += c;
                currentPosition++;
            }
            return identifer;
        }
        private double ParseDouble()
        {
            var num = "";
            char c;
            while ((c = PeekCharacter()) != 0 && (char.IsDigit(c) || c == '.'))
            {
                num += c;
                currentPosition++;
            }
            return double.Parse(num, CultureInfo.InvariantCulture);
        }
        private char PeekCharacter()
        {
            if (currentPosition >= input.Length - 1)
            {
                return (char)0;
            }
            return input[currentPosition + 1];
        }
    }
}
