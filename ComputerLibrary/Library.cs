using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;

namespace ComputerLibrary
{
    public struct VirtualKeyCodes
    {
        public const int LEFT = 0x25;
        public const int UP = 0x26;
        public const int RIGHT = 0x27;
        public const int DOWN = 0x28;
        public const int RETURN = 0x0D;
    }

    #region Enumerazioni
    public enum ArrowKey
    {
        Left = VirtualKeyCodes.LEFT, Up = VirtualKeyCodes.UP,
        Right = VirtualKeyCodes.RIGHT, Down = VirtualKeyCodes.DOWN
    }

    public enum Stato
    {
        Rilasciato, Premuto
    }
    #endregion

    public struct ArrowImages
    {
        public static Bitmap[] GetAlternativesFor(ArrowKey index)
        {
            List<Bitmap> ret = new List<Bitmap>();

            //default ritorna una freccia verso il basso
            switch (index)
            {
                case ArrowKey.Left:
                    ret.Add(new Bitmap(ArrowImages.Released.Left));
                    ret.Add(new Bitmap(ArrowImages.Pressed.Left));
                    break;
                case ArrowKey.Up:
                    ret.Add(new Bitmap(ArrowImages.Released.Up));
                    ret.Add(new Bitmap(ArrowImages.Pressed.Up));
                    break;
                case ArrowKey.Right:
                    ret.Add(new Bitmap(ArrowImages.Released.Right));
                    ret.Add(new Bitmap(ArrowImages.Pressed.Right));
                    break;
                case ArrowKey.Down:
                    ret.Add(new Bitmap(ArrowImages.Released.Down));
                    ret.Add(new Bitmap(ArrowImages.Pressed.Down));
                    break;
            }
            return ret.ToArray();
        }

        #region Paths
        public struct Up
        {
            public static string Pressed { get { return @"Images\Arrow_Keys\!U.png"; } }
            public static string Released { get { return @"Images\Arrow_Keys\U.png"; } }
            public static List<string> Alternatives { get { return new List<string>() { Released, Pressed }; } }
        }

        public struct Down
        {
            public static string Pressed { get { return @"Images\Arrow_Keys\!D.png"; } }
            public static string Released { get { return @"Images\Arrow_Keys\D.png"; } }
            public static List<string> Alternatives { get { return new List<string>() { Released, Pressed }; } }
        }

        public struct Left
        {
            public static string Pressed { get { return @"Images\Arrow_Keys\!L.png"; } }
            public static string Released { get { return @"Images\Arrow_Keys\L.png"; } }
            public static List<string> Alternatives { get { return new List<string>() { Released, Pressed }; } }
        }

        public struct Right
        {
            public static string Pressed { get { return @"Images\Arrow_Keys\!R.png"; } }
            public static string Released { get { return @"Images\Arrow_Keys\R.png"; } }
            public static List<string> Alternatives { get { return new List<string>() { Released, Pressed }; } }
        }

        public struct Pressed
        {
            public static string Up { get { return @"Images\Arrow_Keys\!U.png"; } }
            public static string Down { get { return @"Images\Arrow_Keys\!D.png"; } }
            public static string Left { get { return @"Images\Arrow_Keys\!L.png"; } }
            public static string Right { get { return @"Images\Arrow_Keys\!R.png"; } }
        }

        public struct Released
        {
            public static string Up { get { return @"Images\Arrow_Keys\U.png"; } }
            public static string Down { get { return @"Images\Arrow_Keys\D.png"; } }
            public static string Left { get { return @"Images\Arrow_Keys\L.png"; } }
            public static string Right { get { return @"Images\Arrow_Keys\R.png"; } }
        }
        #endregion
    }

    public struct FormImages
    {
        public static Bitmap Background { get { return new Bitmap(@"Images\Form\BackgroundImage.png"); } }
        public static Bitmap Ruota { get { return new Bitmap(@"Images\Form\Ruota.png"); } }
        public struct Robot
        {
            public static Bitmap ConPinzaAperta { get { return new Bitmap(@"Images\Form\NUI_PA.png"); } }
            public static Bitmap ConPinzaChiusa { get { return new Bitmap(@"Images\Form\NUI_PC.png"); } }
        }
    }

