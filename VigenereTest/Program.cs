using System.IO;


var alphabet = new Dictionary<int, char>()
{
    {0, 'а'}, {1, 'б'}, {2, 'в'}, {3, 'г'}, {4, 'д'}, {5, 'е'}, {6, 'ж'}, {7, 'з'}, {8, 'и'}, {9, 'й'},
    {10, 'к'}, {11, 'л'}, {12, 'м'}, {13, 'н'}, {14, 'о'}, {15, 'п'}, {16, 'р'}, {17, 'с'}, {18, 'т'},
    {19, 'у'}, {20, 'ф'}, {21, 'х'}, {22, 'ц'}, {23, 'ч'}, {24, 'ш'}, {25, 'щ'}, {26, 'ъ'}, {27, 'ы'}, {28, 'ь'},
    {29, 'э'}, {30, 'ю'}, {31, 'я'}
};

var revAlphabet = new Dictionary<char, int>()
{
    {'а', 0}, {'б', 1}, {'в', 2}, {'г', 3}, {'д', 4}, {'е', 5}, {'ж', 6}, {'з', 7}, {'и', 8}, {'й', 9},
    {'к', 10}, {'л', 11}, {'м', 12}, {'н', 13}, {'о', 14}, {'п', 15}, {'р', 16}, {'с', 17}, {'т', 18},
    {'у', 19}, {'ф', 20}, {'х', 21}, {'ц', 22}, {'ч', 23}, {'ш', 24}, {'щ', 25}, {'ъ', 26}, {'ы', 27}, {'ь', 28},
    {'э', 29}, {'ю', 30}, {'я', 31}
};

{
    int minKeyLength = 2;
    int maxKeyLength = 30;

    string[] stopWords = GetStopWords("H:\\StopWords.txt");
    string text = ReadTheText("H:\\Text.txt");
    text = ConvertTheText(text);
    text = Encode(text, "ячсонапрпсммывповафывкепфы");

    string key = GetCypherKey(text, stopWords, minKeyLength, maxKeyLength);

    Console.WriteLine(Decode(text, key));
    Console.WriteLine();
    Console.WriteLine(key);
}

string ReadTheText(string path)
{
    string text = "";
    String line;

    try
    {
        StreamReader sr = new StreamReader(path);
        line = sr.ReadLine();
        while (line != null)
        {
            text = text + line;
            line = sr.ReadLine();
        }
        sr.Close();
    }
    catch (Exception e)
    {
        Console.WriteLine("Exception: " + e.Message);
    }
    finally
    {
        Console.WriteLine("Everything is ok.");
    }

    return text;
}

string[] GetStopWords(string path)
{
    var stopWords = new List<string>();
    String line;

    try
    {
        StreamReader sr = new StreamReader(path);
        line = sr.ReadLine();
        while (line != null)
        {
            stopWords.Add(line);
            line = sr.ReadLine();
        }
        sr.Close();
    }
    catch (Exception e)
    {
        Console.WriteLine("Exception: " + e.Message);
    }
    finally
    {
        Console.WriteLine("Stop words is received.");
    }
    return stopWords.ToArray();
}

string ConvertTheText(string text)
{
    text = text.ToLower();
    text = text.Replace(" ", "");
    text = text.Replace("ё", "е");
    for (int i = 0; i < text.Length; i++)
    {
        if (!alphabet.ContainsValue(text[i]))
        {
            text = text.Remove(i, 1);
            i--;
        }
    }
    return text;
}

string Encode(string text, string key)
{
    char[] charText = text.ToCharArray();
    char[] charKey = key.ToCharArray();
    char[] cypherText = new char[text.Length];
    int keySize = charKey.Length;
    for (int i = 0; i < charText.Length; i++)
    {
        if (revAlphabet.ContainsKey(charText[i]))
        {
            cypherText[i] = alphabet[revAlphabet[charText[i]] ^ revAlphabet[charKey[i % keySize]]];
        }
        else
        {
            cypherText[i] = charText[i];
        }    
    }
    return new string(cypherText);
}

string Decode(string cypherText, string key)
{
    char[] charCypher = cypherText.ToCharArray();
    char[] charKey = key.ToCharArray();
    char[] text = new char[cypherText.Length];
    int keySize = charKey.Length;
    for (int i = 0; i < charCypher.Length; i++)
    {
        if (revAlphabet.ContainsKey(charCypher[i]))
        {
            text[i] = alphabet[revAlphabet[charCypher[i]] ^ revAlphabet[charKey[i % keySize]]];
        }
        else
        {
            text[i] = charCypher[i];
        }
    }
    return new string(text);
}


double[] CountFrequency(string text)
{
    double[] frequency = new double[alphabet.Count];
    for (int i = 0; i < frequency.Length; i++)
    {
        frequency[i] = 0;
    }
    for (int i = 0; i < text.Length; i++)
    {
        frequency[revAlphabet[text[i]]]++;
    }
    for (int i = 0; i < frequency.Length; i++)
    {
        frequency[i] /= text.Length;
    }
    return frequency;
}

double[][] CountBigramFrequency(string text)
{
    double[][] bigramFrequency = new double[alphabet.Count][];
    for(int i = 0; i < bigramFrequency.Length; i++)
    {
        bigramFrequency[i] = new double[alphabet.Count];
    }
    
    for(int i = 0; i < bigramFrequency.Length; i++)
    {
        for(int j = 0; j < bigramFrequency[i].Length; j++)
        {
            bigramFrequency[i][j] = 0;
        }
    }

    for(int i = 0; i < text.Length - 1; i++)
    {
        bigramFrequency[revAlphabet[text[i]]][revAlphabet[text[i + 1]]]++;
    }

    for (int i = 0; i < bigramFrequency.Length; i++)
    {
        for (int j = 0; j < bigramFrequency[i].Length; j++)
        {
            bigramFrequency[i][j] /= text.Length - 1;
        }
    }

    return bigramFrequency;
}

