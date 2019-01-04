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
        /// <summary>
        /// Algorithem current position
        /// </summary>
        private int currentPosition;
        /// <summary>
        /// Newick format string input
        /// </summary>
        public string input;
        /// <summary>
        /// All branches depths
        /// </summary>
        private List<double> depths = new List<double>();
        /// <summary>
        /// Sum of current branch
        /// </summary>
        private double sum = 0;
        /// <summary>
        /// Max depth of tree
        /// </summary>
        public double MaxDepth
        {
            get
            {
                return depths.Max();
            }
        }

        /// <summary>
        /// Creates instance of tree
        /// </summary>
        /// <param name="text">Newick format tree text</param>
        public Tree(string text)
        {
            input = new string(text.Where(c => !char.IsWhiteSpace(c)).ToArray());
            currentPosition = 0;
        }

        /// <summary>
        /// Validates that the inpu string is in valid newick format
        /// </summary>
        /// <param name="error">Error message</param>
        /// <returns>If format is valid</returns>
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

        /// <summary>
        /// Parse a single branch of tree
        /// </summary>
        /// <returns>A single branch of tree</returns>
        public Branch Parse()
        {
            return new Branch { SubBranches = ParseBranchSet() };
        }

        /// <summary>
        /// Parse a branch set of tree
        /// </summary>
        /// <returns>A branch set of tree</returns>
        private List<Branch> ParseBranchSet()
        {
            var ret = new List<Branch>();
            ret.Add(ParseBranch());
            while (PeekCharacter() == ',')
            {
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
            return tree;
        }

        /// <summary>
        /// Parse sub tree
        /// </summary>
        /// <returns>Sub tree</returns>
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

        /// <summary>
        /// Parse newick format symbols
        /// </summary>
        /// <returns>Newick format symbol</returns>
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

        /// <summary>
        /// Parse newick format number
        /// </summary>
        /// <returns>Newick format number</returns>
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

        /// <summary>
        /// Check current position of char
        /// </summary>
        /// <returns>Current char</returns>
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
