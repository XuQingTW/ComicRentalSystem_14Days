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
            LoadLogFiles();
        }

        private void LoadLogFiles()
        {
            listViewLogs.Items.Clear();
            var dir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ComicRentalApp", "Logs"));
            foreach (var file in dir.GetFiles("*.txt"))
            {
                listViewLogs.Items.Add(new ListViewItem(file.Name));
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _logger.SetRetentionDays((int)numericRetentionDays.Value);
            MessageBox.Show("保留天數已更新", "資訊", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewLogs.SelectedItems)
            {
                if (DateTime.TryParseExact(Path.GetFileNameWithoutExtension(item.Text), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var date))
                {
                    _logger.DeleteLogByDate(date);
                }
            }
            LoadLogFiles();
        }
    }
}
