using System;

namespace StreamLauncher.Wpf.Infrastructure
{
    [Serializable]
    public sealed class BindingExceptionInformation
    {
        internal BindingExceptionInformation(string callstack,
            DateTime datetime, string logicalOperationStack,
            int processId, int threadId, long timestamp)
        {
            Callstack = callstack;
            DateTime = datetime;
            LogicalOperationStack = logicalOperationStack;
            ProcessId = processId;
            ThreadId = threadId;
            Timestamp = timestamp;
        }

        public string Callstack { get; private set; }
        public DateTime DateTime { get; private set; }
        public string LogicalOperationStack { get; private set; }
        public int ProcessId { get; private set; }
        public int ThreadId { get; private set; }
        public long Timestamp { get; private set; }
    }
}