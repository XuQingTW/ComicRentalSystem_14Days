using System;
using ComicRentalSystem_14Days.Interfaces;
using System.Windows.Forms;
using System.Drawing; 
using System.ComponentModel; 

namespace ComicRentalSystem_14Days
{
    [DesignerCategory("Form")]
    public class BaseForm : ModernBaseForm // Changed inheritance
    {
        protected ILogger? Logger { get; private set; }

        protected BaseForm() : base() // Added call to base constructor
        {
            // InitializeBaseFormProperties(); // Properties now set by ModernBaseForm or can be removed
        }

        protected BaseForm(ILogger logger) : this()
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Removed InitializeBaseFormProperties as its settings are covered by ModernBaseForm
        // or are default/undesired in the new styling context.

        public void SetLogger(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected void LogActivity(string message)
        {
            if (Logger != null)
            {
                Logger.Log($"[{this.Name} ACTIVITY]: {message}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[LOG - {this.Name} - NO LOGGER]: {message} at {DateTime.Now}");
            }
        }

        protected void LogErrorActivity(string message, Exception? ex = null)
        {
            if (Logger != null)
            {
                Logger.LogError($"[{this.Name} ERROR]: {message}", ex);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR LOG - {this.Name} - NO LOGGER]: {message} {(ex != null ? ex.ToString() : "")} at {DateTime.Now}");
            }
        }
    }
}