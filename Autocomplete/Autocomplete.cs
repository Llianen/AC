using System;

namespace Autocomplete
{
    public class Autocomplete : IAutocomplete
    {
        private char[][] inicialItems;
        private char[] inputBuffer;
        private char[][] filteredItems;

        public Autocomplete(char[][] items)
        {
            inicialItems = new char[items.Length][];
            items.CopyTo(inicialItems,0);

            filteredItems = new char[0][];
            inputBuffer = new char[0];
        }

        public char[][] NewSearch(out char[] options)
        {
            options = new char[0];

            if (inicialItems.Length != filteredItems.Length)
            {
                Array.Resize(ref filteredItems, inicialItems.Length);
                
                for (int i = 0; i < inicialItems.Length; i++)
                {
                    filteredItems[i] = inicialItems[i];
                }
            }
            
            foreach(var s in filteredItems)
                addOption(ref options, s, 0);

            inputBuffer = new char[0];

            return filteredItems;
        }

        private void addToInput(char c)
        {
            Array.Resize(ref inputBuffer, inputBuffer.Length+1);

            inputBuffer[inputBuffer.Length-1] = c;
        }

        /// <summary>
        /// For when the user Backspaces '\b'
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private char[][] BackSpace(out char[] options)
        {
            options = null;

            // delete 1 character from input
            if (inputBuffer.Length > 0)
            {
                Array.Resize(ref inputBuffer, inputBuffer.Length - 1);
            }

            var temp = new string(inputBuffer);

            NewSearch(out options);

            //re-filter the remaining input
            if (temp != null && temp.Length > 0)
            {
                foreach (char c in temp)
                {
                    filter(c, out options);
                }
            }
            return filteredItems;
        }

        /// <summary>
        /// Filters remaining items by the next input char 'c', outputs the next available characters as options
        /// </summary>
        /// <param name="c"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public virtual char[][] filter(char c, out char[] options)
        {
            options = new char[0];

            // '\b' == Backspace character (you should know this)
            if (c == '\b')
            {
                filteredItems = BackSpace(out options);
                return filteredItems;
            }
            else if (Char.IsControl(c) || c < 32)
            {
                return filteredItems;
            }

            //Save input Character
            addToInput(c);

            var pos = inputBuffer.Length - 1;

            if (pos < 0)
                return filteredItems;

            //Lets filter the options
            for (int i = 0; i < filteredItems.Length; )
            {
                var s = filteredItems[i];
                
                if (s.Length <= pos) //options is smaller - remove
                {
                    RemoveAt(ref filteredItems, i);
                }
                else if (s[pos] == c) //Character matches!
                {
                    addOption(ref options, s, pos+1);
                    i++;
                }
                else // character does not match
                {
                    RemoveAt(ref filteredItems, i);
                }
            }

            return filteredItems;
        }

        private void addOption(ref char[] options, char[] s, int pos)
        {
            if (s.Length > pos)
            {
                Array.Resize(ref options, options.Length + 1);
                options[options.Length - 1] = s[pos];
            }
        }

        private static void RemoveAt<T>(ref T[] array, int index)
        {
            
            for(int i = index; i < array.Length-1; i++)
            {
                array[i] = array[i+1];
            }

            Array.Resize(ref array, array.Length - 1);
        }

        //private static void AddResult(ref char[][] array, char[] elem)
        //{
        //    Array.Resize(ref array, array.Length + 1);

        //    array[array.Length - 1] = elem;
        //}
    }
}
