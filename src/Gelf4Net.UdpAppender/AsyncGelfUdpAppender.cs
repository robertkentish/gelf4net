﻿using Gelf4Net.Util;
using log4net.Core;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace Gelf4Net.Appender
{
    public class AsyncGelfUdpAppender : GelfUdpAppender
    {
        private readonly BufferedLogSender _sender;
        public int Threads { get; set; }
        public int BufferSize { get; set; }

        public AsyncGelfUdpAppender()
        {
            var options = new BufferedSenderOptions
            {
                BufferSize = BufferSize,
                NumTasks = Threads,
            };
            _sender = new BufferedLogSender(options, SendMessageAsync);
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            foreach (var loggingEvent in loggingEvents)
            {
                Append(loggingEvent);
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (FilterEvent(loggingEvent))
            {
                _sender.QueueSend(RenderLoggingEvent(loggingEvent));
            }
        }

        protected override void OnClose()
        {
            Debug.WriteLine("[Gelf4Net] Closing Async Appender");
            _sender.Stop();
            base.OnClose();
        }
    }
}
