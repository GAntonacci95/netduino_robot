using System;
using System.Threading;

namespace ComputerLibrary.Moduli
{
    namespace Temperatura
    {
        public delegate void TemperaturaRefreshEventHandler(TemperaturaRefreshEventArgs e);

        public static class ModuloTemperatura
        {
            public const int RESISTORE = 10000, ADCBITS = 10, PRECISION = 1024;
            public const double FIRST_CONSTANT = 0.001129148,
                SECOND_CONSTANT = 0.000234125,
                THIRD_CONSTANT = 0.0000000876741, KELVIN_CONSTANT = 273.15;

            public static event TemperaturaRefreshEventHandler TemperaturaRefreshEvent;

            private static double temperatura;
            public static double Temperatura { get { return temperatura; } }

            public static void BeginProcessing(int campione)
            {
                temperatura = GetTemperature(campione).Round();

                Thread th = new Thread(new ThreadStart(Processor));
                th.Start();
            }

            private static void Processor()
            {
                while (TemperaturaRefreshEvent == null) ;
                if (TemperaturaRefreshEvent != null)
                    TemperaturaRefreshEvent(new TemperaturaRefreshEventArgs(temperatura));
            }

            public static double GetTemperature(int bites)
            {
                if (bites != 0)
                {
                    double tmp = System.Math.Log(((PRECISION * RESISTORE / bites) - RESISTORE));
                    double ret = (1 / (FIRST_CONSTANT + (SECOND_CONSTANT + (THIRD_CONSTANT * tmp * tmp)) * tmp)) - KELVIN_CONSTANT;
                    return ret;
                }
                else return -1;
            }
        }

        public class TemperaturaRefreshEventArgs : EventArgs
        {
            private double _temperatura;
            public string GradiCelsius
            {
                get
                {
                    bool invalid = _temperatura < 0 ? true : false;
                    return invalid ? "" : _temperatura.ToString();
                }
            }

            public string GradiKelvin
            {
                get
                {
                    bool invalid = _temperatura < 0 ? true : false;
                    return invalid ? "" : (_temperatura + ModuloTemperatura.KELVIN_CONSTANT).ToString();
                }
            }

            public TemperaturaRefreshEventArgs(Nullable<double> temperatura)
            {
                if (temperatura == null)
                    _temperatura = -1;
                else
                    _temperatura = (double)temperatura;
            }
        }

        public class FrameTemperatura : Frames.FrameInIngresso
        {
            public class CaratteristicheFrameTemperatura : CaratteristicheFrameInIngresso
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

            #region Costruttori
            public FrameTemperatura(Frames.FrameInIngresso inframe)
                : base(inframe)
            {
                Assign(inframe.Caratteristiche);
            }

            public FrameTemperatura(CaratteristicheFrameInIngresso caratt)
                : base(caratt)
            { Assign(caratt); }
            #endregion

            private void Assign(CaratteristicheFrameInIngresso caratt)
            {
                string[] current = GetSplitted(caratt.Testo);
                this.Caratteristiche.Campione = Convert.ToInt32(current[1]);
                GetLength();
            }
        }

        public class TemperaturaEventArgs : BaseEventArgs
        {
            private FrameTemperatura _frame;
            public FrameTemperatura Frame { get { return this._frame; } }

            public TemperaturaEventArgs(FrameTemperatura frame)
            {
                this._frame = frame;
            }

            public TemperaturaEventArgs(Frames.FrameInIngresso frame)
            {
                this._frame = new FrameTemperatura(frame);
            }

            public override void Dispose()
            {
                this._frame = null;
                base.Dispose();
            }
        }

        #region Extension Methods
        public static class DoubleExtension
        {
            public static double Round(this double num)
            {
                return Math.Round(num, 2);
            }
        }
        #endregion
    }
}
