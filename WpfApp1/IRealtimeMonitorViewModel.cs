﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using OxyPlot;

namespace WpfApp1
{
    public interface IRealtimeMonitorViewModel
    {
        TimeSpan SamplingInterval { get; set; }
        bool SmoothScroll { get; set; }
        TimeSpan VisibleDuration { get; set; }
        IReadOnlyList<TimeSpan> PossibleVisibleDurations { get; }
        IReadOnlyList<TimeSpan> PossibleSamplingIntervals { get; }
        bool IsEnabled { get; }
        PlotModel Model { get; }
        PlotController Controller { get; }
        IReadOnlyList<RealtimeSeriesViewModel> Series { get; }
        event PropertyChangedEventHandler? PropertyChanged;
    }
}