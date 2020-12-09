using System;
using System.Runtime.InteropServices;

namespace SilverlightEnumPrinters
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class PRINTER_INFO_2
    {
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pServerName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pPrinterName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pShareName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pPortName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pDriverName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pComment;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pLocation;
        public IntPtr pDevMode;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pSepFile;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pPrintProcessor;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pDatatype;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pParameters;
        public IntPtr pSecurityDescriptor;
        public uint Attributes;
        public uint Priority;
        public uint DefaultPriority;
        public uint StartTime;
        public uint UntilTime;
        public uint Status;
        public uint cJobs;
        public uint AveragePPM;
    }


    public class PrinterInformation
    {
        public string ServerName { get; set; }
        public string PrinterName { get; set; }
        public string ShareName { get; set; }
        public string PortName { get; set; }
        public string DriverName { get; set; }
        public string Comment { get; set; }
        public string Location { get; set; }
        public string SeparatorPageFileName { get; set; }
        public string PrintProcessor { get; set; }
        public string DefaultPrintProcessorParameters { get; set; }
        public PrinterAttributes Attributes { get; set; }
        public uint Priority { get; set; }
        public uint DefaultPriority { get; set; }
        public uint StartTime { get; set; }
        public uint UntilTime { get; set; }
        public PrinterStatus Status { get; set; }
        public uint QueuedJobCount { get; set; }
        public uint AveragePagesPerMinute { get; set; }
    }
}
