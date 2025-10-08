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
        public List<Word> Set { get; set; }

        public WordSet(int index, List<Word> words)
        {
            Index = index;
            Set = words;
        }

        public override string ToString()
        {

            return "Index: " + Index + " Hint 1: " + Set.ElementAt(0).Hint + " Word 1: " + Set.ElementAt(0).Answer + " Hint 2: " + Set.ElementAt(1).Hint + " Word 2: " + Set.ElementAt(1).Answer + " Hint 3: " + Set.ElementAt(2).Hint + " Word 3: " + Set.ElementAt(2).Answer + "\n";
        }

        public List<Word> getWordSet()
        {
            return Set;
        }

        public Word getWordByIdx(int idx)
        {
            return Set[idx];
        }
    }
}