    public struct Icons
    {
        public struct Pinza
        {
            public struct Aperta
            {
                public static Bitmap Premuto { get { return new Bitmap(@"Images\Icons\Pinza\Apri.png"); } }
                public static Bitmap Rilasciato { get { return new Bitmap(@"Images\Icons\Pinza\!Apri.png"); } }
            }

            public struct Chiusa
            {
                public static Bitmap Premuto { get { return new Bitmap(@"Images\Icons\Pinza\Chiudi.png"); } }
                public static Bitmap Rilasciato { get { return new Bitmap(@"Images\Icons\Pinza\!Chiudi.png"); } }
            }
        }

        public struct Elevatore
        {
            public struct Alzato
            {
                public static Bitmap Premuto { get { return new Bitmap(@"Images\Icons\Elevatore\Alza.png"); } }
                public static Bitmap Rilasciato { get { return new Bitmap(@"Images\Icons\Elevatore\!Alza.png"); } }
            }

            public struct Abbassato
            {
                public static Bitmap Premuto { get { return new Bitmap(@"Images\Icons\Elevatore\Abbassa.png"); } }
                public static Bitmap Rilasciato { get { return new Bitmap(@"Images\Icons\Elevatore\!Abbassa.png"); } }
            }
        }

        public struct Buzzer
        {
            public static Bitmap Suonato { get { return new Bitmap(@"Images\Icons\Buzzer\Buzzer.png"); } }
            public static Bitmap Normale { get { return new Bitmap(@"Images\Icons\Buzzer\!Buzzer.png"); } }
        }

        public struct Ferma
        {
            public static Bitmap Fermato { get { return new Bitmap(@"Images\Icons\Ferma\Ferma.png"); } }
            public static Bitmap Normale { get { return new Bitmap(@"Images\Icons\Ferma\!Ferma.png"); } }
        }
    }

    namespace ArrowHandle
    {
        #region Delegati
        public delegate void ArrowEventHandler(object sender, ArrowEventArgs e);
        public delegate void ExecuteEventHandler(object sender, EventArgs e);
        #endregion

        public struct EventCode
        {
            public const int WM_KEYDOWN = 0x0100,
                WM_KEYUP = 0x0101;
        }

        public class ArrowEventArgs : EventArgs
        {
            public int Code { get; set; }
            public ArrowKey Key { get; set; }
            public Stato StatoTasto { get; set; }
            public int Elapsed { get; set; }

            public ArrowEventArgs(int code, Stato handled)
            {
                Code = code;
                Key = (ArrowKey)code;
                StatoTasto = handled;
            }

            public ArrowEventArgs(int code, Stato handled, int elapsedtime)
            {
                Code = code;
                Key = (ArrowKey)code;
                StatoTasto = handled;
                Elapsed = elapsedtime;
            }
        }

        public class ArrowHandler
        {
            private static PersPB[] pictureboxes;
            public ExecuteEventHandler ArrowHandlerEvent;

            public ArrowHandler(ref PictureBox[] pbs)
            {
                pictureboxes = PersPB.GetArray(pbs);

                for (int i = 0; i < pictureboxes.Length; i++)
                {
                    pictureboxes[i].Released();
                }
            }

            public void Handler(object sender, ArrowEventArgs e)
            {
                string kind = e.Key.ToString();

                PersPB oldstate = pictureboxes.GetElementByString(kind);

                PersPB[] oth = pictureboxes.Except(pictureboxes.Where(elem => elem.BasePB.Name == "pb" + kind)).ToArray();
                bool othok = true;

                foreach (PersPB p in oth)
                {
                    if (p.StatoTasto == Stato.Premuto)
                        othok = false;
                }
                PersPB clicked = (PersPB)(pictureboxes.Where(elem => elem.BasePB.Name == "pb" + kind).ToArray()[0]);

                if (e.StatoTasto != oldstate.StatoTasto)
                {
                    int pressed = clicked.ElapsedTime;
                    if (e.StatoTasto == Stato.Premuto && othok)
                    {
                        clicked.Pressed();
                        Moduli.Movimento.Direzioni m = (Moduli.Movimento.Direzioni)Enum.ToObject(typeof(Moduli.Movimento.Direzioni), e.Code);
                        if (ArrowHandlerEvent != null)
                            ArrowHandlerEvent(this, new Moduli.Movimento.MovimentoEventArgs(m));
                    }
                    else
                    {
                        clicked.Released();
                        if (ArrowHandlerEvent != null)
                            ArrowHandlerEvent(this, new Moduli.Movimento.MovimentoEventArgs(Moduli.Movimento.Direzioni.Fermo));
                    }
                }
            }

