using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QOI.Core.Chunk
{
    internal class LumaWriter
    {
        /*
        2-bit tag b10
        6-bit green channel difference from the previous pixel -32..31
        4-bit red channel difference minus green channel difference -8..7
        4-bit blue channel difference minus green channel difference -8..7

        The difference to the current channel values are using a wraparound
        operation, so 10 - 13 will result in 253, while 250 + 7 will result 
        in 1
        */

        public bool CanHandlePixel(QoiColorDiff diff)
        {

            var (lGdiff, lRdiff, lBdiff) = GetLumaDiff(diff);
            return -32 <= lGdiff && lGdiff <= 31
                && -8 <= lRdiff && lRdiff <= 7
                && -8 <= lBdiff && lBdiff <= 7;
        }

        internal void WriteChunk(QoiColorDiff diff, Stream stream)
        {
            var (lGdiff, lRdiff, lBdiff) = GetLumaDiff(diff);
            Span<byte> buffer = stackalloc byte[2];
            buffer[0] = Tag.LUMA.Apply((byte)(lGdiff + 32));
            buffer[1] = (byte)((lRdiff + 8) << 4 | (lBdiff + 8));
            stream.Write(buffer);
        }

        private (int lGDiff, int lRdiff, int lBdiff) GetLumaDiff(QoiColorDiff diff)
        {
            var lGdiff = diff.Gdiff;
            var lRdiff = diff.Rdiff - diff.Gdiff;
            var lBdiff = diff.Bdiff - diff.Gdiff;
            return (lGdiff, lRdiff, lBdiff);
        }
    }
}
