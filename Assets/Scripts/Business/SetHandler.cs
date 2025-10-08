using AnagrowLoader.Models;
using Assets.Scripts.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Assets.Scripts.Business
{
    internal class SetHandler
    {
        private SetDAO setDao = new SetDAO();
        WordSet currentSet;
        public Button button;


        public SetHandler()
        {

        }

        public SetHandler(WordSet wordSet)
        {
            currentSet = wordSet;
        }

        public bool checkWord(string word, int idx)
        {
            Word checkWord = currentSet.getWordByIdx(idx);
            if (word == checkWord.Answer)
            {
                return true;
            }

            return false;
        }

        public WordSet getCurrentWordSet()
        {
            return currentSet;
        }

        public WordSet getWordSetById(int idx)
        {
            return setDao.GetWordSetByIndex(idx);
        }

        public void getNextSet()
        {
            int currentDateIndex = setDao.GetCurrentDateIndex();
            currentSet = getWordSetById(currentDateIndex);
        }

    }
}
