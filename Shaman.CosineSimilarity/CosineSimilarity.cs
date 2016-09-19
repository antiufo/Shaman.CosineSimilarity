using System;
using System.Collections.Generic;
using System.Globalization;

namespace Shaman.Runtime
{
    // http://blog.nishtahir.com/2015/09/19/fuzzy-string-matching-using-cosine-similarity/
    public static class CosineSimilarity
    {
        /**
         * @param terms values to analyze
         * @return a map containing unique 
         * terms and their frequency
         */
        private static Dictionary<T, int> GetTermFrequencyMap<T>(IEnumerable<T> terms)
        {
            var termFrequencyMap = new Dictionary<T, int>();
            foreach (var term in terms)
            {
                int n;
                termFrequencyMap.TryGetValue(term, out n);
                n++;
                termFrequencyMap[term] = n;
            }
            return termFrequencyMap;
        }


        public static double CalculateSimilarity(String text1, String text2)
        {
            return CalculateSimilarity<ValueString>(GetWords(text1), GetWords(text2));
            
        }
        public static double CalculateSimilarity<T>(IEnumerable<T> text1, IEnumerable<T> text2)
        {
            return CalculateSimilarity<T>(GetTermFrequencyMap(text1), GetTermFrequencyMap(text2));
            
        }
        /**
         * @param text1 
         * @param text2 
         * @return cosine similarity of text1 and text2
         */
        public static double CalculateSimilarity<T>(Dictionary<T, int> a, Dictionary<T, int> b)
        {


            //Get unique words from both sequences
            var intersection = new HashSet<T>(a.Keys);
            intersection.IntersectWith(b.Keys);
            

            double dotProduct = 0, magnitudeA = 0, magnitudeB = 0;

            //Calculate dot product
            foreach (var item in intersection)
            {
                dotProduct += a[item] * b[item];
            }

            //Calculate magnitude a
            foreach (var k in a.Keys)
            {
                var v = a[k];
                magnitudeA += v * v;
            }

            //Calculate magnitude b
            foreach (var k in b.Keys)
            {
                var v = b[k];
                magnitudeB += v * v;
            }

            //return cosine similarity
            return dotProduct / Math.Sqrt(magnitudeA * magnitudeB);
        }

        private static IEnumerable<ValueString> GetWords(string str)
        {



            var currentWordStart = 0;
            var currentWordLength = 0;
            bool prevCharWasSep = true;
            for (int i = 0; i < str.Length; i++)
            {
                bool curCharIsSep;
                char c = str[i];

                if (char.IsLetter(c) || char.IsDigit(c) || CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)

                {
                    if (prevCharWasSep)
                    {
                        currentWordLength = 0;
                        prevCharWasSep = false;
                    }
                    if (currentWordLength == 0) currentWordStart = i;
                    currentWordLength++;

                    curCharIsSep = CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.OtherLetter;
                }
                else
                {
                    curCharIsSep = true;
                }
                if (curCharIsSep)
                {
                    if (currentWordLength != 0)
                    {
                        yield return new ValueString(str, currentWordStart, currentWordLength);
                        currentWordLength = 0;
                    }
                    prevCharWasSep = true;
                }
            }
            if (currentWordLength != 0)
            {
                yield return new ValueString(str, currentWordStart, currentWordLength);
            }

        }

    }

}

