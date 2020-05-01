using System;

namespace BlazorClockSVG.Shared
{
    public partial class ClockModel
    {
        public class ThresholdReachedEventArgs : EventArgs
        {
            public int Threshold { get; set; }
            public DateTime TimeReached { get; set; }
            public string Name {get;set;}
        }
    }
}