using ComicRentalSystem_14Days.Models; // Added for AdminComicStatusViewModel
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Forms;
using ComicRentalSystem_14Days.Interfaces;
using System;
using System.Collections.Generic; // Added for List
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace ComicRentalSystem_14Days
{
    public partial class MainForm : BaseForm
    {
        private readonly User _currentUser;
        private readonly ComicService _comicService;
        private readonly MemberService _memberService;
        private readonly IReloadService _reloadService;
        private readonly ILogger _logger;

        public MainForm() : base()
        {
            InitializeComponent();
            _currentUser = null!;
            _comicService = null!;
            _memberService = null!;
            _reloadService = null!;
            _logger = null!;
        }

        public MainForm(ILogger logger, ComicService comicService, MemberService memberService, IReloadService reloadService, User currentUser)
            : base()
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            this._memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            this._reloadService = reloadService ?? throw new ArgumentNullException(nameof(reloadService));
            this._currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

            base.SetLogger(logger);
            InitializeComponent();

            _logger.Log($"MainForm initialized for user: {_currentUser.Username}, Role: {_currentUser.Role}");
            SetupUIAccessControls();
            UpdateStatusBar();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this._logger?.Log("MainForm is loading.");
            SetupDataGridView();

            if (_currentUser.Role == UserRole.Admin)
            {
                LoadAllComicsStatusForAdmin();
            }
            else
            {
                LoadAvailableComics();
                LoadMyRentedComics();
            }

            this._comicService.ComicsChanged += ComicService_ComicsChanged;
            if (dgvAvailableComics != null)
            {
                dgvAvailableComics.SelectionChanged += dgvAvailableComics_SelectionChanged;
                dgvAvailableComics_SelectionChanged(this, EventArgs.Empty);
            }
        }

        private void dgvAvailableComics_SelectionChanged(object? sender, EventArgs e)
        {
            if (_currentUser == null) return;

            bool isMember = _currentUser.Role == UserRole.Member;
            if (isMember)
            {
                if (btnRentComic != null && dgvAvailableComics != null) btnRentComic.Enabled = dgvAvailableComics.SelectedRows.Count > 0;
            }
            else
            {
                if (btnRentComic != null) btnRentComic.Enabled = false;
            }
        }

        private void ComicService_ComicsChanged(object? sender, EventArgs e)
        {
            this._logger?.Log("ComicsChanged event received.");
            if (_currentUser.Role == UserRole.Admin)
            {
                this._logger?.Log("Reloading all comics status for admin.");
                LoadAllComicsStatusForAdmin();
            }
            else
            {
                this._logger?.Log("Reloading available comics and member's rented comics.");
                LoadAvailableComics();
                LoadMyRentedComics();
            }
        }

        private void SetupDataGridView()
        {
            if (dgvAvailableComics == null)
            {
                _logger?.LogError("SetupDataGridView: dgvAvailableComics is null.");
                return;
            }

            this._logger?.Log("Setting up DataGridView for comics.");
            dgvAvailableComics.AutoGenerateColumns = false;
            dgvAvailableComics.Columns.Clear();
            dgvAvailableComics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (_currentUser.Role == UserRole.Admin)
            {
                _logger.Log("Configuring DataGridView for Admin view (All Comics Status).");
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "書名", FillWeight = 25 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Author", HeaderText = "作者", FillWeight = 20 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "狀態", FillWeight = 10 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "BorrowerName", HeaderText = "借閱會員", FillWeight = 15 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "BorrowerPhoneNumber", HeaderText = "會員電話", FillWeight = 15 });

                var rentalDateColumn = new DataGridViewTextBoxColumn {
                    DataPropertyName = "RentalDate",
                    HeaderText = "借閱日期",
                    FillWeight = 15
                };
                if (rentalDateColumn.DefaultCellStyle != null)
                {
                    rentalDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
                }
                dgvAvailableComics.Columns.Add(rentalDateColumn);

                var returnDateColumn = new DataGridViewTextBoxColumn {
                    DataPropertyName = "ReturnDate",
                    HeaderText = "歸還日期",
                    FillWeight = 15
                };
                if (returnDateColumn.DefaultCellStyle != null)
                {
                    returnDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
                }
                dgvAvailableComics.Columns.Add(returnDateColumn);
            }
            else // Member view
            {
                _logger.Log("Configuring DataGridView for Member view (Available Comics).");
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "書名", FillWeight = 40 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Author", HeaderText = "作者", FillWeight = 30 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Genre", HeaderText = "類型", FillWeight = 20 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Isbn", HeaderText = "ISBN", FillWeight = 30 });
            }

            dgvAvailableComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAvailableComics.MultiSelect = false;
            dgvAvailableComics.ReadOnly = true;
            dgvAvailableComics.AllowUserToAddRows = false;
        }

        private void LoadAvailableComics()
        {
            if (dgvAvailableComics == null) return;
            this._logger?.Log("Loading available comics into MainForm DataGridView.");
            try
            {
                var availableComics = this._comicService.GetAllComics().Where(c => !c.IsRented).ToList();
                Action updateGrid = () => {
                    dgvAvailableComics.DataSource = null;
                    dgvAvailableComics.DataSource = availableComics;
                };

                if (dgvAvailableComics.IsHandleCreated && this.InvokeRequired) { this.Invoke(updateGrid); }
                else if (dgvAvailableComics.IsHandleCreated) { updateGrid(); }

                this._logger?.Log($"Successfully loaded {availableComics.Count} available comics.");
            }
            catch (Exception ex)
            {
                LogErrorActivity("Error loading available comics.", ex);
                MessageBox.Show($"載入可用漫畫列表時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAllComicsStatusForAdmin()
        {
            if (dgvAvailableComics == null) return;
            _logger?.Log("Loading all comics status for Admin view.");
            try
            {
                var allComics = _comicService.GetAllComics();
                var comicStatuses = new List<AdminComicStatusViewModel>();

                foreach (var comic in allComics)
                {
                    var viewModel = new AdminComicStatusViewModel
                    {
                        Id = comic.Id,
                        Title = comic.Title,
                        Author = comic.Author,
                        Genre = comic.Genre,
                        Isbn = comic.Isbn,
                        RentalDate = comic.RentalDate,
                        ReturnDate = comic.ReturnDate
                    };

                    if (comic.IsRented)
                    {
                        viewModel.Status = "被借閱";
                        Member? member = _memberService.GetMemberById(comic.RentedToMemberId);
                        if (member != null)
                        {
                            viewModel.BorrowerName = member.Name;
                            viewModel.BorrowerPhoneNumber = member.PhoneNumber;
                        }
                        else
                        {
                            viewModel.BorrowerName = "不明";
                            _logger?.LogWarning($"Could not find member with ID {comic.RentedToMemberId} for rented comic ID {comic.Id}");
                        }
                    }
                    else
                    {
                        viewModel.Status = "在館中";
                    }
                    comicStatuses.Add(viewModel);
                }

                Action updateGrid = () => {
                    dgvAvailableComics.DataSource = null;
                    dgvAvailableComics.DataSource = comicStatuses;
                };

                if (dgvAvailableComics.IsHandleCreated && this.InvokeRequired) { this.Invoke(updateGrid); }
                else if (dgvAvailableComics.IsHandleCreated) { updateGrid(); }

                _logger?.Log($"Successfully loaded {comicStatuses.Count} comics for admin view.");
            }
            catch (Exception ex)
            {
                LogErrorActivity("Error loading all comics status for admin.", ex);
                MessageBox.Show($"載入所有漫畫狀態時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupUIAccessControls()
        {
            bool isAdmin = _currentUser.Role == UserRole.Admin;
            _logger.Log($"Setting up UI controls. User is Admin: {isAdmin}");

            var menuStrip = this.Controls.Find("menuStrip2", true).FirstOrDefault() as MenuStrip;
            if (menuStrip != null)
            {
                var managementMenuItem = menuStrip.Items.OfType<ToolStripMenuItem>().FirstOrDefault(item => item.Name == "管理ToolStripMenuItem");
                if (managementMenuItem != null) managementMenuItem.Visible = isAdmin;

                var toolsMenuItem = menuStrip.Items.OfType<ToolStripMenuItem>().FirstOrDefault(item => item.Name == "工具ToolStripMenuItem");
                if (toolsMenuItem != null) toolsMenuItem.Visible = isAdmin;

                var userRegItem = menuStrip.Items.OfType<ToolStripMenuItem>().FirstOrDefault(item => item.Name == "使用者註冊ToolStripMenuItem");
                if (userRegItem != null) userRegItem.Visible = isAdmin;
            }
            else { _logger.LogWarning("MenuStrip control 'menuStrip2' not found."); }

            if (isAdmin)
            {
                if (lblAvailableComics != null)
                {
                    lblAvailableComics.Text = "所有漫畫狀態";
                    lblAvailableComics.Visible = true;
                }
                if (dgvAvailableComics != null) dgvAvailableComics.Visible = true;

                if (lblMyRentedComicsHeader != null) lblMyRentedComicsHeader.Visible = false;
                if (dgvMyRentedComics != null) dgvMyRentedComics.Visible = false;
                if (btnRentComic != null)
                {
                    btnRentComic.Visible = false;
                    btnRentComic.Enabled = false;
                }
            }
            else // User is a Member
            {
                if (lblAvailableComics != null)
                {
                    lblAvailableComics.Text = "目前可借閱的漫畫";
                    lblAvailableComics.Visible = true;
                }
                if (dgvAvailableComics != null) dgvAvailableComics.Visible = true;

                if (lblMyRentedComicsHeader != null) lblMyRentedComicsHeader.Visible = true;
                if (dgvMyRentedComics != null) dgvMyRentedComics.Visible = true;
                if (btnRentComic != null)
                {
                    btnRentComic.Visible = true;
                }
            }
            _logger.Log($"UI controls visibility and text updated based on admin status ({isAdmin}).");
        }

        private void UpdateStatusBar()
        {
            var statusStrip = this.Controls.Find("statusStrip1", true).FirstOrDefault() as StatusStrip;
            if (statusStrip != null)
            {
                var statusLabel = statusStrip.Items.OfType<ToolStripStatusLabel>().FirstOrDefault(item => item.Name == "toolStripStatusLabelUser");
                if (statusLabel != null)
                {
                    statusLabel.Text = $"使用者: {_currentUser.Username} | 角色: {_currentUser.Role}";
                    this._logger.Log($"Status bar updated: User: {_currentUser.Username}, Role: {_currentUser.Role}");
                }
                else { this._logger.LogWarning("ToolStripStatusLabel 'toolStripStatusLabelUser' not found."); }
            }
            else { this._logger.LogWarning("StatusStrip control 'statusStrip1' not found."); }
        }

        private void SetupMyRentedComicsDataGridView()
        {
            if (dgvMyRentedComics == null) return;
            _logger?.Log("Setting up DataGridView for member's rented comics (dgvMyRentedComics).");
            dgvMyRentedComics.AutoGenerateColumns = false;
            dgvMyRentedComics.Columns.Clear();
            dgvMyRentedComics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvMyRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "書名", FillWeight = 35 });
            dgvMyRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Author", HeaderText = "作者", FillWeight = 25 });

            var rentalDateColumn = new DataGridViewTextBoxColumn { DataPropertyName = "RentalDate", HeaderText = "租借日期", FillWeight = 20 };
            rentalDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvMyRentedComics.Columns.Add(rentalDateColumn);

            var returnDateColumn = new DataGridViewTextBoxColumn { DataPropertyName = "ReturnDate", HeaderText = "歸還日期", FillWeight = 20 };
            returnDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvMyRentedComics.Columns.Add(returnDateColumn);

            dgvMyRentedComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMyRentedComics.MultiSelect = false;
            dgvMyRentedComics.ReadOnly = true;
            dgvMyRentedComics.AllowUserToAddRows = false;
        }

        private void LoadMyRentedComics()
        {
            if (dgvMyRentedComics == null) return;
            if (_currentUser == null || _comicService == null || _memberService == null || _logger == null)
            {
                _logger?.LogWarning("LoadMyRentedComics: CurrentUser or critical services are null. Clearing DGV.");
                ClearDgvMyRentedComics();
                return;
            }
            if (_currentUser.Role != UserRole.Member)
            {
                _logger?.Log("LoadMyRentedComics: User is not a Member. Clearing DGV.");
                ClearDgvMyRentedComics();
                return;
            }

            _logger?.Log($"LoadMyRentedComics: Loading comics for member '{_currentUser.Username}'.");
            try
            {
                Member? currentMember = _memberService.GetMemberByUsername(_currentUser.Username);
                if (currentMember == null)
                {
                    _logger?.LogWarning($"LoadMyRentedComics: Member profile not found for username '{_currentUser.Username}'.");
                    ClearDgvMyRentedComics();
                    return;
                }

                var myRentedComics = _comicService.GetAllComics()
                    .Where(c => c.IsRented && c.RentedToMemberId == currentMember.Id)
                    .Select(c => new {
                        c.Title,
                        c.Author,
                        RentalDate = c.RentalDate,
                        ReturnDate = c.ReturnDate
                    })
                    .ToList();

                Action updateGrid = () => {
                    dgvMyRentedComics.DataSource = null;
                    dgvMyRentedComics.DataSource = myRentedComics;
                };

                if (dgvMyRentedComics.IsHandleCreated && this.InvokeRequired) { this.Invoke(updateGrid); }
                else if (dgvMyRentedComics.IsHandleCreated) { updateGrid(); }

                _logger?.Log($"LoadMyRentedComics: Successfully loaded {myRentedComics.Count} rented comics for member ID {currentMember.Id}.");
            }
            catch (Exception ex)
            {
                _logger?.LogError("LoadMyRentedComics: Error loading member's rented comics.", ex);
                MessageBox.Show($"載入您的租借漫畫列表時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearDgvMyRentedComics();
            }
        }

        private void ClearDgvMyRentedComics()
        {
            if (dgvMyRentedComics == null) return;
            Action clearGrid = () => dgvMyRentedComics.DataSource = null;
            if (dgvMyRentedComics.IsHandleCreated && this.InvokeRequired) { this.Invoke(clearGrid); }
            else if (dgvMyRentedComics.IsHandleCreated) { clearGrid(); }
        }

        private void 離開ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("Exit menu item clicked. Application will exit.");
            Application.Exit();
        }

        private void 漫畫管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("Opening ComicManagementForm.");
            ComicManagementForm comicMgmtForm = new ComicManagementForm(this._logger!, this._comicService, this._currentUser);
            comicMgmtForm.ShowDialog(this);
        }

        private void 會員管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("Opening MemberManagementForm.");
            if (Program.AppAuthService != null)
            {
                MemberManagementForm memberMgmtForm = new MemberManagementForm(this._logger!, this._memberService, Program.AppAuthService, this._currentUser);
                memberMgmtForm.ShowDialog(this);
            }
            else
            {
                this._logger?.LogError("AuthenticationService (Program.AppAuthService) is null. Cannot open MemberManagementForm.");
                MessageBox.Show("無法開啟會員管理功能，因為驗證服務未正確初始化。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rentalManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("Opening RentalForm.");
            try
            {
                RentalForm rentalForm = new RentalForm(this._comicService, this._memberService, this._logger!, this._reloadService);
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
                if (this._logger != null && Program.AppAuthService != null && this._memberService != null)
                {
                    var regForm = new ComicRentalSystem_14Days.Forms.RegistrationForm(this._logger, Program.AppAuthService, this._memberService);
                    regForm.ShowDialog(this);
                }
                else
                {
                    MessageBox.Show("Logger, AuthenticationService, 或 MemberService 未初始化，無法開啟使用者註冊。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this._logger?.LogError("RegistrationForm could not be opened due to null logger, AppAuthService, or _memberService.");
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

        private void btnRentComic_Click(object? sender, EventArgs e)
        {
            if (_currentUser == null || _comicService == null || _memberService == null || _logger == null)
            {
                MessageBox.Show("System components are not properly initialized. Cannot proceed with rental.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger?.LogError("btnRentComic_Click: Critical services or _currentUser is null.");
                return;
            }

            if (dgvAvailableComics == null || dgvAvailableComics.SelectedRows.Count == 0)
            {
                _logger?.Log("btnRentComic_Click: No comic selected by user.");
                MessageBox.Show("請先選擇一本漫畫。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_currentUser.Role == UserRole.Admin)
            {
                _logger?.LogWarning("Admin user attempted to click Rent button. This action is for members only.");
                return;
            }

            Comic? selectedComic = dgvAvailableComics.SelectedRows[0].DataBoundItem as Comic;
            if (selectedComic == null)
            {
                MessageBox.Show("選擇的項目無效或不是有效的漫畫資料。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger?.LogError("btnRentComic_Click: Selected item is null or not a Comic object.");
                return;
            }

            if (selectedComic.IsRented)
            {
                _logger?.Log($"btnRentComic_Click: User '{_currentUser.Username}' attempted to rent comic '{selectedComic.Title}' (ID: {selectedComic.Id}) which is already rented.");
                MessageBox.Show($"漫畫 '{selectedComic.Title}' 已經被借出。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAvailableComics();
                return;
            }

            DateTime today = DateTime.Today;
            DateTime minRentalReturnDate = today.AddDays(3);
            DateTime maxRentalReturnDate = today.AddMonths(1);

            using (RentalPeriodForm rentalDialog = new RentalPeriodForm(minRentalReturnDate, maxRentalReturnDate))
            {
                if (rentalDialog.ShowDialog(this) == DialogResult.OK)
                {
                    DateTime selectedReturnDate = rentalDialog.SelectedReturnDate;
                    Member? member = _memberService.GetMemberByUsername(_currentUser.Username);

                    if (member == null)
                    {
                        _logger?.LogWarning($"btnRentComic_Click: No member found for username '{_currentUser.Username}'. Cannot rent comic.");
                        MessageBox.Show($"找不到使用者 '{_currentUser.Username}' 對應的會員資料。請確認會員資料是否存在。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    selectedComic.IsRented = true;
                    selectedComic.RentedToMemberId = member.Id;
                    selectedComic.RentalDate = DateTime.Now;
                    selectedComic.ReturnDate = selectedReturnDate;

                    try
                    {
                        _comicService.UpdateComic(selectedComic);
                        _logger?.Log($"Comic '{selectedComic.Title}' (ID: {selectedComic.Id}) rented to member ID {member.Id} (Username: {_currentUser.Username}) until {selectedReturnDate:yyyy-MM-dd}.");
                        MessageBox.Show($"漫畫 '{selectedComic.Title}' 已成功租借至 {selectedReturnDate:yyyy-MM-dd}。", "租借成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAvailableComics();
                        LoadMyRentedComics();
                        if (dgvAvailableComics != null) dgvAvailableComics_SelectionChanged(null, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError($"btnRentComic_Click: Failed to update comic (ID: {selectedComic.Id}) after rental attempt. {ex.Message}", ex);
                        MessageBox.Show($"更新漫畫狀態時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        selectedComic.IsRented = false;
                        selectedComic.RentedToMemberId = 0;
                        selectedComic.RentalDate = null;
                        selectedComic.ReturnDate = null;
                    }
                }
                else
                {
                    _logger?.Log($"User '{_currentUser.Username}' cancelled the rental process for comic '{selectedComic.Title}'.");
                }
            }
        }

        private void 檢視日誌ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log($"View Log menu item clicked by user '{_currentUser.Username}'.");
            if (_currentUser.Role == UserRole.Admin)
            {
                this._logger?.Log($"Admin user '{_currentUser.Username}' viewing logs.");
                try
                {
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
            else
            {
                this._logger?.Log($"User '{_currentUser.Username}' (Role: {_currentUser.Role}) attempted to view logs. Permission denied.");
                MessageBox.Show("權限不足", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            this._logger?.Log("MainForm is closing. Unsubscribing from events.");
            if (this._comicService != null)
            {
                this._comicService.ComicsChanged -= ComicService_ComicsChanged;
            }
            if (dgvAvailableComics != null)
            {
                 dgvAvailableComics.SelectionChanged -= dgvAvailableComics_SelectionChanged;
            }
            base.OnFormClosing(e);
        }
    }
}