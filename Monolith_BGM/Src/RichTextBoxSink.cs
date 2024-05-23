using Serilog.Core;
using Serilog.Events;
using System;
using System.Drawing;
using System.Windows.Forms; 

namespace Monolith_BGM.Src
{
    public class RichTextBoxSink : ILogEventSink
    {
        private readonly RichTextBox _richTextBox;
        private readonly IFormatProvider _formatProvider;

        public RichTextBoxSink(RichTextBox richTextBox, IFormatProvider formatProvider = null)
        {
            _richTextBox = richTextBox;
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            if (_richTextBox == null || logEvent == null)
            {
                // Exit if the RichTextBox is null or if there's no log event to process
                return;
            }

            var message = logEvent.RenderMessage(_formatProvider);
            var output = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logEvent.Level}] {message}{Environment.NewLine}";

            // Make sure the RichTextBox is invoked on the main UI thread
            _richTextBox.Invoke(new Action(() =>
            {
                if (_richTextBox.IsDisposed)
                {
                    return; // Avoid writing to disposed control
                }

                // Set text color based on log level
                Color color;
                switch (logEvent.Level)
                {
                    case LogEventLevel.Error:
                    case LogEventLevel.Fatal:
                        color = Color.Red;
                        break;
                    case LogEventLevel.Warning:
                        color = Color.Orange;
                        break;
                    case LogEventLevel.Information:
                        color = Color.Black;
                        break;
                    case LogEventLevel.Debug:
                    case LogEventLevel.Verbose:
                        color = Color.Gray;
                        break;
                    default:
                        color = Color.Black;
                        break;
                }

                // Apply color and append the log message
                _richTextBox.SelectionStart = _richTextBox.Text.Length;
                _richTextBox.SelectionColor = color;
                _richTextBox.AppendText(output);
                _richTextBox.SelectionStart = _richTextBox.Text.Length;
                _richTextBox.ScrollToCaret();
            }));
        }

    }
}
