using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Manager
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        private List<Dictionary<string,object>> scoreData = new List<Dictionary<string,object>>();
        public void Awake()
        {
            LoadSaveData();
            if (!Directory.Exists("Save"))
            {
                Directory.CreateDirectory("Save");
            }
        }
        public void LoadSaveData()
        {
            if (!File.Exists("Save/Score.json"))
            {
                Debug.Log("Setting data loaded fail.");
                return;
            }
            var json = File.ReadAllText("Save/Score.json");
            scoreData = JsonConvert.DeserializeObject<List<Dictionary<string,object>>>(json);
            Debug.Log("Setting data loaded successfully.");
        }

        public void SaveScoreData()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
                Formatting = Formatting.Indented
            };
            var json = JsonConvert.SerializeObject(scoreData,settings);
            File.WriteAllText("Save/Score.json", json);
        }

        public void AddScore(string username,int level, float score)
        {
            var newEntry = new Dictionary<string, object>
            {
                { "username", username },
                { "level", level },
                { "score", score }
            };
            scoreData.Add(newEntry);
        }
        public List<Dictionary<string,object>> ScoreData
        {
            get => scoreData;
            set => scoreData = value;
        }
    }
}