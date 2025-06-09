using ComicRentalSystem_14Days.Logging;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class LogManagementForm : BaseForm
    {
        private readonly FileLogger _logger;

        public LogManagementForm(FileLogger logger) : base(logger)
        {
            InitializeComponent();
            _logger = logger;
            numericRetentionDays.Value = _logger != null ? _logger.GetType().GetField("_retentionDays", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(_logger) as int? ?? 90 : 90;
            // LoadLogFiles will be called in Form_Load
        }

        private async void LogManagementForm_Load(object sender, EventArgs e)
        {
            if (_logger == null)
            {
                LogErrorActivity("LogManagementForm_Load: Logger is null, cannot load logs.", null);
                MessageBox.Show("日誌服務未初始化。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LogActivity("LogManagementForm_Load: Loading log files asynchronously.");
            await LoadLogFilesAsync();
        }

        private async Task LoadLogFilesAsync()
        {
            if (!listViewLogs.IsHandleCreated || listViewLogs.IsDisposed) return;

            listViewLogs.Items.Clear();
            try
            {
                var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ComicRentalApp", "Logs");
                if (!Directory.Exists(logDirectoryPath))
                {
                    LogActivity($"LoadLogFilesAsync: Log directory does not exist: {logDirectoryPath}");
                    Directory.CreateDirectory(logDirectoryPath); // Ensure directory exists
                }
                var dir = new DirectoryInfo(logDirectoryPath);

                // Run synchronous file operation on a background thread
                FileInfo[] files = await Task.Run(() => dir.GetFiles("*.txt"));

                if (!listViewLogs.IsHandleCreated || listViewLogs.IsDisposed) return; // Re-check after await

                foreach (var file in files)
                {
                    listViewLogs.Items.Add(new ListViewItem(file.Name));
                }
                LogActivity($"LoadLogFilesAsync: Loaded {files.Length} log file names.");
            }
            catch (Exception ex)
            {
                LogErrorActivity("LoadLogFilesAsync: Error loading log files.", ex);
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    MessageBox.Show($"載入日誌檔案時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _logger.SetRetentionDays((int)numericRetentionDays.Value);
            MessageBox.Show("保留天數已更新", "資訊", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (_logger == null)
            {
                LogErrorActivity("btnDeleteSelected_Click: Logger is null, cannot delete logs.", null);
                MessageBox.Show("日誌服務未初始化。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LogActivity("btnDeleteSelected_Click: Deleting selected log files.");
            foreach (ListViewItem item in listViewLogs.SelectedItems)
            {
                if (DateTime.TryParseExact(Path.GetFileNameWithoutExtension(item.Text), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var date))
                {
                    // _logger.DeleteLogByDate is synchronous. If it involved heavy I/O, it would also need async treatment.
                    // For this subtask, we assume DeleteLogByDate is quick enough or will be handled in FileLogger's own async update.
                    _logger.DeleteLogByDate(date);
                    LogActivity($"btnDeleteSelected_Click: Requested deletion for log of date: {date:yyyyMMdd}.");
                }
            }
            LogActivity("btnDeleteSelected_Click: Deletion requests processed. Refreshing log file list asynchronously.");
            await LoadLogFilesAsync();
        }
    }
}
