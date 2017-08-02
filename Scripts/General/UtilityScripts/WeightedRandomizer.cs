/*************************
 * Original url: http://wiki.unity3d.com/index.php/WeightedRandomizer
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/WeightedRandomizer.cs
 * File based on original modification date of: 1 June 2013, at 15:51. 
 *
 * Author: Edgar Neto 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    
    Contents [hide] 
    1 Description 
    2 Usage 
    3 Limitations 
    4 Code 
    
    DescriptionThis class provides a way to retrieve a random element from a collection, where elements have different weights (different chances of being spawned) 
    UsageExample: 
    WeightedRandomizer.From(weights).TakeOne();For this example to work we assume that weights is a Dictionary that contains the spawn rates. Example: 
    var weights = new Dictionary<Animal, int>();
    weights.Add(new Dog(), 90); // 90% spawn chance;
    weights.Add(new Cat(), 5); // 5% spawn chance;
    weights.Add(new Bird(), 5); // 5% spawn chance;
     
    Animal selected = WeightedRandomizer.From(weights).TakeOne(); // Strongly-typed object returned. No casting necessary.LimitationsPlease keep in mind that, for this to work in iOS environment, the type used as the key of the Dictionary (in the example above, the Animal class) cannot be a Reference type (i.e., Class), it must be a Value type (i.e., Struct). This happens because the AOT compiler does not support Reference types as Generic Type Arguments for dictionary keys. 
    Code    /// <summary>
        /// Static class to improve readability
        /// Example:
        /// <code>
        /// var selected = WeightedRandomizer.From(weights).TakeOne();
        /// </code>
        /// 
        /// </summary>
        public static class WeightedRandomizer
        {
            public static WeightedRandomizer<R> From<R>(Dictionary<R, int> spawnRate)
            {
                return new WeightedRandomizer<R>(spawnRate);
            }
        }
     
        public class WeightedRandomizer<T>
        {
            private static Random _random = new Random();
            private Dictionary<T, int> _weights;
     
            /// <summary>
            /// Instead of calling this constructor directly,
            /// consider calling a static method on the WeightedRandomizer (non-generic) class
            /// for a more readable method call, i.e.:
            /// 
            /// <code>
            /// var selected = WeightedRandomizer.From(weights).TakeOne();
            /// </code>
            /// 
            /// </summary>
            /// <param name="weights"></param>
            public WeightedRandomizer(Dictionary<T, int> weights)
            {
                _weights = weights;
            }
     
            /// <summary>
            /// Randomizes one item
            /// </summary>
            /// <param name="spawnRate">An ordered list withe the current spawn rates. The list will be updated so that selected items will have a smaller chance of being repeated.</param>
            /// <returns>The randomized item.</returns>
            public T TakeOne()
            {
                // Sorts the spawn rate list
                var sortedSpawnRate = Sort(_weights);
     
                // Sums all spawn rates
                int sum = 0;
                foreach (var spawn in _weights)
                {
                    sum += spawn.Value;
                }
     
                // Randomizes a number from Zero to Sum
                int roll = _random.Next(0, sum);
     
                // Finds chosen item based on spawn rate
                T selected = sortedSpawnRate[sortedSpawnRate.Count - 1].Key;
                foreach (var spawn in sortedSpawnRate)
                {
                    if (roll < spawn.Value)
                    {
                        selected = spawn.Key;
                        break;
                    }
                    roll -= spawn.Value;
                }
     
                // Returns the selected item
                return selected;
            }
     
            private List<KeyValuePair<T, int>> Sort(Dictionary<T, int> weights)
            {
                var list = new List<KeyValuePair<T, int>>(weights);
     
                // Sorts the Spawn Rate List for randomization later
                list.Sort(
                    delegate(KeyValuePair<T, int> firstPair,
                             KeyValuePair<T, int> nextPair)
                    {
                        return firstPair.Value.CompareTo(nextPair.Value);
                    }
                 );
     
                return list;
            }
    }
}
