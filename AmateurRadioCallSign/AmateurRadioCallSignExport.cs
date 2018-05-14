using System;
using System.Collections.Generic;

namespace AmateurRadioCallSign
{
    public class AmateurRadioCallSignExport
    {
        public Dictionary<string, AmateurRadioCallSign> CallSigns { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}