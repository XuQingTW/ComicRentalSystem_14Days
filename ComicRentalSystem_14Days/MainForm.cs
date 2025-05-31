using ComicRentalSystem_14Days.Models; // Added for AdminComicStatusViewModel
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Forms;
using ComicRentalSystem_14Days.Interfaces;
using System;
using System.Collections.Generic; // Added for List
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.ComponentModel;
using ComicRentalSystem_14Days.Controls; // Added for AdminDashboardUserControl

namespace ComicRentalSystem_14Days
{
    public partial class MainForm : BaseForm
    {
        private readonly User _currentUser;
        private readonly ComicService _comicService;
        private readonly MemberService _memberService;
        private readonly IReloadService _reloadService;
        private readonly ILogger _logger;

        private List<AdminComicStatusViewModel>? _allAdminComicStatuses;
        private string _currentSortColumnName = string.Empty;
        private ListSortDirection _currentSortDirection = ListSortDirection.Ascending;

        private Button? _currentSelectedNavButton; // Added for new UI
        private AdminDashboardUserControl? _adminDashboardControl; // Added for Admin Dashboard UC

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

        private async void MainForm_Load(object sender, EventArgs e)
        {
            this._logger?.Log("MainForm is loading.");
            // Style MenuStrip and StatusStrip using ModernBaseForm
            if (this.menuStrip2 != null)
            {
                this.menuStrip2.BackColor = ModernBaseForm.SecondaryColor;
                this.menuStrip2.ForeColor = ModernBaseForm.TextColor;
                this.menuStrip2.Font = ModernBaseForm.PrimaryFontBold ?? new Font("Segoe UI", 9F, FontStyle.Bold);
                foreach (ToolStripMenuItem item in this.menuStrip2.Items.OfType<ToolStripMenuItem>())
                {
                    item.Font = ModernBaseForm.PrimaryFontBold ?? new Font("Segoe UI", 9F, FontStyle.Bold);
                    item.ForeColor = ModernBaseForm.TextColor;
                }
            }
            if (this.statusStrip1 != null)
            {
                this.statusStrip1.BackColor = ModernBaseForm.SecondaryColor;
                this.statusStrip1.Font = ModernBaseForm.PrimaryFont ?? new Font("Segoe UI", 9F);
                if (this.toolStripStatusLabelUser != null) this.toolStripStatusLabelUser.ForeColor = ModernBaseForm.TextColor;
            }

            // Attach Event Handlers and Style Nav Buttons
            if (this.leftNavPanel != null)
            {
                // Ensure direct field references for buttons are used
                Button[] navButtons = { this.btnNavDashboard, this.btnNavComicMgmt, this.btnNavMemberMgmt, this.btnNavRentalMgmt, this.btnNavUserReg, this.btnNavLogs };
                foreach (Button navBtn in navButtons)
                {
                    if (navBtn != null) // Ensure button exists from Designer.cs
                    {
                        // Initial styling (can be refined in SelectNavButton)
                        navBtn.BackColor = ModernBaseForm.SecondaryColor;
                        navBtn.ForeColor = ModernBaseForm.TextColor;
                        navBtn.Font = ModernBaseForm.ButtonFont ?? new Font("Segoe UI Semibold", 9.75F);

                        // Assign click handlers
                        if (navBtn == this.btnNavDashboard) navBtn.Click += btnNavDashboard_Click;
                        else if (navBtn == this.btnNavComicMgmt) navBtn.Click += btnNavComicMgmt_Click;
                        else if (navBtn == this.btnNavMemberMgmt) navBtn.Click += btnNavMemberMgmt_Click;
                        else if (navBtn == this.btnNavRentalMgmt) navBtn.Click += btnNavRentalMgmt_Click;
                        else if (navBtn == this.btnNavUserReg) navBtn.Click += btnNavUserReg_Click;
                        else if (navBtn == this.btnNavLogs) navBtn.Click += btnNavLogs_Click;
                    }
                }
            }

            SetupDataGridView(); // Original call

            if (_currentUser.Role == UserRole.Admin)
            {
                await LoadAllComicsStatusForAdminAsync();
                if(this.dgvAvailableComics != null) // Null check
                   this.dgvAvailableComics.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvAvailableComics_ColumnHeaderMouseClick);
            }
            else
            {
                LoadAvailableComics();
                LoadMyRentedComics();
            }

            if (this._comicService != null) // Null check
                this._comicService.ComicsChanged += ComicService_ComicsChanged;

            if (this.dgvAvailableComics != null) // Null check
            {
                this.dgvAvailableComics.SelectionChanged += dgvAvailableComics_SelectionChanged;
                dgvAvailableComics_SelectionChanged(this, EventArgs.Empty);
            }

