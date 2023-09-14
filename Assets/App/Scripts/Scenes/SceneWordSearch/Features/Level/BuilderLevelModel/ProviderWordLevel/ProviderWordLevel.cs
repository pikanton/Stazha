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
            string filePath = $"Assets/App/Resources/WordSearch/Levels/{levelIndex}.json";
            string jsonString = File.ReadAllText(filePath);
            LevelInfo levelInfo = JsonUtility.FromJson<LevelInfo>(jsonString);
            return levelInfo;
        }
    }
}