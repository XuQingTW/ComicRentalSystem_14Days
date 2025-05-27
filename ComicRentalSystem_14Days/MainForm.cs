// In ComicRentalSystem_14Days/MainForm.cs

using ComicRentalSystem_14Days.Forms;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models; // 引用 Models
using ComicRentalSystem_14Days.Services;
using System;
using System.Data; // 引用 System.Data
using System.Diagnostics;
using System.Linq; // 引用 Linq
using System.Windows.Forms;

namespace ComicRentalSystem_14Days
{
    public partial class MainForm : BaseForm
    {
        private readonly ILogger? _logger;
        private readonly IReloadService? _reloadService;
        private ComicService? _comicService; // 新增 ComicService 欄位

        public MainForm() : base()
        {
            InitializeComponent();
            if (this.DesignMode)
            {
                // this.Text = "MainForm (設計模式)";
            }
        }

        public MainForm(ILogger logger) : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            base.SetLogger(logger);
            _reloadService = new ReloadService();

            // 初始化 FileHelper 和 ComicService
            var fileHelper = new FileHelper();
            _comicService = new ComicService(fileHelper, _logger);

            _logger.Log("MainForm initialized with logger and services.");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _logger?.Log("MainForm is loading.");
            if (_comicService != null)
            {
                SetupDataGridView();
                LoadAvailableComics();
                // 訂閱事件，以便在漫畫資料變更時自動更新列表
                _comicService.ComicsChanged += ComicService_ComicsChanged;
            }
        }

        private void ComicService_ComicsChanged(object? sender, EventArgs e)
        {
            // 當資料變更時，重新載入可借閱漫畫列表
            _logger?.Log("ComicsChanged event received, reloading available comics.");
            LoadAvailableComics();
        }

        private void SetupDataGridView()
        {
            _logger?.Log("Setting up DataGridView for available comics.");
            dgvAvailableComics.AutoGenerateColumns = false;
            dgvAvailableComics.Columns.Clear();
            dgvAvailableComics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "書名", FillWeight = 40 });
            dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Author", HeaderText = "作者", FillWeight = 30 });
            dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Genre", HeaderText = "類型", FillWeight = 20 });
            dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Isbn", HeaderText = "ISBN", FillWeight = 30 });

            dgvAvailableComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAvailableComics.MultiSelect = false;
            dgvAvailableComics.ReadOnly = true;
            dgvAvailableComics.AllowUserToAddRows = false;
        }

        private void LoadAvailableComics()
        {
            if (_comicService == null) return;
            _logger?.Log("Loading available comics into MainForm DataGridView.");

            try
            {
                // 從服務取得所有漫畫，並篩選出未被租借的
                var availableComics = _comicService.GetAllComics().Where(c => !c.IsRented).ToList();

                // 使用Invoke確保UI操作在主執行緒上執行
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => {
                        dgvAvailableComics.DataSource = null;
                        dgvAvailableComics.DataSource = availableComics;
                    }));
                }
                else
                {
                    dgvAvailableComics.DataSource = null;
                    dgvAvailableComics.DataSource = availableComics;
                }

                _logger?.Log($"Successfully loaded {availableComics.Count} available comics.");
            }
            catch (Exception ex)
            {
                LogErrorActivity("Error loading available comics.", ex);
                MessageBox.Show($"載入可用漫畫列表時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 離開ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger?.Log("Exit menu item clicked. Application will exit.");
            Application.Exit();
        }

        private void 漫畫管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger?.Log("Opening ComicManagementForm.");
            if (_logger != null)
            {
                ComicManagementForm comicMgmtForm = new ComicManagementForm(_logger);
                comicMgmtForm.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Logger 未初始化，無法開啟漫畫管理。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 會員管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger?.Log("Opening MemberManagementForm.");
            if (_logger != null)
            {
                MemberManagementForm memberMgmtForm = new MemberManagementForm(_logger);
                memberMgmtForm.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Logger 未初始化，無法開啟會員管理。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rentalManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger?.Log("Opening RentalForm.");
            if (_logger == null || _reloadService == null)
            {
                MessageBox.Show("Logger 或 ReloadService 未初始化，無法開啟租借管理。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                FileHelper fileHelper = new FileHelper();
                // 租借表單會自己建立新的 Service 實例，這裡傳入 Logger 和 ReloadService 即可
                ComicService comicServiceForRental = new ComicService(fileHelper, _logger);
                MemberService memberServiceForRental = new MemberService(fileHelper, _logger);

                RentalForm rentalForm = new RentalForm(comicServiceForRental, memberServiceForRental, _logger, _reloadService);
                rentalForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                _logger?.LogError("Failed to open RentalForm.", ex);
                MessageBox.Show($"開啟租借表單時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 檢視日誌ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger?.Log("View Log menu item clicked.");
            try
            {
                string logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ComicRentalApp", "Logs");
                // Program.cs 中設定的日誌檔名為 "ComicRentalSystemLog.txt"
                string logFilePath = Path.Combine(logDirectory, "ComicRentalSystemLog.txt");

                if (File.Exists(logFilePath))
                {
                    Process.Start(new ProcessStartInfo(logFilePath) { UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show("日誌檔案尚未建立或找不到。", "資訊", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError("Failed to open the log file.", ex);
                MessageBox.Show($"無法開啟日誌檔案: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 覆寫 OnFormClosing 來取消訂閱事件，避免記憶體洩漏
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _logger?.Log("MainForm is closing. Unsubscribing from events.");
            if (_comicService != null)
            {
                _comicService.ComicsChanged -= ComicService_ComicsChanged;
            }
            base.OnFormClosing(e);
        }
    }
}