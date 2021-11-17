using System;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.IO.Ports;
using NetduinoPlusSide.Moduli;

namespace NetduinoPlusSide
{
    using Moduli.Temperatura;
    using Moduli.Movimento;
    using Moduli.Carrello;
    using Moduli.Buzzer;
    using Frames;

    public class Netduino
    {
        static XBeeExecutor xbee = new XBeeExecutor();
        static Movimento mov;
        static Carrello carr;

        public static void Main()
        {
            xbee.Received += new FrameRicevutoEventHandler(SmistatoreRicezione);
            Run();
        }

        public static void Run()
        {
            xbee.Connect();
            xbee.BeginReceive();

            Buzzer.Inizializza(Pinouts.Buzzer.PinSegnale);
            Temperatura.Inizializza(ref xbee, Tempi.SecondiToMS(2), Pinouts.Temperatura.Canale);

            //RICORDA DI ACCENDERE LE PILE!
            mov = new Movimento(new Motore(Pinouts.Motori.DX.PinNero, Pinouts.Motori.DX.PinRosso),
                new Motore(Pinouts.Motori.SX.PinNero, Pinouts.Motori.SX.PinRosso));

            carr = new Carrello(new Pinza(Pinouts.Carrello.Pinza.PinNero, Pinouts.Carrello.Pinza.PinRosso),
                new Elevatore(Pinouts.Carrello.Elevatore.PinNero, Pinouts.Carrello.Elevatore.PinRosso));
        }

        #region FrameRicevuti
        public static void SmistatoreRicezione(object sender, FrameReceivedEventArgs e)
        {
            FrameInIngresso frame = new FrameInIngresso(e.FlussoBytes);
            Type T = frame.GetFrameType();

            if (T != null)
            {
                switch (T.Name)
                {
                    case "FrameMovimento":
                        FrameRicevuto(sender, new MovimentoEventArgs(frame));
                        break;
                    case "FrameCarrello":
                        FrameRicevuto(sender, new CarrelloEventArgs(frame));
                        break;
                    case "FrameBuzzer":
                        FrameRicevuto(sender, new BuzzerEventArgs(frame));
                        break;
                }
            }
        }

        public static void FrameRicevuto(object sender, MovimentoEventArgs e)
        {
            FrameMovimento frame = e.Frame;
            mov.Execute(frame.Caratteristiche.Direzione);
        }

        public static void FrameRicevuto(object sender, CarrelloEventArgs e)
        {
            FrameCarrello frame = e.Frame;
            carr.Execute(frame.Caratteristiche.AzioneCarrello);
        }

        public static void FrameRicevuto(object sender, BuzzerEventArgs e)
        {
            FrameBuzzer frame = e.Frame;
            Buzzer.Execute(frame.Caratteristiche.AzioneBuzzer,
                frame.Caratteristiche.Frequenza,
                frame.Caratteristiche.Durata);
        }
        #endregion
    }

    public static class Pinouts
    {
        public struct Buzzer
        {
            private static OutputPort pinsegnale = new OutputPort(Pins.GPIO_PIN_D4, false);
            public static OutputPort PinSegnale { get { return pinsegnale; } }
        }

        public struct Motori
        {
            public struct DX
            {
                private static OutputPort pinrosso = new OutputPort(Pins.GPIO_PIN_D8, false),
                    pinnero = new OutputPort(Pins.GPIO_PIN_D9, false);
                public static OutputPort PinRosso { get { return pinrosso; } }
                public static OutputPort PinNero { get { return pinnero; } }
            }

            public struct SX
            {
                private static OutputPort pinrosso = new OutputPort(Pins.GPIO_PIN_D6, false),
                    pinnero = new OutputPort(Pins.GPIO_PIN_D7, false);
                public static OutputPort PinRosso { get { return pinrosso; } }
                public static OutputPort PinNero { get { return pinnero; } }
            }
        }

        public struct XBee
        {
            private static string com1 = SerialPorts.COM1;
            public static string COM1 { get { return com1; } }

            public const int RATE = 9600;
        }

        public struct Carrello
        {
            public struct Elevatore
            {
                private static OutputPort pinrosso = new OutputPort(Pins.GPIO_PIN_D10, false),
                    pinnero = new OutputPort(Pins.GPIO_PIN_D11, false);
                public static OutputPort PinRosso { get { return pinrosso; } }
                public static OutputPort PinNero { get { return pinnero; } }
            }

            public struct Pinza
            {
                private static OutputPort pinrosso = new OutputPort(Pins.GPIO_PIN_D12, false),
                    pinnero = new OutputPort(Pins.GPIO_PIN_D13, false);
                public static OutputPort PinRosso { get { return pinrosso; } }
                public static OutputPort PinNero { get { return pinnero; } }

            }
        }

        public struct Temperatura
        {
            private static Cpu.AnalogChannel canale = Cpu.AnalogChannel.ANALOG_5;
            public static Cpu.AnalogChannel Canale { get { return canale; } }
        }
    }

    public static class Tempi
    {
        public static int SecondiToMS(double sec)
        {
            return (int)(sec * 1000);
        }

        public static int MinutiToMS(int min)
        {
            return SecondiToMS(60 * min);
        }
    }
}
