using System;

namespace Autocomplete
{
    public class Autocomplete : IAutocomplete
    {
        private string[] inicialItems;
        private char[] inputBuffer;
        private string [] filteredItems;

        public Autocomplete(string[] items)
        {
            inicialItems = new string[items.Length];
            filteredItems = new string[items.Length];

            items.CopyTo(inicialItems,0);
            items.CopyTo(filteredItems, 0);
            inputBuffer = new char[0];
        }

        public string[] start(out char[] options)
        {
            options = new char[0];

            Array.Resize(ref filteredItems, inicialItems.Length);
            inicialItems.CopyTo(filteredItems, 0);

            foreach(var s in inicialItems)
                addOption(ref options, s, 0);

            return inicialItems;
        }

        private void add(char c)
        {
            Array.Resize(ref inputBuffer, inputBuffer.Length+1);

            inputBuffer[inputBuffer.Length-1] = c;
        }

        private string[] remove(out char[] options)
        {
            options = null;

            start(out options);

            if (inputBuffer.Length < 1)
            {
                return filteredItems;
            }

            Array.Resize(ref inputBuffer, inputBuffer.Length-1);

            var temp = inputBuffer;

            Array.Resize(ref inputBuffer, 0);

            foreach (char c in temp)
            {
                filter(c, out options);
            }

            return filteredItems;
        }

        /// <summary>
        /// Filters all items by the input char, options the next available characters
        /// </summary>
        /// <param name="c"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public virtual string[] filter(char c, out char[] options)
        {
            options = new char[0];

            if (c == '\b')
            {
                filteredItems = remove(out options);
                return filteredItems;
            }
            else if (Char.IsControl(c) || c < 32)
            {
                return filteredItems;
            }

            add(c);

            var pos = inputBuffer.Length - 1;

            if (pos < 0)
                return filteredItems;

            for (int i = 0; i < filteredItems.Length; )
            {
                var s = filteredItems[i];

                if (s.Length <= pos)
                {
                    RemoveAt(ref filteredItems, i);
                }
                else if (s[pos] == c)
                {
                    addOption(ref options, s, pos+1);
                    i++;
                }
                else
                {
                    RemoveAt(ref filteredItems, i);
                }
            }

            return filteredItems;
        }

        private void addOption(ref char[] options, string s, int pos)
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
        public virtual string[] filterByString(string s, out char[] options)
        {
            options = new char[0];

            //inputBuffer = s.ToCharArray();

            //var pos = inputBuffer.Length - 1;

            //if (pos < 0)
            //    return filteredItems;

            //for (int i = 0; i < inicialItems.Length;)
            //{
            //    var f = inicialItems[i];

            //    if (s.Length <= pos)
            //    {
            //        RemoveAt(ref filteredItems, i);
            //    }
            //    else if (f[pos] == c)
            //    {
            //        addOption(ref options, f, pos + 1);
            //        i++;
            //    }
            //    else
            //    {
            //        RemoveAt(ref filteredItems, i);
            //    }
            //}

            return filteredItems;
        }
    }
}
