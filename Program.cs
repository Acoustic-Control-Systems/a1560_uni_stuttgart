using A1560APICOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace A1560ApiApp
{

    class Program
    {
        private static readonly A1560 A1560 = new A1560();
        
//////////////////////////  Eingabe der Variablen  ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static readonly int VectorLength = 120000;                //Wert zwischen 1024 to 131072
        private static readonly int SamplingFrequencyMHz = 100;         //ADC frequency, 1, 2, 5, 10, 25, 50, 100 MHz
        private static readonly int GainDb = 50;
        private static readonly int HighPassFilter = 3;                 //0=10, 1=20, 2=40 oder 3=100kHz
        private static readonly int PulseVoltageV = 200;                //Pulse voltage, 20, 100 or 200 Volts
        private static readonly int PulseDuration = 2;                  //Duration of the pulse burst in half-cycles - Acceptable values 1-10
        private static readonly int PulseFrequencykHz = 50;             //Frequency of the pulse,25-500 kHz depends of transducer
        private static readonly int TransducerType = 0;                 //Transducer type (0-Dual, 1-Single)
        private static readonly int TriggeringMode = 0;                 //Siehe unten

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EINGABE von Dateinamen und Speicherort weiter unten


        static void Main(string[] args)
        {
            A1560.Connect("192.168.1.2");
            //Automatic acquisition is assumed running on connect, stop it.
            A1560.Stop();
            A1560.Flush();



            /////////////////////////////////////////////////////////////// Einstellung Anzahl der Messpunkte //////////////////////////////////////////////////////////////////
            
            A1560.VectorLengthSamples = VectorLength;   //Samples count for each acquired vector, 1024 to 131072
            A1560.SamplingFrequencyMhz = SamplingFrequencyMHz;    //ADC frequency, 1, 2, 5, 10, 25, 50, 100 MHz
            A1560.GainDb = GainDb;                               //Gain at the input, -20 to 80 dB
            A1560.InputFilterNumber = HighPassFilter;           //Hi - Pass filter number at the input, Actual cut-off frequency is versioon-specific 0-3 - See P. 10 Programming Manual
                                                                // 0: 10kHz
                                                                // 1: 20kHz
                                                                // 2: 40kHz
                                                                // 3: 100kHz
            
            A1560.PulseVoltage = PulseVoltageV;                   //Pulse voltage, 20, 100 or 200 Volts
            A1560.BurstFrequencyKhz = PulseFrequencykHz;          //Frequency of the pulse,25-500 kHz depends of transducer


            /////////////////////////////////// Wird nicht beachtet da Anzahl der Perioden festgelegt wird.////////////////////////////////////
            // A1560.BurstPeriodNs = int;              //Period of the pulse, 2000-40000nS - This property sets the period in ns for a pulse burst sent to a transmitting transducer
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////                  


            A1560.BurstLengthNumber = PulseDuration;          //Duration of the pulse burst in half-cycles - Acceptable values 1-10
            A1560.TransducerType = TransducerType;             //Transducer type (0-Dual, 1-Single)

            ///////////////////////////////Wird nur gebraucht wenn man periodisch misst /////////////////////////
            //A1560.AveragingFactor = int;            //Averaging factor  

            //A1560.AveragingPeriodNs = int;         //Averaging acquisition delay constant part, ns
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            A1560.TriggeringMode = TriggeringMode;  //Triggering
                                                    //0: Manual measurement mode
                                                    //1: Periodic mode
                                                    //2: CTP Mode - External CTP Message
                                                    //3: Encoder Mode
                                                    //4: Device chain slave: An acquisition will be initiated by a device chain master.
                                                    //5: TTL mode: An acquisition will be started on every TTL pulse submitted to the CTP input of A1560
                                                    //Triggering Interval only for periodic measurements
                                                    //Device.TriggeringIntervalUs = int XX ;      Microseconds between acquisitions when timer triggering source is used


            Console.WriteLine("Momentane Einstellungen:");
            Console.WriteLine("");
            Console.WriteLine("Vektor Länge = " + VectorLength);
            Console.WriteLine("ADC Frequenz = " + SamplingFrequencyMHz + " MHz");
            Console.WriteLine("Verstärkung = " + GainDb + " Db");
            switch (HighPassFilter)
            {
                case 0:
                    Console.WriteLine("High-Pass Filter = 10 kHz");
                    break;

                case 1:
                    Console.WriteLine("High-Pass Filter = 20 kHz");
                    break;

                case 2:
                    Console.WriteLine("High-Pass Filter = 40 kHz");
                    break;

                case 3:
                    Console.WriteLine("High-Pass Filter = 100 kHz");
                    break;
            }

            Console.WriteLine("Spannung Sensor = " + PulseVoltageV + " V");
            Console.WriteLine("Sensor Frequenz = " + PulseFrequencykHz + " kHz");
            switch (PulseDuration)
            {
                case 1:
                    Console.WriteLine("Puls Dauer = Halbe Periode (Half cycle)");
                    break;

                case 2:
                    Console.WriteLine("Puls Dauer = Volle Periode (Full cycle)");
                    break;

                default:
                    Console.WriteLine("Puls Dauer = " + PulseDuration + " halbe Perioden (" + PulseDuration + " half cycles)");
                    break;
            }
            switch (TransducerType)
            {
                case 0:
                    Console.WriteLine("Sensortyp = Dual Sensor");
                    break;
                case 1:
                    Console.WriteLine("Sensortyp = Single Sensor");
                    break;
                default:
                    Console.WriteLine("Sensortyp = Unbekannt");
                    break;
            }
            switch (TriggeringMode)
            {
                case 0:
                    Console.WriteLine("Art der Messauslösung = Manuell");
                    break;
                case 1:
                    Console.WriteLine("Art der Messauslösung = Periodisch");
                    break;
                case 2:
                    Console.WriteLine("Art der Messauslösung = Extern CTP");
                    break;
                case 3:
                    Console.WriteLine("Art der Messauslösung = Encoder Modus");
                    break;
                case 4:
                    Console.WriteLine("Art der Messauslösung = Mehrere 1560 messen - Sklave");
                    break;
                case 5:
                    Console.WriteLine("Art der Messauslösung = TTL Trigger");
                    break;
                default:
                    Console.WriteLine("Art der Messauslösung = FALSCH");
                    break;
            }

            //A1560.DataReceived += A1560OnDataReceived;
            //Launch 
            A1560.Start();
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Programm wurde gestartet");
            if (TriggeringMode == 5)
            {
                Console.WriteLine("Es wird auf ein TTL Signal gewartet");
            }
            // Hier können Sie das Ereignis abonnieren
            A1560.DataReceived += A1560OnDataReceived;




            // Den vollständigen Pfad zur Datei erstellen, indem der Platzhalter ersetzt wird
            //string filePath = Path.Combine(directoryPath, fileName);

            // Beispiel für den Aufruf von A1560OnDataReceived
           
        }

        private static int lastStep = 1; // Variable, um den Lastschritt zu verfolgen
        private static int sensorPair = 1; // Variable, um das Sensorpaar zu verfolgen

        private static void A1560OnDataReceived(AcquisitionData data)
        {
            
            string fileName = $"Lastschritt {lastStep} - Sensorpaar {sensorPair}.txt";
           
//////////////////////////////////////////////  EINGABE SPEICHERORT  ////////////////////////////////////////////////////////////////////////////////////////////////////
            
            string directoryPath = @"C:\Users\WaltherC\Documents\Excel";

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            var vector = data.Vector;
            Console.WriteLine("Daten empfangen");

            // Hier können Sie den Code hinzufügen, um die Daten zu speichern oder zu verarbeiten.
            // Zum Beispiel, um die Daten in eine Datei zu schreiben:
            string filePath = Path.Combine(directoryPath, fileName);

            // Aktuelles Datum und Uhrzeit abrufen
            string currentDateAndTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            // Berechnung der Messlaufzeit
            double measurementTimeIncrement = CalculateMeasurementTime(VectorLength, SamplingFrequencyMHz);

            // Daten in die Datei schreiben
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Schreiben Sie zuerst die Headerzeile
                writer.WriteLine($"Dateiname: {fileName}");
                writer.WriteLine($"Datum und Uhrzeit: {currentDateAndTime}");
                writer.WriteLine($"Verstärkung: {GainDb}Db´; Spannung Sensor: {PulseVoltageV} V ; Sensorfrequenz: {PulseFrequencykHz} kHz");
                switch (HighPassFilter)
                {
                    case 0:
                        writer.WriteLine("High-Pass Filter = 10 kHz");
                        break;

                    case 1:
                        writer.WriteLine("High-Pass Filter = 20 kHz");
                        break;

                    case 2:
                        writer.WriteLine("High-Pass Filter = 40 kHz");
                        break;

                    case 3:
                        writer.WriteLine("High-Pass Filter = 100 kHz");
                        break;
                }
                switch (PulseDuration)
                {
                    case 1:
                        writer.WriteLine("Puls Dauer = Halbe Periode (Half cycle)");
                        break;

                    case 2:
                        writer.WriteLine("Puls Dauer = Volle Periode (Full cycle)");
                        break;

                    default:
                        writer.WriteLine("Puls Dauer = " + PulseDuration + " halbe Perioden (" + PulseDuration + " half cycles)");
                        break;
                }
                writer.WriteLine($"ADC Frequenz: {SamplingFrequencyMHz} MHz");
                writer.WriteLine($"Vektorlänge: {VectorLength}");
                writer.WriteLine("Daten:Laufzeit [Mikrosekunde] & Amplitude [Db]");
                writer.WriteLine("");
                double currentTime = 0.0;

                foreach (var value in vector)
                {
                    writer.WriteLine($"{currentTime:F3} \t {value:F3}"); // Zeit und Amplitude schreiben //F3 für 3 Nachkommastellen
                    currentTime += measurementTimeIncrement;
                }
            }

            Console.WriteLine("Daten in Datei gespeichert: " + filePath);

            sensorPair++; // Erhöhe die Sensorpaar-Nummer für das nächste Ereignis

            if (sensorPair > 4) // Wenn Sensorpaar 4 erreicht wurde, setze Sensorpaar auf 1 und erhöhe den Lastschritt
            {
                sensorPair = 1;
                lastStep++;
            }
        }

        private static double CalculateMeasurementTime(int vectorLength, double samplingFrequencyMHz)
        {
            double totalMeasurementTime = vectorLength / samplingFrequencyMHz;
            double increment = totalMeasurementTime / vectorLength;
            return increment;
        }
    }


       
}
