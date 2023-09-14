using System;
using UnityEngine;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using System.Collections.Generic;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        private const string _wordsListPath = "Assets/App/Resources/Fillwords/words_list.txt";
        private const string _packPath = "Assets/App/Resources/Fillwords/pack_0.txt";

        public GridFillWords LoadModel(int index)
        {
            CheckLevelValid(index);

            List<int> indexes = GetIndexes(index);
            List<string> words = GetWords(index);
            string letters = string.Join("",words);

            int size = (int)Math.Sqrt(indexes.Count);
            Vector2Int vector = new(size, size);
            GridFillWords grid = new(vector);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    char c = letters[indexes.IndexOf(i * size + j)];
                    CharGridModel model = new(c);
                    grid.Set(i, j, model);
                }
            }
            return grid;
        }

        private List<string> GetWords(int index)
        {
            string levelInfoLine = GetInfoLine(index);
            List<int> wordIndexes = GetWordIndexes(levelInfoLine);

            List<string> words = ReadLinesByIndexes(_wordsListPath, wordIndexes);

            return words;
        }

        private string GetInfoLine(int index)
        {
            using (StreamReader reader = new(_packPath))
            {
                int currentLine = 0;
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    currentLine++;
                    if (currentLine == index)
                    {
                        return line;
                    }
                }
                return null;
            }
        }

        private List<int> GetWordIndexes(string levelInfoLine)
        {
            List<string> splitString = levelInfoLine.Split(' ').ToList();
            List<int> wordIndexes = splitString
                .Where((x, i) => i % 2 == 0)
                .Select(x => int.Parse(x))
                .ToList();

            return wordIndexes;
        }

        private List<string> ReadLinesByIndexes(string filePath, List<int> indexes)
        {
            List<string> lines = new();

            using (StreamReader reader = new(filePath))
            {
                int currentLine = -1;
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    currentLine++;
                    if (indexes.Contains(currentLine))
                    {
                        lines.Add(line);
                    }
                }
            }
            return lines;
        }

        private List<int> GetIndexes(int index)
        {
            List<List<int>> indexesForWords = GetIndexesForWords(index);
            List<int> letterIndexes = indexesForWords.SelectMany(list => list).ToList();
            return letterIndexes;
        }

        private List<List<int>> GetIndexesForWords(int index)
        {
            string levelInfoLine = GetInfoLine(index);
            List<string> indexesStrings = GetIndexesStrings(levelInfoLine);

            List<List<int>> indexesForWords = indexesStrings
                .Select(s => s.Split(';').Select(x => int.Parse(x)).ToList())
                .ToList();

            return indexesForWords;
        }

        private List<string> GetIndexesStrings(string levelInfoLine)
        {
            List<string> splitString = levelInfoLine.Split(' ').Where((x, i) => i % 2 == 1).ToList();
            return splitString;
        }


        private void CheckLevelValid(int index)
        {
            List<int> letterIndexes = GetIndexes(index);
            List<string> words = GetWords(index);
            List<List<int>> indexesForWords = GetIndexesForWords(index);

            ValidateWordCount(words, indexesForWords);
            ValidateWordLengths(words, indexesForWords);
            ValidateIndexRanges(letterIndexes);
            EnsureUniqueIndexes(letterIndexes);
            ValidateGridSize(letterIndexes);
        }

        private void ValidateWordCount(List<string> words, List<List<int>> indexesForWords)
        {
            if (words.Count != indexesForWords.Count)
            {
                throw new Exception("The count of words does not match the count of indexes strings.");
            }
        }

        private void ValidateWordLengths(List<string> words, List<List<int>> indexesForWords)
        {
            for (int i = 0; i < words.Count; i++)
            {
                if (words[i].Length != indexesForWords[i].Count)
                {
                    throw new Exception("Word length does not match the number of indexes for word " + words[i]);
                }
            }
        }

        private void ValidateIndexRanges(List<int> letterIndexes)
        {
            foreach (int currentIndex in letterIndexes)
            {
                if (currentIndex < 0 || currentIndex >= letterIndexes.Count)
                {
                    throw new Exception("Invalid letter index: " + currentIndex);
                }
            }
        }

        private void EnsureUniqueIndexes(List<int> letterIndexes)
        {
            if (new HashSet<int>(letterIndexes).Count != letterIndexes.Count)
            {
                throw new Exception("Duplicate indexes found.");
            }
        }

        private void ValidateGridSize(List<int> letterIndexes)
        {
            double size = Math.Sqrt(letterIndexes.Count);
            if (size % 1 != 0)
            {
                throw new Exception("Impossible to create a square level.");
            }
        }
    }
}