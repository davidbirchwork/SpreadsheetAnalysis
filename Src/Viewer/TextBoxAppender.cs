using System;
using System.Windows;
using System.Windows.Controls;
using log4net.Appender;

namespace HyperGraphViewer {
    /// <summary>
    /// Allows Log4net to speak to WPF
    /// https://stackoverflow.com/questions/14114614/configuring-log4net-textboxappender-custom-appender-via-xml-file/54110133#54110133
    /// </summary>
    public class TextBoxAppender : AppenderSkeleton {
        private TextBox AppenderTextBox { get; set; }
        private Window window;

        public string WindowName { get; set; }
        public string TextBoxName { get; set; }

        private T FindControl<T>(Control root, string textBoxName) where T:class{
            if (root.Name == textBoxName) {
                return root as T;
            }

            return root.FindName(textBoxName) as T;
        }

        protected override void Append(log4net.Core.LoggingEvent loggingEvent) {
            if (window == null || AppenderTextBox == null) {
                if (string.IsNullOrEmpty(WindowName) ||
                    string.IsNullOrEmpty(TextBoxName))
                    return;

                foreach (Window w in Application.Current.Windows) {
                    if (w.Name == WindowName) {
                        this.window = w;
                    }
                }
                if (window == null)
                    return;

                AppenderTextBox = FindControl<TextBox>(window, TextBoxName);
                if (AppenderTextBox == null)
                    return;

                window.Closing += (s, e) => AppenderTextBox = null;
            }
            window.Dispatcher.BeginInvoke( new Action(delegate {
                AppenderTextBox.AppendText(RenderLoggingEvent(loggingEvent));
                AppenderTextBox.ScrollToEnd();
            }));
        }
    }
}