void PrintMatrix(double[][] mas)
{
    Console.Write("  ");
    for (int i = 0; i < mas.Length; i++)
    {
        Console.Write(alphabet[i] + "   ");
    }
    Console.WriteLine();
    for (int i = 0; i < mas.Length; i++)
    {
        Console.Write(alphabet[i] + " ");
        for (int j = 0; j < mas[i].Length; j++)
        {
            if (mas[i][j] != 0)
                Console.Write(String.Format("{0:0.0}", mas[i][j]) + " ");
            else
                Console.Write(mas[i][j] + "   ");
        }
        Console.WriteLine();
    }
}

double[] CountLiterals(string text)
{
    double[] count = new double[alphabet.Count];
    for (int i = 0; i < count.Length; i++)
    {
        count[i] = 0;
    }
    for (int i = 0; i < text.Length; i++)
    {
        count[revAlphabet[text[i]]]++;
    }
    return count;
}

double CalculateFriedmanIndex(string text)
{
    double friedmanIndex = 0;
    int length = text.Length;
    double[] count = CountLiterals(text);
    for (int i = 0; i < count.Length; i++)
    {
        friedmanIndex += (count[i]*(count[i] - 1))/(length*(length-1));
    }
    return friedmanIndex;
}

double CalculateMutualIndex(string text1, string text2)
{
    double mutualIndex = 0;
    for(int i = 0; i < alphabet.Count; i++)
    {
        mutualIndex += (CountLiterals(text1)[i] * CountLiterals(text2)[i]) / (text1.Length * text2.Length);
    }
    return mutualIndex;
}

string Shift(string text, int s)
{
    char[] charText = text.ToCharArray();
    for(int i = 0; i < charText.Length; i++)
    {
        charText[i] = alphabet[revAlphabet[charText[i]] ^ s];
    }
    return new string(charText);
}


int FindKeyLength(string text, int minKeyLen = 2, int maxKeyLen = 30)
{
    double friedman_index = 0.0553;
    var normIndexes = new List<double>();
    for(int i = minKeyLen; i < maxKeyLen; i++)
    {
        var indexes = new List<double>();
        for (int j = 0; j < i; j++)
        {
            var temp = new List<char>();
            for(int k = j; k < text.Length; k += i)
            {
                temp.Add(text[k]);
            }
            indexes.Add(CalculateFriedmanIndex(String.Join("", temp)));
        }

        double norm = 0;
        for(int p = 0; p < indexes.Count; p++)
        {
            indexes[p] -= friedman_index;
            norm += indexes[p]*indexes[p];
        }
        normIndexes.Add(Math.Sqrt(norm));
    }
    return normIndexes.IndexOf(normIndexes.Min()) + minKeyLen;
}

string[] FindKeys(string text, int minKeyLength = 2, int maxKeyLength = 30)
{
    double friedman_index = 0.0553;
    var keys = new List<string>();
    int keyLen = FindKeyLength(text, minKeyLength, maxKeyLength);
    var columns = new List<List<char>>();
    for (int i = 0; i < keyLen; i++)
    {
        var temp = new List<char>();
        for (int j = i; j < text.Length; j += keyLen)
        {
            temp.Add(text[j]);
        }
        columns.Add(temp);
    }
    int[,] mutualShifts = new int[keyLen - 1, keyLen - 1];
    for(int i = 0; i < keyLen - 1; i++)
    {
        for(int j = i + 1; j < keyLen; j++)
        {
            int shift = 0;
            double mlIndex = 0;
            for(int s = 0; s < alphabet.Count; s++)
            {
                double temp = CalculateMutualIndex(String.Join("", columns[i]), Shift(String.Join("", columns[j]), s));
                if (temp > mlIndex)
                {
                    mlIndex = temp;
                    shift = s;
                }
            }
            mutualShifts[i, j - 1] = shift;
        }
    }
    var indexDict = new Dictionary<int, double>();
    int[] difIndexes = new int[alphabet.Count];
    for(int i = 0; i < alphabet.Count; i++)
    {
        indexDict.Add(i, Math.Abs(CalculateFriedmanIndex(Shift(String.Join("", columns[0]), i)) - friedman_index));
    }

    int tc = 0;
    foreach (var pair in indexDict.OrderBy(pair => pair.Value))
    {
        difIndexes[tc] = pair.Key;
        tc++;
    }

    for (int i = 0; i < difIndexes.Length; i++)
    {
        keys.Add(alphabet[difIndexes[i]].ToString());
        for (int j = 1; j < keyLen; j++)
        {
            keys[i] += Shift(alphabet[difIndexes[i]].ToString(), mutualShifts[0, j - 1]);
        }
    }
    return keys.ToArray();
}

string GetCypherKey(string text, string[] stopWords, int minKeyLength, int maxKeyLength)
{
    string[] keys = FindKeys(text, minKeyLength, maxKeyLength);

    int[] coincidences = new int[keys.Length];
    for (int i = 0; i < keys.Length; i++)
    {
        string tempText = Decode(text, keys[i]);
        coincidences[i] = 0;
        foreach (string s in stopWords)
        {
            if (tempText.Contains(s))
                coincidences[i]++;
        }
    }
    return keys[Array.IndexOf(coincidences, coincidences.Max())];
}