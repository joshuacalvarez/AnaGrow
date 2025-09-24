using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AnagrowLoader.Models
{
    internal class WordSet
    {
        public int Index { get; set; }
        public List<Word> Words { get; set; }

        public WordSet(int index, List<Word> words)
        {
            Index = index;
            Words = words;
        }

        public override string ToString()
        {

            return "Index: " + Index + " Hint 1: " + Words.ElementAt(0).Hint + " Word 1: " + Words.ElementAt(0).Answer + " Hint 2: " + Words.ElementAt(1).Hint + " Word 2: " + Words.ElementAt(1).Answer + " Hint 3: " + Words.ElementAt(2).Hint + " Word 3: " + Words.ElementAt(2).Answer + "\n";
        }
    }
}
