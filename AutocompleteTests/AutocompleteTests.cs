using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Autocomplete.Tests
{
    [TestClass()]
    public class AutocompleteTests
    {
        public static readonly string[] testData = {"Aeroporto",
                        "Alameda",
                        "Alfornelos",
                        "Alto dos Moinhos",
                        "Alvalade",
                        "Amadora Este",
                        "Ameixoeira",
                        "Anjos",
                        "Areeiro",
                        "Arroios",
                        "Avenida",
                        "Baixa-Chiado",
                        "Bela Vista",
                        "Cabo Ruivo",
                        "Cais do Sodré",
                        "Campo Grande",
                        "Campo Pequeno",
                        "Carnide",
                        "Chelas",
                        "Cidade Universitária",
                        "Colégio Militar/Luz",
                        "Encarnação",
                        "Entre Campos",
                        "Intendente",
                        "Jardim Zoológico",
                        "Laranjeiras",
                        "Lumiar",
                        "Marquês de Pombal",
                        "Martim Moniz",
                        "Moscavide",
                        "Odivelas",
                        "Olaias",
                        "Olivais",
                        "Oriente",
                        "Parque",
                        "Picoas",
                        "Pontinha",
                        "Praça de Espanha",
                        "Quinta das Conchas",
                        "Rato",
                        "Reboleira",
                        "Restauradores",
                        "Roma",
                        "Rossio",
                        "Saldanha",
                        "Santa Apolónia",
                        "São Sebastião",
                        "Senhor Roubado",
                        "Telheiras",
                        "Terreiro do Paço"};

        [TestMethod()]
        public void basicCharSearch()
        {
            IAutocomplete ac = new Autocomplete(testData);

            //"~Qu..."

            char[] options;
            var results = ac.filter('Q', out options);

            Assert.AreEqual(options[0], 'u', "Expected 'u' as an option");
            Assert.AreEqual(true, results.Length == 1 && results[0] == "Quinta das Conchas", "Expected \"Quinta das Conchas\" as a result");
        }

        [TestMethod()]
        public void BackSpace()
        {
            IAutocomplete ac = new Autocomplete(testData);

            //"~Qu..."

            char[] options;
            var results = ac.filter('\b', out options);
            results = ac.filter('Q', out options);
            results = ac.filter('A', out options);
            results = ac.filter('\b', out options);

            Assert.AreEqual('u', options[0]);

            results = ac.filter('u', out options);

            Assert.AreEqual('i', options[0]);
            Assert.AreEqual("Quinta das Conchas", results[0]);

            results = ac.filter('\b', out options);
            results = ac.filter('\b', out options);
            results = ac.filter('\b', out options);

            Assert.AreEqual(50, options.Length);
            Assert.AreEqual(50, results.Length);
        }

        [TestMethod()]
        public void SearchCorrectness()
        {
            IAutocomplete ac = new Autocomplete(testData);

            char[] options;
            var filtered = ac.filter('A', out options);

            Assert.AreEqual(11, options.Length, "Expected 11 char options");

            Assert.AreEqual(11, filtered.Length, "Expected 11 items filtered");

            Assert.AreEqual(true, options.Contains('m'), "Char 'm' expected as an option");

            filtered = ac.filter('m', out options);

            Assert.AreEqual(2, options.Length, "Expected 2 char options");

            Assert.AreEqual(2, filtered.Length, "Expected 2 items filtered");
        }

        [TestMethod()]
        public void PerformanceTest()
        {
            int dataSize = 2000; // Nº of items * Nº characters = dataSize^2
            var allPossibilities = generateBigData(dataSize);

            List<string> bigDataToTest = new List<string>();

            foreach (char[] array in allPossibilities)
            {
                bigDataToTest.Add(new string(array));
            }

            char[] options;
            string[] results;

            // Start the test

            var startingTime = DateTime.Now;

            foreach (char[] array in allPossibilities)
            {
                IAutocomplete ac = new Autocomplete(bigDataToTest.ToArray());

                foreach (char c in array)
                {
                    results = ac.filter(c, out options);
                }
            }

            var elapsedTime = (DateTime.Now - startingTime).TotalMilliseconds;

            // Complexity = n + n log n
            var acceptedTime = (10 * dataSize) + dataSize * Math.Log((double)dataSize, (double)2);
            Assert.AreEqual(true, elapsedTime < acceptedTime, elapsedTime.ToString() + " > " + acceptedTime);
        }

        private char[][] generateBigData(int dataSize)
        {
            if (dataSize + 33 > char.MaxValue / 2)
                return null;

            // Char.MaxValue = 65535
            // 1 Char = 2 bytes

            var x = dataSize;
            var y = dataSize;

            char[][] allPossibilities = new char[x][];

            for (int i = 0; i < x; i++)
            {
                allPossibilities[i] = new char[y];

                for (int w = 0; w < y; w++)
                {
                    int z = 32 + i + w;

                    allPossibilities[i][w] = (char)z;
                }
            }

            return allPossibilities;
        }
    }
}