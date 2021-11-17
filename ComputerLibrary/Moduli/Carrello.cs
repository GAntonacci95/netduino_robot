using System;
using System.Threading.Tasks;

namespace ComputerLibrary.Moduli
{
    namespace Carrello
    {
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

        public class FrameCarrello : Frames.FrameInUscita
        {
            public class CaratteristicheFrameCarrello : CaratteristicheFrameInUscita
            {
                private AzioniCarrello azionecarrello = AzioniCarrello.undefined;
                public AzioniCarrello AzioneCarrello { get { return this.azionecarrello; }
                    set { this.azionecarrello = value; } }

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

            public FrameCarrello(AzioniCarrello azione)
            {
                base.Caratteristiche.Tipo = this.Caratteristiche.Tipo = Frames.TipoFrame.carr;
                this.Caratteristiche.AzioneCarrello = azione;
                SubBuild();
                Build();
            }

            protected void SubBuild()
            {
                base.Caratteristiche.Testo = InitFine + Separatore + Frames.EnumExt.GetName(base.Caratteristiche.Tipo)
                    + Separatore + this.Caratteristiche.AzioneCarrello.GetName() + Separatore + InitFine;
            }
        }

        public class CarrelloEventArgs : BaseEventArgs
        {
            private FrameCarrello _frame;

            public FrameCarrello Frame { get { return this._frame; } }
            public AzioniCarrello AzioneCarrello { get { return this._frame.Caratteristiche.AzioneCarrello; } }

            public CarrelloEventArgs(FrameCarrello frame)
            {
                this._frame = frame;
            }

            public CarrelloEventArgs(AzioniCarrello azione)
            {
                this._frame = new FrameCarrello(azione);
            }

            public override void Dispose()
            {
                this._frame = null;
                base.Dispose();
            }
        }
    }
}
