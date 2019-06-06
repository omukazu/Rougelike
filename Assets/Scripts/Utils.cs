using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace Rougelike
{
    public struct Coordinates
    {
        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public struct RL
    {
        public RL(int r, int l)
        {
            R = r;
            L = l;
        }
        public int R { get; set; }
        public int L { get; set; }
    }

    public class CsvReader
    {
        public char delimiter = ',';
        public List<string[]> ReadFile(string path)
        {
            TextAsset csvFile = Resources.Load(path) as TextAsset;
            List<string[]> data = new List<string[]>();
            StringReader sr = new StringReader(csvFile.text);
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                data.Add(line.Split(delimiter));
            }
            return data;
        }
    }
}
