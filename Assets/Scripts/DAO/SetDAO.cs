using AnagrowLoader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEditor.PackageManager.Requests;
using UnityEngine.Networking;
using AnagrowLoader.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.DAO
{
    internal class SetDAO
    {

        public static List<WordSet> WordSets;

        public static bool loaded = false;

        private static int indexOffset = 0;


        public SetDAO() { }

        private IEnumerator LoadWordSetsCoroutine()
        {
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "verified.json");

            Debug.Log("Loading from: " + filePath);

            UnityWebRequest request = UnityWebRequest.Get(filePath);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load JSON: " + request.error);
                yield break;
            }

            try
            {
                string jsonString = request.downloadHandler.text;

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                List<WordSetRaw> rawSets = JsonSerializer.Deserialize<List<WordSetRaw>>(jsonString, options);

                WordSets = new List<WordSet>();

                foreach (WordSetRaw wordSetRaw in rawSets)
                {
                    WordSet newset = new WordSet(
                        wordSetRaw.Index,
                        new List<Word> { wordSetRaw.FirstWord, wordSetRaw.SecondWord, wordSetRaw.ThirdWord }
                    );
                    WordSets.Add(newset);
                }

                loaded = true;
                Debug.Log("Word sets loaded successfully. Count: " + WordSets.Count);
            }
            catch (Exception ex)
            {
                Debug.LogError("JSON Parse Error: " + ex.ToString());
            }
        }

        

        public WordSet GetWordSetByIndex(int index)
        {
            if (!loaded)
            {
                LoadWordSetsCoroutine();
            }


            WordSet wordSet = WordSets.ElementAt(index + indexOffset);
            Debug.Log(wordSet.ToString());

            return wordSet;

        }


        public int GetCurrentDateIndex()
        {
            DateTime beginDate = new DateTime(2025, 9, 21);
            DateTime currentDate = DateTime.Today;

            TimeSpan difference = currentDate - beginDate;

            int dateIndex = (int)difference.TotalDays;

            Debug.Log("Today's date: " + currentDate);
            Debug.Log("Begin date: " + beginDate);
            Debug.Log("Current date index: " + dateIndex);

            return dateIndex;



        }
    }
}
