using System;
using System.IO;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using UnityEngine;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        public LevelInfo LoadLevelData(int levelIndex)
        {
            string resourcePath = $"WordSearch/Levels/{levelIndex}";
            TextAsset levelAsset = Resources.Load<TextAsset>(resourcePath);

            if (levelAsset != null)
            {
                string jsonString = levelAsset.text;
                LevelInfo levelInfo = JsonUtility.FromJson<LevelInfo>(jsonString);
                return levelInfo;
            }
            else
            {
                throw new Exception($"Level {levelIndex} not found in resources.");
            }
        }
    }
}