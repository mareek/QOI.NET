﻿using System;
using System.Collections.Generic;
using System.Text;

namespace QOI.Core.Interface
{
    public interface IImageWriter
    {
        void Init(uint width, uint height, bool hasAlpha, bool isSrgb);
        void WritePixel(byte r, byte g, byte b, byte a);

        bool IsComplete { get; }
    }
}
