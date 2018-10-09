using System;
using System.Collections.Generic;

namespace Autocomplete
{
    public class Autocomplete : IAutocomplete
    {
        private char[][] initialItems;
        private char[] initialOptions;
        private List<char> inputBuffer;
        private List<char[]> filteredItems;

        public Autocomplete(char[][] items)
        {
            initialItems = new char[items.Length][];
            initialOptions = new char[items.Length];

            items.CopyTo(initialItems,0);

            int i = 0;
            foreach (var s in items)
                initialOptions[i++] = s[0];

            filteredItems = new List<char[]>();
            inputBuffer = new List<char>();
        }

        public virtual char[][] Search(char[] criteria, out char[] options)
        {
            options = new char[0];

            if (criteria != null && criteria.Length > 0)
                inputBuffer = new List<char>(criteria);
            else
                inputBuffer.Clear();

            return expand(inputBuffer.ToArray(), ref options);
        }

        /// <summary>
        /// For when the user Backspaces '\b'
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private char[][] BackSpace(out char[] options)
        {
            options = new char[0];

            // delete 1 character from input
            if (inputBuffer.Count > 0)
            {
                inputBuffer.RemoveAt(inputBuffer.Count-1);
            }

            return expand(inputBuffer.ToArray(), ref options);
        }

        /// <summary>
        /// Filters remaining items by the next input char 'c', outputs the next available characters as options
        /// </summary>
        /// <param name="c"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public virtual char[][] Filter(char c, out char[] options)
        {
            options = new char[0];

            // '\b' == Backspace character (you should know this)
            if (c == '\b')
            {
                return BackSpace(out options);
            }
            else if (Char.IsControl(c) || c < 32)
            {
                return filteredItems.ToArray();
            }

            //Save input Character
            inputBuffer.Add(c);

            //Lets filter the options
            if (inputBuffer.Count == 1)
                return expand(inputBuffer.ToArray(), ref options);
            else
                return filter(c, ref options, inputBuffer.Count - 1);
        }

        /// <summary>
        /// Start a search result
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private char[][] expand(char[] searchCriteria, ref char[] options)
        {
            filteredItems.Clear();

            var searchSize = searchCriteria != null ? searchCriteria.Length : 0;

            if (searchSize == 0)
            {
                options = initialOptions;
                return initialItems;
            }

            for (int i = 0; i < initialItems.Length; i++)
            {
                var item = initialItems[i];

                var nMatchedChars = 0;

                for (int z = 0; z < searchSize; z++)
                {
                    if (item.Length > z && item[z] == searchCriteria[z]) //Character matches!
                    {
                        nMatchedChars++;
                    }
                    else
                        break;
                }

                if(nMatchedChars >= searchCriteria.Length)
                {
                    filteredItems.Add(item);
                    addOption(ref options, item, searchSize);
                }
            }

            return filteredItems.ToArray();
        }

        /// <summary>
        /// Remove result options from the filtered items by filtering the Char
        /// </summary>
        /// <param name="c"></param>
        /// <param name="options"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private char[][] filter(char c, ref char[] options, int pos)
        {
            for (int i = 0; i < filteredItems.Count;)
            {
                var s = filteredItems[i];

                if (s.Length <= pos) //options is smaller - remove
                {
                    filteredItems.RemoveAt(i);
                }
                else if (s[pos] == c) //Character matches!
                {
                    addOption(ref options, s, pos + 1);
                    i++;
                }
                else // character does not match
                {
                    filteredItems.RemoveAt(i);
                }
            }

            return filteredItems.ToArray();
        }

        /// <summary>
        /// Add Character Options to result
        /// </summary>
        /// <param name="options"></param>
        /// <param name="s"></param>
        /// <param name="pos"></param>
        private void addOption(ref char[] options, char[] s, int pos)
        {
            if (s.Length > pos)
            {
                Array.Resize(ref options, options.Length + 1);
                options[options.Length - 1] = s[pos];
            }
        }

        /// <summary>
        /// Remove element from Array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        private static void RemoveAt<T>(ref T[] array, int index)
        {
            
            for(int i = index; i < array.Length-1; i++)
            {
                array[i] = array[i+1];
            }

            Array.Resize(ref array, array.Length - 1);
        }

        /// <summary>
        /// Add a char[] to a char[][]
        /// </summary>
        /// <param name="array"></param>
        /// <param name="elem"></param>
        private static void AddStringToArray(ref char[][] array, char[] elem)
        {
            Array.Resize(ref array, array.Length + 1);

            array[array.Length - 1] = elem;
        }
    }
}