            // Initialize ComboBox for Admin filter using direct field reference
            if (this.cmbAdminComicFilterStatus != null) // This is part of Comic Management view
            {
                this.cmbAdminComicFilterStatus.Items.Clear();
                this.cmbAdminComicFilterStatus.Items.Add("All");
                this.cmbAdminComicFilterStatus.Items.Add("Rented");
                this.cmbAdminComicFilterStatus.Items.Add("Available");
                this.cmbAdminComicFilterStatus.SelectedItem = "All";
                this.cmbAdminComicFilterStatus.SelectedIndexChanged += new System.EventHandler(this.cmbAdminComicFilterStatus_SelectedIndexChanged);
            }

            // Initialize AdminDashboardUserControl for Admin users
            if (_currentUser.Role == UserRole.Admin && _comicService != null && _memberService != null && _logger != null) // Ensure services are available
            {
                _adminDashboardControl = new AdminDashboardUserControl(_comicService, _memberService, _logger); // Pass services
                _adminDashboardControl.Dock = DockStyle.Fill;
                _adminDashboardControl.Visible = false;
                if (mainContentPanel != null)
                {
                     mainContentPanel.Controls.Add(_adminDashboardControl);
                }
                else
                {
                    _logger.LogError("MainForm_Load: mainContentPanel is null. Cannot add AdminDashboardUserControl.");
                }
            }

            // Style controls now inside TabPages for Member view
            if (_currentUser.Role == UserRole.Member)
            {
                if (btnRentComic != null) StyleModernButton(btnRentComic); // Method from ModernBaseForm
                if (dgvAvailableComics != null) StyleModernDataGridView(dgvAvailableComics); // Method from ModernBaseForm
                if (dgvMyRentedComics != null) StyleModernDataGridView(dgvMyRentedComics); // Method from ModernBaseForm
            }
            // Style TabControl
            if (memberViewTabControl != null)
            {
                // memberViewTabControl.Appearance = TabAppearance.FlatButtons; // Optional
                // memberViewTabControl.ItemSize = new Size(100, 28); // Optional
                foreach(TabPage page in memberViewTabControl.TabPages)
                {
                    page.BackColor = ModernBaseForm.SecondaryColor;
                }
            }

            // Placeholder text logic for txtSearchAvailableComics
            if (txtSearchAvailableComics != null)
            {
                txtSearchAvailableComics.GotFocus += (s, ev) => { if (txtSearchAvailableComics.Text == "Search by Title/Author...") { txtSearchAvailableComics.Text = ""; txtSearchAvailableComics.ForeColor = ModernBaseForm.TextColor; } };
                txtSearchAvailableComics.LostFocus += (s, ev) => { if (string.IsNullOrWhiteSpace(txtSearchAvailableComics.Text)) { txtSearchAvailableComics.Text = "Search by Title/Author..."; txtSearchAvailableComics.ForeColor = System.Drawing.Color.Gray; } };
                // Styling
                txtSearchAvailableComics.Font = ModernBaseForm.PrimaryFont ?? new System.Drawing.Font("Segoe UI", 9F);
                txtSearchAvailableComics.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }

            // Populate cmbGenreFilter
            if (cmbGenreFilter != null && _comicService != null)
            {
                cmbGenreFilter.Items.Clear();
                cmbGenreFilter.Items.Add("All Genres");
                try
                {
                    var genres = _comicService.GetAllComics().Select(c => c.Genre).Where(g => !string.IsNullOrWhiteSpace(g)).Distinct().OrderBy(g => g);
                    foreach (var genre in genres) { cmbGenreFilter.Items.Add(genre); }
                }
                catch (Exception ex)
                {
                    _logger?.LogError("Failed to populate genre filter.", ex);
                }
                cmbGenreFilter.SelectedIndex = 0;
                // Styling
                cmbGenreFilter.Font = ModernBaseForm.PrimaryFont ?? new System.Drawing.Font("Segoe UI", 9F);
            }

            // Attach Event Handlers for filters
            if (txtSearchAvailableComics != null)
                txtSearchAvailableComics.TextChanged += (s, ev) => { if (IsMemberViewActive()) ApplyAvailableComicsFilter(); };
            if (cmbGenreFilter != null)
                cmbGenreFilter.SelectedIndexChanged += (s, ev) => { if (IsMemberViewActive()) ApplyAvailableComicsFilter(); };

            // Attach event handler for dgvMyRentedComics CellFormatting (Member's rented comics)
            if (dgvMyRentedComics != null)
            {
                dgvMyRentedComics.CellFormatting -= dgvMyRentedComics_CellFormatting;
                dgvMyRentedComics.CellFormatting += dgvMyRentedComics_CellFormatting;
            }

