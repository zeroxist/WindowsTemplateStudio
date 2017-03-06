﻿using Microsoft.Templates.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemTasks = System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using EnvDTE;
using EnvDTE80;
using System.Reflection;
using Microsoft.Templates.Core;

namespace Microsoft.Templates.Extension
{
    class VsOutputPane
    {
        private const string UWPCommunityTemplatesPaneGuid = "45480fff-0658-42e1-97f0-82cac23603aa";
        private OutputWindowPane _pane;
        private Guid _paneGuid;
        public VsOutputPane() 
        {
            _paneGuid = Guid.Parse(UWPCommunityTemplatesPaneGuid);
            _pane = GetOrCreatePane(_paneGuid, true, false);
            if (_pane != null)
            {
                _pane.Activate();
            }
        }
        public void Write(string data)
        {
            _pane.OutputString(data);
        }
        private OutputWindowPane GetOrCreatePane(Guid paneGuid, bool visible, bool clearWithSolution)
        {
            OutputWindowPane result = null;
            try
            {
                string title = "UWP Community Templates";
                if (ServiceProvider.GlobalProvider.GetService(typeof(DTE)) is DTE2 dte)
                {
                    result = GetUwpPane(dte);
                    if (result == null)
                    {
                        CreateUwpPane(paneGuid, visible, clearWithSolution, title);
                        result = GetUwpPane(dte);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception creating Visual Studio Output window pane. {ex.ToString()}");
            }
            return result;
        }

        private static void CreateUwpPane(Guid paneGuid, bool visible, bool clearWithSolution, string title)
        {
            IVsOutputWindow output = ServiceProvider.GlobalProvider.GetService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            // Create a new pane.  
            int createRetVal = output.CreatePane(
                ref paneGuid,
                title,
                Convert.ToInt32(visible),
                Convert.ToInt32(clearWithSolution));

            output.GetPane(ref paneGuid, out var pane);
            pane.OutputString($"UWP Communit Templates {DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff")}\n");
            pane.OutputString($"Version: {GetVersion()}\n");
            pane.OutputString($">\n");
        }

        private static string GetVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetName().Version.ToString();
        }

        private OutputWindowPane GetUwpPane(DTE2 dte)
        {
            OutputWindowPanes panes = dte.ToolWindows.OutputWindow.OutputWindowPanes;
            OutputWindowPane result = null;
            foreach (OutputWindowPane p in panes)
            {
                if (Guid.Parse(p.Guid).ToString() == _pane.ToString())
                {
                    result = p;
                }
            }

            return result;
        }

        private async SystemTasks.Task SafeTrackAsync(Action trackAction)
        {
            try
            {
                var task = SystemTasks.Task.Run(() => {
                    trackAction();
                });
                await task;
            }
            catch (AggregateException aggEx)
            {
                foreach (Exception ex in aggEx.InnerExceptions)
                {
                    Trace.TraceError("Error writing to Visual Studio Output: {0}", ex.ToString());
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error writing to Visual Studio Output: {0}", ex.ToString());
            }
        }
    }
}