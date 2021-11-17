using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ComputerLibrary;

namespace ComputerSide
{
    using ComputerLibrary.ArrowHandle;
    using ComputerLibrary.Frames;
    using ComputerLibrary.Moduli.Temperatura;
    using ComputerLibrary.Moduli.Movimento;
    using ComputerLibrary.Moduli.Carrello;
    using ComputerLibrary.Moduli.Buzzer;

    public partial class NUI : Form
    {
        static XBeeCoordinator coord;
        static PictureBox[] ArrowPictureBoxes;
        static Button[] btnIcons;
        static ArrowHandler AH;
        static PBRuote RuotaDX, RuotaSX;

        public NUI()
        {
            coord = new XBeeCoordinator("COM3");
            coord.NetduinoConnection += new NetduinoConnectionEventHandler(NetConnection);
            coord.FrameRicevuto += new FrameRicevutoEventHandler(SmistatoreRicezione);
            try
            {
                coord.Connect();
                coord.BeginReceive();
            }
            catch (System.IO.IOException ex) { MessageBox.Show(ex.Message, ex.GetType().ToString()); }
            InitializeComponent();

            ArrowPictureBoxes = new PictureBox[] { pbLeft, pbUp, pbRight, pbDown };
            btnIcons = new Button[] { btnBuzzer, btnFerma, btnApri, btnChiudi, btnAlza, btnAbbassa };
            RuotaDX = new PBRuote(ref pbRDx); RuotaSX = new PBRuote(ref pbRSx);
            AH = new ArrowHandler(ref ArrowPictureBoxes);

            #region Gestione NUI
            this.BackgroundImage = FormImages.Background;
            pbNUI.Image = FormImages.Robot.ConPinzaAperta;
            pbRDx.Image = FormImages.Ruota;
            pbRSx.Image = FormImages.Ruota;
            btnBuzzer.Image = Icons.Buzzer.Normale;
            btnFerma.Image = Icons.Ferma.Normale;
            btnApri.Image = Icons.Pinza.Aperta.Rilasciato;
            btnChiudi.Image = Icons.Pinza.Chiusa.Rilasciato;
            btnAlza.Image = Icons.Elevatore.Alzato.Rilasciato;
            btnAbbassa.Image = Icons.Elevatore.Abbassato.Rilasciato;
            #endregion
        }

        private void Locker()
        {
            gbInput.Enabled = false;
            treeLog.Nodes.Clear();
            this.KeyPreview = false;
        }

