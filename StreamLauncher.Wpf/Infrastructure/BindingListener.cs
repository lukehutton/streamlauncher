using System;
using System.Diagnostics;
using System.Reflection;

namespace StreamLauncher.Wpf.Infrastructure
{
    public sealed class BindingListener : DefaultTraceListener
    {
        public BindingListener(TraceOptions options)
        {
            IsFirstWrite = true;
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(this);
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
            TraceOutputOptions = options;
            DetermineInformationPropertyCount();
        }

        private string Callstack { get; set; }
        private string DateTime { get; set; }
        private int InformationPropertyCount { get; set; }
        private bool IsFirstWrite { get; set; }
        private string LogicalOperationStack { get; set; }
        private string Message { get; set; }
        private string ProcessId { get; set; }
        private string ThreadId { get; set; }
        private string Timestamp { get; set; }

        private void DetermineInformationPropertyCount()
        {
            foreach (TraceOptions traceOptionValue in Enum.GetValues(typeof (TraceOptions)))
            {
                if (traceOptionValue != TraceOptions.None)
                {
                    InformationPropertyCount += GetTraceOptionEnabled(traceOptionValue);
                }
            }
        }

        private int GetTraceOptionEnabled(TraceOptions option)
        {
            return (TraceOutputOptions & option) == option ? 1 : 0;
        }

        public override void WriteLine(string message)
        {
            if (IsFirstWrite)
            {
                Message = message;
                IsFirstWrite = false;
            }
            else
            {
                var propertyInformation = message.Split(new[] {"="}, StringSplitOptions.None);

                if (propertyInformation.Length == 1)
                {
                    LogicalOperationStack = propertyInformation[0];
                }
                else
                {
                    GetType().GetProperty(propertyInformation[0],
                        BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Instance)
                        .SetValue(this, propertyInformation[1], null);
                }

                InformationPropertyCount--;
            }

            Flush();

            if (InformationPropertyCount == 0)
            {
                PresentationTraceSources.DataBindingSource.Listeners.Remove(this);
                throw new BindingException(Message,
                    new BindingExceptionInformation(Callstack,
                        System.DateTime.Parse(DateTime),
                        LogicalOperationStack, int.Parse(ProcessId),
                        int.Parse(ThreadId), long.Parse(Timestamp)));
            }
        }
    }
}