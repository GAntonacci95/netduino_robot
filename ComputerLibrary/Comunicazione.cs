using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;

namespace ComputerLibrary
{
    using ComputerLibrary.Moduli.Temperatura;
    using ComputerLibrary.Moduli.Movimento;
    using ComputerLibrary.Moduli.Carrello;
    using ComputerLibrary.Moduli.Buzzer;
    using Frames;

    public enum StatoConnessione { Connesso, Disconnesso }

    public delegate void FrameRicevutoEventHandler(object sender, FrameReceivedEventArgs e);
    public delegate void NetduinoConnectionEventHandler(object sender, NetduinoConnectionEventArgs e);

    public class NetduinoConnectionEventArgs : EventArgs
    {
        private string _testo;
        private StatoConnessione _stato;
        public NetduinoConnectionEventArgs(StatoConnessione stato)
        {
            _stato = stato;
            _testo = "Netduino " + stato + "!";
        }

        public NetduinoConnectionEventArgs(bool stato)
        {
            if (stato)
                _stato = StatoConnessione.Connesso;
            else _stato = StatoConnessione.Disconnesso;
            _testo = "Netduino " + stato + "!";
        }

        public string Testo { get { return _testo; } }
        public StatoConnessione Stato { get { return _stato; } }
    }
    public class FrameReceivedEventArgs : EventArgs
    {
        private byte[] flusso;
        public FrameReceivedEventArgs(byte[] arr)
        {
            flusso = arr;
        }
        public byte[] Flusso { get { return flusso; } }
    }

    public class XBeeCoordinator
    {
        public event FrameRicevutoEventHandler FrameRicevuto;
        public event NetduinoConnectionEventHandler NetduinoConnection;

        private SerialPort XBee;
        private bool connesso = false;

        public XBeeCoordinator(string port, int baudrate = 9600,
            Parity parity = Parity.None, int databits = 8)
        {
            XBee = new SerialPort(port, baudrate, parity, databits);
        }

        #region Methods
        #region Connessione
        public void Connect()
        {
            XBee.DtrEnable = true;
            XBee.RtsEnable = true;
            XBee.Handshake = Handshake.XOnXOff;
            XBee.Open();
        }

        public void Disconnect()
        {
            XBee.Close();
        }
        #endregion

        #region Invio
        public void Send(FrameInUscita frame)
        {
            if (frame.Caratteristiche.Accettabile)
            {
                string tosend = frame.ToString();

                byte[] ts = Frames.Frame.NAndCoder.UTF8.GetBytes(tosend);
                XBee.Write(ts, 0, ts.Length);
            }
        }
        #endregion

        #region Ricezione
        public void BeginReceive()
        {
            XBee.DataReceived += new SerialDataReceivedEventHandler(Receive);
        }

        private byte[] buf = null;
        private string alldata = "";
        private int error = 0;
        private void Receive(object sender, SerialDataReceivedEventArgs e)
        {
            alldata += XBee.ReadExisting();
            buf = Frame.NAndCoder.UTF8.GetBytes(alldata);

            if (Frame.Utilities.ContainsOnlyValidChars(alldata))
            {
                if (buf.Count(elem => elem == (byte)Frame.InitFine[0]) == 2)
                {
                    if (FrameRicevuto != null)
                    {
                        if (!connesso)
                        {
                            if (NetduinoConnection != null)
                                NetduinoConnection(this,
                                    new NetduinoConnectionEventArgs(StatoConnessione.Connesso));
                            connesso = true;
                        }
                        FrameRicevuto(this, new FrameReceivedEventArgs((byte[])buf));
                        ClearBuffers();
                    }
                }
                else if (buf != null)
                {
                    error++;
                    if (error > 3)
                        ClearBuffers();
                }
            }
            else ClearBuffers();
        }

        private void ClearBuffers()
        {
            XBee.DiscardInBuffer();
            error = 0;
            alldata = "";
            buf = null;
        }
        #endregion
        #endregion
    }

    namespace Frames
    {
        public enum TipoFrame
        {
            mov, temp, cam, carr, buz, undefined
        }

