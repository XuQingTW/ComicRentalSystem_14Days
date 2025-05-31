using ComicRentalSystem_14Days.Models; // Added
using ComicRentalSystem_14Days.Services; // Existing, ensure it's comprehensive
using ComicRentalSystem_14Days.Forms; // Existing, ensure it's comprehensive
using ComicRentalSystem_14Days.Interfaces; // Existing
using System; // For ArgumentNullException, EventArgs
using System.Linq; // Added
using System.Windows.Forms; // For MessageBox, etc.
using System.Diagnostics; // Existing
// Removed: using ComicRentalSystem_14Days.Helpers; (Not directly used in provided snippets of MainForm)
// Removed: using System.Data; (Not directly used in provided snippets of MainForm)


namespace ComicRentalSystem_14Days
{
    public partial class MainForm : BaseForm
    {
        private readonly User _currentUser;
        private readonly ComicService _comicService;
        private readonly MemberService _memberService;
        private readonly IReloadService _reloadService;
        private readonly ILogger _logger;

        public MainForm() : base() // Parameterless constructor for Windows Forms designer
        {
            InitializeComponent();
            if (this.DesignMode)
            {
                // this.Text = "MainForm (設計模式)";
            }
        }

        // Primary constructor
        public MainForm(ILogger logger, ComicService comicService, MemberService memberService, IReloadService reloadService, User currentUser) : this()
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            this._memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            this._reloadService = reloadService ?? throw new ArgumentNullException(nameof(reloadService));
            this._currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

            base.SetLogger(logger); // Assumes BaseForm has public void SetLogger(ILogger logger)

            _logger.Log($"MainForm initialized for user: {_currentUser.Username}, Role: {_currentUser.Role}");

            SetupUIAccessControls();
            UpdateStatusBar();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this._logger?.Log("MainForm is loading.");
            // _comicService is guaranteed non-null by constructor, direct use.
            SetupDataGridView();
            LoadAvailableComics();
            this._comicService.ComicsChanged += ComicService_ComicsChanged;
        }

        private void ComicService_ComicsChanged(object? sender, EventArgs e)
        {
            this._logger?.Log("ComicsChanged event received, reloading available comics.");
            LoadAvailableComics();
        }

        private void SetupDataGridView()
        {
            this._logger?.Log("Setting up DataGridView for available comics.");
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
            // _comicService is guaranteed non-null by constructor
            this._logger?.Log("Loading available comics into MainForm DataGridView.");

            try
            {
                var availableComics = this._comicService.GetAllComics().Where(c => !c.IsRented).ToList();

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

                this._logger?.Log($"Successfully loaded {availableComics.Count} available comics.");
            }
            catch (Exception ex)
            {
                // Assuming LogErrorActivity is a method in BaseForm or this class
                LogErrorActivity("Error loading available comics.", ex);
                MessageBox.Show($"載入可用漫畫列表時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupUIAccessControls()
        {
            bool isAdmin = _currentUser.Role == UserRole.Admin;
            this._logger.Log($"Setting up UI controls. User is Admin: {isAdmin}");

            var comicMgmtItem = this.Controls.Find("漫畫管理ToolStripMenuItem", true).FirstOrDefault() as ToolStripMenuItem;
            if (comicMgmtItem != null)
            {
                comicMgmtItem.Visible = isAdmin;
                comicMgmtItem.Enabled = isAdmin;
            }

            var memberMgmtItem = this.Controls.Find("會員管理ToolStripMenuItem", true).FirstOrDefault() as ToolStripMenuItem;
            if (memberMgmtItem != null)
            {
                memberMgmtItem.Visible = isAdmin;
                memberMgmtItem.Enabled = isAdmin;
            }

            var userRegItem = this.Controls.Find("使用者註冊ToolStripMenuItem", true).FirstOrDefault() as ToolStripMenuItem;
            if (userRegItem != null)
            {
                userRegItem.Visible = isAdmin;
                userRegItem.Enabled = isAdmin;
            }
        }

        private void UpdateStatusBar()
        {
            var statusLabel = this.Controls.Find("toolStripStatusLabelUser", true).FirstOrDefault() as ToolStripStatusLabel;
            if (statusLabel != null)
            {
                statusLabel.Text = $"使用者: {_currentUser.Username} | 角色: {_currentUser.Role}";
                this._logger.Log($"Status bar updated: User: {_currentUser.Username}, Role: {_currentUser.Role}");
            }
            else
            {
                this._logger.Log("toolStripStatusLabelUser not found. Cannot update status bar.");
            }
        }

        private void 離開ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("Exit menu item clicked. Application will exit.");
            Application.Exit();
        }

        private void 漫畫管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("Opening ComicManagementForm.");
            // Services are guaranteed non-null by constructor
            ComicManagementForm comicMgmtForm = new ComicManagementForm(this._logger, this._comicService);
            comicMgmtForm.ShowDialog(this);
        }

        private void 會員管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("Opening MemberManagementForm.");
            // Services are guaranteed non-null by constructor
            MemberManagementForm memberMgmtForm = new MemberManagementForm(this._logger, this._memberService);
            memberMgmtForm.ShowDialog(this);
        }

        private void rentalManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("Opening RentalForm.");
            // Services are guaranteed non-null by constructor here
            // No need for explicit null check of this._logger, this._comicService, this._memberService, this._reloadService
            // as they are checked in the constructor.
            try
            {
                RentalForm rentalForm = new RentalForm(this._comicService, this._memberService, this._logger, this._reloadService);
                rentalForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                this._logger?.LogError("Failed to open RentalForm.", ex);
                MessageBox.Show($"開啟租借表單時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 使用者註冊ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("使用者註冊ToolStripMenuItem clicked.");
            if (_currentUser.Role == UserRole.Admin)
            {
                if (this._logger != null && Program.AppAuthService != null)
                {
                    var regForm = new ComicRentalSystem_14Days.Forms.RegistrationForm(this._logger, Program.AppAuthService);
                    regForm.ShowDialog(this);
                }
                else
                {
                    MessageBox.Show("Logger 或 AuthenticationService 未初始化，無法開啟使用者註冊。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this._logger?.LogError("RegistrationForm could not be opened due to null logger or AppAuthService.");
                }
            }
            else
            {
                MessageBox.Show("只有管理員才能註冊新使用者。", "權限不足", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this._logger?.Log($"Non-admin user '{_currentUser.Username}' tried to open RegistrationForm.");
            }
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log($"User '{_currentUser.Username}' logging out.");
            Application.Restart();
        }

        private void 檢視日誌ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("View Log menu item clicked.");
            try
            {
                // Assuming Path.Combine and File.Exists are from System.IO
                string logDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ComicRentalApp", "Logs");
                string logFilePath = System.IO.Path.Combine(logDirectory, "ComicRentalSystemLog.txt");

                if (System.IO.File.Exists(logFilePath))
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
                this._logger?.LogError("Failed to open the log file.", ex);
                MessageBox.Show($"無法開啟日誌檔案: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            this._logger?.Log("MainForm is closing. Unsubscribing from events.");
            // _comicService is guaranteed non-null here
            this._comicService.ComicsChanged -= ComicService_ComicsChanged;
            base.OnFormClosing(e);
        }
    }
}