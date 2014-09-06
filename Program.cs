using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Speech.Synthesis;

namespace Jarvis
{
    class Program
    {
        //Program version number (as a float)
        public static float versionNum = 1.3f;

        //This is a switch to turn off the version and name stuff at the beginning (used for testing purposes to make the program faster)
        public static bool intro = true;

        //Create a speech synthesiser
        private static SpeechSynthesizer synth = new SpeechSynthesizer();

        //Create some max load messages for what to say when the CPU is at 100%
        public static List<String> maxLoadMessages = new List<string>();

        //Create some menu options for when Jarvis asks us what we want to do
        public static List<String> menuOptions = new List<string>();

        //Create a randomiser to pick between one of the max load messages
        public static Random random = new Random();

        #region Performance Meters
            //Gets the current CPU usage as a percent
            public static PerformanceCounter perfCPUCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            
            //Gets the available memory (RAM) in megabytes
            public static PerformanceCounter perfRAMCount = new PerformanceCounter("Memory", "Available MBytes");

            //Gets the time (in seconds) for how long the system has been up
            public static PerformanceCounter perfUpTimeCount = new PerformanceCounter("System", "System Up Time");
            #endregion

            static void Main(string[] args)
            {
                //Before starting, add some messages to the maxLoadMessage list for when the CPU is at max
                maxLoadMessages.Add("Oh my god, the CPU is hot");
                maxLoadMessages.Add("HELP ME!");
                maxLoadMessages.Add("I'm burning up in here!");
                maxLoadMessages.Add("It's getting hot in here, so take off all your clothes");

                //Add menu options
                menuOptions.Add("Check CPU");
                menuOptions.Add("Check RAM");
                menuOptions.Add("Check uptime");
                menuOptions.Add("Exit");

                //Run each PerformanceCounter.NextValue() once to initialise, otherwise it won't work
                perfUpTimeCount.NextValue();
                perfRAMCount.NextValue();
                perfCPUCount.NextValue();

                //Using a TimeSpan object to format the system up time nicely
                TimeSpan timeSpan = TimeSpan.FromSeconds(perfUpTimeCount.NextValue());

                //This is the string made up of the uptime and formatted by timeSpan. It's used in getUpTime();
                String upTime = String.Format("The system has been up for {0} days, {1} hours, {2} minutes and {3} seconds.",
                    (int)timeSpan.TotalDays,
                    (int)timeSpan.Hours,
                    (int)timeSpan.Minutes,
                    (int)timeSpan.Seconds);

                if (intro)
                {
                    //Starts the program with the welcome message
                    Welcome(versionNum);

                    //Program asks who the user is
                    String whoAreYou = "Who are you?";
                    PrintSpeak(whoAreYou);

                    //User enters name, and we store it in variable
                    String userName = Console.ReadLine();

                    //Program responds using the name in speech (cool!)
                    String hiResponse = String.Format("Hi {0}, I'm Jarvis!", userName);
                    String specialResponse = String.Format("You're not Jarvis, I'm Jarvis!!");
                    if (userName.ToLower() == "jarvis")
                    {
                        PrintSpeak(specialResponse);
                    }
                    else
                    {
                        PrintSpeak(hiResponse);
                    }
                }

                //Print the menu options to the user
                PrintMenu();

                //Get the user's input for what they want to do in the main menu
                String userMenuInput = getUserInput();

                //This block of code is quite confusing. It basically says: if the user entered something, as long as it doesn't say exit, and as long as it's in the list of things we can do
                // then check it against the switch. Since we already know it's something we can do (because it's in the menuOptions list) we should be able to do it.
                if (userMenuInput != "")
                {
                    while (userMenuInput.ToLower() != "exit")
                    {
                        if (menuOptions.Contains(userMenuInput))
                        {
                            switch (userMenuInput)
                            {
                                case "Check CPU":
                                    {
                                        checkCPU();
                                        userMenuInput = getUserInput();
                                        break;
                                    }
                                case "Check RAM":
                                    {
                                        checkRAM();
                                        userMenuInput = getUserInput();
                                        break;
                                    }
                                case "Check uptime":
                                    {
                                        getUpTime(upTime);
                                        userMenuInput = getUserInput();
                                        break;
                                    }
                                case "Exit":
                                    {
                                        userMenuInput = getUserInput();
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            PrintSpeak("That wasn't a valid option, try again");
                            userMenuInput = getUserInput();
                        }
                    }
                }
                else
                {
                    PrintSpeak("You didn't say anything, so shutting down");
                }
            }

        /// <summary>
        /// Welcomes the user to the program with a welcome message
        /// </summary>
        /// <param name="version">version number of the program</param>
        public static void Welcome(float version)
        {
            String message = String.Format("Welcome to Jarvis, version {0}",version);
            Console.WriteLine(message);
            Speak(message, 3);
        }

        #region Print and Speak methods
        /// <summary>
        /// Uses the synth.speak method to turn text into speech
        /// </summary>
        /// <param name="message">What the computer should say</param>
        public static void Speak(String message)
        {
            synth.Speak(message);
        }

        /// <summary>
        /// Uses the synth.speak method to turn text into speech, this time with rate changer built-in
        /// </summary>
        /// <param name="message">What the computer should say</param>
        public static void Speak(String message, int rate)
        {
            synth.Rate = rate;
            Speak(message);
        }

        /// <summary>
        /// Uses the synth.speak method to turn text into speech and also print to the console
        /// </summary>
        /// <param name="message">What the computer should say</param>
        public static void PrintSpeak(String message)
        {
            Console.WriteLine(message);
            Speak(message, 2);
        }
        #endregion

        #region checkMethods
        /// <summary>
        /// Prints and speaks the current CPU Load
        /// </summary>
        public static void checkCPU()
        {
            //Set variables to current values for cpu
            int cpuLoad = (int)perfCPUCount.NextValue();

            //Print and speak CPU Load
            Console.WriteLine("CPU Load:      {0}%", cpuLoad);
            Speak(String.Format("The current CPU load is {0} percent", cpuLoad),3);
        }
           
        /// <summary>
        /// Prints and speaks the current available RAM
        /// </summary>
        public static void checkRAM() 
        {
            //Set variable for RAM availibility
            int freeRam = (int)perfRAMCount.NextValue();

            //Print and speak freeRam
            Console.WriteLine("Available RAM: {0}MB", freeRam);
            Speak(String.Format("The available memory is {0} megabytes",freeRam),3);
        }

        /// <summary>
        /// Prints and speaks how long the system has been online
        /// </summary>
        /// <param name="upTime"></param>
        public static void getUpTime(String upTime)
        {
            //Tells the user how long the system has been up for
            Console.WriteLine(upTime);
            Speak(upTime, 3);
        }
        #endregion

        public static void PrintMenu()
        {
            //What would you like to do?
            PrintSpeak("What would you like to do?");
            foreach (String option in menuOptions)
            {
                Console.Write("| ");
                Console.Write(option);
                Console.Write(" |");
            }
            Console.Write("\n");
        }

        /// <summary>
        /// Returns what the user wrote in the console. Result can be stored in a variable
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        public static String getUserInput()
        {
            return Console.ReadLine();
        }

    }
}