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
            // Initialize fields to null! for the designer context or if not properly initialized.
            // This addresses CS8618 for these fields in the parameterless constructor.
            _currentUser = null!;
            _comicService = null!;
            _memberService = null!;
            _reloadService = null!;
            _logger = null!; // BaseForm's logger might be initialized if BaseForm() handles it.

            if (this.DesignMode)
            {
                // this.Text = "MainForm (設計模式)";
            }
        }
        // Primary constructor
        public MainForm(ILogger logger, ComicService comicService, MemberService memberService, IReloadService reloadService, User currentUser)
            : base() // Explicitly call BaseForm's parameterless constructor
        {
            // Initialize fields FIRST
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            this._memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            this._reloadService = reloadService ?? throw new ArgumentNullException(nameof(reloadService));
            this._currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

            base.SetLogger(logger); // Initialize BaseForm's logger

            InitializeComponent(); // NOW call InitializeComponent

            // These are still needed to apply role-based UI logic
            _logger.Log($"MainForm initialized for user: {_currentUser.Username}, Role: {_currentUser.Role}");
            SetupUIAccessControls();
            UpdateStatusBar();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this._logger?.Log("MainForm is loading.");
            // _comicService is guaranteed non-null by constructor, direct use.
            SetupDataGridView(); // Sets up dgvAvailableComics
            SetupMyRentedComicsDataGridView(); // Sets up dgvMyRentedComics

            LoadAvailableComics(); // Loads data for available comics
            LoadMyRentedComics();  // Loads data for member's rented comics

            this._comicService.ComicsChanged += ComicService_ComicsChanged;
            dgvAvailableComics.SelectionChanged += dgvAvailableComics_SelectionChanged;

            // Call selection changed logic to set initial state of btnRentComic
            // This primarily affects btnRentComic based on dgvAvailableComics selection.
            // And SetupUIAccessControls would have already run, setting initial visibility.
            dgvAvailableComics_SelectionChanged(this, EventArgs.Empty);
        }

        private void dgvAvailableComics_SelectionChanged(object? sender, EventArgs e)
        {
            if (_currentUser == null) return; // Should not happen if form is loaded correctly

            bool isMember = _currentUser.Role == UserRole.Member;
            // if (isMember)
            // {
            //     btnRentComic.Enabled = dgvAvailableComics.SelectedRows.Count > 0;
            // }
            // else
            // {
            //     btnRentComic.Enabled = false;
            // }
        }

        private void ComicService_ComicsChanged(object? sender, EventArgs e)
        {
            this._logger?.Log("ComicsChanged event received, reloading available comics and member's rented comics.");
            LoadAvailableComics();
            LoadMyRentedComics(); // Also reload member's rented comics
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
            _logger.Log($"Setting up UI controls. User is Admin: {isAdmin}");

            var menuStrip = this.Controls.Find("menuStrip2", true).FirstOrDefault() as MenuStrip;
            if (menuStrip != null)
            {
                var managementMenuItem = menuStrip.Items.OfType<ToolStripMenuItem>()
                                                 .FirstOrDefault(item => item.Name == "管理ToolStripMenuItem");
                if (managementMenuItem != null)
                {
                    managementMenuItem.Visible = isAdmin;
                    _logger.Log($"管理ToolStripMenuItem visibility set to {isAdmin}");
                }
                else
                {
                    _logger.LogWarning("管理ToolStripMenuItem not found in menuStrip2.");
                }

                var toolsMenuItem = menuStrip.Items.OfType<ToolStripMenuItem>()
                                             .FirstOrDefault(item => item.Name == "工具ToolStripMenuItem");
                if (toolsMenuItem != null)
                {
                    toolsMenuItem.Visible = isAdmin;
                    _logger.Log($"工具ToolStripMenuItem visibility set to {isAdmin}");
                }
                else
                {
                    _logger.LogWarning("工具ToolStripMenuItem not found in menuStrip2.");
                }

                // Specific items under "管理ToolStripMenuItem" that might still need individual control
                // For example, rentalManagementToolStripMenuItem is visible to all users.
                var rentalMgmtItem = managementMenuItem?.DropDownItems.OfType<ToolStripMenuItem>()
                                                       .FirstOrDefault(item => item.Name == "rentalManagementToolStripMenuItem");
                if (rentalMgmtItem != null)
                {
                    rentalMgmtItem.Visible = true;
                    rentalMgmtItem.Enabled = true;
                    _logger.Log("rentalManagementToolStripMenuItem is visible and enabled for all users.");
                }
                else if (managementMenuItem != null) // Only log warning if parent was found
                {
                    _logger.LogWarning("rentalManagementToolStripMenuItem not found under 管理ToolStripMenuItem.");
                }


                // Handle top-level "使用者註冊ToolStripMenuItem" - this seems to be a top-level item based on Designer.cs
                var userRegItem = menuStrip.Items.OfType<ToolStripMenuItem>()
                                                 .FirstOrDefault(item => item.Name == "使用者註冊ToolStripMenuItem");
                if (userRegItem != null)
                {
                    userRegItem.Visible = isAdmin;
                    userRegItem.Enabled = isAdmin;
                    _logger.Log($"使用者註冊ToolStripMenuItem visibility/enabled set to {isAdmin}");
                }
                else
                {
                    _logger.LogWarning("使用者註冊ToolStripMenuItem (top level) not found in menuStrip2.");
                }
            }
            else
            {
                _logger.LogWarning("MenuStrip control 'menuStrip2' not found on the form.");
            }

            // Setup for btnRentComic and related member-specific UI
            // if (btnRentComic != null && lblMyRentedComicsHeader != null && dgvMyRentedComics != null)
            if (lblMyRentedComicsHeader != null && dgvMyRentedComics != null)
            {
                if (!isAdmin) // User is a Member
                {
                    // btnRentComic.Visible = true;
                    // btnRentComic.Enabled = dgvAvailableComics.SelectedRows.Count > 0;
                    lblMyRentedComicsHeader.Visible = true;
                    dgvMyRentedComics.Visible = true;
                }
                else // User is an Admin
                {
                    // btnRentComic.Visible = false;
                    // btnRentComic.Enabled = false;
                    lblMyRentedComicsHeader.Visible = false;
                    dgvMyRentedComics.Visible = false;
                }
                // _logger.Log($"btnRentComic visibility set to {!isAdmin}, enabled state based on selection/role.");
                _logger.Log($"lblMyRentedComicsHeader and dgvMyRentedComics visibility set to {!isAdmin}.");
            }
            else
            {
                // Updated log message to reflect that btnRentComic is not checked here anymore
                _logger.LogWarning("One or more UI controls (lblMyRentedComicsHeader, dgvMyRentedComics) not found during SetupUIAccessControls.");
            }
        }

        private void UpdateStatusBar()
        {
            // Assuming the StatusStrip control is named 'statusStrip1'. This should be verified from MainForm.Designer.cs.
            var statusStrip = this.Controls.Find("statusStrip1", true).FirstOrDefault() as StatusStrip;
            if (statusStrip != null)
            {
                var statusLabel = statusStrip.Items.OfType<ToolStripStatusLabel>()
                                           .FirstOrDefault(item => item.Name == "toolStripStatusLabelUser");
                if (statusLabel != null)
                {
                    statusLabel.Text = $"使用者: {_currentUser.Username} | 角色: {_currentUser.Role}";
                    this._logger.Log($"Status bar updated: User: {_currentUser.Username}, Role: {_currentUser.Role}");
                }
                else
                {
                    this._logger.LogWarning("ToolStripStatusLabel 'toolStripStatusLabelUser' not found in statusStrip1.");
                }
            }
            else
            {
                this._logger.LogWarning("StatusStrip control 'statusStrip1' not found on the form.");
            }
        }

        private void SetupMyRentedComicsDataGridView()
        {
            _logger?.Log("Setting up DataGridView for member's rented comics (dgvMyRentedComics).");
            dgvMyRentedComics.AutoGenerateColumns = false;
            dgvMyRentedComics.Columns.Clear();
            dgvMyRentedComics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvMyRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "書名", FillWeight = 35 });
            dgvMyRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Author", HeaderText = "作者", FillWeight = 25 });

            var rentalDateColumn = new DataGridViewTextBoxColumn {
                DataPropertyName = "RentalDate",
                HeaderText = "租借日期",
                FillWeight = 20
            };
            rentalDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd"; // Format as date only
            dgvMyRentedComics.Columns.Add(rentalDateColumn);

            var returnDateColumn = new DataGridViewTextBoxColumn {
                DataPropertyName = "ReturnDate",
                HeaderText = "歸還日期",
                FillWeight = 20
            };
            returnDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd"; // Format as date only
            dgvMyRentedComics.Columns.Add(returnDateColumn);

            dgvMyRentedComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMyRentedComics.MultiSelect = false;
            dgvMyRentedComics.ReadOnly = true;
            dgvMyRentedComics.AllowUserToAddRows = false;
        }

        private void LoadMyRentedComics()
        {
            if (_currentUser == null || _comicService == null || _memberService == null || _logger == null)
            {
                _logger?.LogWarning("LoadMyRentedComics: CurrentUser or critical services are null. Clearing DGV.");
                if (dgvMyRentedComics.IsHandleCreated && this.InvokeRequired) { this.Invoke(new Action(() => dgvMyRentedComics.DataSource = null)); }
                else if (dgvMyRentedComics.IsHandleCreated) { dgvMyRentedComics.DataSource = null; }
                return;
            }

            if (_currentUser.Role != UserRole.Member)
            {
                _logger?.Log("LoadMyRentedComics: User is not a Member. Clearing DGV.");
                 if (dgvMyRentedComics.IsHandleCreated && this.InvokeRequired) { this.Invoke(new Action(() => dgvMyRentedComics.DataSource = null)); }
                else if (dgvMyRentedComics.IsHandleCreated) { dgvMyRentedComics.DataSource = null; }
                return;
            }

            _logger?.Log($"LoadMyRentedComics: Loading comics for member '{_currentUser.Username}'.");

            try
            {
                Member? currentMember = null;
                try
                {
                    currentMember = _memberService.GetMemberByUsername(_currentUser.Username);
                }
                catch (NotImplementedException nie)
                {
                    _logger?.LogError($"LoadMyRentedComics: _memberService.GetMemberByUsername is not implemented. {nie.Message}");
                    if (dgvMyRentedComics.IsHandleCreated && this.InvokeRequired) { this.Invoke(new Action(() => dgvMyRentedComics.DataSource = null)); }
                    else if (dgvMyRentedComics.IsHandleCreated) { dgvMyRentedComics.DataSource = null; }
                    return;
                }

                if (currentMember == null)
                {
                    _logger?.LogWarning($"LoadMyRentedComics: Member profile not found for username '{_currentUser.Username}'.");
                     if (dgvMyRentedComics.IsHandleCreated && this.InvokeRequired) { this.Invoke(new Action(() => dgvMyRentedComics.DataSource = null)); }
                    else if (dgvMyRentedComics.IsHandleCreated) { dgvMyRentedComics.DataSource = null; }
                    return;
                }
                int currentMemberId = currentMember.Id;

                var allComics = _comicService.GetAllComics();
                var myRentedComics = allComics.Where(c => c.IsRented && c.RentedToMemberId == currentMemberId).ToList();

                Action updateDataSource = () => {
                    dgvMyRentedComics.DataSource = null;
                    dgvMyRentedComics.DataSource = myRentedComics;
                };

                if (dgvMyRentedComics.IsHandleCreated && this.InvokeRequired)
                {
                    this.Invoke(updateDataSource);
                }
                else if (dgvMyRentedComics.IsHandleCreated)
                {
                    updateDataSource();
                }
                _logger?.Log($"LoadMyRentedComics: Successfully loaded {myRentedComics.Count} rented comics for member ID {currentMemberId}.");
            }
            catch (Exception ex)
            {
                _logger?.LogError("LoadMyRentedComics: Error loading member's rented comics.", ex);
                MessageBox.Show($"載入您的租借漫畫列表時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dgvMyRentedComics.IsHandleCreated && this.InvokeRequired) { this.Invoke(new Action(() => dgvMyRentedComics.DataSource = null)); }
                else if (dgvMyRentedComics.IsHandleCreated) { dgvMyRentedComics.DataSource = null; }
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
            // Applied null-forgiving operator to _logger as it's expected to be non-null here.
            ComicManagementForm comicMgmtForm = new ComicManagementForm(this._logger!, this._comicService);
            comicMgmtForm.ShowDialog(this);
        }

        private void 會員管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("Opening MemberManagementForm.");
            // Services are guaranteed non-null by constructor
            // Also ensure Program.AppAuthService is available and not null
            if (Program.AppAuthService != null)
            {
                // Applied null-forgiving operator to _logger as it's expected to be non-null here.
                MemberManagementForm memberMgmtForm = new MemberManagementForm(this._logger!, this._memberService, Program.AppAuthService);
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
            // Services are guaranteed non-null by constructor here
            // No need for explicit null check of this._logger, this._comicService, this._memberService, this._reloadService
            // as they are checked in the constructor.
            try
            {
                // Applied null-forgiving operator to _logger as it's expected to be non-null here.
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

            if (dgvAvailableComics.SelectedRows.Count == 0)
            {
                MessageBox.Show("請先選擇一本漫畫。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Comic? selectedComic = dgvAvailableComics.SelectedRows[0].DataBoundItem as Comic;
            if (selectedComic == null)
            {
                MessageBox.Show("選擇的項目無效或不是有效的漫畫資料。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger?.LogError("btnRentComic_Click: Selected item is null or not a Comic object.");
                return;
            }

            if (selectedComic.IsRented) // Should not happen if dgvAvailableComics is up-to-date
            {
                MessageBox.Show($"漫畫 '{selectedComic.Title}' 已經被借出。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAvailableComics(); // Refresh list in case of stale data
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

                    // Assumption: _memberService.GetMemberByUsername(string username) exists or will be implemented.
                    // This part might need adjustment based on the actual MemberService capabilities.
                    Member? member = null;
                    try
                    {
                        // For now, we'll assume GetMemberByUsername exists and works as expected.
                        // If MemberService is not yet updated, this line will cause a compile error
                        // or runtime issue if the method is not found/implemented.
                        member = _memberService.GetMemberByUsername(_currentUser.Username);
                    }
                    catch (NotImplementedException nie)
                    {
                         _logger?.LogError($"btnRentComic_Click: _memberService.GetMemberByUsername is not implemented. {nie.Message}");
                         MessageBox.Show("無法驗證會員身份，租借功能暫時無法使用 (開發者提示: GetMemberByUsername 未實現)。", "功能錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                         return;
                    }
                    catch (Exception ex) // Catch other potential exceptions from member service
                    {
                        _logger?.LogError($"btnRentComic_Click: Error calling _memberService.GetMemberByUsername. {ex.Message}", ex);
                        MessageBox.Show("驗證會員身份時發生錯誤。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                    if (member == null)
                    {
                        _logger?.LogWarning($"btnRentComic_Click: No member found for username '{_currentUser.Username}'. Cannot rent comic.");
                        MessageBox.Show($"找不到使用者 '{_currentUser.Username}' 對應的會員資料。請確認會員資料是否存在。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    selectedComic.IsRented = true;
                    selectedComic.RentedToMemberId = member.Id;
                    selectedComic.RentalDate = DateTime.Now; // Record current date and time of rental
                    selectedComic.ReturnDate = selectedReturnDate;

                    try
                    {
                        _comicService.UpdateComic(selectedComic);
                        _logger?.Log($"Comic '{selectedComic.Title}' (ID: {selectedComic.Id}) rented to member ID {member.Id} (Username: {_currentUser.Username}) until {selectedReturnDate:yyyy-MM-dd}.");
                        MessageBox.Show($"漫畫 '{selectedComic.Title}' 已成功租借至 {selectedReturnDate:yyyy-MM-dd}。", "租借成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAvailableComics(); // Refresh the list of available comics
                        LoadMyRentedComics();  // Refresh the list of member's rented comics
                        dgvAvailableComics_SelectionChanged(null, EventArgs.Empty); // Update button state for btnRentComic
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError($"btnRentComic_Click: Failed to update comic (ID: {selectedComic.Id}) after rental attempt. {ex.Message}", ex);
                        MessageBox.Show($"更新漫畫狀態時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // Potentially revert comic object's state if persistence failed, though this might be complex.
                        // For now, just log and inform user. The comic might be in an inconsistent state in memory vs. file.
                        selectedComic.IsRented = false; // Try to revert in-memory state
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
            else
            {
                this._logger?.Log($"User '{_currentUser.Username}' (Role: {_currentUser.Role}) attempted to view logs. Permission denied.");
                MessageBox.Show("權限不足", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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