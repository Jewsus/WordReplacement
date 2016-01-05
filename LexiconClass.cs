using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexicon_Plugin
{
    public static class LexiconClass
    {
        public static Random rand = new Random();

        public static string Lexiconify(string msg)
        {
            char[] array = new char[0];
            string[] array2 = msg.Split(new char[]
            {
                ' '
            });
            string result;
            if (LexiconClass.rand.Next(0, 100) <= LexiconPlugin.config.HurrDurrChancePercentage && LexiconPlugin.config.ReplaceWithHurrDurr)
            {
                result = LexiconClass.ReplaceWithHurrDurr(msg);
            }
            else if (LexiconClass.rand.Next(0, 100) <= LexiconPlugin.config.RandomPhrasesChancePercentage && LexiconPlugin.config.SayRandomPhrases)
            {
                result = LexiconClass.SayRandomBullshit();
            }
            else
            {
                for (int i = 0; i < array2.Length; i++)
                {
                    string key = array2[i].ToLower();
                    if (LexiconPlugin.config.LexiconWords.ContainsKey(key))
                    {
                        array = LexiconPlugin.config.LexiconWords[key].ToArray<char>();
                        for (int j = 0; j < array.Length; j++)
                        {
                            if (j < array2[i].Length)
                            {
                                if (char.IsUpper(array2[i][j]))
                                {
                                    array[j] = char.ToUpper(array[j]);
                                }
                            }
                        }
                        array2[i] = new string(array);
                    }
                }
                string text = string.Join(" ", array2);
                if (LexiconClass.rand.Next(0, 100) <= LexiconPlugin.config.TypoChancePercentage && LexiconPlugin.config.InsertRandomTypos)
                {
                    text = LexiconClass.InsertRandomTypo(string.Join(" ", array2));
                }
                result = text;
            }
            return result;
        }

        public static string SayRandomBullshit()
        {
            return LexiconPlugin.config.RandomPhrases[LexiconClass.rand.Next(0, LexiconPlugin.config.RandomPhrases.Length)];
        }

        public static string ReplaceWithHurrDurr(string s)
        {
            string[] array = s.Split(new char[]
            {
                ' '
            });
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ((LexiconClass.rand.Next(0, 2) == 0) ? "hurr" : "durr");
            }
            return string.Join(" ", array);
        }

        public static string InsertRandomTypo(string s)
        {
            List<char> list = s.ToList<char>();
            if (LexiconClass.rand.Next(0, 5) == 0 && list.Count > 0)
            {
                int index = LexiconClass.rand.Next(0, s.Length);
                switch (LexiconClass.rand.Next(0, 3))
                {
                    case 0:
                        list.RemoveAt(index);
                        break;
                    case 1:
                        list.Insert(index, LexiconClass.RandChar());
                        s = new string(list.ToArray());
                        break;
                    case 2:
                        list[index] = LexiconClass.RandChar();
                        s = new string(list.ToArray());
                        break;
                }
            }
            return new string(list.ToArray());
        }

        public static char RandChar()
        {
            string text = "abcdefghijklmnopqrstuvwxyz ',.;123456789";
            return text[LexiconClass.rand.Next(0, text.Length)];
        }
    }
}
