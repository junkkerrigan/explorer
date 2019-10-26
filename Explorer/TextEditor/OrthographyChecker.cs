using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public static class OrthographyChecker
    {
        private static List<string> Vocabulary = new List<string>()
        {
            "a",
            "B",
            "b",
            "sd",
            "hello",
            "world",
            "oop",
            "foo",
            "bar",
            "lose",
        };

        private static int Min(int a, int b, int c) => Math.Min(Math.Min(a, b), c);

        private static int DamerauLevenshteinDistance(string firstWord, string secondWord)
        {
            int n = firstWord.Length + 1;
            int m = secondWord.Length + 1;

            int[,] dist = new int[n, m];

            for (int i = 0; i < n; i++)
            {
                dist[i, 0] = i;
            }

            for (int j = 0; j < m; j++)
            {
                dist[0, j] = j;
            }

            for (int i = 1; i < n; i++)
            {
                for (int j = 1; j < m; j++)
                {
                    int cost = firstWord[i - 1] == secondWord[j - 1] ? 0 : 1;

                    dist[i, j] = Min(dist[i - 1, j] + 1,          
                        dist[i, j - 1] + 1, dist[i - 1, j - 1] + cost); 

                    if (i > 1 && j > 1
                        && firstWord[i - 1] == secondWord[j - 2]
                        && firstWord[i - 2] == secondWord[j - 1])
                    {
                        dist[i, j] = Math.Min(dist[i, j], dist[i - 2, j - 2] + cost); 
                    }
                }
            }

            return dist[n - 1, m - 1];
        }

        public static bool IsCorrect(string word)
        {
            return Vocabulary.Contains(word);
        }

        public static List<string> GetSimilar(string word)
        {
            int minDist = word.Length;
            List<string> similar = new List<string>();

            foreach(string suggestion in Vocabulary)
            {
                int curDist = DamerauLevenshteinDistance(word, suggestion);
                
                if(curDist < minDist)
                {
                    minDist = curDist;

                    similar.Clear();
                    similar.Add(suggestion);
                }
                else if (curDist == minDist)
                {
                    similar.Add(suggestion);
                }
            }

            return similar;
        }
    }
}
