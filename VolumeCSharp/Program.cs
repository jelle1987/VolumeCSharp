using System;
using System.IO.Ports;
using System.Threading;
using CoreAudioApi;

namespace VolumeControlApp
{
    class Program
    {
        static SerialPort arduino = new SerialPort("COM4", 9600);  // Initialize immediately
        static MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();  // Initialized immediately
        static MMDevice? defaultDevice;  // Nullable MMDevice
        static AudioEndpointVolume? volumeControl;  // Nullable AudioEndpointVolume

        static void Main(string[] args)
        {
            // Initialize serial communication with Arduino
            arduino.Open();
            Console.WriteLine("Connected to Arduino.");

            // Initialize audio system
            InitializeAudio();

            // Main loop to check for commands
            while (true)
            {
                if (arduino.BytesToRead > 0)
                {
                    string command = arduino.ReadLine().Trim();

                    if (command == "RIGHT")
                    {
                        // You could add functionality here to switch between apps if needed
                        // For now, we will just print a message.
                        Console.WriteLine("Switching to next application - Not Implemented");
                    }
                    else if (command == "UP")
                    {
                        AdjustVolume(0.05f); // Increase volume
                    }
                    else if (command == "DOWN")
                    {
                        AdjustVolume(-0.05f); // Decrease volume
                    }
                }

                Thread.Sleep(100); // Check every 100ms for commands
            }
        }

        // Initialize audio system using Core Audio API (directly)
        static void InitializeAudio()
        {
            // Get the default audio device (output)
            defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            if (defaultDevice == null)
            {
                Console.WriteLine("No default audio device found.");
                return;  // Exit early if no default device is found
            }

            // Initialize volume control for the default audio device
            volumeControl = defaultDevice.AudioEndpointVolume;
            Console.WriteLine("System volume control initialized.");
        }

        // Adjust volume by a certain percentage (positive or negative)
        static void AdjustVolume(float change)
        {
            if (volumeControl == null) return; // If no volume control, do nothing

            // Adjust the system volume
            float currentVolume = volumeControl.MasterVolumeLevelScalar;
            float newVolume = Math.Max(0.0f, Math.Min(1.0f, currentVolume + change));
            volumeControl.MasterVolumeLevelScalar = newVolume;
            Console.WriteLine($"System volume adjusted to {newVolume * 100}%");
        }
    }
}
