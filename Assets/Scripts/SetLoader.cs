using AnagrowLoader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using UnityEngine;
using UnityEngine.UI;

namespace AnagrowLoader
{
    internal class Program : MonoBehaviour
    {

        public static List<WordSet> WordSets;

        public Button button;

        void Start()
        {
            //Calls the TaskOnClick/TaskWithParameters/ButtonClicked method when you click the Button
            button.onClick.AddListener(LoadWordSets);
           
        }

        
        static void LoadWordSets()
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

                // PRINT SETS (DEBUG):

                //foreach (WordSet wordSet in wordSets)
                //{
                //    Console.Write(wordSet.ToString());

                //    Console.WriteLine("Index: " + wordSet.Index);
                //    Console.WriteLine("Hint 1: " + wordSet.Words.ElementAt(0).Hint);
                //    Console.WriteLine("Word 1: " + wordSet.Words.ElementAt(0).Answer);
                //    Console.WriteLine("Hint 2: " + wordSet.Words.ElementAt(1).Hint);
                //    Console.WriteLine("Word 2: " + wordSet.Words.ElementAt(1).Answer);
                //    Console.WriteLine("Hint 3: " + wordSet.Words.ElementAt(2).Hint);
                //    Console.WriteLine("Word 3: " + wordSet.Words.ElementAt(2).Answer);
                //}

            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }

            GetCurrentSet();


        }


        public static void GetCurrentSet()
        {
            DateTime beginDate = new DateTime(2025, 9, 21);
            DateTime currentDate = DateTime.Today;

            TimeSpan difference = currentDate - beginDate;

            int dateIndex = (int)difference.TotalDays;

            Debug.Log("Today's date: " + currentDate);
            Debug.Log("Begin date: " + beginDate);
            Debug.Log("Current date index: " + dateIndex);

            Debug.Log(WordSets.ElementAt(dateIndex).ToString());


        }
    }
}
