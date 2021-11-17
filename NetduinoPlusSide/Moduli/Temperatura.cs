using System;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace NetduinoPlusSide.Moduli
{
    namespace Temperatura
    {
        public static class Temperatura
        {
            private static int refresh;
            private static Cpu.AnalogChannel onchannel;
            private static XBeeExecutor xbee;

            public static Cpu.AnalogChannel Channel { get { return onchannel; } }
            public static bool Init { get { return onchannel != Cpu.AnalogChannel.ANALOG_NONE; } }

            public static void Inizializza(ref XBeeExecutor asymodxbee, int refreshtime, Cpu.AnalogChannel channel)
            {
                xbee = asymodxbee;
                refresh = refreshtime; onchannel = channel;
                if (Init)
                {
                    Thread th = new Thread(new ThreadStart(Acquisizione));
                    th.Start();
                }
            }

            private static void Acquisizione()
            {
                AnalogInput Input = new AnalogInput(onchannel);
                while (true)
                {
                    int currentlevel = Input.ReadRaw();
                    xbee.Send(new FrameTemperatura(currentlevel));
                    Thread.Sleep(refresh);
                }
            }
        }

        public class FrameTemperatura : Frames.FrameInUscita
        {
            public class CaratteristicheFrameTemperatura : CaratteristicheFrameInUscita
            {
                private int camp = 0;
                public int Campione { get { return this.camp; } set { this.camp = value; } }
                public new bool Accettabile
                {
                    get
                    {
                        return base.Tipo != Frames.TipoFrame.undefined && this.Campione > 0;
                    }
                }
            }
            protected new CaratteristicheFrameTemperatura _caratt = new CaratteristicheFrameTemperatura();
            public new CaratteristicheFrameTemperatura Caratteristiche { get { return this._caratt; } }

            public FrameTemperatura(int camp)
            {
                base.Caratteristiche.Tipo = this.Caratteristiche.Tipo = Frames.TipoFrame.temp;
                this.Caratteristiche.Campione = camp;
                SubBuild();
                Build();
            }

            protected void SubBuild()
            {
                base.Caratteristiche.Testo = InitFine + Separatore + Frames.EnumExt.GetName(base.Caratteristiche.Tipo)
                    + Separatore + this.Caratteristiche.Campione.ToString() + Separatore + InitFine;
            }
        }
    }
}
