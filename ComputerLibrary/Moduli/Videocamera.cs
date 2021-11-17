using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace ComputerLibrary.Moduli
{
    namespace Videocamera
    {
        using System.Linq;

        public delegate void LastFrameReceived(EventArgs e);
        public delegate void ImageReadyEventHandler(ImageReadyEventArgs e);

        public struct Risoluzione
        {
            private int _width, _height;

            public Risoluzione(int width, int height)
            {
                _width = width; _height = height;
            }

            public int Width { get { return _width; } }
            public int Height { get { return _height; } }
            //640x480   320x240   160x120
            public static Risoluzione High
            {
                get { return new Risoluzione(640, 480); }
            }

            public static Risoluzione Mid
            {
                get { return new Risoluzione(320, 240); }
            }

            public static Risoluzione Low
            {
                get { return new Risoluzione(160, 120); }
            }
        }

        public static class ModuloVideocamera
        {
            private static Bitmap screen;
            private static Risoluzione res;
            private static List<FrameVideocamera> _vcbuf = new List<FrameVideocamera>();
            private static List<byte> _buf = new List<byte>();

            public static event ImageReadyEventHandler ImageReadyEvent;

            private static string defaultimage = "_pred_";
            public static string DefaultImage { get { return defaultimage; } }
            public static Bitmap CurrentImage { get { return screen; } }
            public static Risoluzione Risoluzione { get { return res; } }
            public static List<byte> Buffer { get { return _buf; } }
            public static List<FrameVideocamera> FrameBuffer { get { return _vcbuf; } }

            public static void AddFrame(FrameVideocamera frame)
            {
                _vcbuf.Add(frame);

                if (frame.Caratteristiche.Numero == -1)
                    BeginDrawing();
            }

            private static void BeginDrawing()
            {
                res = Risoluzione.Low;
                screen = new Bitmap(res.Width, res.Height);
                Thread th = new Thread(new ThreadStart(Drawer));
                th.Start();
            }

            private static void Drawer()
            {
                _vcbuf = _vcbuf.OrderBy(elem => elem.Caratteristiche.Numero).ToList();
                _vcbuf.Add(_vcbuf[0]);
                _vcbuf.RemoveAt(0);

                for (int i = 0; i < _vcbuf.Count; i++)
                {
                    if (_vcbuf[i].Caratteristiche.Numero == i)
                        _buf.AddRange(_vcbuf[i].Caratteristiche.StreamVideocamera);
                }

                string tmp = Frames.Frame.NAndCoder.Base64.GetString(_buf.ToArray());

                bool accept = !(tmp == Frames.TipoFrame.undefined.ToString()
                    || string.IsNullOrEmpty(tmp));
                bool ex = false;
                string stream = accept ? tmp : defaultimage;

                while (ImageReadyEvent == null) ;
                if (stream == defaultimage)
                {
                    screen.DrawDefault();
                }
                else
                {
                    //Adafruit CAM Image
                    try
                    {
                        screen.DrawFromBase64(stream);
                    }
                    catch (ArgumentException exc) { ex = true; Console.WriteLine(exc.Message); }
                    catch (FormatException exc) { ex = true; Console.WriteLine(exc.Message); }
                }

                if (ex)
                    screen.DrawDefault();

                //screen.RotateFlip(RotateFlipType.Rotate180FlipX);
                while (ImageReadyEvent == null) ;
                if (ImageReadyEvent != null)
                    ImageReadyEvent(new ImageReadyEventArgs(screen));

                _buf.Clear();
            }

            private static void DrawDefault(this Bitmap desk)
            {
                desk = DrawDefault();
            }

            private static void DrawFromBase64(this Bitmap desk, string base64)
            {
                desk = DrawFromBase64(base64);
            }

            private static void DrawFromByteArray(this Bitmap desk, byte[] _base)
            {
                desk = DrawFromByteArray(_base);
            }

            public static Bitmap DrawDefault()
            {
                Bitmap desk = new Bitmap(screen.Width, screen.Height);
                bool isinuse = false;
                //dall'alto a sinistra verso destra e a scendere
                for (int y = 0; y < res.Height; y++)
                {
                    if (!isinuse)
                    {
                        for (int x = 0; x < res.Width; x++)
                        {
                            if (!isinuse)
                            {
                                try
                                {
                                    //Red      Green     Blue
                                    desk.SetPixel(x, y, Color.FromArgb(y % 255, x % 255, y % 255));
                                }
                                catch (InvalidOperationException)
                                { isinuse = true; }
                            }
                            else break;
                        }
                    }
                    else break;
                }
                return desk;
            }

            public static Bitmap DrawFromBase64(string base64)
            {
                byte[] arr = Convert.FromBase64String(base64);
                return DrawFromByteArray(arr);
            }

            public static Bitmap DrawFromByteArray(byte[] _base)
            {
                MemoryStream ms = new MemoryStream(_base);

                try
                {
                    Image image = Image.FromStream(ms);
                    return (Bitmap)image;
                }
                catch (ArgumentException ex) { throw ex; }
            }
        }

        public class ImageReadyEventArgs : EventArgs
        {
            private Bitmap _image;
            public Bitmap Image { get { return _image; } }

            public ImageReadyEventArgs(Bitmap image)
            { _image = image; }
        }

        public class FrameVideocamera : Frames.FrameInIngresso
        {
            public class CaratteristicheFrameVideocamera : CaratteristicheFrameInIngresso
            {
                private int numero = 0;
                private byte[] streamvideo = new byte[] { };

                public int Numero { get { return this.numero; } set { this.numero = value; } }
                public byte[] StreamVideocamera { get { return this.streamvideo; } set { this.streamvideo = value; } }
                public string StreamVideocameraBase64 { get { return NAndCoder.Base64.GetString(this.streamvideo); } }
                public string StreamVideocameraUTF8 { get { return NAndCoder.UTF8.GetString(this.streamvideo); } }

                public new bool Accettabile
                {
                    get
                    {
                        return base.Tipo != Frames.TipoFrame.undefined && this.StreamVideocamera.Length > 0;
                    }
                }
            }
            protected new CaratteristicheFrameVideocamera _caratt = new CaratteristicheFrameVideocamera();
            public new CaratteristicheFrameVideocamera Caratteristiche { get { return this._caratt; } }

            public FrameVideocamera(Frames.FrameInIngresso inframe)
                : base(inframe)
            {
                Assign(inframe.Caratteristiche);
            }

            public FrameVideocamera(CaratteristicheFrameInIngresso caratt)
                : base(caratt)
            { Assign(caratt); }

            private void Assign(CaratteristicheFrameInIngresso caratt)
            {
                string[] current = GetSplitted(caratt.Testo);
                this.Caratteristiche.Numero = Convert.ToInt32(current[1]);
                string content = GetSplitted(current[2]).Last();

                this.Caratteristiche.StreamVideocamera = NAndCoder.Base64.GetBytes(content);
                //NAndCoder.Other.GetEachByte(content);

                GetLength();
            }
        }

        public class VideocameraEventArgs : BaseEventArgs
        {
            private FrameVideocamera _frame;
            public FrameVideocamera Frame { get { return this._frame; } }

            public VideocameraEventArgs(FrameVideocamera frame)
            {
                this._frame = frame;
            }

            public VideocameraEventArgs(Frames.FrameInIngresso frame)
            {
                this._frame = new FrameVideocamera(frame);
            }

            public override void Dispose()
            {
                this._frame = null;
                base.Dispose();
            }
        }
    }
}
