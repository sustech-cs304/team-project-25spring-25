using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Loader
{
    public class ScoreLoader
    {
        private List<Dictionary<string,int>> scoreData = new List<Dictionary<string,int>>();
        public void Awake()
        {
            LoadSaveData();
        }
        public void LoadSaveData()
        {
            var jsonFile = Resources.Load<TextAsset>("Json/Setting");
            if (jsonFile != null)
            {
                scoreData = JsonConvert.DeserializeObject<List<Dictionary<string,int>>>(jsonFile.text);
                Debug.Log("Setting data loaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to load Setting.json");
            }
        }

        public List<Dictionary<string,int>> ScoreData
        {
            get => scoreData;
            set => scoreData = value;
        }
    }
}