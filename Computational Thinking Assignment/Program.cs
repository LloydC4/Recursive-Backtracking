using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Computational_Thinking_Assignment
{
    class Program
    {
        static void Main(string[] args)
        {
            // creating lists for solution space
            List<Chocolate> chocolates = new List<Chocolate>();
            List<Solution> allItems = new List<Solution>();

            // reading chocolate details in to array from a file, formatting them in to a list, and outputting to console
            string inputPath = Path.Combine(Environment.CurrentDirectory, "chocolates.txt");
            string[] formattedInput = ReadInTextFile(inputPath);
            FormatArrayToList(ref formattedInput, ref chocolates);
            OutputListToConsole(ref chocolates);

            // setting up & calculating solution space
            const int NUM_ITEMS = 20;
            bool[] array = new bool[NUM_ITEMS];
            int solutionSpace = (int)Math.Pow(2, NUM_ITEMS) - 1;

            // generating all possible choices within constraints using recursive backtracking
            GenerateSolutions(0, NUM_ITEMS, ref array, ref allItems, ref chocolates);

            // new solution list to store sorted viable solutions
            List<Solution> SortedItems = allItems.OrderByDescending(o => o.TotalValue).ToList();

            // optimal choice chosen and outputted to file
            string outputPath = Path.Combine(Environment.CurrentDirectory, "all_chocolatesFinal.txt");
            OutputToFile(SortedItems[0].ToString(), outputPath);

            Console.ReadLine();
        }

        // class to store chocolate details
        class Chocolate
        {
            public string Name
            {
                get; set;
            }
            public double ProductionCost
            {
                get; set;
            }
            public double RetailValue
            {
                get; set;
            }

            // constructor to set member variables
            public Chocolate(string name, double productionCost, double retailValue)
            {
                Name = name;
                ProductionCost = productionCost;
                RetailValue = retailValue;
            }

            // .ToString() override allowing for consistent output
            public override string ToString()
            {
                return "Name: " + Name + "\tProduction Cost: £" + ProductionCost + "\tRetail Value: £" + RetailValue;
            }
        }

        // class to store all solutions within the given constraints
        class Solution
        {
            public List<string> ChocolateNames = new List<string>();

            public double TotalCost
            {
                get; set;
            }
            public double TotalValue
            {
                get; set;
            }

            // .ToString() override using a stringbuilder allowing for required output format
            public override string ToString()
            {
                StringBuilder Names = new StringBuilder();

                for (int i = 0; i < ChocolateNames.Count; i++)
                {
                    Names.Append(i+1 + ". " + String.Join("\n", ChocolateNames.ElementAt(i)) + "\n");
                }

                return Names + "\nCost: £" + TotalCost + "\nRetail Price: £" + TotalValue + "\nProfit: £" + (TotalValue - TotalCost);
            }
        }

        // function to output to txt file
        static void OutputToFile(string output, string outputPath)
        {
            System.IO.File.WriteAllText(outputPath, output);
        }

        // reads text from .txt file in to string array, then splits.
        static string[] ReadInTextFile(string inputPath)
        {
            String input = File.ReadAllText(inputPath);
            string[] formattedInput = input.Split(',', '\n');
            return formattedInput;
        }

        // converts the array to a list of objects, each 3 elements in array is a new object with field 1 being name as a string, and 2/3 being prices as doubles.
        static void FormatArrayToList(ref string[] formattedInput, ref List<Chocolate> chocolates)
        {
            for (int i = 0; i < formattedInput.Length; i++)
            {
                if (i % 3 == 0)
                {
                    chocolates.Add(new Chocolate(formattedInput[i], Convert.ToDouble(formattedInput[i + 1]), Convert.ToDouble(formattedInput[i + 2])));
                }
            }
        }

        // outputs the list of objects to the console
        static void OutputListToConsole(ref List<Chocolate> chocolates)
        {
            for (int i = 0; i < chocolates.Count; i++)
            {
                Console.WriteLine(chocolates[i]);
            }
        }

        // using recursive backtracking to generate all possible solutions in the scope
        static void GenerateSolutions(int i, int NUM_ITEMS, ref bool[] array, ref List<Solution> list, ref List<Chocolate> listTwo)
        {
            if (i != NUM_ITEMS)
            {
                array[i] = false;
                GenerateSolutions(i + 1, NUM_ITEMS, ref array, ref list, ref listTwo);

                array[i] = true;
                GenerateSolutions(i + 1, NUM_ITEMS, ref array, ref list, ref listTwo);
            }
            else
            {
                EvaluateSolutions(NUM_ITEMS, ref array, ref list, ref listTwo);
                return;
            }
        }

        // takes the solutions generated by recursive backtracking, and checks them against given constraints. If valid, solution is added to list.
        static void EvaluateSolutions(int NUM_ITEMS, ref bool[] array, ref List<Solution> solutionList, ref List<Chocolate> chocolateList)
        {
            // checking if solution contains more than 14 chocolates
            int counter = 0;
            for (int i = 0; i < NUM_ITEMS; i++)
            {
                if (array[i])
                {
                    counter++;
                }
            }
            if (counter >= 14)
            {
                Solution solution = new Solution();
                for (int j = 0; j < array.Length; j++)
                {
                    // add all chocolate names and calculate prices to solution
                    if (array[j])
                    {
                        solution.ChocolateNames.Add(chocolateList.ElementAt(j).Name);
                        solution.TotalCost += chocolateList.ElementAt(j).ProductionCost;
                        solution.TotalValue += chocolateList.ElementAt(j).RetailValue;
                    }
                }
                // add to list if solution costs less than £1.97
                if (solution.TotalCost <= 1.96)
                {
                    solutionList.Add(solution);
                }
            }
        }
    }
}