            // Attach event handler for dgvAvailableComics CellFormatting (Admin's view of all comics)
            if (dgvAvailableComics != null && _currentUser.Role == UserRole.Admin)
            {
                dgvAvailableComics.CellFormatting -= dgvAvailableComics_AdminView_CellFormatting;
                dgvAvailableComics.CellFormatting += dgvAvailableComics_AdminView_CellFormatting;
            }

            // Attach event handler for TabControl SelectedIndexChanged
            if (memberViewTabControl != null)
            {
                memberViewTabControl.SelectedIndexChanged -= memberViewTabControl_SelectedIndexChanged;
                memberViewTabControl.SelectedIndexChanged += memberViewTabControl_SelectedIndexChanged;
            }

            // Style cmbAdminComicFilterStatus
            if (cmbAdminComicFilterStatus != null)
            {
                cmbAdminComicFilterStatus.Font = ModernBaseForm.PrimaryFont ?? new Font("Segoe UI", 9F);
                // Optional: cmbAdminComicFilterStatus.FlatStyle = FlatStyle.Flat; (might not look good on all OS)
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

        private async void ComicService_ComicsChanged(object? sender, EventArgs e)
        {
            this._logger?.Log("ComicsChanged event received.");
            if (_currentUser.Role == UserRole.Admin)
            {
                this._logger?.Log("Reloading all comics status for admin.");
                await LoadAllComicsStatusForAdminAsync();
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
                // Adjusted FillWeights for admin view
                dgvAvailableComics!.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "Title", FillWeight = 20 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Author", HeaderText = "Author", FillWeight = 15 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status", FillWeight = 10 }); // For "被借閱", "在館中"
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "BorrowerName", HeaderText = "Borrower", FillWeight = 15 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "BorrowerPhoneNumber", HeaderText = "Borrower Phone", FillWeight = 15 });

                var rentalDateColumn = new DataGridViewTextBoxColumn {
                    DataPropertyName = "RentalDate",
                    HeaderText = "Rented On", // English header
                    FillWeight = 12,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" }
                };
                dgvAvailableComics.Columns.Add(rentalDateColumn);

