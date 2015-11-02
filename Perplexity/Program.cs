using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perplexity
{
    class Program
    {
        static void Main(string[] args)
        {
            string LMFile = @"C:\compling570\hw5_dir\examples\lm_file";
            double lambda1 = 0.33;
            double lambda2 = 0.33;
            double lambda3 = 0.33;
            string inputFile = @"C:\compling570\hw5_dir\examples\test_data_ex";
            string outputFile = @"C:\compling570\hw5_dir\examples\ppl_file";
            Dictionary<String, double> GramDict = new Dictionary<string, double>();
            CreateDictionary(LMFile, GramDict);
            string line;
            StreamWriter Sw = new StreamWriter(outputFile);
            int sentenceCount=0;
            double totalPpl=0, totalLogprob=0,totalProb=0;
            string header;
            using (StreamReader Sr = new StreamReader(inputFile))
            {
                while ((line = Sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    sentenceCount++;
                    line = "<s> " + line + " </s>";
                    header = String.Format("Sent #{0}:{1}", sentenceCount, line);
                    Sw.WriteLine(header);
                    string[] words = line.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
                    string temp2,temp3;
                    double uniProb, biProb, triProb,totalWordProb,ppl;
                    string output;
                    double sum = 0;
                    int cnt = 0;
                    int oov = 0;
                    for (int i = 1; i < words.Length; i++)
                    {
                        if(i==1)
                        {
                            temp2 = words[0] + " " + words[1];
                            if(GramDict.ContainsKey(temp2))
                                biProb = GramDict[temp2];
                            else
                                biProb = 0;

                            if (GramDict.ContainsKey(words[i]))
                            {
                                uniProb = GramDict[words[i]];
                            }
                            else
                            {
                                oov++;
                                uniProb = 0;
                            }

                            totalWordProb = (lambda2 * biProb) + (lambda1 * uniProb);
                            totalWordProb = (totalWordProb > 0) ? Math.Log10(totalWordProb) : 0;
                            sum += totalWordProb;
                            cnt++;
                            if (biProb >0)
                                output = String.Format("{3}: lg P({1} | {0}) = {2}", words[0], words[1], totalWordProb,i);
                            else if (uniProb > 0 )
                                output = String.Format("{3}: lg P({1} | {0}) = {2} (unseen ngrams)", words[0], words[1], totalWordProb,i);
                            else
                                output = String.Format("{2}: lg P({1} | {0}) =  -inf (unknown word)", words[0], words[1],i);

                        }
                        else
                        {
                            temp3 = words[i-2] + " " + words[i-1] + " " + words[i];
                            temp2 = words[i - 1] + " " + words[i];
                            if (GramDict.ContainsKey(temp3))
                                triProb = GramDict[temp3];
                            else
                                triProb = 0;
                            if (GramDict.ContainsKey(temp2))
                                biProb = GramDict[temp2];
                            else
                                biProb = 0;

                            if (GramDict.ContainsKey(words[i]))
                            {
                                uniProb = GramDict[words[i]];
                            }
                            else
                            {
                                oov++;
                                uniProb = 0;
                            }
                            totalWordProb = (lambda3 * triProb) + (lambda2 * biProb) + (lambda1 * uniProb);
                            totalWordProb = (totalWordProb > 0) ? Math.Log10(totalWordProb) : 0;
                            sum += totalWordProb;
                            cnt++;
                            if (triProb > 0)
                                output = String.Format("{4}: lg P({2} | {0} {1}) = {3}", words[i - 2], words[i - 1], words[i], totalWordProb,i);
                            else if (biProb > 0 | uniProb > 0)
                                output = String.Format("{4}: lg P({2} | {0} {1}) = {3} (unseen ngrams)", words[i - 2], words[i - 1], words[i], totalWordProb,i);
                            else
                                output = String.Format("{3}: lg P({2} | {0} {1}) =  -inf (unknown word)", words[i - 2], words[i - 1], words[i],i);
                        }
                        Sw.WriteLine(output);
                    }
                    --cnt;
                    output = String.Format("1 sentence, {0} words, {1} OOVs",cnt,oov);
                    Sw.WriteLine(output);
                    //TODO:check this logic plus 1 to include the last </s>
                    int actualWords =(cnt-oov+1);
                    totalProb = (sum * (-1)) / actualWords;
                    //for final calc
                    totalLogprob += totalProb;
                    ppl = Math.Pow(10, totalProb);
                    //for final calc
                    totalPpl += ppl;
                    output = String.Format("lgprob={0} ppl={1}", sum, ppl);
                    Sw.WriteLine(output);
                    Sw.WriteLine();
                    Sw.WriteLine();
                    Sw.WriteLine();
                }
            }
            Sw.Close();


        }
        public static void CreateDictionary(String LMFile, Dictionary<String, double> GramDict)
        {
            string line, key;
            double prob;
            using (StreamReader sr = new StreamReader(LMFile))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (String.IsNullOrWhiteSpace(line))
                        continue;
                    else if (line.Contains("data") || line.Contains(@"gram") || line.Contains(@"end"))
                        continue;
                    string[] words = line.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
                    prob = Convert.ToDouble(words[1]);
                    int count = 0, index = 0;
                    while (count < 3)
                    {
                        if (line[index++] == ' ')
                            count++;
                        if (index >= line.Length)
                            throw new Exception("incorrect number of entries in lm_file");
                    }
                    key = line.Substring(index);
                    GramDict.Add(key,prob);
                }
            }
        }
    }
}
