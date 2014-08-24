using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Speech.Recognition;

namespace Jarvis
{
    class Program
    {

        //Program version number (as a float)
        public static float versionNum = 1.1f;

        //Create a speech synthesiser
        private static SpeechSynthesizer synth = new SpeechSynthesizer();

        static void Main(string[] args)
        {
            //Starts the program with the welcome message
            Welcome(versionNum);

            #region Performance Meters
            //Gets the current CPU usage as a percent
            PerformanceCounter perfCPUCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            perfCPUCount.NextValue();

            //Gets the available memory (RAM) in megabytes
            PerformanceCounter perfRAMCount = new PerformanceCounter("Memory", "Available MBytes");
            perfRAMCount.NextValue();

            //Gets the time (in seconds) for how long the system has been up
            PerformanceCounter perfUpTimeCount = new PerformanceCounter("System", "System Up Time");
            perfUpTimeCount.NextValue();
            #endregion
            
            //Using a TimeSpan object to format the upTimeInSeconds nicely
            TimeSpan timeSpan = TimeSpan.FromSeconds(perfUpTimeCount.NextValue());

            String upTime = String.Format("The system has been up for {0} days, {1} hours, {2} minutes and {3} seconds.",
                (int)timeSpan.TotalDays,
                (int)timeSpan.Hours,
                (int)timeSpan.Minutes,
                (int)timeSpan.Seconds);

            //Tells the user how long the system has been up for
            Console.WriteLine(upTime);
            Speak(upTime);

            //Create the int value variables for cpu load and available memory
            int cpuLoad;
            int freeRam;
            
            //Infinite while loop (loops every second, depending on sleep timer)
            while(true) 
            {
                //Set variables to current values for cpu and RAM usage/availibility
                cpuLoad = (int)perfCPUCount.NextValue();
                freeRam = (int)perfRAMCount.NextValue();

                //Write the CPU Load and available RAM
                Console.WriteLine("CPU Load:      {0}%",cpuLoad);
                Console.WriteLine("Available RAM: {0}MB", freeRam);
                Console.Write(".");
                Thread.Sleep(333);
                Console.Write(".");
                Thread.Sleep(333);
                Console.Write(".");
                Thread.Sleep(333);
                Console.Clear();

                //If the CPU load is equal to or over 50%, alert the user
                if (cpuLoad >= 50)
                {
                    Speak(String.Format("Mike, CPU usage is at {0} percent", cpuLoad));
                }

                //Similarly, if the available RAM is under 1GB, alert the user
                if (freeRam < 1024)
                {
                    Speak(String.Format("Mike, there is only {0} megabytes of RAM left", freeRam));
                }
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

        /// <summary>
        /// Uses the synth.speak method to turn text into speech
        /// </summary>
        /// <param name="message">What the computer should say</param>
        public static void Speak(string message)
        {
            synth.Speak(message);
        }

        /// <summary>
        /// Uses the synth.speak method to turn text into speech, this time with rate changer built-in
        /// </summary>
        /// <param name="message">What the computer should say</param>
        public static void Speak(string message, int rate)
        {
            synth.Rate = rate;
            Speak(message);
        }
    }
}