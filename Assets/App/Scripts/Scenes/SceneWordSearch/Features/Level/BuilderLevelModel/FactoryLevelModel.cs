using System;
using System.Collections.Generic;
using System.Linq;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{
    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();

            model.LevelNumber = levelNumber;

            model.Words = value.words;
            model.InputChars = BuildListChars(value.words);

            return model;
        }

        private List<char> BuildListChars(List<string> words)
        {
            Dictionary<char, int> characterCountMap = new();
            foreach (string word in words)
            {
                HashSet<char> chars = new(word);
                foreach (char ch in chars)
                {
                    if (characterCountMap.ContainsKey(ch))
                        characterCountMap[ch] = Math.Max(characterCountMap[ch], word.Count(x => x == ch));
                    else
                        characterCountMap[ch] = word.Count(x => x == ch);
                }
            }
            List<char> result = string.Concat(characterCountMap.Select(p => new string(p.Key, p.Value)))
                                      .ToCharArray()
                                      .ToList();
            return result;
        }
    }
}