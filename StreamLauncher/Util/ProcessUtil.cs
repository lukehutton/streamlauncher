using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace StreamLauncher.Util
{
    public class ProcessUtil : IDisposable
    {     
        private readonly Process _process = new Process();
        private readonly object _eventLock = new object();

        public ProcessUtil(string path, string args = null)
        {
            ProcessStartInfo startInfo = _process.StartInfo;
            startInfo.FileName = path;
            if (args != null)
            {
                startInfo.Arguments = args;
            }

            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            _process.OutputDataReceived += OnOutputDataReceived;
            _process.ErrorDataReceived += OnErrorDataReceived;
        }

        ~ProcessUtil()
        {
            Dispose(false);
        }

        public event EventHandler<DataReceivedEventArgs> OutputDataReceived = delegate { };

        public event EventHandler<DataReceivedEventArgs> ErrorDataReceived = delegate { };

        public int ExitCode
        {
            get { return _process.ExitCode; }
        }

        public ProcessStartInfo StartInfo
        {
            get { return _process.StartInfo; }
        }

        public void Start()
        {
            try
            {
                _process.Start();
                _process.BeginErrorReadLine();
                _process.BeginOutputReadLine();
            }
            catch (Win32Exception exception)
            {
                throw new InvalidOperationException(exception.Message, exception);
            }
        }

        public void Kill()
        {
            _process.Kill();
        }

        public void Wait()
        {
            _process.WaitForExit();
        }

        public void Wait(CancellationToken token)
        {
            using (var waitHandle = new SafeWaitHandle(_process.Handle, false))
            {
                using (var processFinishedEvent = new ManualResetEvent(false))
                {
                    processFinishedEvent.SafeWaitHandle = waitHandle;

                    int index = WaitHandle.WaitAny(new[] { processFinishedEvent, token.WaitHandle });
                    if (index == 1)
                    {                     
                        Kill();                     
                        throw new OperationCanceledException();
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _process.Dispose();
            }
        }

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            lock (_eventLock)
            {
                ErrorDataReceived(sender, e);
            }
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            lock (_eventLock)
            {
                OutputDataReceived(sender, e);
            }
        }
    }
}