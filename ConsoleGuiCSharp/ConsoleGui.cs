using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleGuiCSharp
{
    public class ConsoleGui
    {
            //method that clears terminal
            public static void clearTerminal()
            {
                for (int i = 0; i < 100; i++)
                {
                    Console.Out.WriteLine(" ");
                }
            }
        
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
            
            //method that returns int representing ans of multipleChoice question
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
            
            //method that returns int
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

            //prints Debug line
            public static void debugLine(string line)
            {
                Console.Out.WriteLine("# DEBUG #" + line);
            }
            
            //checks if all values given are free of ConsoleGui ERRORS
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
            
            //abstract method to make handling classes with ConsoleGui functionality easier
            public abstract class Element
            {
                private string key;
                
                public abstract void list();

                public abstract string getMPQListing();

                public void setKey(string inp_key)
                {
                    key = inp_key;
                }

                public string getKey()
                {
                    return key;
                }

                public Element()
                {
                    setKey(DataManager.getUniqueKey(this));
                }

            }

            //methods that return a ConsoleGui Element from iterable by Multiplechoice
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
            
            //method that prints out the list() strings of all ConsoleGui Elements
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


            public static class StateEngine
            {
                private static State currentState = null;
                private static bool running = false;

                // stops the state engine
                public static void stop()
                {
                    running = false;
                }

                // starts the state engine
                public static void start()
                {
                    running = true;
                    while (running)
                    {
                        currentState.run();
                    }
                }

                // set new state
                public static void setState(State newState)
                {
                    currentState = newState;
                    if (!running)
                    {
                       start();
                    }
                }
                
                // lets user choose from given states by their description
                public static void setStateByMultipleChoice(string question, params State[] states)
                {
                    string[] descs = new string[states.Length];
                    for (int i = 0; i < states.Length; i++)
                    {
                        descs[i] = i + states[i].getDescription();
                    }
                    setState(states[multipleChoice(question, descs)]);
                }
                
                // class thate contains an action and description of the state to
                // minimize boilerplate code
                public class State
                {
                    
                    private Action action;
                    private string description;
                    
                    public State( string inp_description, Action inp_action)
                    {
                        action = inp_action;
                        description = inp_description;
                    }

                    public void run()
                    {
                        action();
                    }

                    public string getDescription()
                    {
                        return description;
                    }
                    
                }
                
            }

            public static class DataManager
            {
                // dict that holds all dicts of registered objects accessed by key: 
                private static Dictionary<Type, Dictionary<string, Element>> mainDict = new Dictionary<Type, Dictionary<string, Element>>();

                // function that adds element to its own dict and if it doesn't exist, creates that dict 
                public static void add(string key, Element inp_element)
                {
                    Type inp_type = inp_element.GetType();
                    if (mainDict.ContainsKey(inp_type))
                    {
                        mainDict[inp_type].Add(key, inp_element);
                    }
                    else
                    {
                        mainDict.Add(inp_type, new Dictionary<string, Element>());
                        mainDict[inp_type].Add(key, inp_element);
                    }
                    inp_element.setKey(key);

                }

                public static void add(Element inp_element)
                {
                    add(getUniqueKey(inp_element), inp_element);
                }

                // Same as above but uses int as key (int get converted to string)
                public static void add(int key, Element inp_element)
                {
                    add(key.ToString(), inp_element);
                }

                // Generate unique keys for elements
                private static int lastUniqueKey = 0;

                public static string getUniqueKey(Element inp_element)
                {
                    Type type = inp_element.GetType();
                    if (mainDict.ContainsKey(type))
                    {
                        while (mainDict[type].ContainsKey(lastUniqueKey.ToString()))
                        {
                            lastUniqueKey++;
                        }
                        return lastUniqueKey.ToString();
                    }
                    else
                    {
                        lastUniqueKey++;
                        return lastUniqueKey.ToString();
                    }
                    
                }

                public static string listDictOfTypeFrom(Element inp_element)
                {
                    Type inp_type = inp_element.GetType();
                    if (mainDict.ContainsKey(inp_type))
                    {
                        list(mainDict[inp_type]);
                        return "SUCCES";
                    }
                    else
                    {
                        debugLine("no dict of element-type found");
                        return "ERROR";
                    }
                }

                // returns ConsoleGui Element from dict
                public static Element ChooseElementInDictOfTypeFrom(string question, Element inp_element)
                {
                    Type inp_type = inp_element.GetType();
                    if (mainDict.ContainsKey(inp_type))
                    {
                        return getElementByMultipleChoice(question, mainDict[inp_type]);
                    }
                    else
                    {
                        debugLine("no dict of element-type found");
                        return null;
                    }
                }

                // Removes ConsoleGui Element from dict
                public static string RemoveElementInDictOfTypeFrom(string question, Element inp_element)
                {
                    Element ElementToBeRemoved = ChooseElementInDictOfTypeFrom(question, inp_element);
                    if(ElementToBeRemoved == null)
                    {
                        return "ERROR";
                    }
                    else
                    {
                        string removeKey = inp_element.getKey();
                        Type typeOfinp_element = ElementToBeRemoved.GetType();
                        debugLine("lenght of dict: " + mainDict[typeOfinp_element].Count);
                        debugLine(mainDict[typeOfinp_element].Remove(removeKey).ToString());
                        debugLine("lenght of dict: " + mainDict[typeOfinp_element].Count);
                        return "SUCCES";
                    }
                }
            }
    }
}