using System;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace NetduinoPlusSide.Moduli
{
    namespace Buzzer
    {
        using System.Reflection;

        public enum AzioniBuzzer
        {
            Error, Emit, undefined
        }

        public static class EnumExt
        {
            public static AzioniBuzzer GetAzioneBuzzerFromString(string name)
            {
                AzioniBuzzer ret;
                switch (name)
                {
                    case "Error":
                        ret = AzioniBuzzer.Error;
                        break;
                    case "Emit":
                        ret = AzioniBuzzer.Emit;
                        break;
                    default:
                        ret = AzioniBuzzer.undefined;
                        break;
                }
                return ret;
            }

            public static string GetName(this AzioniBuzzer tipo)
            {
                string ret;
                switch (tipo)
                {
                    case AzioniBuzzer.Error:
                        ret = "Error";
                        break;
                    case AzioniBuzzer.Emit:
                        ret = "Emit";
                        break;
                    default:
                        ret = "undefined";
                        break;
                }
                return ret;
            }
        }

        public static class Buzzer
        {
            private static int d;
            private static bool doit = true;
            private static OutputPort pinsegnale;
            public static OutputPort PinSegnale { get { return pinsegnale; } }
            public static bool Init { get { return pinsegnale != null; } }

            public static void Emit(object[] gen)
            {
                Emit((int)gen[0], (int)gen[1]);
            }

            public static void Inizializza(OutputPort pin)
            {
                pinsegnale = pin;
            }

            public static void Emit(int freq, int durata)
            {
                if (Init)
                {
                    doit = true;
                    d = durata;
                    Thread th = new Thread(new ThreadStart(Time));
                    th.Start();

                    while (doit)
                    {
                        for (int i = 0; i < freq; i++)
                        {
                            PinSegnale.Write(true);
                            Thread.Sleep(1);
                            PinSegnale.Write(false);
                        }
                    }
                }
            }

            public static void Error() { Emit(440, 2000); }

            private static void Time()
            {
                Thread.Sleep(d);
                doit = false;
                try
                {
                    Thread.CurrentThread.Abort();
                }
                catch (ArgumentException) { }
            }

            public static void Execute(AzioniBuzzer act, int frequenza, int durata)
            {
                MethodInfo method = typeof(Buzzer).GetMethod(act.GetName());
                if (method != null)
                {
                    if (act == EnumExt.GetAzioneBuzzerFromString("Error"))
                    {
                        method.Invoke(null, null);
                    }
                    else if (act == EnumExt.GetAzioneBuzzerFromString("Emit"))
                    {
                        method.Invoke(null, new object[] { frequenza, durata });
                    }
                }
            }
        }

        public class FrameBuzzer : Frames.FrameInIngresso
        {
            public class CaratteristicheFrameBuzzer : CaratteristicheFrameInIngresso
            {
                private AzioniBuzzer _azionebuffer = AzioniBuzzer.undefined;
                private int frequenza = 0, durata = 0;

                public AzioniBuzzer AzioneBuzzer { get { return this._azionebuffer; } set { this._azionebuffer = value; } }
                public int Frequenza { get { return this.frequenza; } set { this.frequenza = value; } }
                public int Durata { get { return this.durata; } set { this.durata = value; } }

                public new bool Accettabile
                {
                    get
                    {
                        return base.Tipo != Frames.TipoFrame.undefined && this.AzioneBuzzer != AzioniBuzzer.undefined;
                    }
                }
            }
            protected new CaratteristicheFrameBuzzer _caratt = new CaratteristicheFrameBuzzer();
            public new CaratteristicheFrameBuzzer Caratteristiche { get { return this._caratt; } }

            #region Costruttori
            public FrameBuzzer(Frames.FrameInIngresso inframe)
                : base(inframe)
            { Assign(inframe.Caratteristiche); }

            public FrameBuzzer(CaratteristicheFrameInIngresso caratt)
                : base(caratt)
            { Assign(caratt); }
            #endregion

            private void Assign(CaratteristicheFrameInIngresso caratt)
            {
                string[] current = GetSplitted(caratt.Testo);
                if (current[1] == AzioniBuzzer.Emit.GetName())
                    this.Caratteristiche.AzioneBuzzer = AzioniBuzzer.Emit;
                else if (current[1] == AzioniBuzzer.Error.GetName())
                    this.Caratteristiche.AzioneBuzzer = AzioniBuzzer.Error;
                else
                    this.Caratteristiche.AzioneBuzzer = AzioniBuzzer.undefined;

                if (this.Caratteristiche.AzioneBuzzer == AzioniBuzzer.Emit)
                {
                    this.Caratteristiche.Frequenza = Convert.ToInt32(current[2]);
                    this.Caratteristiche.Durata = Convert.ToInt32(current[3]);
                }
                GetLength();
            }
        }

        public class BuzzerEventArgs : BaseEventArgs
        {
            private FrameBuzzer _frame;
            public FrameBuzzer Frame { get { return this._frame; } }

            public BuzzerEventArgs(FrameBuzzer frame)
            {
                this._frame = frame;
            }

            public BuzzerEventArgs(object frame)
            {
                this._frame = new FrameBuzzer((Frames.FrameInIngresso)frame);
            }

            public override void Dispose()
            {
                this._frame = null;
                base.Dispose();
            }
        }
    }
}
