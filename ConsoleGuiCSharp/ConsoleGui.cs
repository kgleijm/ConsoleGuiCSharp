using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleGuiCSharp
{
    public class ConsoleGui
    {
            //method that will create a choice dialog
            public static string openQuestion(string question, string[] checks, string negativeResponse)
            {
                
                string output = "";

                //base output: 
                if (checks == null && negativeResponse == null)
                {
                    //just returns answer
                    Console.Out.WriteLine("\n" + question);
                    output = Console.ReadLine();
                    return output;
                }
                //go into code that see if the checks are met
                else
                {
                    //will keep asking same question until checks are met
                    //will return ERROR when exit is typed
                    Console.Out.WriteLine("\n" + question);
                    Console.Out.WriteLine("(type exit if you don't know)");
                    while (true)
                    {
                        output = Console.ReadLine();
                        bool satisfactitory = true;

                        //exit if exit command is given
                        if (output.Equals("exit"))
                        {
                            return "ERROR";
                        }

                        //loop throught the checks and see if they're met
                        foreach (string check in checks)
                        {
                            if (!output.Contains(check))
                            {
                                satisfactitory = false;
                                break;
                            }
                        }

                        //act base on the outcome of the checks
                        if (satisfactitory)
                        {
                            return output;
                        }
                        else if (negativeResponse != null)
                        {
                            Console.Out.WriteLine(negativeResponse);
                            Console.Out.WriteLine("try again or type exit if you don't know");
                        }
                        else
                        {
                            Console.Out.WriteLine("that's not right, try again or type exit if you don't know");
                        }
                    }
                }


                return output;
            }
            

            //shorthand way of above method for no checks
            public static string openQuestion(string question)
            {
                return openQuestion(question, null, null);
            }
            
            public static int multipleChoice(string question, params string[] options)
            {
                Console.Out.WriteLine("\n" + question);
                while (true)
                {
                    //list all possible inputs and prepare for answer
                    string[] possibleInputs = new string[options.Length];
                    for (int i = 0; i < options.Length; i++)
                    {
                        string option = options[i];
                        
                        //count how many numeral chars are in this possible answer to account for inputs beginning with numbers
                        int lenghtOfExpectedUserInput = 1;
                        for (int j = 0; j < option.Length; j++)
                        {
                            if (!Char.IsDigit(option[j]))
                            {
                                break;
                            }
                            else if(j+1 > lenghtOfExpectedUserInput)
                            {
                                lenghtOfExpectedUserInput = j+1;
                            }
                        }
                        
                        //distill info
                        string newAns = option.Substring(0, lenghtOfExpectedUserInput).ToUpper();
                        string firstChar = option.Substring(lenghtOfExpectedUserInput, 1).ToUpper();
                        string listOption = "[" + newAns + "]" + " " + firstChar + option.Substring(lenghtOfExpectedUserInput+1);

                        //list option and save possible answer
                        possibleInputs[i] = newAns;
                        Console.Out.WriteLine(listOption);
                    }

                    //add exit for escape
                    Console.Out.WriteLine("[X] Exit\n");
                    string ans = Console.ReadLine().ToUpper();


                    //get and check answer against possible answers
                    if (ans.Equals("X"))
                    {
                        return -1;
                    }
                    else
                    {
                        for (int i = 0; i < possibleInputs.Length; i++)
                        {
                            if (ans.Equals(possibleInputs[i]))
                            {
                                return i;
                            }
                        }

                        Console.Out.WriteLine("Thats not an option, type \"X\" if you don't know");
                    }
                }
            } 
            
            public static int getInteger(string question)
            {
                Console.Out.WriteLine("\n" + question);


                while (true)
                {
                    string input = Console.ReadLine();

                    //create escape
                    if (input.Equals("exit"))
                    {
                        return int.MinValue;
                    }

                    if (int.TryParse(input, out int output))
                    {
                        return output;
                    }
                    else
                    {
                        Console.Out.WriteLine("That's not a valid aswer, please try again" +
                                              "\nor type exit to go back");
                    }
                }
            }

            public static void debugLine(string line)
            {
                Console.Out.WriteLine("# DEBUG #" + line);
            }

            public static bool noErrorsInValue(params string[] values)
            {
                foreach (var value in values)
                {
                    if (value.Equals("ERROR") || value.Equals("-1"))
                    {
                        return false;
                    }
                }
                return true;
            }
            
            
            public abstract class Element
            {
                public abstract void list();

                public abstract string getMPQListing();

            }

            public static Element getElementByMultipleChoice(String question, List<Element> inputList)
            {
                // extract possible ans as string from elements
                string[] elements = new string[inputList.Count];
                for (int i = 0; i < elements.Length; i++)
                {
                    elements[i] = i + inputList[i].getMPQListing();
                }

                int ans = multipleChoice(question, elements);

                if (ans >= 0)
                {
                    return inputList[ans];
                }
                return null;
            }

            public static Element getElementByMultipleChoice(String question, Dictionary<string, Element> inputDict)
            {
                return getElementByMultipleChoice(question, inputDict.Values.ToList());
            }
            
            public static Element getElementByMultipleChoice(String question, Element[] inputArray)
            {
                return getElementByMultipleChoice(question, inputArray.ToList());
            }
            
            
            public static void list(IEnumerable iterable)
            {

                
                if (iterable is IDictionary)
                {
                    IDictionary dict = (IDictionary) iterable;
                    foreach (Element element in dict.Values)
                    {
                        element.list();
                    }
                }
                else
                {
                    foreach (Element element in (IEnumerable)iterable)
                    {
                        element.list();
                    }
                }
                
                
            }
            
        }
}