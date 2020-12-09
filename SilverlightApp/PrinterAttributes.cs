using System;

namespace SilverlightEnumPrinters
{
    [Flags]
    public enum PrinterAttributes
    {
        Queued = 0x1,
        Direct = 0x2,
        Default = 0x4,
        Shared = 0x8,
        Network = 0x10,
        Hidden = 0x20,
        Local = 0x40,
        WorkOffline = 0x400,
        EnableBidi = 0x800
    }
}
