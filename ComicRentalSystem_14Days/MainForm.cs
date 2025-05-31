using ComicRentalSystem_14Days.Forms;
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days
{
    public partial class MainForm : BaseForm
    {
        private readonly ILogger? _logger;
        private readonly ComicService? _comicService;
        private readonly MemberService? _memberService;
        private readonly IReloadService? _reloadService;

        public MainForm() : base()
        {
            InitializeComponent();
            if (this.DesignMode)
            {
                // this.Text = "MainForm (設計模式)";
            }
        }

        public MainForm(ILogger logger, ComicService comicService, MemberService memberService, IReloadService reloadService) : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _reloadService = reloadService ?? throw new ArgumentNullException(nameof(reloadService));
            base.SetLogger(logger);

            _logger.Log("MainForm initialized with shared logger and services.");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _logger?.Log("MainForm is loading.");
            if (_comicService != null)
            {
                SetupDataGridView();
                LoadAvailableComics();
                _comicService.ComicsChanged += ComicService_ComicsChanged;
            }
        }

        private void ComicService_ComicsChanged(object? sender, EventArgs e)
        {
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
                var availableComics = _comicService.GetAllComics().Where(c => !c.IsRented).ToList();

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
            if (_logger != null && _comicService != null) // Use injected _comicService
            {
                ComicManagementForm comicMgmtForm = new ComicManagementForm(_logger, _comicService);
                comicMgmtForm.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Logger 或 ComicService 未初始化，無法開啟漫畫管理。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 會員管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger?.Log("Opening MemberManagementForm.");
            if (_logger != null && _memberService != null) // Use injected _memberService
            {
                MemberManagementForm memberMgmtForm = new MemberManagementForm(_logger, _memberService);
                memberMgmtForm.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Logger 或 MemberService 未初始化，無法開啟會員管理。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rentalManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger?.Log("Opening RentalForm.");
            // Use injected services
            if (_logger == null || _comicService == null || _memberService == null || _reloadService == null)
            {
                MessageBox.Show("核心服務未初始化，無法開啟租借管理。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                RentalForm rentalForm = new RentalForm(_comicService, _memberService, _logger, _reloadService);
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