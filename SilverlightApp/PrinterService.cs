using System;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

namespace SilverlightEnumPrinters
{
    [Flags]
    public enum PrinterEnumFlags
    {
        Default = 0x00000001,
        Local = 0x00000002,
        Connections = 0x00000004,
        Favorite = 0x00000004,
        Name = 0x00000008,
        Remote = 0x00000010,
        Shared = 0x00000020,
        Network = 0x00000040,
        Expand = 0x00004000,
        Container = 0x00008000,
        IconMask = 0x00ff0000,
        Icon1 = 0x00010000,
        Icon2 = 0x00020000,
        Icon3 = 0x00040000,
        Icon4 = 0x00080000,
        Icon5 = 0x00100000,
        Icon6 = 0x00200000,
        Icon7 = 0x00400000,
        Icon8 = 0x00800000,
        Hide = 0x01000000
    }

    public class PrinterService
    {
        [Flags]
        private enum LocalMemoryFlags
        {
            LMEM_FIXED = 0x0000,
            LMEM_MOVEABLE = 0x0002,
            LMEM_NOCOMPACT = 0x0010,
            LMEM_NODISCARD = 0x0020,
            LMEM_ZEROINIT = 0x0040,
            LMEM_MODIFY = 0x0080,
            LMEM_DISCARDABLE = 0x0F00,
            LMEM_VALID_FLAGS = 0x0F72,
            LMEM_INVALID_HANDLE = 0x8000,
            LHND = (LMEM_MOVEABLE | LMEM_ZEROINIT),
            LPTR = (LMEM_FIXED | LMEM_ZEROINIT),
            NONZEROLHND = (LMEM_MOVEABLE),
            NONZEROLPTR = (LMEM_FIXED)
        }
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumPrinters(PrinterEnumFlags Flags, string Name, uint Level, IntPtr pPrinterEnum, uint cbBuf, ref uint pcbNeeded, ref uint pcReturned);

        [DllImport("kernel32.dll", EntryPoint = "LocalAlloc")]
        private static extern IntPtr LocalAlloc_NoSafeHandle(LocalMemoryFlags uFlags, IntPtr sizetdwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LocalFree(IntPtr hMem);

        private const int ERROR_INSUFFICIENT_BUFFER = 122;


        private ObservableCollection<PrinterInformation> _printers = new ObservableCollection<PrinterInformation>();
        public ObservableCollection<PrinterInformation> Printers
        {
            get { return _printers; }
        }
        public void ClearPrinter()
        {
            Printers.Clear();
        }
        public void LoadPrinters(PrinterEnumFlags Flags)
        {
            

            uint cbNeeded = 0;
            uint cReturned = 0;
            if (EnumPrinters(Flags, null, 2, IntPtr.Zero, 0, ref cbNeeded, ref cReturned))
            {
                // nothing
                return;
            }

            int lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error == ERROR_INSUFFICIENT_BUFFER)
            {
                IntPtr pAddr = LocalAlloc_NoSafeHandle(LocalMemoryFlags.LMEM_FIXED, (IntPtr)cbNeeded);

                if (EnumPrinters(Flags, null, 2, pAddr, cbNeeded, ref cbNeeded, ref cReturned))
                {
                    int offset = pAddr.ToInt32();
                    int increment = Marshal.SizeOf(typeof(PRINTER_INFO_2));
                    for (int i = 0; i < cReturned; i++)
                    {
                        PrinterInformation printer = new PrinterInformation();
                        PRINTER_INFO_2 pinfo = new PRINTER_INFO_2();

                        Marshal.PtrToStructure(new IntPtr(offset), pinfo);

                        printer.Attributes = (PrinterAttributes)pinfo.Attributes;
                        printer.AveragePagesPerMinute = pinfo.AveragePPM;
                        printer.Comment = pinfo.pComment;
                        printer.DefaultPrintProcessorParameters = pinfo.pParameters;
                        printer.DefaultPriority = pinfo.Priority;
                        printer.DriverName = pinfo.pDriverName;
                        printer.Location = pinfo.pLocation;
                        printer.PortName = pinfo.pPortName;
                        printer.PrinterName = pinfo.pPrinterName;
                        printer.PrintProcessor = pinfo.pPrintProcessor;
                        printer.Priority = pinfo.Priority;
                        printer.QueuedJobCount = pinfo.cJobs;
                        printer.SeparatorPageFileName = pinfo.pSepFile;
                        printer.ServerName = pinfo.pServerName;
                        printer.ShareName = pinfo.pShareName;
                        printer.StartTime = pinfo.StartTime;    // should xform this
                        printer.Status = (PrinterStatus)pinfo.Status;
                        printer.UntilTime = pinfo.UntilTime;    // should xform this

                        Printers.Add(printer);

                        offset += increment;
                    }

                    LocalFree(pAddr);
                }
                else
                {
                    lastWin32Error = Marshal.GetLastWin32Error();
                    throw new Exception("Win32 Error: " + lastWin32Error);
                }
            }
        }
    }
}
