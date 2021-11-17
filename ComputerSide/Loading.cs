using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace ComputerSide
{
    public partial class Loading : Form
    {
        public Loading()
        {
            InitializeComponent();
            this.BackgroundImage = new Bitmap(@"Images\Loading\Loading.png");
        }

        private void Loading_Shown(object sender, EventArgs e)
        {
            this.Opacity = 1;
            Thread.Sleep(2000);
            for (double i = 1; i >= 0; i -= 0.01)
            {
                Thread.Sleep(5);
                this.Opacity = i;
            }
            this.Close();

            /*
            image = new Bitmap(pbFunction.Width, pbFunction.Height);
            Thread th = new Thread(new ThreadStart(Executor));
            th.Start();*/
        }

        #region Sinusoidale
        /*
         
        static Bitmap image; 
         
        private void Executor()
        {
            for (int x = 0; x < image.Width; x++)
            {
                Thread.Sleep(10);
                int y = f(x);
                int y1 = meno_f(x);

                try
                {
                    // if(y1<image.Height) image.SetPixel(x, y1, Function.DrawDefaultColor);
                    for (int i = 0; i < 3; i++)
                    {
                        if (y + i < image.Height)
                            image.SetPixel(x, y + i, Function.DrawDefaultColor);
                        if (y - i > image.Height)
                            image.SetPixel(x, y - i, Function.DrawDefaultColor);
                    }
                }
                catch (InvalidOperationException)
                {
                    Thread.Sleep(5);                    
                    //if (y1 < image.Height) image.SetPixel(x, y1, Function.DrawDefaultColor);
                    for (int i = 0; i < 3; i++)
                    {
                        if (y + i < image.Height)
                            image.SetPixel(x, y + i, Function.DrawDefaultColor);
                        if (y - i > image.Height)
                            image.SetPixel(x, y - i, Function.DrawDefaultColor);
                    }
                }
                pbFunction.Image = image;
            }
            

            Thread.Sleep(100);
            for (double i = 1; i >= 0; i -= 0.01)
            {
                Thread.Sleep(5);
                this.Invoke(new MethodInvoker(delegate { this.Opacity = i; }));
            }

            this.Invoke(new MethodInvoker(delegate { this.Close(); }));
        }
        
        private struct Function
        {
            private static int imgW = image.Width, imgH = image.Height;
            public static int WaveNumber = 10, VMax = imgH - 1, Trans = imgH / 2;
            public static int WaveLength = imgW / WaveNumber;
            public static int functMax = WaveLength / 4;

            public static Color DrawDefaultColor = Color.Blue;
        }
        
        private int meno_f(int x)
        {
            double freq = Math.PI / (4 * Math.PI * Function.functMax);
            double funct = ((Function.VMax - Function.Trans) * Math.Sin(2 * Math.PI * freq * x)) + Function.Trans;
            return (int)Math.Round(funct);
        }

        private int f(int x)
        {
            return Function.VMax - meno_f(x);
        }
        */

        #endregion

        private void Loading_FormClosed(object sender, FormClosedEventArgs e)
        {
            Thread t = new Thread(new ThreadStart(ThreadProc));
            t.Start();
        }

        public static void ThreadProc()
        {
            Application.Run(new NUI());
        }
    }
}
