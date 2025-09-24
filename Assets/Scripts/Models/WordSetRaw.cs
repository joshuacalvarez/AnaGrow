using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnagrowLoader.Models
{
    internal class WordSetRaw
    {
        public int Index { get; set; }
        public Word FirstWord { get; set; }
        public Word SecondWord { get; set; }
        public Word ThirdWord { get; set; }
        public string Verified { get; set; }

    }
}
