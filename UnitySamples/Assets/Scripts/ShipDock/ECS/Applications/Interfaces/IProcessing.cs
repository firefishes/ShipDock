﻿using ShipDock.Interfaces;
using ShipDock.Pooling;
using System;

namespace ShipDock.Applications
{
    public interface IProcessing : IPoolable, IReclaim
    {
        void OnProcessing();
        void ProcessingReady();
        bool Finished { get; set; }
        int Type { get; }
        Action AfterProcessing { get; set; }
    }
}