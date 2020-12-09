using System;

namespace SilverlightEnumPrinters
{
    [Flags]
    public enum PrinterStatus
    {
        Busy = 0x200,
        DoorOpen = 0x400000,
        Error = 0x2,
        Initializing = 0x8000,
        IOActive = 0x100,
        ManualFeed = 0x20,
        NoToner = 0x40000,
        NotAvailable = 0x1000,
        Offline = 0x80,
        OutOfMemory = 0x200000,
        OutputBinFull = 0x800,
        PagePunt = 0x80000,
        PaperJam = 0x8,
        PaperOut = 0x10,
        PaperProblem = 0x40,
        Paused = 0x1,
        PendingDeletion = 0x4,
        Printing = 0x400,
        Processing = 0x4000,
        TonerLow = 0x20000,
        UserIntervention = 0x100000,
        Waiting = 0x2000,
        WarmingUp = 0x10000
    }
}
