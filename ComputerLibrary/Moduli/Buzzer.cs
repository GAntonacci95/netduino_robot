using System;
using System.Threading.Tasks;

namespace ComputerLibrary.Moduli
{
    namespace Buzzer
    {
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

        public class FrameBuzzer : Frames.FrameInUscita
        {
            public class CaratteristicheFrameBuzzer : CaratteristicheFrameInUscita
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

            public FrameBuzzer(AzioniBuzzer azione, int frequenza = 0, int durata = 0)
            {
                base.Caratteristiche.Tipo = this.Caratteristiche.Tipo = Frames.TipoFrame.buz;
                this.Caratteristiche.AzioneBuzzer = azione;
                this.Caratteristiche.Frequenza = frequenza;
                this.Caratteristiche.Durata = durata;
                SubBuild();
                Build();
            }

            protected void SubBuild()
            {
                base.Caratteristiche.Testo = InitFine + Separatore + Frames.EnumExt.GetName(base.Caratteristiche.Tipo)
                    + Separatore + this.Caratteristiche.AzioneBuzzer.GetName() + Separatore + this.Caratteristiche.Frequenza
                    + Separatore + this.Caratteristiche.Durata + Separatore + InitFine;
            }
        }

        public class BuzzerEventArgs : BaseEventArgs
        {
            private FrameBuzzer _frame;

            public FrameBuzzer Frame { get { return this._frame; } }
            public AzioniBuzzer AzioneBuzzer { get { return this._frame.Caratteristiche.AzioneBuzzer; } }
            public int Frequenza { get { return this._frame.Caratteristiche.Frequenza; } }
            public int Durata { get { return this._frame.Caratteristiche.Durata; } }

            public BuzzerEventArgs(FrameBuzzer frame)
            {
                this._frame = frame;
            }

            public BuzzerEventArgs(AzioniBuzzer azione)
            {
                this._frame = new FrameBuzzer(azione);
            }

            public BuzzerEventArgs(AzioniBuzzer azione, int frequenza, int durata)
            {
                this._frame = new FrameBuzzer(azione, frequenza, durata);
            }

            public override void Dispose()
            {
                this._frame = null;
                base.Dispose();
            }
        }
    }
}