        private void Unlocker()
        {
            gbInput.Enabled = true;
            treeLog.Nodes.Clear();
            this.KeyPreview = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.KeyPreview)
            {
                if (ArrowHandler.IsAnArrow(msg.WParam))
                {
                    if (msg.Msg == EventCode.WM_KEYDOWN)
                        keyDown(this, new KeyEventArgs(keyData));
                    else if (msg.Msg == EventCode.WM_KEYUP)
                        keyUp(this, new KeyEventArgs(keyData));
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void NUI_Load(object sender, EventArgs e)
        {
            this.Activate();

            #region Gestione Arrow Keys
            PersPB[] ArrowKeys = { new PersPB(ref pbUp), new PersPB(ref pbDown),
                                     new PersPB(ref pbLeft), new PersPB(ref pbRight) };

            for (int i = 0; i < ArrowKeys.Length; i++)
            {
                ArrowKeys[i].BasePB.SizeMode = PictureBoxSizeMode.StretchImage;

                ArrowKeys[i].BasePB.MouseDown += new MouseEventHandler(pbClicked);
                ArrowKeys[i].BasePB.MouseUp += new MouseEventHandler(pbReleased);
            }

            AH.ArrowHandlerEvent += new ExecuteEventHandler(SmistatoreInvio);
            #endregion

            #region Icons
            for (int i = 0; i < btnIcons.Length; i++)
            {
                btnIcons[i].MouseDown += new MouseEventHandler(btnClicked);
                btnIcons[i].MouseUp += new MouseEventHandler(btnReleased);
            }
            #endregion

            #region Gestione Ruote
            RuotaDX.ImageRefresh += new ImageRefreshEventHandler(ImageSetterRuotaDX);
            RuotaSX.ImageRefresh += new ImageRefreshEventHandler(ImageSetterRuotaSX);
            #endregion

            this.KeyPreview = true;
            Locker();

            this.KeyDown += new KeyEventHandler(keyDown);
            this.KeyUp += new KeyEventHandler(keyUp);

            //ModuloVideocamera.ImageReadyEvent += new ImageReadyEventHandler(ImageSetter);
            ModuloTemperatura.TemperaturaRefreshEvent += new TemperaturaRefreshEventHandler(TempSetter);
        }

        private void NUI_Shown(object sender, EventArgs e)
        {
            #region Gestione Opacity
            this.Opacity = 0;
            for (double i = 0; i <= 1; i += 0.01)
            {
                Thread.Sleep(5);
                this.Opacity = i;
            }
            #endregion
        }

        private void NetConnection(object sender, NetduinoConnectionEventArgs e)
        {
            if (e.Stato == StatoConnessione.Connesso)
                try
                {
                    this.Invoke(new MethodInvoker(Unlocker));
                }
                catch (Exception) { Application.Restart(); }
            else
                this.Invoke(new MethodInvoker(Locker));
            TreeSetter(null, new LogRefreshEventArgs(new TreeNode(e.Testo)));
        }

        #region Ricezione
        private void SmistatoreRicezione(object sender, FrameReceivedEventArgs e)
        {
            FrameInIngresso frame = new FrameInIngresso(e.Flusso);
            Type T = frame.GetFrameType();

            if (T != null)
            {
                switch (T.Name)
                {
                    case "FrameTemperatura":
                        FrameRicevuto(sender, new TemperaturaEventArgs(frame));
                        break;/*
                    case "FrameVideocamera":
                        FrameRicevuto(sender, new VideocameraEventArgs(frame));
                        break;*/
                    default:
                        MessageBox.Show("Unknown Received, maybe something went wrong!");
                        break;
                }
            }
        }

        private void FrameRicevuto(object sender, TemperaturaEventArgs e)
        {
            FrameTemperatura frame = e.Frame;

            if (frame.Caratteristiche.Campione > 0)
                ModuloTemperatura.BeginProcessing(frame.Caratteristiche.Campione);
        }
        /*
        private void FrameRicevuto(object sender, VideocameraEventArgs e)
        {
            FrameVideocamera frame = e.Frame;

            if (frame.Caratteristiche.StreamVideocamera != null)
                ModuloVideocamera.AddFrame(frame);
        }*/
        #endregion

        #region Event Handler Moduli
        public void TempSetter(TemperaturaRefreshEventArgs e)
        {
            lbltemperatura.Invoke(new MethodInvoker(delegate { lbltemperatura.Text = e.GradiCelsius + "°C"; }));
        }

        public void TreeSetter(object sender, LogRefreshEventArgs e)
        {
            try
            {
                treeLog.Invoke(new MethodInvoker(delegate
                {
                    treeLog.Nodes.Add(e.Node);
                    treeLog.Nodes[treeLog.Nodes.Count - 1].EnsureVisible();
                }));
            }
            catch (InvalidOperationException) { }
        }

        /*
        public void ImageSetter(ImageReadyEventArgs e)
        {
            try
            {
                pbCam.Invoke(new MethodInvoker(delegate
                {
                    pbCam.Image = e.Image;
                }));
            }
            catch (InvalidOperationException) { }
        }*/
        #endregion

        #region Gestione Arrow Keys
        #region Mouse
        private void pbClicked(object sender, MouseEventArgs e)
        {
            PictureBox tmp = (PictureBox)sender;
            PersPB tempforindex = new PersPB(ref tmp);
            AH.Handler(null, new ArrowEventArgs(tempforindex.KeyNumber, Stato.Premuto));
        }

        private void pbReleased(object sender, MouseEventArgs e)
        {
            PictureBox tmp = (PictureBox)sender;
            PersPB tempforindex = new PersPB(ref tmp);
            AH.Handler(null, new ArrowEventArgs(tempforindex.KeyNumber, Stato.Rilasciato));
        }
        #endregion

        #region Tastiera
        private void keyDown(object sender, KeyEventArgs e)
        {
            int code = (int)e.KeyCode;
            if (ArrowHandler.IsAnArrow(code))
            {
                AH.Handler(null, new ArrowEventArgs(code, Stato.Premuto));
            }
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            int code = (int)e.KeyCode;
            if (ArrowHandler.IsAnArrow(code))
            {
                AH.Handler(null, new ArrowEventArgs(code, Stato.Rilasciato));
            }
        }
        #endregion
        #endregion

        #region Gestione Immagini Ruote
        private void ImageSetterRuotaDX(object sender, ImageRefreshEventArgs e)
        {
            try
            {
                pbRDx.Invoke(new MethodInvoker(delegate { pbRDx.Image = e.Image; }));
            }
            catch (Exception) { }
        }

        private void ImageSetterRuotaSX(object sender, ImageRefreshEventArgs e)
        {
            try
            {
                pbRSx.Invoke(new MethodInvoker(delegate { pbRSx.Image = e.Image; }));
            }
            catch (Exception) { }
        }
        #endregion

        #region Invii
        private void SmistatoreInvio(object sender, EventArgs e)
        {
            Type T = e.GetType();
            if (T != null)
            {
                switch (T.Name)
                {
                    case "MovimentoEventArgs":
                        PreInvio(sender, (MovimentoEventArgs)e);
                        break;
                    case "CarrelloEventArgs":
                        PreInvio(sender, (CarrelloEventArgs)e);
                        break;
                    case "BuzzerEventArgs":
                        PreInvio(sender, (BuzzerEventArgs)e);
                        break;
                    default:
                        MessageBox.Show("Unknown Selection, maybe something went wrong!");
                        break;
                }
            }
        }

        private void PreInvio(object sender, MovimentoEventArgs e)
        {
            TreeNode node = new TreeNode(e.Direzione.ToString());
            FrameMovimento frm = e.Frame;

            SendCMD(frm);

            switch (e.Direzione)
            {
                case Direzioni.Avanti: RuotaDX.Avanti(); RuotaSX.Avanti(); break;
                case Direzioni.Indietro: RuotaDX.Indietro(); RuotaSX.Indietro(); break;
                case Direzioni.CurvaDestra: RuotaDX.Indietro(); RuotaSX.Avanti(); break;
                case Direzioni.CurvaSinistra: RuotaDX.Avanti(); RuotaSX.Indietro(); break;
                case Direzioni.Fermo: RuotaDX.Ferma(); RuotaSX.Ferma(); break;
            }

            TreeSetter(null, new LogRefreshEventArgs(node));
        }

        private void PreInvio(object sender, CarrelloEventArgs e)
        {
            TreeNode node = new TreeNode(e.AzioneCarrello.ToString());
            FrameCarrello frm = e.Frame;

            SendCMD(frm);

            switch (e.AzioneCarrello)
            {
                case AzioniCarrello.Apri:
                    pbNUI.Invoke(new MethodInvoker(delegate { pbNUI.Image = FormImages.Robot.ConPinzaAperta; }));
                    break;
                case AzioniCarrello.Chiudi:
                    pbNUI.Invoke(new MethodInvoker(delegate { pbNUI.Image = FormImages.Robot.ConPinzaChiusa; }));
                    break;
            }

            TreeSetter(null, new LogRefreshEventArgs(node));
        }

        private void PreInvio(object sender, BuzzerEventArgs e)
        {
            TreeNode node = new TreeNode(e.AzioneBuzzer.ToString());
            FrameBuzzer frm = e.Frame;

            SendCMD(frm);

            TreeSetter(null, new LogRefreshEventArgs(node));
        }
        #endregion

        private void SendCMD(FrameInUscita comando)
        {
            coord.Send(comando);
        }

        private void NUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            coord.Disconnect();
            RuotaDX.Ferma();
            RuotaSX.Ferma();
        }

        #region Gestione Bottoni
        private void btnClicked(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            string name = btn.Name.Replace("btn", "");
            switch (name)
            {
                case "Buzzer":
                    btnBuzzer.Image = Icons.Buzzer.Suonato;
                    PreInvio(null, new BuzzerEventArgs(AzioniBuzzer.Error));
                    break;
                case "Ferma":
                    btnFerma.Image = Icons.Ferma.Fermato;
                    PreInvio(null, new CarrelloEventArgs(AzioniCarrello.FermaTutto));
                    break;
                case "Apri":
                    btnApri.Image = Icons.Pinza.Aperta.Premuto;
                    PreInvio(null, new CarrelloEventArgs(AzioniCarrello.Apri));
                    break;
                case "Chiudi":
                    btnChiudi.Image = Icons.Pinza.Chiusa.Premuto;
                    PreInvio(null, new CarrelloEventArgs(AzioniCarrello.Chiudi));
                    break;
                case "Alza":
                    btnAlza.Image = Icons.Elevatore.Alzato.Premuto;
                    PreInvio(null, new CarrelloEventArgs(AzioniCarrello.Alza));
                    break;
                case "Abbassa":
                    btnAbbassa.Image = Icons.Elevatore.Abbassato.Premuto;
                    PreInvio(null, new CarrelloEventArgs(AzioniCarrello.Abbassa));
                    break;
            }
        }

        private void btnReleased(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            string name = btn.Name.Replace("btn", "");
            switch (name)
            {
                case "Buzzer":
                    btnBuzzer.Image = Icons.Buzzer.Normale;
                    break;
                case "Ferma":
                    btnFerma.Image = Icons.Ferma.Normale;
                    break;
                case "Apri":
                    btnApri.Image = Icons.Pinza.Aperta.Rilasciato;
                    break;
                case "Chiudi":
                    btnChiudi.Image = Icons.Pinza.Chiusa.Rilasciato;
                    break;
                case "Alza":
                    btnAlza.Image = Icons.Elevatore.Alzato.Rilasciato;
                    break;
                case "Abbassa":
                    btnAbbassa.Image = Icons.Elevatore.Abbassato.Rilasciato;
                    break;
            }
        }
        #endregion
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