                var returnDateColumn = new DataGridViewTextBoxColumn {
                    DataPropertyName = "ReturnDate",
                    HeaderText = "Due Date", // English header
                    FillWeight = 13,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" }
                };
                dgvAvailableComics.Columns.Add(returnDateColumn);
                StyleModernDataGridView(dgvAvailableComics); // Apply styling after columns are set for Admin
            }
            else // Member view
            {
                _logger.Log("Configuring DataGridView for Member view (Available Comics).");
                dgvAvailableComics!.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "書名", FillWeight = 40 });
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
                var queryResult = this._comicService.GetAllComics().Where(c => !c.IsRented).ToList();
                var availableComics = queryResult ?? new List<Comic>(); // Ensure availableComics is not null

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

        private async Task LoadAllComicsStatusForAdminAsync()
        {
            if (dgvAvailableComics == null || _memberService == null || _comicService == null) return;
            _logger?.Log("Loading all comics status for Admin view asynchronously.");

            try
            {
                List<Member> allMembers = await Task.Run(() => _memberService.GetAllMembers());
                List<AdminComicStatusViewModel> comicStatuses = await Task.Run(() => _comicService.GetAdminComicStatusViewModels(allMembers));

                _allAdminComicStatuses = new List<AdminComicStatusViewModel>(comicStatuses);
                ApplyAdminComicsView(); // Apply current filter/sort to the newly loaded data

                _logger?.Log($"Successfully loaded {comicStatuses.Count} comics for admin view asynchronously and stored in _allAdminComicStatuses.");
            }
            catch (Exception ex)
            {
                LogErrorActivity("Error loading all comics status for admin asynchronously.", ex);
                Action showError = () => MessageBox.Show($"載入所有漫畫狀態時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (this.IsHandleCreated && !this.IsDisposed) // Check before invoking MessageBox too
                {
                    if (this.InvokeRequired) { this.Invoke(showError); } else { showError(); }
                }
            }
        }

        private void SetupUIAccessControls()
        {
            bool isAdmin = _currentUser.Role == UserRole.Admin;
            _logger.Log($"Setting up UI controls. User is Admin: {isAdmin}");

            if (mainContentPanel == null) { _logger?.LogError("SetupUIAccessControls: mainContentPanel is null."); return; }

            // Hide all major view containers/specific controls first
            if (leftNavPanel != null) leftNavPanel.Visible = false;
            if (_adminDashboardControl != null) _adminDashboardControl.Visible = false;
            if (memberViewTabControl != null) memberViewTabControl.Visible = false; // Hide new TabControl

            // Hide admin-specific controls that might be directly on mainContentPanel
            if (cmbAdminComicFilterStatus != null) cmbAdminComicFilterStatus.Visible = false;
            // dgvAvailableComics & lblAvailableComics are now managed by TabControl or Admin's ComicMgmt view

            // Menu strip items visibility (already here, good)
            if (this.menuStrip2 != null)
            {
                var managementMenuItem = this.menuStrip2.Items.OfType<ToolStripMenuItem>().FirstOrDefault(item => item.Name == "管理ToolStripMenuItem");
                if (managementMenuItem != null) managementMenuItem.Visible = isAdmin;

                var toolsMenuItem = this.menuStrip2.Items.OfType<ToolStripMenuItem>().FirstOrDefault(item => item.Name == "工具ToolStripMenuItem");
                if (toolsMenuItem != null) toolsMenuItem.Visible = isAdmin;

                var userRegItem = this.menuStrip2.Items.OfType<ToolStripMenuItem>().FirstOrDefault(item => item.Name == "使用者註冊ToolStripMenuItem");
                if (userRegItem != null) userRegItem.Visible = isAdmin;
            }
            else { _logger.LogWarning("MenuStrip control 'menuStrip2' (field) not found or null."); }

            if (isAdmin)
            {
                if (leftNavPanel != null) leftNavPanel.Visible = true;
                this.Text = "Comic Rental System - Admin"; // Set Form title for Admin
                if (btnNavDashboard != null)
                {
                    SelectNavButton(btnNavDashboard);
                }
                else if (btnNavComicMgmt != null) // Fallback
                {
                     SelectNavButton(btnNavComicMgmt);
                }
            }
            else // Member
            {
                this.Text = "Comic Rental System"; // Set Form title for Member
                if (memberViewTabControl != null)
                {
                    memberViewTabControl.Visible = true;
                    // Update text for labels inside tabs for clarity
                    if(lblAvailableComics != null) lblAvailableComics.Text = "Select a comic below to rent:";
                    if(lblMyRentedComicsHeader != null) lblMyRentedComicsHeader.Text = "Your currently rented items:";
                }
                else
                {
                    _logger?.LogError("SetupUIAccessControls: memberViewTabControl is null for Member view. Showing fallback direct controls.");
                    if(lblAvailableComics != null) { lblAvailableComics.Visible = true; lblAvailableComics.Text = "Available Comics"; }
                    if(dgvAvailableComics != null) dgvAvailableComics.Visible = true;
                    if(btnRentComic != null) btnRentComic.Visible = true;
                    if(lblMyRentedComicsHeader != null) lblMyRentedComicsHeader.Visible = true;
                    if(dgvMyRentedComics != null) dgvMyRentedComics.Visible = true;
                }
                // Initial call to load data for the initially selected tab
                if (memberViewTabControl != null && memberViewTabControl.TabPages.Count > 0) // Ensure TabPages exist
                {
                    if (memberViewTabControl.SelectedTab == availableComicsTabPage)
                    {
                        ApplyAvailableComicsFilter();
                    }
                    else if (memberViewTabControl.SelectedTab == myRentalsTabPage)
                    {
                        LoadMyRentedComics();
                    }
                }
            }
            _logger.Log($"UI controls visibility and text updated based on admin status ({isAdmin}).");
        }

        private void UpdateStatusBar()
        {
            // Use direct field reference for statusStrip1
            if (this.statusStrip1 != null)
            {
                // Use direct field reference for toolStripStatusLabelUser
                if (this.toolStripStatusLabelUser != null)
                {
                    this.toolStripStatusLabelUser.Text = $"使用者: {_currentUser.Username} | 角色: {_currentUser.Role}";
                    this._logger.Log($"Status bar updated: User: {_currentUser.Username}, Role: {_currentUser.Role}");
                }
                else { this._logger.LogWarning("ToolStripStatusLabel 'toolStripStatusLabelUser' (field) not found or null."); }
            }
            else { this._logger.LogWarning("StatusStrip control 'statusStrip1' (field) not found or null."); }
        }

        private void SetupMyRentedComicsDataGridView()
        {
            if (dgvMyRentedComics == null) return;
            _logger?.Log("Setting up DataGridView for member's rented comics (dgvMyRentedComics).");
            dgvMyRentedComics.AutoGenerateColumns = false;
            dgvMyRentedComics.Columns.Clear(); // Clear existing columns before adding new ones
            dgvMyRentedComics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvMyRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "書名", FillWeight = 30 });
            dgvMyRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Author", HeaderText = "作者", FillWeight = 20 });

            var rentalDateColumn = new DataGridViewTextBoxColumn { DataPropertyName = "RentalDate", HeaderText = "租借日期", FillWeight = 18 };
            if (rentalDateColumn.DefaultCellStyle == null)
            {
                rentalDateColumn.DefaultCellStyle = new DataGridViewCellStyle();
            }
            rentalDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvMyRentedComics.Columns.Add(rentalDateColumn);

            var returnDateColumn = new DataGridViewTextBoxColumn { DataPropertyName = "ReturnDate", HeaderText = "歸還日期", FillWeight = 18 };
            if (returnDateColumn.DefaultCellStyle == null)
            {
                returnDateColumn.DefaultCellStyle = new DataGridViewCellStyle();
            }
            returnDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvMyRentedComics.Columns.Add(returnDateColumn);

            // Add Status column if it doesn't exist
            if (!dgvMyRentedComics.Columns.Contains("statusColumn"))
            {
                var statusColumn = new DataGridViewTextBoxColumn
                {
                    Name = "statusColumn",
                    HeaderText = "Status",
                    FillWeight = 14 // Adjusted FillWeight to make space
                };
                dgvMyRentedComics.Columns.Add(statusColumn);
            }

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
            if (Program.AppAuthService != null && this._comicService != null)
            {
                MemberManagementForm memberMgmtForm = new MemberManagementForm(this._logger!, this._memberService, Program.AppAuthService, this._comicService, this._currentUser);
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

        private void ApplyAdminComicsView()
        {
            if (_allAdminComicStatuses == null || this.dgvAvailableComics == null) return;
            if (!this.dgvAvailableComics.IsHandleCreated || this.dgvAvailableComics.IsDisposed) return;

            IEnumerable<AdminComicStatusViewModel> viewToShow = _allAdminComicStatuses;

            // 1. Apply Filter (using field this.cmbAdminComicFilterStatus)
            if (this.cmbAdminComicFilterStatus != null && this.cmbAdminComicFilterStatus.SelectedItem != null)
            {
                string? selectedStatus = this.cmbAdminComicFilterStatus.SelectedItem.ToString();
                if (selectedStatus == "Rented") // Assuming "被借閱" is represented as "Rented" in ComboBox
                {
                    viewToShow = viewToShow.Where(vm => vm.Status == "被借閱");
                }
                else if (selectedStatus == "Available") // Assuming "在館中" is represented as "Available" in ComboBox
                {
                    viewToShow = viewToShow.Where(vm => vm.Status == "在館中");
                }
                // "All" implies no status filter, so viewToShow remains as is.
            }

            // 2. Apply Sort
            if (!string.IsNullOrEmpty(_currentSortColumnName))
            {
                var prop = typeof(AdminComicStatusViewModel).GetProperty(_currentSortColumnName);
                if (prop != null)
                {
                    if (_currentSortDirection == ListSortDirection.Ascending)
                    {
                        viewToShow = viewToShow.OrderBy(vm => prop.GetValue(vm, null));
                    }
                    else
                    {
                        viewToShow = viewToShow.OrderByDescending(vm => prop.GetValue(vm, null));
                    }
                }
            }

            var finalViewList = viewToShow.ToList();

            Action updateGridAction = () => {
                dgvAvailableComics.DataSource = null;
                dgvAvailableComics.DataSource = finalViewList ?? new List<AdminComicStatusViewModel>();
                // Optional: Update sort glyphs on column headers
                foreach (DataGridViewColumn column in dgvAvailableComics.Columns)
                {
                    column.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
                if (!string.IsNullOrEmpty(_currentSortColumnName) && dgvAvailableComics.Columns.Contains(_currentSortColumnName))
                {
                     dgvAvailableComics.Columns[_currentSortColumnName]!.HeaderCell.SortGlyphDirection =
                        _currentSortDirection == ListSortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending;
                }
            };

            if (this.dgvAvailableComics.InvokeRequired) { this.dgvAvailableComics.Invoke(updateGridAction); }
            else { updateGridAction(); }
        }

        private void dgvAvailableComics_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (_currentUser == null || _currentUser.Role != UserRole.Admin) return;
            if (this.dgvAvailableComics == null || e.ColumnIndex < 0 || e.ColumnIndex >= this.dgvAvailableComics.Columns.Count) return;
            if (this.dgvAvailableComics.Columns[e.ColumnIndex].DataPropertyName == null) return;

            string newSortColumnName = this.dgvAvailableComics.Columns[e.ColumnIndex].DataPropertyName;

            if (string.IsNullOrEmpty(newSortColumnName)) return;

            if (_currentSortColumnName == newSortColumnName)
            {
                _currentSortDirection = (_currentSortDirection == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            else
            {
                _currentSortColumnName = newSortColumnName;
                _currentSortDirection = ListSortDirection.Ascending;
            }
            ApplyAdminComicsView();
        }

        private void cmbAdminComicFilterStatus_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_currentUser == null || _currentUser.Role != UserRole.Admin) return;
            ApplyAdminComicsView();
        }

        // --- New Navigation Methods ---
        private void SelectNavButton(Button selectedButton)
        {
            if (_currentSelectedNavButton != null)
            {
                _currentSelectedNavButton.BackColor = ModernBaseForm.SecondaryColor;
                _currentSelectedNavButton.ForeColor = ModernBaseForm.TextColor;
                _currentSelectedNavButton.Font = ModernBaseForm.ButtonFont ?? new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            }
            selectedButton.BackColor = ModernBaseForm.PrimaryColor;
            selectedButton.ForeColor = Color.White;
            // Ensure ModernBaseForm.ButtonFont is not null before creating a new Font based on it
            Font baseFont = ModernBaseForm.ButtonFont ?? new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            selectedButton.Font = new Font(baseFont, FontStyle.Bold);
            _currentSelectedNavButton = selectedButton;

            if (mainContentPanel == null) return;
            // Hide all major UserControls/TabControls in mainContentPanel first
            if (_adminDashboardControl != null) _adminDashboardControl.Visible = false;
            if (memberViewTabControl != null) memberViewTabControl.Visible = false; // Hide member tab control for admin views
            // Example for future UserControls: if (_adminComicManagementControl != null) _adminComicManagementControl.Visible = false;

            // Hide direct controls that might be used by other views (like Comic Management's own dgv/labels if not part of a UC)
            // This is important if admin views use some of the same named controls as member views but not through the tab control.
            // However, the current setup for admin's Comic Management reuses lblAvailableComics, cmbAdminComicFilterStatus, dgvAvailableComics.
            // So, we only hide them if they are NOT part of the view being selected.

            // Show the selected UserControl or view's specific controls
            if (selectedButton == btnNavDashboard)
            {
                // Hide comic management specific controls if they were visible
                if (lblAvailableComics != null) lblAvailableComics.Visible = false;
                if (cmbAdminComicFilterStatus != null) cmbAdminComicFilterStatus.Visible = false;
                if (dgvAvailableComics != null) dgvAvailableComics.Visible = false;

                if (_adminDashboardControl != null)
                {
                    _adminDashboardControl.Visible = true;
                    _adminDashboardControl.LoadDashboardData();
                    this.Text = "Comic Rental System - Dashboard";
                }
                 _logger?.Log("Dashboard view selected.");
            }
            else if (selectedButton == btnNavComicMgmt)
            {
                if(lblAvailableComics != null) { lblAvailableComics.Text="所有漫畫狀態"; lblAvailableComics.Visible = true;} // Re-show and set text
                if(cmbAdminComicFilterStatus != null) cmbAdminComicFilterStatus.Visible = true; // Re-show
                if(dgvAvailableComics != null) dgvAvailableComics.Visible = true; // Re-show
                this.Text = "Comic Rental System - Comic Management";
                 _logger?.Log("Comic Management view selected.");
            }
            else if (selectedButton == btnNavMemberMgmt)
            {
                // Show Member Management related controls
                // _logger?.Log("Member Management view selected."); // Original logging
                this.會員管理ToolStripMenuItem_Click(this, EventArgs.Empty);
            }
            else if (selectedButton == btnNavRentalMgmt)
            {
                // Show Rental Management related controls
                // _logger?.Log("Rental Management view selected."); // Original logging
                this.rentalManagementToolStripMenuItem_Click(this, EventArgs.Empty);
            }
            else if (selectedButton == btnNavUserReg)
            {
                // Show User Registration related controls
                // _logger?.Log("User Registration view selected."); // Original logging
                this.使用者註冊ToolStripMenuItem_Click(this, EventArgs.Empty);
            }
            else if (selectedButton == btnNavLogs)
            {
                // Show Logs related controls
                // _logger?.Log("Logs view selected."); // Original logging
                this.檢視日誌ToolStripMenuItem_Click(this, EventArgs.Empty);
            }
        }

        private void btnNavDashboard_Click(object? sender, EventArgs e) { if (sender is Button cb) SelectNavButton(cb); _logger?.Log("Dashboard navigation button clicked."); }
        private void btnNavComicMgmt_Click(object? sender, EventArgs e) { if (sender is Button cb) SelectNavButton(cb); _logger?.Log("Comic Mgmt navigation button clicked."); }
        private void btnNavMemberMgmt_Click(object? sender, EventArgs e) { if (sender is Button cb) SelectNavButton(cb); _logger?.Log("Member Mgmt navigation button clicked."); }
        private void btnNavRentalMgmt_Click(object? sender, EventArgs e) { if (sender is Button cb) SelectNavButton(cb); _logger?.Log("Rental Mgmt navigation button clicked."); }
        private void btnNavUserReg_Click(object? sender, EventArgs e) { if (sender is Button cb) SelectNavButton(cb); _logger?.Log("User Reg navigation button clicked."); }
        private void btnNavLogs_Click(object? sender, EventArgs e) { if (sender is Button cb) SelectNavButton(cb); _logger?.Log("Logs navigation button clicked."); }

        // --- Admin dgvAvailableComics Cell Formatting ---
        private void dgvAvailableComics_AdminView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (_currentUser == null || _currentUser.Role != UserRole.Admin) return;
            if (dgvAvailableComics == null || e.RowIndex < 0 || e.RowIndex >= dgvAvailableComics.Rows.Count) return;

            DataGridViewRow row = dgvAvailableComics.Rows[e.RowIndex];
            if (row.DataBoundItem is not AdminComicStatusViewModel comicStatus) return;

            // Status column text color
            if (dgvAvailableComics.Columns[e.ColumnIndex].DataPropertyName == "Status")
            {
                if (e.Value?.ToString() == "被借閱") // Rented
                {
                    e.CellStyle.ForeColor = ModernBaseForm.DangerColor;
                }
                else if (e.Value?.ToString() == "在館中") // Available
                {
                    e.CellStyle.ForeColor = ModernBaseForm.SuccessColor;
                }
            }

            // ReturnDate cell background/foreground for overdue/due soon
            if (dgvAvailableComics.Columns[e.ColumnIndex].DataPropertyName == "ReturnDate" ||
                dgvAvailableComics.Columns[e.ColumnIndex].DataPropertyName == "Status") // Apply to status cell too for emphasis
            {
                if (comicStatus.Status == "被借閱" && comicStatus.ReturnDate.HasValue)
                {
                    DateTime returnDate = comicStatus.ReturnDate.Value;
                    if (returnDate.Date < DateTime.Today) // Overdue
                    {
                        // Apply to the whole row for overdue items for high visibility
                        row.DefaultCellStyle.BackColor = ModernBaseForm.DangerColor;
                        row.DefaultCellStyle.ForeColor = Color.White;
                        // Ensure selection style also contrasts if needed, though DGV usually handles this
                    }
                    else if (returnDate.Date <= DateTime.Today.AddDays(3)) // Due within 3 days
                    {
                        // Apply to the whole row for items due soon
                        row.DefaultCellStyle.BackColor = ModernBaseForm.AccentColor;
                        row.DefaultCellStyle.ForeColor = ModernBaseForm.TextColor;
                    }
                    else
                    {
                        // Reset to default if not overdue or due soon to handle reused rows
                        // This might conflict with alternating row styles. A more robust solution
                        // would be to set these properties only if they need to be non-default.
                        // For now, let's assume DGV handles resetting on scroll if not explicitly set here.
                        // Or, ensure that the default cell style is reapplied.
                        // e.CellStyle.BackColor = dgvAvailableComics.DefaultCellStyle.BackColor;
                        // e.CellStyle.ForeColor = dgvAvailableComics.DefaultCellStyle.ForeColor;
                    }
                }
            }
        }


        // --- My Rentals Tab Enhancements ---
        private void dgvMyRentedComics_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvMyRentedComics == null || e.RowIndex < 0 || e.RowIndex >= dgvMyRentedComics.Rows.Count)
                return;

            DataGridViewRow row = dgvMyRentedComics.Rows[e.RowIndex];
            if (row.DataBoundItem == null)
                return;

            DateTime? returnDate = null;
            try
            {
                // Using reflection to get ReturnDate from anonymous type
                var returnDateObj = row.DataBoundItem.GetType().GetProperty("ReturnDate")?.GetValue(row.DataBoundItem, null);
                if (returnDateObj != null && returnDateObj != DBNull.Value) // Check for DBNull if data comes from DB directly
                {
                    returnDate = Convert.ToDateTime(returnDateObj);
                }
            }
            catch(Exception ex)
            {
                _logger?.LogError($"Error getting ReturnDate via reflection in CellFormatting: {ex.Message} for item type {row.DataBoundItem.GetType().Name}");
                return;
            }

            if (returnDate == null) return;

            // Status column formatting
            if (dgvMyRentedComics.Columns[e.ColumnIndex].Name == "statusColumn")
            {
                TimeSpan remainingTime = returnDate.Value.Date - DateTime.Today; // Compare date parts only
                if (remainingTime.TotalDays < 0)
                {
                    e.Value = $"Overdue by {-remainingTime.TotalDays} day(s)";
                }
                else if (remainingTime.TotalDays == 0)
                {
                    e.Value = "Due Today";
                }
                else
                {
                    e.Value = $"{remainingTime.TotalDays} day(s) remaining";
                }
                e.FormattingApplied = true;
            }

            // Row style formatting based on due date (applied to all cells in the row)
            // Ensure this styling is applied after any default/alternating row styles for it to take precedence.
            // The CellFormatting event is suitable for this.
            if (returnDate.Value.Date < DateTime.Today) // Overdue
            {
                e.CellStyle.BackColor = ModernBaseForm.DangerColor;
                e.CellStyle.ForeColor = Color.White;
            }
            else if (returnDate.Value.Date <= DateTime.Today.AddDays(3)) // Due within 3 days (includes today)
            {
                e.CellStyle.BackColor = ModernBaseForm.AccentColor;
                // Ensure good contrast with AccentColor (often yellow)
                e.CellStyle.ForeColor = ModernBaseForm.TextColor; // Or Color.Black if TextColor is too light
            }
            // else default styling applies, no need to explicitly reset here unless default styles are complex
        }

        private void memberViewTabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (memberViewTabControl == null) return;

            if (memberViewTabControl.SelectedTab == myRentalsTabPage)
            {
                _logger?.Log("My Rentals tab selected. Reloading rented comics.");
                LoadMyRentedComics(); // Ensure this method is efficient
            }
            else if (memberViewTabControl.SelectedTab == availableComicsTabPage)
            {
                _logger?.Log("Available Comics tab selected. Re-applying filters.");
                ApplyAvailableComicsFilter();
            }
        }

        // --- Filter Methods ---
        private bool IsMemberViewActive()
        {
            return _currentUser != null &&
                   _currentUser.Role == UserRole.Member &&
                   memberViewTabControl != null &&
                   memberViewTabControl.Visible &&
                   memberViewTabControl.SelectedTab == availableComicsTabPage;
        }

        private void ApplyAvailableComicsFilter()
        {
            if (_comicService == null || dgvAvailableComics == null || _currentUser == null)
            {
                _logger?.LogWarning("ApplyAvailableComicsFilter: Critical components are null. Skipping filter application.");
                return;
            }

            // This filter is designed for the member's view of available comics.
            // If an admin is viewing dgvAvailableComics (e.g., in Comic Management), this filter might not be appropriate
            // or might need to operate on a different dataset (_allAdminComicStatuses).
            // The IsMemberViewActive() check in event handlers helps restrict when this is called.
            // However, LoadAvailableComics() also needs to be updated or this method needs to be smart
            // about which underlying list to filter if dgvAvailableComics is shared.
            // For now, we assume it filters the list of comics that are NOT rented.

            try
            {
                _logger?.Log("Applying available comics filter.");
                string searchText = (txtSearchAvailableComics != null && txtSearchAvailableComics.Text != "Search by Title/Author...")
                                    ? txtSearchAvailableComics.Text.ToLowerInvariant() : "";
                string selectedGenre = (cmbGenreFilter != null && cmbGenreFilter.SelectedItem != null && cmbGenreFilter.SelectedIndex > 0)
                                     ? cmbGenreFilter.SelectedItem.ToString()! : "All Genres";

                var comicsToFilter = _comicService.GetAllComics().Where(c => !c.IsRented).ToList();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    comicsToFilter = comicsToFilter.Where(c =>
                        (c.Title != null && c.Title.ToLowerInvariant().Contains(searchText)) ||
                        (c.Author != null && c.Author.ToLowerInvariant().Contains(searchText))
                    ).ToList();
                }

                if (selectedGenre != "All Genres")
                {
                    comicsToFilter = comicsToFilter.Where(c => c.Genre == selectedGenre).ToList();
                }

                dgvAvailableComics.DataSource = comicsToFilter;
                _logger?.Log($"Filter applied. Search: '{searchText}', Genre: '{selectedGenre}'. Found {comicsToFilter.Count} comics.");
            }
            catch (Exception ex)
            {
                _logger?.LogError("Error applying available comics filter.", ex);
                if (dgvAvailableComics != null) dgvAvailableComics.DataSource = null; // Clear grid on error
            }
        }
    }
}