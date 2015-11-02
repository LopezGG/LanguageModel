using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProbLM
{
    class Program
    {
        static void Main(string[] args)
        {
            string CountFile = @"C:\compling570\hw5_dir\examples\ngram_count_file";
            string LMFile = @"C:\compling570\hw5_dir\examples\lm_file";
            string line;

            Ngram unigram = new Ngram();
            Ngram bigram = new Ngram();
            Ngram trigram = new Ngram();
            using (StreamReader file = new StreamReader(CountFile))
            {

                while( (line = file.ReadLine())!=null)
                {
                    if (String.IsNullOrEmpty(line))
                        continue;
                    String[] words = line.Split(new string [] {"\t"}, StringSplitOptions.RemoveEmptyEntries);
                    int countWhiteSpaces = words[1].Count(Char.IsWhiteSpace);
                    if(countWhiteSpaces == 0)
                    {
                        readData(unigram, words);
                    }  
                    if (countWhiteSpaces == 1)
                    {
                        readData(bigram, words);
                    }
                    if (countWhiteSpaces == 2)
                    {
                        readData(trigram, words);
                    }
                }
            }
            using (StreamWriter fileWrite = new StreamWriter(LMFile))
            {
                fileWrite.WriteLine(@"\data\");
                fileWrite.WriteLine("ngram 1: type={0} token={1}", unigram.typeCount,unigram.tokenCount);
                fileWrite.WriteLine("ngram 2: type={0} token={1}", bigram.typeCount, bigram.tokenCount);
                fileWrite.WriteLine("ngram 3: type={0} token={1}", trigram.typeCount, trigram.tokenCount);

                fileWrite.WriteLine();
                fileWrite.WriteLine(@"\1-grams:");
                WriteGram(unigram, bigram, trigram, unigram, fileWrite);

                fileWrite.WriteLine();
                fileWrite.WriteLine(@"\2-grams:");
                WriteGram(unigram, bigram, trigram, bigram, fileWrite);

                fileWrite.WriteLine();
                fileWrite.WriteLine(@"\3-grams:");
                WriteGram(unigram, bigram, trigram, trigram, fileWrite);

                fileWrite.WriteLine();
                fileWrite.WriteLine(@"\end\");


            }
            
        }

        public static void readData(Ngram gram, String[] words)
        {
            int count;
            gram.typeCount++;
            count = Convert.ToInt32(words[0]);
            gram.tokenCount += count;
            if (gram.gramDict.ContainsKey(words[1]))
                throw new Exception("duplicate values");
            gram.gramDict.Add(words[1], count);
        }

        public static void WriteGram(Ngram unigram, Ngram bigram, Ngram trigram, Ngram writeGram, StreamWriter fileWrite)
        {
            double countw1;
            double prob, logprob;
            string output;
            foreach (KeyValuePair<String, int> pair in writeGram.gramDict)
            {
                string[] wordgram = pair.Key.Split(' ');
                if (pair.Value == 0)
                    throw new Exception("input contains key with 0 occurance");
                if (wordgram.Length == 1)
                    countw1 = unigram.tokenCount;
                else if (wordgram.Length == 2)
                    countw1 = unigram.gramDict.ContainsKey(wordgram[0]) ? Convert.ToDouble(unigram.gramDict[wordgram[0]]) : 0;
                else
                {
                    string temp = wordgram[0] + " " + wordgram[1];
                    countw1 = bigram.gramDict.ContainsKey(temp) ? Convert.ToDouble(bigram.gramDict[temp]) : 0;
                }
                if (countw1 > 0)
                {
                    prob = pair.Value / countw1;
                    logprob = Math.Log10(prob);
                }

                else
                {
                    prob = 0;
                    logprob = 0;
                }
                output = string.Format("{0} {1} {2} {3}", pair.Value, prob, logprob, pair.Key);
                fileWrite.WriteLine(output);

            }
        }
    }
}
