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
        public int currentPosition;
        private string input;

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
               // TODO: Validate input.
            }

            return success;
        }

        public Branch Parse()
        {
            return new Branch { SubBranches = ParseBranchSet() };
        }
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