            public static bool IsAnArrow(int keycode)
            {
                if (keycode == VirtualKeyCodes.LEFT || keycode == VirtualKeyCodes.UP ||
                   keycode == VirtualKeyCodes.RIGHT || keycode == VirtualKeyCodes.DOWN)
                    return true;
                return false;
            }

            public static bool IsAnArrow(IntPtr keycode)
            {
                int code = (int)keycode;
                if (code == VirtualKeyCodes.LEFT || code == VirtualKeyCodes.UP ||
                   code == VirtualKeyCodes.RIGHT || code == VirtualKeyCodes.DOWN)
                    return true;
                return false;
            }
        }
    }

    public static class Extensions
    {
        public static PersPB GetElementByString(this PersPB[] arr, string index)
        {
            foreach (PersPB elem in arr)
            {
                if (elem.BasePB.Name.Contains(index))
                    return elem;
            }
            return null;
        }

        public static int IndexOf(this PersPB[] arr, PersPB element)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == element)
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this ArrowKey[] arr, string element)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].ToString() == element)
                    return i;
            }
            return -1;
        }
    }

    public class PersPB
    {
        private PictureBox basepb;
        private Image currentimage;
        private int currentimageindex;
        private Image[] imagealternatives;
        private Stato statotasto;
        private ArrowKey key;
        private int keynum;
        private static int elapsed;
        private static bool executetimeout;
        private static Thread timer;

        public Image CurrentImage { get { return currentimage; } }
        public int CurrentImageIndex { get { return currentimageindex; } }
        public Image[] ImageAlternatives { get { return imagealternatives; } }
        public Stato StatoTasto { get { return statotasto; } }
        public PictureBox BasePB { get { return basepb; } }
        public ArrowKey Key { get { return key; } }
        public int KeyNumber { get { return keynum; } }
        public int ElapsedTime { get { return elapsed; } }
        public bool ExecuteTimeout { get { return executetimeout; } }

        public PersPB(ref PictureBox pb)
        {
            basepb = pb;
            string kind = pb.Name.Remove(0, 2);
            keynum = ((ArrowKey[])Enum.GetValues(typeof(ArrowKey))).IndexOf(kind) + 37;
            key = (ArrowKey)Enum.ToObject(typeof(ArrowKey), keynum);
            elapsed = 0;
            executetimeout = false;
            timer = new Thread(new ThreadStart(Timeout));

            imagealternatives = ArrowImages.GetAlternativesFor(key);

            currentimage = imagealternatives[0];
            currentimageindex = 0;
            statotasto = Stato.Rilasciato;
        }

        public static explicit operator PersPB(PictureBox pb)
        {
            return new PersPB(ref pb);
        }

        public static PersPB[] GetArray(PictureBox[] pb)
        {
            List<PersPB> list = new List<PersPB>();
            foreach (PictureBox curr in pb)
            {
                list.Add((PersPB)curr);
            }
            return list.ToArray();
        }

        public void SwitchImage()
        {
            currentimageindex = currentimageindex == 0 ? 1 : 0;
            basepb.Image = imagealternatives.Where(elem => elem != currentimage).ToArray()[0];
            currentimage = basepb.Image;

            statotasto = (Stato)Enum.ToObject(typeof(Stato), CurrentImageIndex);
        }

        public void Pressed()
        {
            currentimageindex = 1;
            basepb.Image = imagealternatives[currentimageindex];
            currentimage = imagealternatives[currentimageindex];

            statotasto = Stato.Premuto;

            executetimeout = true;
            elapsed = 0;
            timer = new Thread(new ThreadStart(Timeout));
            timer.Start();
        }

        public void Released()
        {
            currentimageindex = 0;
            basepb.Image = imagealternatives[currentimageindex];
            currentimage = imagealternatives[currentimageindex];

            statotasto = Stato.Rilasciato;
            executetimeout = false;
        }

        private static void Timeout()
        {
            while (executetimeout)
            {
                elapsed++;
                Thread.Sleep(1);
            }
        }
    }

    public delegate void ImageRefreshEventHandler(object sender, ImageRefreshEventArgs e);

    public class PBRuote
    {
        public event ImageRefreshEventHandler ImageRefresh;

        private PictureBox basepb;
        private bool doin = false;
        private const int roundslip = 10, degreeinc = 5;

        public PictureBox BasePB { get { return basepb; } }

        public PBRuote(ref PictureBox pb)
        {
            basepb = pb;
        }

        public static explicit operator PBRuote(PictureBox pb)
        {
            return new PBRuote(ref pb);
        }

        #region DrawThread-Callers
        public void Avanti()
        {
            Thread th = new Thread(new ThreadStart(_Avanti));
            th.Start();
        }

        public void Indietro()
        {
            Thread th = new Thread(new ThreadStart(_Indietro));
            th.Start();
        }
        #endregion

        #region Executors
        private void _Avanti()
        {
            doin = true;
            int degrees = 0;
            while (doin)
            {
                Thread.Sleep(roundslip);
                if (ImageRefresh != null)
                    ImageRefresh(this, new ImageRefreshEventArgs(RotateImage(FormImages.Ruota, degrees)));
                degrees += degreeinc;
            }
        }

        private void _Indietro()
        {
            doin = true;
            int degrees = 0;
            while (doin)
            {
                Thread.Sleep(roundslip);
                if (ImageRefresh != null)
                    ImageRefresh(this, new ImageRefreshEventArgs(RotateImage(FormImages.Ruota, degrees)));
                degrees -= degreeinc;
            }
        }
        #endregion

        public void Ferma()
        {
            doin = false;
        }

        public Bitmap RotateImage(Image image, int angle)
        {
            Bitmap rotatedBmp = null;
            lock (image)
            {
                const int trans = 50;
                bool ex = false;

                do
                {
                    try
                    {
                        rotatedBmp = new Bitmap(image.Width, image.Height);
                        rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                        ex = false;
                    }
                    catch (InvalidOperationException)
                    {
                        ex = true;
                    }
                }
                while (ex);

                if (rotatedBmp != null)
                {
                    Graphics g = Graphics.FromImage(rotatedBmp);
                    g.TranslateTransform(trans, trans);
                    g.RotateTransform(angle);
                    g.TranslateTransform(-trans, -trans);
                    g.DrawImage(image, 0, 0);
                    g.Dispose();
                }
            }

            return rotatedBmp;
        }
    }

    public class ImageRefreshEventArgs : EventArgs
    {
        Bitmap _image;

        public ImageRefreshEventArgs(Bitmap image)
        {
            _image = image;
        }

        public Bitmap Image { get { return _image; } }
    }

    public class PersTreeNode
    {
        private TreeNode _base;
        public TreeNode BaseNode { get { return _base; } }

        public PersTreeNode(Moduli.Movimento.Direzioni mov)
        {
            _base = new TreeNode("Direzione: " + mov.ToString());
        }

        public PersTreeNode(Moduli.Carrello.AzioniCarrello act)
        {
            _base = new TreeNode("Azione: " + act.ToString());
        }

        public PersTreeNode(Moduli.Buzzer.AzioniBuzzer buzz)
        {
            _base = new TreeNode("Buzzer: " + buzz.ToString());
        }

        public static implicit operator TreeNode(PersTreeNode node)
        {
            return node._base;
        }
    }

    public class LogRefreshEventArgs : EventArgs
    {
        TreeNode _node;

        public LogRefreshEventArgs(TreeNode node)
        {
            _node = node;
        }

        public TreeNode Node { get { return _node; } }
    }
}
