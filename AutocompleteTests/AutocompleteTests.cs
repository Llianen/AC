using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;
using DataProj;

namespace Autocomplete.Tests
{
    [TestClass()]
    public class AutocompleteTests
    {
        [TestMethod()]
        public void basicCharSearch()
        {
            IAutocomplete ac = new Autocomplete(Data.EstacoesMetro.Select(s => s.ToCharArray()).ToArray());

            //"~Qu..."
            
            var results = ac.Filter('Q', out char[] options);

            Assert.AreEqual(options[0], 'u', "Expected 'u' as an option");
            Assert.AreEqual(true, results.Length == 1 && new string(results[0]) == "Quinta das Conchas", "Expected \"Quinta das Conchas\" as a result");
        }

        [TestMethod()]
        public void BackSpace()
        {
            IAutocomplete ac = new Autocomplete(Data.EstacoesMetro.Select(s => s.ToCharArray()).ToArray());

            //"~Qu..."
            
            var results = ac.Filter('\b', out char[] options);
            results = ac.Filter('Q', out options);
            results = ac.Filter('A', out options);
            results = ac.Filter('\b', out options);

            Assert.AreEqual('u', options[0]);

            results = ac.Filter('u', out options);

            Assert.AreEqual('i', options[0]);
            Assert.AreEqual("Quinta das Conchas", new string(results[0]));

            results = ac.Filter('\b', out options);
            results = ac.Filter('\b', out options);
            results = ac.Filter('\b', out options);

            Assert.AreEqual(50, options.Length);
            Assert.AreEqual(50, results.Length);
        }

        [TestMethod()]
        public void SearchCorrectness()
        {
            IAutocomplete ac = new Autocomplete(Data.EstacoesMetro.Select(s => s.ToCharArray()).ToArray());
            
            var filtered = ac.Filter('A', out char[] options);

            Assert.AreEqual(11, options.Length, "Expected 11 char options");

            Assert.AreEqual(11, filtered.Length, "Expected 11 items filtered");

            Assert.AreEqual(true, options.Contains('m'), "Char 'm' expected as an option");

            filtered = ac.Filter('m', out options);

            Assert.AreEqual(2, options.Length, "Expected 2 char options");

            Assert.AreEqual(2, filtered.Length, "Expected 2 items filtered");
        }

        [TestMethod()]
        public void InitialData()
        {
            IAutocomplete ac = new Autocomplete(Data.EstacoesMetro.Select(s => s.ToCharArray()).ToArray());

            Assert.AreEqual(Data.EstacoesMetro.Length, ac.AllItems.Length);

            Assert.AreEqual(Data.EstacoesMetro.Length, ac.InitialOptions.Length);

            var filtered = ac.Search(null, out char[] options);

            Assert.AreEqual(Data.EstacoesMetro.Length, filtered.Length);

            Assert.AreEqual(Data.EstacoesMetro.Length, options.Length);
        }

        [TestMethod()]
        public void SearchByString()
        {
            IAutocomplete ac = new Autocomplete(Data.EstacoesMetro.Select(s => s.ToCharArray()).ToArray());

            var filtered = ac.Search(Data.EstacoesMetro[0].ToCharArray(), out char[] options);

            Assert.AreEqual(1, filtered.Length);

            Assert.AreEqual(0, options.Length);
        }

        [TestMethod()]
        public void Performance_SearchFor_AllCharacters_InAllWords()
        {
            char[] options;
            char[][] results;

            // Start the test

            IAutocomplete ac = new Autocomplete(Data.Cities.Select(s => s.ToCharArray()).ToArray());

            Stopwatch sw = Stopwatch.StartNew();

            foreach (string s in Data.Cities)
            {
                //Get initial full results
                results = ac.Search(null, out options);

                //filter Char by Char
                foreach (char c in s)
                {
                    results = ac.Filter(c, out options);
                }
            }

            sw.Stop();

            var av = (double)sw.ElapsedMilliseconds / Data.Cities.Length;

            var acceptedTime = (double)1.700; //milliseconds
            Assert.AreEqual(true, av < acceptedTime, av.ToString() + " > " + acceptedTime);
        }

        [TestMethod()]
        public void Performance_SearchBy_FullString()
        {
            //int quantity = 4000;
            char[] options;
            char[][] results;

            // Start the test

            IAutocomplete ac = new Autocomplete(Data.Cities.Select(s => s.ToCharArray()).ToArray());

            Stopwatch sw = Stopwatch.StartNew();

            foreach (var c in ac.AllItems)
            {
                results = ac.Search(c, out options);
            }

            sw.Stop();

            var av = (double)sw.ElapsedMilliseconds / Data.Cities.Length;

            var acceptedTime = (double) 0.300; //milliseconds
            Assert.AreEqual(true, av < acceptedTime, av.ToString() + " > " + acceptedTime);
        }
    }
}