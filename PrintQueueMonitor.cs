using System;
using System.Collections.Generic;
using System.Printing;
using System.Runtime.InteropServices;
using System.Threading;

namespace PhotoBooth
{
    public class PrintQueueMonitor
    {
        #region DLL Import Functions

        [DllImport("winspool.drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi,
             ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter(String pPrinterName, out IntPtr phPrinter, Int32 pDefault);

        [DllImport("winspool.drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true,
             CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(Int32 hPrinter);

        [DllImport("winspool.drv", EntryPoint = "FindFirstPrinterChangeNotification", SetLastError = true,
             CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FindFirstPrinterChangeNotification([In()] IntPtr hPrinter, [In()] Int32 fwFlags,
            [In()] Int32 fwOptions,
            [In(), MarshalAs(UnmanagedType.LPStruct)] PRINTER_NOTIFY_OPTIONS pPrinterNotifyOptions);

        [DllImport("winspool.drv", EntryPoint = "FindNextPrinterChangeNotification", SetLastError = true,
             CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern bool FindNextPrinterChangeNotification([In()] IntPtr hChangeObject,
            [Out()] out Int32 pdwChange,
            [In(), MarshalAs(UnmanagedType.LPStruct)] PRINTER_NOTIFY_OPTIONS pPrinterNotifyOptions,
            [Out()] out IntPtr lppPrinterNotifyInfo);

        #endregion

        #region Events

        public event Action OnJobAdded;

        #endregion

        #region private variables

        private IntPtr _printerHandle = IntPtr.Zero;
        private readonly string _spoolerName;
        private readonly ManualResetEvent _mrEvent = new ManualResetEvent(false);
        private IntPtr _changeHandle = IntPtr.Zero;
        private readonly PRINTER_NOTIFY_OPTIONS _notifyOptions = new PRINTER_NOTIFY_OPTIONS();
        private readonly Dictionary<int, string> _objJobDict = new Dictionary<int, string>();
        private PrintQueue _spooler;

        #endregion

        #region constructor

        public PrintQueueMonitor(string strSpoolName)
        {
            // Let us open the printer and get the printer handle.
            _spoolerName = strSpoolName;
            //Start Monitoring
            Start();

        }

        #endregion

        #region destructor

        ~PrintQueueMonitor()
        {
            Stop();
        }

        #endregion

        #region StartMonitoring

        public void Start()
        {
            OpenPrinter(_spoolerName, out _printerHandle, 0);
            if (_printerHandle != IntPtr.Zero)
            {
                //We got a valid Printer handle.  Let us register for change notification....
                _changeHandle = FindFirstPrinterChangeNotification(_printerHandle,
                    (int) PRINTER_CHANGES.PRINTER_CHANGE_JOB, 0, _notifyOptions);
                // We have successfully registered for change notification.  Let us capture the handle...
                _mrEvent.Handle = _changeHandle;
                //Now, let us wait for change notification from the printer queue....
                ThreadPool.RegisterWaitForSingleObject(_mrEvent, PrinterNotifyWaitCallback, _mrEvent, -1, true);
            }

            _spooler = new PrintQueue(new PrintServer(), _spoolerName);
            foreach (var psi in _spooler.GetPrintJobInfoCollection())
            {
                _objJobDict[psi.JobIdentifier] = psi.Name;
            }
        }

        #endregion

        #region StopMonitoring

        public void Stop()
        {
            if (_printerHandle == IntPtr.Zero)
                return;

            try
            {
                ClosePrinter((int)_printerHandle);
                _printerHandle = IntPtr.Zero;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion

        #region Callback Function

        public void PrinterNotifyWaitCallback(Object state, bool timedOut)
        {
            if (_printerHandle == IntPtr.Zero)
                return;

            #region read notification details

            _notifyOptions.Count = 1;
            int pdwChange;
            IntPtr pNotifyInfo;
            var bResult = FindNextPrinterChangeNotification(_changeHandle, out pdwChange, _notifyOptions, out pNotifyInfo);
            //If the Printer Change Notification Call did not give data, exit code
            if (bResult == false)
                return;

            //If the Change Notification was not relgated to job, exit code
            if ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_ADD_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_ADD_JOB)
                OnJobAdded?.Invoke();

            //if ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_SET_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_SET_JOB)
            //    IRaiseItemChangedEvents();

            //if ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_DELETE_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_DELETE_JOB)
            //    IRaiseItemChangedEvents();

            //if ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_WRITE_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_WRITE_JOB)
            //    IRaiseItemChangedEvents();

            #endregion 

            #region reset the Event and wait for the next event

            _mrEvent.Reset();
            ThreadPool.RegisterWaitForSingleObject(_mrEvent, PrinterNotifyWaitCallback, _mrEvent, -1, true);

            #endregion 

        }

        #endregion
    }
}