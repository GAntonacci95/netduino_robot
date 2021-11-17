using System;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace NetduinoPlusSide.Moduli
{
    namespace Movimento
    {
        using System.Reflection;

        public enum Direzioni
        {
            CurvaSinistra, Avanti,
            CurvaDestra, Indietro, Fermo, undefined
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

        #region Componenti
        public class Motore
        {
            public const bool H = true, L = false;
            private OutputPort _nero, _rosso;

            public OutputPort PinNero { get { return _nero; } }
            public OutputPort PinRosso { get { return _rosso; } }

            public Motore(OutputPort nero, OutputPort rosso)
            {
                _rosso = rosso;
                _nero = nero;
                SetDefault();
            }

            public void SetDefault()
            {
                _rosso.Write(L);
                _nero.Write(L);
            }

            public void Avanti()
            {
                _rosso.Write(H);
                _nero.Write(L);
            }

            public void Indietro()
            {
                _nero.Write(H);
                _rosso.Write(L);
            }

            public void Ferma()
            {
                SetDefault();
            }
        }
        #endregion

        public class Movimento
        {
            private static Motore _motoredestro, _motoresinistro;

            public static Motore MotoreDestro { get { return _motoredestro; } }
            public static Motore MotoreSinistro { get { return _motoresinistro; } }

            public Movimento(Motore destro, Motore sinistro)
            {
                _motoresinistro = sinistro;
                _motoredestro = destro;
                while (_motoredestro == null || _motoresinistro == null) ;
            }

            #region Methods
            #region Azionamento
            public void Avanti()
            {
                _motoredestro.Avanti();
                _motoresinistro.Avanti();
            }

            public void Indietro()
            {
                _motoredestro.Indietro();
                _motoresinistro.Indietro();
            }

            public void CurvaDestra()
            {
                _motoredestro.Indietro();
                _motoresinistro.Avanti();
            }

            public void CurvaSinistra()
            {
                _motoredestro.Avanti();
                _motoresinistro.Indietro();
            }

            public void Fermo()
            {
                _motoredestro.Ferma();
                _motoresinistro.Ferma();
            }
            #endregion

            public void Execute(Direzioni cmd)
            {
                MethodInfo method = typeof(Movimento).GetMethod(cmd.GetName());
                if (method != null)
                    method.Invoke(null, null);
            }
            #endregion
        }

        public class FrameMovimento : Frames.FrameInIngresso
        {
            public class CaratteristicheFrameMovimento : CaratteristicheFrameInIngresso
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

            #region Costruttori
            public FrameMovimento(Frames.FrameInIngresso inframe)
                : base(inframe)
            {
                Assign(inframe.Caratteristiche);
            }

            public FrameMovimento(CaratteristicheFrameInIngresso caratt)
                : base(caratt)
            { Assign(caratt); }
            #endregion

            private void Assign(CaratteristicheFrameInIngresso caratt)
            {
                string[] current = GetSplitted(caratt.Testo);
                this.Caratteristiche.Direzione = EnumExt.GetDirezioneFromString(current[1]);
                GetLength();
            }
        }

        public class MovimentoEventArgs : BaseEventArgs
        {
            private FrameMovimento _frame;
            public FrameMovimento Frame { get { return _frame; } }

            public MovimentoEventArgs(FrameMovimento frame)
            {
                this._frame = frame;
            }

            public MovimentoEventArgs(object frame)
            {
                this._frame = new FrameMovimento((Frames.FrameInIngresso)frame);
            }

            public override void Dispose()
            {
                this._frame = null;
                base.Dispose();
            }
        }
    }
}
