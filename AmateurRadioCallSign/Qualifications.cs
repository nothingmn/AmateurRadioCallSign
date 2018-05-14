using System;

namespace AmateurRadioCallSign
{
    [Flags]
    public enum Qualifications
    {
        Basic = 1,
        Morse5Wpm = 2,
        Morse12Wpm = 4,
        Advanced = 8,
        BasicWithHonours = 16,
    }
}