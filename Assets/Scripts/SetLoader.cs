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

namespace AnagrowLoader
{
    internal class Program : MonoBehaviour
    {

        public static List<WordSet> WordSets;

        public Button button;

        public TMP_Text hint1;
        public TMP_Text hint2;
        public TMP_Text hint3;

        public static bool loaded;

        private static int indexOffset = 0;

        void Start()
        {
            //Calls the TaskOnClick/TaskWithParameters/ButtonClicked method when you click the Button
            button.onClick.AddListener(GetWordSet);
            loaded = false;
            
           
        }

        void GetWordSet()
        {
            if (!loaded)
            {
                LoadWordSets();
            }

            int index = GetCurrentDateIndex();

            WordSet currentSet = WordSets.ElementAt(index + indexOffset);
            Debug.Log(currentSet.ToString());

            indexOffset++;

            hint1.text = currentSet.Words.ElementAt(0).Hint;
            hint2.text = currentSet.Words.ElementAt(1).Hint;
            hint3.text = currentSet.Words.ElementAt(2).Hint;

        }





        public void LoadWordSets()
        {
            StartCoroutine(LoadWordSetsCoroutine());
        }

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
