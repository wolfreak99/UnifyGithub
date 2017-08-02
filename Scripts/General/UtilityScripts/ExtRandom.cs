/*************************
 * Original url: http://wiki.unity3d.com/index.php/ExtRandom
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/ExtRandom.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Article Original Author: Dreamblur 
 *
 * Description 
 *   
 * Usage 
 *   
 * Methods 
 *   
 * ExtRandom.cs 
 *   
 * Testing 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    ExtRandom.cs v.1.0 Original Script Author: Dreamblur 
    Description ExtRandom is an extension of the Unity class Random. Its main purpose is to automate the common operations which implement the Random class. 
    Usage Place the ExtRandom script in your project's Standard Assets folder. 
    Methods RandomizeSeed() 
    SplitChance() 
    Chance(int, int) 
    Choice(T[]) 
    Choice(List<T>) 
    WeightedChoice(T[], int[]) 
    WeightedChoice(List<T>, int[]) 
    Shuffle(T[]) 
    Shuffle(List<T>) 
    
    ExtRandom<T>.RandomizeSeed() 
    public static void RandomizeSeed()This method sets a random seed for the RNG using 2 convoluted formulas. Using this method at any time other than during downtime is not recommended. This method will never see use 99.9% of the time, but is available in cases where a truly random seed is required. 
    
    ExtRandom<T>.SplitChance() 
    public static bool SplitChance()This method returns either true or false with an equal chance. 
    
    ExtRandom<T>.Chance(int, int) 
    public static bool Chance(int nProbabilityFactor, int nProbabilitySpace)This method returns either true or false with the chance of the former derived from the parameters passed to the method. 
    ExtRandom<int>.Chance(1, 10);The above code has a 10% chance of returning true. 
    
    ExtRandom<T>.Choice(T[]) 
    public static T Choice(T[] array)This method returns a random element chosen from an array of elements. 
    string[] strArray = new string[3] {"sun", "moon", "star"};
    ExtRandom<string>.Choice(strArray);The above code has a 33.3% chance of returning "sun", a 33.3% chance of returning "moon" and a 33.3% chance of returning "star". 
    
    ExtRandom<T>.Choice(List<T>) 
    public static T Choice(List<T> list)This method returns a random element chosen from a list of elements. 
    List<string> strList = new List<string>(3) {"sun", "moon", "star"};
    ExtRandom<string>.Choice(strList);The above code has a 33.3% chance of returning "sun", a 33.3% chance of returning "moon" and a 33.3% chance of returning "star". 
    
    ExtRandom<T>.WeightedChoice(T[], int[]) 
    public static T WeightedChoice(T[] array, int[] nWeights)This method returns a random element chosen from an array of elements based on the respective weights of the elements. 
    string[] strArray = new string[3] {"sun", "moon", "star"};
    int[] nWeights = new int[3] {10, 25, 65}
    ExtRandom<string>.WeightedChoice(strArray, nWeights);The above code has a 10% chance of returning "sun", a 25% chance of returning "moon" and a 65% chance of returning "star". 
    
    ExtRandom<T>.WeightedChoice(List<T>, int[]) 
    public static T WeightedChoice(List<T> list, int[] nWeights)This method returns a random element chosen from a list of elements based on the respective weights of the elements. 
    List<string> strList = new List<string>(3) {"sun", "moon", "star"};
    int[] nWeights = new int[3] {10, 25, 65}
    ExtRandom<string>.WeightedChoice(strList, nWeights);The above code has a 10% chance of returning "sun", a 25% chance of returning "moon" and a 65% chance of returning "star". 
    
    ExtRandom<T>.Shuffle(T[]) 
    public static T[] Shuffle(T[] array)This method rearranges the elements of an array randomly and returns the rearranged array. 
    string[] strArray = new string[3] {"sun", "moon", "star"};
    ExtRandom<string>.Shuffle(strArray);The above code will shuffle the elements of strArray and return the shuffled array. 
    
    ExtRandom<T>.Shuffle(List<T>) 
    public static List<T> Shuffle(List<T> list)This method rearranges the elements of a list randomly and returns the rearranged list. 
    List<string> strList = new List<string>(3) {"sun", "moon", "star"};
    ExtRandom<string>.Shuffle(strList);The above code will shuffle the elements of strList and return the shuffled list. 
    
    
    ExtRandom.cs using UnityEngine;
    using System.Collections.Generic;
     
    public class ExtRandom<T>
    {
        public static void RandomizeSeed()
        {
            Random.seed = System.Math.Abs(((int)(System.DateTime.Now.Ticks % 2147483648l) - (int)(Time.realtimeSinceStartup + 2000f)) / ((int)System.DateTime.Now.Day - (int)System.DateTime.Now.DayOfWeek * System.DateTime.Now.DayOfYear));
            Random.seed = System.Math.Abs((int)((Random.value * (float)System.DateTime.Now.Ticks * (float)Random.Range(0, 2)) + (Random.value * Time.realtimeSinceStartup * Random.Range(1f, 3f))) + 1);
        }
     
        public static bool SplitChance()
        {
            return Random.Range(0, 2) == 0 ? true : false;
        }
     
        public static bool Chance(int nProbabilityFactor, int nProbabilitySpace)
        {
            return Random.Range(0, nProbabilitySpace) < nProbabilityFactor ? true : false;
        }
     
        public static T Choice(T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
     
        public static T Choice(List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }
     
        public static T WeightedChoice(T[] array, int[] nWeights)
        {
            int nTotalWeight = 0;
            for(int i = 0; i < array.Length; i++)
            {
                nTotalWeight += nWeights[i];
            }
            int nChoiceIndex = Random.Range(0, nTotalWeight);
            for(int i = 0; i < array.Length; i++)
            {
                if(nChoiceIndex < nWeights[i])
                {
                    nChoiceIndex = i;
                    break;
                }
                nChoiceIndex -= nWeights[i];
            }
     
            return array[nChoiceIndex];
        }
     
        public static T WeightedChoice(List<T> list, int[] nWeights)
        {
            int nTotalWeight = 0;
            for(int i = 0; i < list.Count; i++)
            {
                nTotalWeight += nWeights[i];
            }
            int nChoiceIndex = Random.Range(0, nTotalWeight);
            for(int i = 0; i < list.Count; i++)
            {
                if(nChoiceIndex < nWeights[i])
                {
                    nChoiceIndex = i;
                    break;
                }
                nChoiceIndex -= nWeights[i];
            }
     
            return list[nChoiceIndex];
        }
     
        public static T[] Shuffle(T[] array)
        {
            T[] shuffledArray = new T[array.Length];
            List<int> elementIndices = new List<int>(0);
            for(int i = 0; i < array.Length; i++)
            {
                elementIndices.Add(i);
            }
            int nArrayIndex;
            for(int i = 0; i < array.Length; i++)
            {
                nArrayIndex = elementIndices[Random.Range(0, elementIndices.Count)];
                shuffledArray[i] = array[nArrayIndex];
                elementIndices.Remove(nArrayIndex);
            }
     
            return shuffledArray;
        }
     
        public static List<T> Shuffle(List<T> list)
        {
            List<T> shuffledList = new List<T>(0);
            int nListCount = list.Count;
            int nElementIndex;
            for(int i = 0; i < nListCount; i++)
            {
                nElementIndex = Random.Range(0, list.Count);
                shuffledList.Add(list[nElementIndex]);
                list.RemoveAt(nElementIndex);
            }
     
            return shuffledList;
        }
    }
    
    Testing The following script can be used to test the statistical accuracy of the methods of the ExtRandom class. This script is not required to use ExtRandom. 
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
     
    public class ExtRandomTest : MonoBehaviour
    {
        private void Awake()
        {
            SplitChanceTest();
            ChanceTest();
            ChoiceTestArray();
            ChoiceTestList();
            WeightedChoiceTestArray();
            WeightedChoiceTestList();
        }
     
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.O))
            {
                ShuffleTestArray();
            }
            else if(Input.GetKeyDown(KeyCode.P))
            {
                ShuffleTestList();
            }
        }
     
        private void SplitChanceTest()
        {
            int nSplitChanceTest1 = 0;
            int nSplitChanceTest2 = 0;
            for(int i = 0; i < 50000000; i++)
            {
                if(ExtRandom<int>.SplitChance())
                {
                    nSplitChanceTest1++;
                }
                else
                {
                    nSplitChanceTest2++;
                }
            }
            Debug.Log("After 50mil tests, SplitChance() results are as follows:");
            Debug.Log("First half: " + nSplitChanceTest1.ToString());
            Debug.Log("Second half: " + nSplitChanceTest2.ToString());
        }
     
        private void ChanceTest()
        {
            int nProbabilityFactor = 32;
            int nProbabilitySpace = 100;
            float fChanceExpected1 = (float)nProbabilityFactor / (float)nProbabilitySpace;
            float fChanceExpected2 = ((float)nProbabilitySpace - (float)nProbabilityFactor) / (float)nProbabilitySpace;
            int nChanceTest1 = 0;
            int nChanceTest2 = 0;
            for(int i = 0; i < 50000000; i++)
            {
                if(ExtRandom<int>.Chance(nProbabilityFactor, nProbabilitySpace))
                {
                    nChanceTest1++;
                }
                else
                {
                    nChanceTest2++;
                }
            }
            float fChanceTest1 = (float)nChanceTest1 / 50000000f;
            float fChanceTest2 = (float)nChanceTest2 / 50000000f;
            Debug.Log("After 50mil tests, Chance() results are as follows:");
            Debug.Log("First half: Expected: " + fChanceExpected1.ToString() + "  ; Actual: " + fChanceTest1.ToString());
            Debug.Log("Second half: Expected: " + fChanceExpected2.ToString() + "  ; Actual: " + fChanceTest2.ToString());
        }
     
        private void ChoiceTestArray()
        {
            string[] strChoices = new string[6] {"who", "what", "where", "when", "why", "how"};
            int[] nChoiceTest = new int[6];
            for(int i = 0; i < nChoiceTest.Length; i++)
            {
                nChoiceTest[i] = 0;
            }
            for(int i = 0; i < 50000000; i++)
            {
                switch(ExtRandom<string>.Choice(strChoices))
                {
                    case "who":
                    {
                        nChoiceTest[0]++;
                        break;
                    }
                    case "what":
                    {
                        nChoiceTest[1]++;
                        break;
                    }
                    case "where":
                    {
                        nChoiceTest[2]++;
                        break;
                    }
                    case "when":
                    {
                        nChoiceTest[3]++;
                        break;
                    }
                    case "why":
                    {
                        nChoiceTest[4]++;
                        break;
                    }
                    case "how":
                    {
                        nChoiceTest[5]++;
                        break;
                    }
                }
            }
            Debug.Log("After 50mil tests, Choice() results are as follows:");
            Debug.Log("First: " + nChoiceTest[0].ToString());
            Debug.Log("Second: " + nChoiceTest[1].ToString());
            Debug.Log("Third: " + nChoiceTest[2].ToString());
            Debug.Log("Fourth: " + nChoiceTest[3].ToString());
            Debug.Log("Fifth: " + nChoiceTest[4].ToString());
            Debug.Log("Sixth: " + nChoiceTest[5].ToString());
        }
     
        private void ChoiceTestList()
        {
            List<string> strChoices = new List<string>(6) {"who", "what", "where", "when", "why", "how"};
            int[] nChoiceTest = new int[6];
            for(int i = 0; i < nChoiceTest.Length; i++)
            {
                nChoiceTest[i] = 0;
            }
            for(int i = 0; i < 50000000; i++)
            {
                switch(ExtRandom<string>.Choice(strChoices))
                {
                    case "who":
                    {
                        nChoiceTest[0]++;
                        break;
                    }
                    case "what":
                    {
                        nChoiceTest[1]++;
                        break;
                    }
                    case "where":
                    {
                        nChoiceTest[2]++;
                        break;
                    }
                    case "when":
                    {
                        nChoiceTest[3]++;
                        break;
                    }
                    case "why":
                    {
                        nChoiceTest[4]++;
                        break;
                    }
                    case "how":
                    {
                        nChoiceTest[5]++;
                        break;
                    }
                }
            }
            Debug.Log("After 50mil tests, Choice() results are as follows:");
            Debug.Log("First: " + nChoiceTest[0].ToString());
            Debug.Log("Second: " + nChoiceTest[1].ToString());
            Debug.Log("Third: " + nChoiceTest[2].ToString());
            Debug.Log("Fourth: " + nChoiceTest[3].ToString());
            Debug.Log("Fifth: " + nChoiceTest[4].ToString());
            Debug.Log("Sixth: " + nChoiceTest[5].ToString());
        }
     
        private void WeightedChoiceTestArray()
        {
            string[] strChoices = new string[6] {"who", "what", "where", "when", "why", "how"};
            int[] nWeights = new int[6] {3, 16, 33, 12, 1, 35};
            float[] fWeightedChoiceExpected = new float[6];
            int[] nWeightedChoiceTest = new int[6];
            float[] fWeightedChoiceTest = new float[6];
            for(int i = 0; i < nWeights.Length; i++)
            {
                fWeightedChoiceExpected[i] = (float)nWeights[i] / 100f;
                nWeightedChoiceTest[i] = 0;
                fWeightedChoiceTest[i] = 0f;
            }
            for(int i = 0; i < 50000000; i++)
            {
                switch(ExtRandom<string>.WeightedChoice(strChoices, nWeights))
                {
                    case "who":
                    {
                        nWeightedChoiceTest[0]++;
                        break;
                    }
                    case "what":
                    {
                        nWeightedChoiceTest[1]++;
                        break;
                    }
                    case "where":
                    {
                        nWeightedChoiceTest[2]++;
                        break;
                    }
                    case "when":
                    {
                        nWeightedChoiceTest[3]++;
                        break;
                    }
                    case "why":
                    {
                        nWeightedChoiceTest[4]++;
                        break;
                    }
                    case "how":
                    {
                        nWeightedChoiceTest[5]++;
                        break;
                    }
                }
            }
            for(int i = 0; i < nWeights.Length; i++)
            {
                fWeightedChoiceTest[i] = (float)nWeightedChoiceTest[i] / 50000000f;
            }
            Debug.Log("After 50mil tests, WeightedChoice() results are as follows:");
            Debug.Log("First: Expected: " + fWeightedChoiceExpected[0].ToString() + "  ; Actual: " + fWeightedChoiceTest[0].ToString());
            Debug.Log("Second: Expected: " + fWeightedChoiceExpected[1].ToString() + "  ; Actual: " + fWeightedChoiceTest[1].ToString());
            Debug.Log("Third: Expected: " + fWeightedChoiceExpected[2].ToString() + "  ; Actual: " + fWeightedChoiceTest[2].ToString());
            Debug.Log("Fourth: Expected: " + fWeightedChoiceExpected[3].ToString() + "  ; Actual: " + fWeightedChoiceTest[3].ToString());
            Debug.Log("Fifth: Expected: " + fWeightedChoiceExpected[4].ToString() + "  ; Actual: " + fWeightedChoiceTest[4].ToString());
            Debug.Log("Sixth: Expected: " + fWeightedChoiceExpected[5].ToString() + "  ; Actual: " + fWeightedChoiceTest[5].ToString());
        }
     
        private void WeightedChoiceTestList()
        {
            List<string> strChoices = new List<string>(6) {"who", "what", "where", "when", "why", "how"};
            int[] nWeights = new int[6] {3, 16, 33, 12, 1, 35};
            float[] fWeightedChoiceExpected = new float[6];
            int[] nWeightedChoiceTest = new int[6];
            float[] fWeightedChoiceTest = new float[6];
            for(int i = 0; i < nWeights.Length; i++)
            {
                fWeightedChoiceExpected[i] = (float)nWeights[i] / 100f;
                nWeightedChoiceTest[i] = 0;
                fWeightedChoiceTest[i] = 0f;
            }
            for(int i = 0; i < 50000000; i++)
            {
                switch(ExtRandom<string>.WeightedChoice(strChoices, nWeights))
                {
                    case "who":
                    {
                        nWeightedChoiceTest[0]++;
                        break;
                    }
                    case "what":
                    {
                        nWeightedChoiceTest[1]++;
                        break;
                    }
                    case "where":
                    {
                        nWeightedChoiceTest[2]++;
                        break;
                    }
                    case "when":
                    {
                        nWeightedChoiceTest[3]++;
                        break;
                    }
                    case "why":
                    {
                        nWeightedChoiceTest[4]++;
                        break;
                    }
                    case "how":
                    {
                        nWeightedChoiceTest[5]++;
                        break;
                    }
                }
            }
            for(int i = 0; i < nWeights.Length; i++)
            {
                fWeightedChoiceTest[i] = (float)nWeightedChoiceTest[i] / 50000000f;
            }
            Debug.Log("After 50mil tests, WeightedChoice() results are as follows:");
            Debug.Log("First: Expected: " + fWeightedChoiceExpected[0].ToString() + "  ; Actual: " + fWeightedChoiceTest[0].ToString());
            Debug.Log("Second: Expected: " + fWeightedChoiceExpected[1].ToString() + "  ; Actual: " + fWeightedChoiceTest[1].ToString());
            Debug.Log("Third: Expected: " + fWeightedChoiceExpected[2].ToString() + "  ; Actual: " + fWeightedChoiceTest[2].ToString());
            Debug.Log("Fourth: Expected: " + fWeightedChoiceExpected[3].ToString() + "  ; Actual: " + fWeightedChoiceTest[3].ToString());
            Debug.Log("Fifth: Expected: " + fWeightedChoiceExpected[4].ToString() + "  ; Actual: " + fWeightedChoiceTest[4].ToString());
            Debug.Log("Sixth: Expected: " + fWeightedChoiceExpected[5].ToString() + "  ; Actual: " + fWeightedChoiceTest[5].ToString());
        }
     
        private void ShuffleTestArray()
        {
            string[] strChoices = new string[6] {"who", "what", "where", "when", "why", "how"};
            string[] strShuffled = ExtRandom<string>.Shuffle(strChoices);
            Debug.Log("Shuffled: " + strShuffled[0].ToString() + "  ,  " + strShuffled[1].ToString() + "  ,  " + strShuffled[2].ToString() + "  ,  " + strShuffled[3].ToString() + "  ,  " + strShuffled[4].ToString() + "  ,  " + strShuffled[5].ToString());
        }
     
        private void ShuffleTestList()
        {
            List<string> strChoices = new List<string>(6) {"who", "what", "where", "when", "why", "how"};
            List<string> strShuffled = ExtRandom<string>.Shuffle(strChoices);
            Debug.Log("Shuffled: " + strShuffled[0].ToString() + "  ,  " + strShuffled[1].ToString() + "  ,  " + strShuffled[2].ToString() + "  ,  " + strShuffled[3].ToString() + "  ,  " + strShuffled[4].ToString() + "  ,  " + strShuffled[5].ToString());
        }
}
}