        public static class EnumExt
        {
            public static string GetName(this TipoFrame tipo)
            {
                string ret;
                switch (tipo)
                {
                    case TipoFrame.buz:
                        ret = "buz";
                        break;
                    case TipoFrame.cam:
                        ret = "cam";
                        break;
                    case TipoFrame.carr:
                        ret = "carr";
                        break;
                    case TipoFrame.mov:
                        ret = "mov";
                        break;
                    case TipoFrame.temp:
                        ret = "temp";
                        break;
                    default:
                        ret = "undefined";
                        break;
                }
                return ret;
            }

            public static Frames.TipoFrame GetTipoFrameFromString(string name)
            {
                Frames.TipoFrame ret;
                switch (name)
                {
                    case "buz":
                        ret = Frames.TipoFrame.buz;
                        break;
                    case "cam":
                        ret = Frames.TipoFrame.cam;
                        break;
                    case "carr":
                        ret = Frames.TipoFrame.carr;
                        break;
                    case "mov":
                        ret = Frames.TipoFrame.mov;
                        break;
                    case "temp":
                        ret = Frames.TipoFrame.temp;
                        break;
                    default:
                        ret = Frames.TipoFrame.undefined;
                        break;
                }
                return ret;
            }
        }

        public class Frame : IDisposable
        {
            public class CaratteristicheFrame
            {
                private TipoFrame _tipo = TipoFrame.undefined;
                private string _testo = "";

                public TipoFrame Tipo { get { return this._tipo; } set { this._tipo = value; } }
                public bool Accettabile
                {
                    get
                    {
                        bool ret = this.Tipo != TipoFrame.undefined || Utilities.IsValidText(_testo);
                        return ret;
                    }
                }
                public string Testo { get { return this._testo; } set { this._testo = value; } }
            }

            protected static string _initfine = "\n", _sep = "\r", _subsep = ";";
            protected CaratteristicheFrame _caratt = new CaratteristicheFrame();
            protected int _length = 0;

            public CaratteristicheFrame Caratteristiche { get { return this._caratt; } }
            public static string Separatore { get { return _sep; } }
            public static string InitFine { get { return _initfine; } }
            public static string SubSeparatore { get { return _subsep; } }
            public int Length { get { return this._length; } }

            #region Costruttori
            public Frame(TipoFrame tipo)
            {
                this.Caratteristiche.Tipo = tipo;
            }

            public Frame()
            {
                this.Caratteristiche.Tipo = TipoFrame.undefined;
            }
            #endregion

            public static class Utilities
            {
                public static string ValidChars { get { return "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz\n\r"; } }

                public static bool Contains(string into, string piece)
                {
                    return into.IndexOf(piece) != -1;
                }

                public static bool Contains(string into, char character)
                {
                    return into.IndexOf(character) != -1;
                }

                public static bool ContainsOnlyValidChars(string into)
                {
                    bool ret = true;
                    if (into == string.Empty)
                        return false;
                    foreach (char c in into)
                    {
                        if (!Contains(ValidChars, c))
                            ret = false;
                    }
                    return ret;
                }

                public static string[] ExceptFirst(string[] arr)
                {
                    string[] ret = new string[arr.Length - 1];
                    for (int i = 1; i < arr.Length; i++)
                        ret[i - 1] = arr[i];
                    return ret;
                }

                #region Compressione
                public static char UnknownChar { get { return (char)65533; } }

                public static string[] Normalize(string[] input)
                {
                    int dim = 0;
                    foreach (string s in input)
                    {
                        if (s != string.Empty)
                            dim++;
                    }
                    string[] ret = new string[dim];

                    int pos = 0;
                    for (int i = 0; i < input.Length; i++)
                    {
                        if (input[i] != string.Empty)
                        {
                            foreach (char c in input[i])
                                if (c != UnknownChar) ret[pos] += c;
                            pos++;
                        }
                    }
                    return ret;
                }

                public static byte[] Trim(byte[] input)
                {
                    int ncharto = 0, ncharfrom = 0;

                    for (int i = 0; i < input.Length; i++)
                    {
                        if (input[i] != (byte)_initfine[0])
                            ncharto++;
                        else break;
                    }

                    for (int i = input.Length - 1; i >= 0; i--)
                    {
                        if (input[i] != (byte)_initfine[0])
                            ncharfrom++;
                        else break;
                    }

                    byte[] ret = null;

                    try
                    {
                        ret = new byte[input.Length - (ncharto + ncharfrom)];
                    }
                    catch (OverflowException) { return null; }

                    int pos = 0;
                    for (int i = ncharto; i < input.Length - ncharfrom; i++)
                    {
                        ret[pos] = input[i];
                        pos++;
                    }

                    return ret;
                }

