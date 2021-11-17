using System;
using System.Threading.Tasks;

namespace ComputerLibrary.Moduli
{
    namespace Movimento
    {
        public enum Direzioni
        {
            CurvaSinistra = ArrowKey.Left, Avanti = ArrowKey.Up,
            CurvaDestra = ArrowKey.Right, Indietro = ArrowKey.Down, Fermo = 0,
            undefined = -1
        }

        public static class EnumExt
        {
            public static Direzioni GetDirezioneFromString(string name)
            {
                Direzioni ret;
                switch (name)
                {
                    case "CurvaSinistra":
                        ret = Direzioni.CurvaSinistra;
                        break;
                    case "Avanti":
                        ret = Direzioni.Avanti;
                        break;
                    case "CurvaDestra":
                        ret = Direzioni.CurvaDestra;
                        break;
                    case "Indietro":
                        ret = Direzioni.Indietro;
                        break;
                    case "Fermo":
                        ret = Direzioni.Fermo;
                        break;
                    default:
                        ret = Direzioni.undefined;
                        break;
                }
                return ret;
            }

            public static string GetName(this Direzioni tipo)
            {
                string ret;
                switch (tipo)
                {
                    case Direzioni.CurvaSinistra:
                        ret = "CurvaSinistra";
                        break;
                    case Direzioni.Avanti:
                        ret = "Avanti";
                        break;
                    case Direzioni.CurvaDestra:
                        ret = "CurvaDestra";
                        break;
                    case Direzioni.Indietro:
                        ret = "Indietro";
                        break;
                    case Direzioni.Fermo:
                        ret = "Fermo";
                        break;
                    default:
                        ret = "undefined";
                        break;
                }
                return ret;
            }
        }

        public class FrameMovimento : Frames.FrameInUscita
        {
            public class CaratteristicheFrameMovimento : CaratteristicheFrameInUscita
            {
                private Direzioni direzione = Direzioni.undefined;
                public Direzioni Direzione { get { return this.direzione; } set { this.direzione = value; } }

                public new bool Accettabile
                {
                    get
                    {
                        return base.Tipo != Frames.TipoFrame.undefined && this.Direzione != Direzioni.undefined;
                    }
                }
            }
            protected new CaratteristicheFrameMovimento _caratt = new CaratteristicheFrameMovimento();
            public new CaratteristicheFrameMovimento Caratteristiche { get { return this._caratt; } }


            public FrameMovimento(Direzioni direzione)
            {
                base.Caratteristiche.Tipo = this.Caratteristiche.Tipo = Frames.TipoFrame.mov;
                this.Caratteristiche.Direzione = direzione;
                SubBuild();
                Build();
            }

            protected void SubBuild()
            {
                base.Caratteristiche.Testo = InitFine + Separatore + Frames.EnumExt.GetName(base.Caratteristiche.Tipo)
                    + Separatore + this.Caratteristiche.Direzione.GetName() + Separatore + InitFine;
            }
        }
        public class MovimentoEventArgs : BaseEventArgs
        {
            private FrameMovimento _frame;

            public FrameMovimento Frame { get { return this._frame; } }
            public Direzioni Direzione { get { return this._frame.Caratteristiche.Direzione; } }

            public MovimentoEventArgs(FrameMovimento frame)
            {
                this._frame = frame;
            }

            public MovimentoEventArgs(Direzioni movimento)
            {
                this._frame = new FrameMovimento(movimento);
            }

            public override void Dispose()
            {
                this._frame = null;
                base.Dispose();
            }
        }
    }
}
