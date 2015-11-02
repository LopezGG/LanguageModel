using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProbLM
{
    class Ngram
    {

        public int tokenCount;
        public int typeCount;
        public Dictionary <String,int> gramDict;
        public Ngram()
        {
            tokenCount = 0;
            typeCount = 0;
            gramDict = new Dictionary<string, int>();
        }
    }
}
