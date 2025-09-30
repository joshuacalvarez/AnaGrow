using AnagrowLoader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TMPro;
using UnityEngine;
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

        public static Boolean loaded;

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



        void LoadWordSets()
        {

            try
            {

                string filePath = Path.Combine(Application.streamingAssetsPath, "verified.json");

                Debug.Log(filePath);


                string jsonString = File.ReadAllText(filePath);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                List<WordSetRaw> rawSets = JsonSerializer.Deserialize<List<WordSetRaw>>(jsonString, options);


                WordSets = new List<WordSet>();

                foreach (WordSetRaw wordSetRaw in rawSets)
                {
                    WordSet newset = new WordSet(wordSetRaw.Index, new List<Word> { wordSetRaw.FirstWord, wordSetRaw.SecondWord, wordSetRaw.ThirdWord });
                    WordSets.Add(newset);
                }

                loaded = true;

            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
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
