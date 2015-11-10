using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace languageModel
{
    class Program
    {
        static void Main(string[] args)
        {

            Dictionary<String, int> Unigram, Bigram, Trigram;
            if (args.Length < 2)
                throw new Exception("Incorrect Argument length");

            string TrainingPath=args[0];
            string OutputPath = args[1];
            createDictionaries(TrainingPath, out Unigram, out Bigram,out Trigram);
            if (File.Exists(OutputPath))
            {
                File.Delete(OutputPath);
            }
            WriteDict(Unigram, OutputPath);
            WriteDict(Bigram, OutputPath);
            WriteDict(Trigram, OutputPath);
        }
        public static void WriteDict(Dictionary<String, int> Dict, String OutputPath)
        {
            //this is to sort a dictionary
            var sortedDict = from pair in Dict
                             orderby pair.Value ascending
                             select pair;
            using (FileStream fs = new FileStream(OutputPath, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter file = new StreamWriter(fs))
                {
                    foreach (KeyValuePair<String, int> pair in sortedDict)
                    {
                        file.WriteLine("{1}\t{0}", pair.Key, pair.Value);
                    }
                }
            }
        }

        //reads through training data and updates uni, bi and tri dictionaries
        public static void createDictionaries(String TrainingPath, out Dictionary<String, int> Unigram,
            out Dictionary<String, int> Bigram,out Dictionary<String, int> Trigram)
        {
            Unigram = new Dictionary<string, int>();
            Bigram = new Dictionary<string, int>();
            Trigram = new Dictionary<string, int>();
            string line;
            using (StreamReader file = new StreamReader(TrainingPath))
            {
                while ((line = file.ReadLine()) != null )
                {
                    if (String.IsNullOrEmpty(line))
                        continue;
                    line = "<s> " + line + " </s>";
                    string[] words = line.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
                    int bi, tri, wordsCount;
                    wordsCount = words.Length;
                    string unigramS, bigramS, trigramS;
                    for (int i = 0; i < wordsCount; i++)
                    {
                        bi = i + 1;
                        tri = i + 2;
                        unigramS = words[i];
                        if (Unigram.ContainsKey(unigramS))
                            Unigram[unigramS]++;
                        else
                            Unigram.Add(unigramS, 1);
                        StringBuilder sb = new StringBuilder();
                        if (bi < wordsCount)
                        {
                            sb.Append(words[i]);
                            sb.Append(" ");
                            sb.Append(words[bi]);
                            bigramS = sb.ToString();
                            if (Bigram.ContainsKey(bigramS))
                                Bigram[bigramS]++;
                            else
                                Bigram.Add(bigramS, 1);
                        }
                        if (tri < wordsCount)
                        {
                            sb.Append(" ");
                            sb.Append(words[tri]);
                            trigramS = sb.ToString();
                            if (Trigram.ContainsKey(trigramS))
                                Trigram[trigramS]++;
                            else
                                Trigram.Add(trigramS, 1);
                        }

                    }

                }
            }
        }
    }
}
