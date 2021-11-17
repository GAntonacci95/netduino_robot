using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ComputerLibrary.Moduli
{
    namespace Temperatura
    {
        public static class Modulo
        {
            public const int RESISTORE = 10000, ADCBITS = 10, PRECISION = 1024;
            public const double FIRST_CONSTANT = 0.001129148,
                SECOND_CONSTANT = 0.000234125,
                THIRD_CONSTANT = 0.0000000876741, KELVIN_CONSTANT = 273.15;

            private static double volt, q;

            public static void Listen()
            {
                //receive frame: "|voltage|livelloacquisito|"
            }

            public static double GetTemperature(this int bites)
            {
                double tmp = System.Math.Log(((PRECISION * RESISTORE / bites) - RESISTORE));
                double ret = (1 / (FIRST_CONSTANT + (SECOND_CONSTANT + (THIRD_CONSTANT * tmp * tmp)) * tmp)) - KELVIN_CONSTANT;
                return ret;
            }
        }

        public class FrameModulo : Frame
        {

        }
    }
}
