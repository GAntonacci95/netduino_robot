using System;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace NetduinoPlusSide.Moduli
{
    namespace Carrello
    {
        using System.Reflection;

        public enum AzioniCarrello
        {
            Apri, Chiudi, Alza, Abbassa, FermaPinza, FermaElevatore, FermaTutto, undefined
        }

        public static class EnumExt
        {
            public static AzioniCarrello GetAzioneCarrelloFromString(string name)
            {
                AzioniCarrello ret;
                switch (name)
                {
                    case "Apri":
                        ret = AzioniCarrello.Apri;
                        break;
                    case "Chiudi":
                        ret = AzioniCarrello.Chiudi;
                        break;
                    case "Alza":
                        ret = AzioniCarrello.Alza;
                        break;
                    case "Abbassa":
                        ret = AzioniCarrello.Abbassa;
                        break;
                    case "FermaPinza":
                        ret = AzioniCarrello.FermaPinza;
                        break;
                    case "FermaElevatore":
                        ret = AzioniCarrello.FermaElevatore;
                        break;
                    case "FermaTutto":
                        ret = AzioniCarrello.FermaTutto;
                        break;
                    default:
                        ret = AzioniCarrello.undefined;
                        break;
                }
                return ret;
            }

            public static string GetName(this AzioniCarrello tipo)
            {
                string ret;
                switch (tipo)
                {
                    case AzioniCarrello.Apri:
                        ret = "Apri";
                        break;
                    case AzioniCarrello.Chiudi:
                        ret = "Chiudi";
                        break;
                    case AzioniCarrello.Alza:
                        ret = "Alza";
                        break;
                    case AzioniCarrello.Abbassa:
                        ret = "Abbassa";
                        break;
                    case AzioniCarrello.FermaPinza:
                        ret = "FermaPinza";
                        break;
                    case AzioniCarrello.FermaElevatore:
                        ret = "FermaElevatore";
                        break;
                    case AzioniCarrello.FermaTutto:
                        ret = "FermaTutto";
                        break;
                    default:
                        ret = "undefined";
                        break;
                }
                return ret;
            }
        }

        #region Componenti
        public class Pinza
        {
            public const bool H = true, L = false;
            private OutputPort _nero, _rosso;

            public OutputPort PinNero { get { return _nero; } }
            public OutputPort PinRosso { get { return _rosso; } }

            #region Methods
            public Pinza(OutputPort nero, OutputPort rosso)
            {
                _nero = nero;
                _rosso = rosso;
                SetStatoIniziale();
            }

            public void SetStatoIniziale()
            {
                _nero.Write(L);
                _rosso.Write(L);
            }

            public void Apri()
            {
                _nero.Write(H);
                _rosso.Write(L);
            }

            public void Chiudi()
            {
                _nero.Write(L);
                _rosso.Write(H);
            }
            #endregion
        }

        public class Elevatore
        {
            public const bool H = true, L = false;
            private OutputPort _nero, _rosso;

            public OutputPort PinNero { get { return _nero; } }
            public OutputPort PinRosso { get { return _rosso; } }

            #region Methods
            public Elevatore(OutputPort nero, OutputPort rosso)
            {
                _nero = nero;
                _rosso = rosso;
                SetStatoIniziale();
            }

            public void SetStatoIniziale()
            {
                _nero.Write(L);
                _rosso.Write(L);
            }

            public void Abbassa()
            {
                _nero.Write(H);
                _rosso.Write(L);
            }

            public void Alza()
            {
                _nero.Write(L);
                _rosso.Write(H);
            }
            #endregion
        }
        #endregion

        public class Carrello
        {
            private static Pinza _pinza;
            private static Elevatore _elev;
            private const double time = 3.5;

            public Carrello(Pinza pinza, Elevatore elev)
            {
                _pinza = pinza;
                _elev = elev;
                while (_pinza == null || elev == null) ;
            }

            public static Pinza Pinza { get { return _pinza; } }
            public static Elevatore Elevatore { get { return _elev; } }

            #region Methods
            #region Azionamento
            public void Apri()
            {
                _pinza.Apri();
                Thread temp = new Thread(new ThreadStart(TemporizzaPinza));
                temp.Start();
            }
            public void Chiudi()
            {
                _pinza.Chiudi();
                Thread temp = new Thread(new ThreadStart(TemporizzaPinza));
                temp.Start();
            }

            public void Alza()
            {
                _elev.Alza();
                Thread temp = new Thread(new ThreadStart(TemporizzaElevatore));
                temp.Start();
            }
            public void Abbassa()
            {
                _elev.Abbassa();
                Thread temp = new Thread(new ThreadStart(TemporizzaElevatore));
                temp.Start();
            }

            private void TemporizzaPinza()
            {
                Thread.Sleep(Tempi.SecondiToMS(time));
                FermaPinza();

            }

            private void TemporizzaElevatore()
            {
                Thread.Sleep(Tempi.SecondiToMS(time));
                FermaElevatore();
            }

            public void FermaPinza() { _pinza.SetStatoIniziale(); }
            public void FermaElevatore() { _elev.SetStatoIniziale(); }
            public void FermaTutto() { FermaPinza(); FermaElevatore(); }
            #endregion

            public void Execute(AzioniCarrello cmd)
            {
                MethodInfo method = typeof(Carrello).GetMethod(cmd.GetName());
                if (method != null)
                    method.Invoke(null, null);
            }
            #endregion
        }

        public class FrameCarrello : Frames.FrameInIngresso
        {
            public class CaratteristicheFrameCarrello : CaratteristicheFrameInIngresso
            {
                private AzioniCarrello azionecarrello = AzioniCarrello.undefined;
                public AzioniCarrello AzioneCarrello { get { return this.azionecarrello; } set { this.azionecarrello = value; } }

                public new bool Accettabile
                {
                    get
                    {
                        return base.Tipo != Frames.TipoFrame.undefined && this.AzioneCarrello != AzioniCarrello.undefined;
                    }
                }
            }
            protected new CaratteristicheFrameCarrello _caratt = new CaratteristicheFrameCarrello();
            public new CaratteristicheFrameCarrello Caratteristiche { get { return this._caratt; } }

            #region Costruttori
            public FrameCarrello(Frames.FrameInIngresso inframe)
                : base(inframe)
            {
                Assign(inframe.Caratteristiche);
            }

            public FrameCarrello(CaratteristicheFrameInIngresso caratt)
                : base(caratt)
            {
                Assign(caratt);
            }
            #endregion

            private void Assign(CaratteristicheFrameInIngresso caratt)
            {
                string[] current = GetSplitted(caratt.Testo);
                this.Caratteristiche.AzioneCarrello = EnumExt.GetAzioneCarrelloFromString(current[1]);
                GetLength();
            }
        }

        public class CarrelloEventArgs : BaseEventArgs
        {
            private FrameCarrello _frame;
            public FrameCarrello Frame { get { return this._frame; } }

            public CarrelloEventArgs(FrameCarrello frame)
            {
                this._frame = frame;
            }

            public CarrelloEventArgs(object frame)
            {
                this._frame = new FrameCarrello((Frames.FrameInIngresso)frame);
            }

            public override void Dispose()
            {
                this._frame = null;
                base.Dispose();
            }
        }
    }
}