                public static byte[] Normalize(byte[] input)
                {
                    int dim = 0;
                    int cntinitfine = 0;
                    foreach (byte b in input)
                    {
                        if ((char)b == _initfine[0])
                        {
                            cntinitfine++;
                            if (cntinitfine != 2)
                            {
                                dim++;
                            }
                            else { break; }
                        }
                    }
                    byte[] ret = new byte[dim];

                    int pos = 0;
                    for (int i = 0; i < input.Length; i++)
                    {
                        if (input[i] != (byte)0)
                        {
                            ret[pos] = input[i];
                            pos++;
                        }
                    }
                    return ret;
                }
                #endregion

                #region Validità
                public static bool IsValidText(string _testo)
                {
                    int sep = 0, init = 0;
                    foreach (char c in _testo)
                        if (c == Separatore[0]) sep++;
                        else if (c == InitFine[0]) init++;

                    return sep > 2 && init == 2;
                }

                public static bool IsValidByteFlush(byte[] arr)
                {
                    string tmp = NAndCoder.UTF8.GetString(arr);
                    return IsValidText(tmp);
                }
                #endregion
            }

            public static class NAndCoder
            {
                public static class UTF8
                {
                    public static string GetString(byte[] arr)
                    {
                        return Encoding.UTF8.GetString(arr);
                    }

                    public static byte[] GetBytes(string str)
                    {
                        return Encoding.UTF8.GetBytes(str);
                    }
                }

                public static class Base64
                {
                    public static string GetString(byte[] arr)
                    {
                        return Convert.ToBase64String(arr);
                    }

                    public static byte[] GetBytes(string str)
                    {
                        str = str.Replace('*', '/');
                        str = str.Replace('!', '+');

                        byte[] arr = Convert.FromBase64String(str);
                        int finaldim = 60;
                        /*
                        byte[] arr = Encoding.ASCII.GetBytes(str);
                        List<byte> list = new List<byte>();
                        for (int i = 0; i < arr.Length; i++)
                        {
                            byte tmp = (byte)(arr[i] - (byte)65);
                            if (tmp < 64)
                                list.Add(tmp);
                            else
                            {
                                while (tmp >= 64)
                                {
                                    list.Add(64);
                                    tmp -= 64;
                                }
                            }

                        }


                        int length = str.Length;
                        for (int i = 0; i < str.Length; i++)
                        {

                        }
                        */
                        return arr.Length == finaldim ? arr : null;
                    }
                }

                public static class Other
                {
                    public static string GetEachByteString(byte[] arr)
                    {
                        string ret = "";
                        foreach (byte b in arr)
                        {
                            ret += ((int)b).ToString() + _subsep;
                        }
                        return ret;
                    }

                    public static byte[] GetEachByte(string str)
                    {
                        string[] arr = str.Split(SubSeparatore[0]);
                        byte[] ret = new byte[arr.Length];
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (arr[i] != null)
                                ret[i] = Convert.ToByte(arr[i]);
                        }
                        return ret;
                    }
                }
            }

            #region Methods
            protected void GetLength()
            {
                if (this.Caratteristiche.Testo != null)
                    this._length = Caratteristiche.Testo.Length;
            }

            public override string ToString()
            {
                return this.Caratteristiche.Testo;
            }

            public virtual void Dispose()
            {
                this._caratt = null;
                this._length = 0;
            }
            #endregion
        }

        public class FrameInIngresso : Frame
        {
            public class CaratteristicheFrameInIngresso : CaratteristicheFrame
            {
                private byte[] _flussobytes = new byte[] { };

                public byte[] FlussoBytes { get { return this._flussobytes; } set { this._flussobytes = value; } }
                public new bool Accettabile
                {
                    get
                    {
                        return base.Tipo != TipoFrame.undefined && this.FlussoBytes.Length > 0;
                    }
                }
            }

            protected new CaratteristicheFrameInIngresso _caratt = new CaratteristicheFrameInIngresso();
            public new CaratteristicheFrameInIngresso Caratteristiche
            {
                get { return this._caratt; }
                set
                {
                    this._caratt = value;
                }
            }

            #region Costruttori
            #region ByteArray Param
            public FrameInIngresso(byte[] bytes)
            {
                if (Utilities.IsValidByteFlush(bytes))
                {
                    this.Caratteristiche.FlussoBytes = bytes;

                    try
                    {
                        BuildObjectFromBytes();
                    }
                    catch (NullReferenceException) { this.Dispose(); }
                    GetLength();
                }
            }

            protected void BuildObjectFromBytes()
            {
                base.Caratteristiche.Testo = NAndCoder.UTF8.GetString(Caratteristiche.FlussoBytes);

                if (Utilities.IsValidText(base.Caratteristiche.Testo))
                {
                    try
                    {
                        string[] current = GetSplitted(base.Caratteristiche.Testo);
                        base.Caratteristiche.Tipo = EnumExt.GetTipoFrameFromString(current[0]);
                    }
                    catch (NullReferenceException) { this.Dispose(); }

                    this.Caratteristiche.Tipo = base.Caratteristiche.Tipo;
                    this.Caratteristiche.Testo = base.Caratteristiche.Testo;
                }
            }
            #endregion

            #region CaratteristicheFrameInIngresso Param
            public FrameInIngresso(CaratteristicheFrameInIngresso caratt)
            {
                if (caratt != null)
                {
                    this._caratt = caratt;

                    BuildObjectFromCaratteristiche();
                    GetLength();
                }
            }

            protected void BuildObjectFromCaratteristiche()
            {
                base.Caratteristiche.Tipo = this.Caratteristiche.Tipo;
                base.Caratteristiche.Testo = this.Caratteristiche.Testo;
            }
            #endregion

            #region Object Param
            public FrameInIngresso(object inframe)
            {
                FrameInIngresso frame = (FrameInIngresso)inframe;
                this._caratt = frame.Caratteristiche;
                base.Caratteristiche.Testo = frame.Caratteristiche.Testo;

                GetLength();
            }
            #endregion
            #endregion

            #region Methods
            public static string[] GetSplitted(string testo)
            {
                string[] tmp = testo.Split(InitFine[0]);
                tmp = Utilities.Normalize(tmp);
                string[] ret = tmp[0].Split(Separatore[0]);
                ret = Utilities.Normalize(ret);
                return ret;
            }

            #region Type
            public Type GetFrameType()
            {
                try
                {
                    return GetSpecificFrame().GetType();
                }
                catch (NullReferenceException) { return null; }
            }

            public object GetSpecificFrame()
            {
                if (base.Caratteristiche.Tipo == TipoFrame.temp)
                    return new FrameTemperatura(this.Caratteristiche);/*
                else if (base.Caratteristiche.Tipo == TipoFrame.cam)
                    return new FrameVideocamera(this.Caratteristiche);*/
                return null;
            }
            #endregion

            #region Explicit Operators
            public static explicit operator TemperaturaEventArgs(FrameInIngresso frame)
            {
                if (frame.Caratteristiche.Tipo == TipoFrame.temp)
                    return new TemperaturaEventArgs(frame);
                return null;
            }
            /*
            public static explicit operator VideocameraEventArgs(FrameInIngresso frame)
            {
                if (frame.Caratteristiche.Tipo == TipoFrame.cam)
                    return new VideocameraEventArgs(frame);
                return null;
            }*/
            #endregion

            public override void Dispose()
            {
                this.Caratteristiche.FlussoBytes = null;
                try
                {
                    this.Caratteristiche.Testo = "";
                    this.Caratteristiche.Tipo = TipoFrame.undefined;
                    base.Dispose();
                }
                catch (NullReferenceException) { }
            }
            #endregion
        }

        public class FrameInUscita : Frame
        {
            public class CaratteristicheFrameInUscita : CaratteristicheFrame
            { }

            protected new CaratteristicheFrameInUscita _caratt = new CaratteristicheFrameInUscita();
            public new CaratteristicheFrameInUscita Caratteristiche { get { return this._caratt; } }

            public FrameInUscita() { Build(); }

            protected void Build()
            {
                base.Caratteristiche.Tipo = this.Caratteristiche.Tipo;
                base.Caratteristiche.Testo = this.Caratteristiche.Testo;
            }
        }
    }

    public class BaseEventArgs : EventArgs, IDisposable
    {
        public virtual void Dispose() { }
    }
}