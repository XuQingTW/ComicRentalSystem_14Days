using System.Drawing; 
using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Forms;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.ComponentModel;
using ComicRentalSystem_14Days.Controls;

namespace ComicRentalSystem_14Days
{
    public partial class MainForm : BaseForm
    {
        private readonly User _currentUser;
        private readonly IComicService _comicService;
        private readonly MemberService _memberService;
        private readonly IReloadService _reloadService;
        private readonly ILogger _logger;

        private List<AdminComicStatusViewModel>? _allAdminComicStatuses;
        private string _currentSortColumnName = string.Empty;
        private ListSortDirection _currentSortDirection = ListSortDirection.Ascending;

        private Image _placeholderCoverImage = CreatePlaceholderCoverImage();

        private Button? _currentSelectedNavButton;
        private AdminDashboardUserControl? _adminDashboardControl;

        public MainForm() : base()
        {
            InitializeComponent();
            _currentUser = null!;
            _comicService = null!;
            _memberService = null!;
            _reloadService = null!;
            _logger = null!;
        }

        public MainForm(
            ILogger logger,
            IComicService comicService,
            MemberService memberService,
            IReloadService reloadService,
            User currentUser
        ) : base()
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            this._memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            this._reloadService = reloadService ?? throw new ArgumentNullException(nameof(reloadService));
            this._currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

            base.SetLogger(logger);
            InitializeComponent();

            UpdateStatusBar();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            this._logger?.Log("主表單正在載入。");

            if (this.menuStrip2 != null)
            {
                this.menuStrip2.BackColor = ModernBaseForm.SecondaryColor;
                this.menuStrip2.ForeColor = ModernBaseForm.TextColor;
                this.menuStrip2.Font = ModernBaseForm.PrimaryFontBold ?? new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                foreach (ToolStripMenuItem item in this.menuStrip2.Items.OfType<ToolStripMenuItem>())
                {
                    item.Font = ModernBaseForm.PrimaryFontBold ?? new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                    item.ForeColor = ModernBaseForm.TextColor;
                }
            }
            if (this.statusStrip1 != null)
            {
                this.statusStrip1.BackColor = ModernBaseForm.SecondaryColor;
                this.statusStrip1.Font = ModernBaseForm.PrimaryFont ?? new System.Drawing.Font("Segoe UI", 9F);
                if (this.toolStripStatusLabelUser != null)
                    this.toolStripStatusLabelUser.ForeColor = ModernBaseForm.TextColor;
            }

            if (_currentUser.Role == UserRole.Admin && _comicService != null && _memberService != null && _logger != null)
            {
                mainContentPanel.Controls.Clear();

                _adminDashboardControl = new AdminDashboardUserControl(_comicService, _memberService, _logger)
                {
                    Dock = DockStyle.Fill,
                    Visible = false
                };
                mainContentPanel.Controls.Add(_adminDashboardControl);
            }

            SetupUIAccessControls();

            if (btnNavDashboard != null) btnNavDashboard.Click += btnNavDashboard_Click;
            if (btnNavComicMgmt != null) btnNavComicMgmt.Click += btnNavComicMgmt_Click;
            if (btnNavMemberMgmt != null) btnNavMemberMgmt.Click += btnNavMemberMgmt_Click;
            if (btnNavRentalMgmt != null) btnNavRentalMgmt.Click += btnNavRentalMgmt_Click;
            if (btnNavUserReg != null) btnNavUserReg.Click += btnNavUserReg_Click;
            if (btnNavLogs != null) btnNavLogs.Click += btnNavLogs_Click;

            SetupDataGridView();

            if (_currentUser.Role == UserRole.Admin)
            {
                await LoadAllComicsStatusForAdminAsync();
                if (this.dgvAvailableComics != null)
                    this.dgvAvailableComics.ColumnHeaderMouseClick += dgvAvailableComics_ColumnHeaderMouseClick;
            }
            else 
            {
                _logger?.Log("[TARGETED_RUNTIME] Applying visibility settings for availableComicsTabPage and dgvAvailableComics.");

                if (this.availableComicsTabPage != null)
                {
                    this.availableComicsTabPage.Visible = true;
                    _logger?.Log($"[TARGETED_RUNTIME] availableComicsTabPage.Visible = {this.availableComicsTabPage.Visible}");
                }
                else
                {
                    _logger?.LogWarning("[TARGETED_RUNTIME] availableComicsTabPage is NULL.");
                }

                if (this.dgvAvailableComics != null)
                {
                    this.dgvAvailableComics.Visible = true;
                    this.dgvAvailableComics.BringToFront();

                    _logger?.Log($"[TARGETED_RUNTIME] dgvAvailableComics.Visible = {this.dgvAvailableComics.Visible}");
                    _logger?.Log($"[TARGETED_RUNTIME] dgvAvailableComics.Parent = {this.dgvAvailableComics.Parent?.Name}");
                    _logger?.Log($"[TARGETED_RUNTIME] dgvAvailableComics.Location = {this.dgvAvailableComics.Location}");
                    _logger?.Log($"[TARGETED_RUNTIME] dgvAvailableComics.Size = {this.dgvAvailableComics.Size}");
                    _logger?.Log($"[TARGETED_RUNTIME] dgvAvailableComics.BackgroundColor = {this.dgvAvailableComics.BackgroundColor}");
                    _logger?.Log($"[TARGETED_RUNTIME] dgvAvailableComics.DefaultCellStyle.BackColor = {this.dgvAvailableComics.DefaultCellStyle.BackColor}");
                    _logger?.Log($"[TARGETED_RUNTIME] dgvAvailableComics.DefaultCellStyle.ForeColor = {this.dgvAvailableComics.DefaultCellStyle.ForeColor}");
                }
                else
                {
                    _logger?.LogWarning("[TARGETED_RUNTIME] dgvAvailableComics is NULL.");
                }
                await _comicService!.ReloadAsync();
                _logger!.Log($"MainForm_Load (Member) [After ReloadAsync]: _comicService.GetAllComics() reports {_comicService.GetAllComics().Count} comics.");
                LoadAvailableComics();
                LoadMyRentedComics();
            }

            if (this._comicService != null)
                this._comicService.ComicsChanged += ComicService_ComicsChanged;

            if (this.dgvAvailableComics != null)
            {
                this.dgvAvailableComics.SelectionChanged += dgvAvailableComics_SelectionChanged;
                dgvAvailableComics_SelectionChanged(this, EventArgs.Empty);
            }

            if (this.cmbAdminComicFilterStatus != null)
            {
                this.cmbAdminComicFilterStatus.Items.Clear();
                this.cmbAdminComicFilterStatus.Items.Add("全部");
                this.cmbAdminComicFilterStatus.Items.Add("已租借");
                this.cmbAdminComicFilterStatus.Items.Add("可租借");
                this.cmbAdminComicFilterStatus.SelectedItem = "全部";
                this.cmbAdminComicFilterStatus.SelectedIndexChanged += cmbAdminComicFilterStatus_SelectedIndexChanged;
            }

            if (_currentUser.Role == UserRole.Member)
            {
                if (dgvAvailableComics != null) StyleModernDataGridView(dgvAvailableComics);
                if (dgvMyRentedComics != null) StyleModernDataGridView(dgvMyRentedComics);
            }
            if (memberViewTabControl != null)
            {
                foreach (TabPage page in memberViewTabControl.TabPages)
                    page.BackColor = ModernBaseForm.SecondaryColor;
            }

            if (txtSearchAvailableComics != null)
            {
                txtSearchAvailableComics.GotFocus += (s, ev) =>
                {
                    if (txtSearchAvailableComics.Text == "依書名/作者搜尋...")
                    {
                        txtSearchAvailableComics.Text = "";
                        txtSearchAvailableComics.ForeColor = ModernBaseForm.TextColor;
                    }
                };
                txtSearchAvailableComics.LostFocus += (s, ev) =>
                {
                    if (string.IsNullOrWhiteSpace(txtSearchAvailableComics.Text))
                    {
                        txtSearchAvailableComics.Text = "依書名/作者搜尋...";
                        txtSearchAvailableComics.ForeColor = System.Drawing.Color.Gray;
                    }
                };
                txtSearchAvailableComics.Font = ModernBaseForm.PrimaryFont ?? new System.Drawing.Font("Segoe UI", 9F);
                txtSearchAvailableComics.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }

            if (cmbGenreFilter != null && _comicService != null)
            {
                cmbGenreFilter.Items.Clear();
                cmbGenreFilter.Items.Add("所有類型");
                try
                {
                    var genres = _comicService!.GetAllComics()
                        .Select(c => c.Genre)
                        .Where(g => !string.IsNullOrWhiteSpace(g))
                        .Distinct()
                        .OrderBy(g => g);
                    foreach (var genre in genres)
                        cmbGenreFilter.Items.Add(genre);
                }
                catch (Exception ex)
                {
                    _logger?.LogError("填入類型篩選器失敗。", ex);
                }
                cmbGenreFilter.SelectedIndex = 0;
                cmbGenreFilter.Font = ModernBaseForm.PrimaryFont ?? new System.Drawing.Font("Segoe UI", 9F);
            }

            if (txtSearchAvailableComics != null)
                txtSearchAvailableComics.TextChanged += (s, ev) =>
                {
                    if (IsMemberViewActive()) ApplyAvailableComicsFilter();
                };
            if (cmbGenreFilter != null)
                cmbGenreFilter.SelectedIndexChanged += (s, ev) =>
                {
                    if (IsMemberViewActive()) ApplyAvailableComicsFilter();
                };

            if (dgvMyRentedComics != null)
            {
                dgvMyRentedComics.CellFormatting -= dgvMyRentedComics_CellFormatting;
                dgvMyRentedComics.CellFormatting += dgvMyRentedComics_CellFormatting;
            }
            if (dgvAvailableComics != null)
            {
                dgvAvailableComics.CellFormatting -= dgvAvailableComics_AdminView_CellFormatting;
                dgvAvailableComics.CellFormatting -= dgvAvailableComics_MemberView_CellFormatting;
                if (_currentUser.Role == UserRole.Admin)
                {
                    dgvAvailableComics.CellFormatting += dgvAvailableComics_AdminView_CellFormatting;
                }
                else
                {
                    dgvAvailableComics.CellFormatting += dgvAvailableComics_MemberView_CellFormatting;
                }
            }
            if (memberViewTabControl != null)
            {
                memberViewTabControl.SelectedIndexChanged -= memberViewTabControl_SelectedIndexChanged;
                memberViewTabControl.SelectedIndexChanged += memberViewTabControl_SelectedIndexChanged;
            }
            if (cmbAdminComicFilterStatus != null)
            {
                cmbAdminComicFilterStatus.Font = ModernBaseForm.PrimaryFont ?? new System.Drawing.Font("Segoe UI", 9F);
            }
        }

        private void dgvAvailableComics_SelectionChanged(object? sender, EventArgs e)
        {
            if (_currentUser == null) return;

            bool isMember = _currentUser.Role == UserRole.Member;
            if (isMember)
            {
                // 移除租借功能後，按鈕不存在，無需處理
            }
        }

        private async void ComicService_ComicsChanged(object? sender, EventArgs e)
        {
            this._logger?.Log("收到 ComicsChanged 事件。");
            if (_currentUser.Role == UserRole.Admin)
            {
                this._logger?.Log("正在為管理員重新載入所有漫畫狀態。");
                await LoadAllComicsStatusForAdminAsync();
            }
            else
            {
                this._logger?.Log("正在重新載入可借閱漫畫和會員已租借的漫畫。");
                LoadAvailableComics();
                LoadMyRentedComics();
            }
        }

        private void SetupDataGridView()
        {
            if (dgvAvailableComics == null)
            {
                _logger?.LogError("設定DataGridView失敗：dgvAvailableComics 為空。");
                return;
            }

            this._logger?.Log("正在為漫畫設定 DataGridView。");
            dgvAvailableComics.AutoGenerateColumns = false;
            dgvAvailableComics.Columns.Clear();
            dgvAvailableComics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (_currentUser.Role == UserRole.Admin)
            {
                _logger!.Log("正在為管理員視圖設定 DataGridView (所有漫畫狀態)。");
                dgvAvailableComics!.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "書名", FillWeight = 20 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Author", HeaderText = "作者", FillWeight = 15 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "狀態", FillWeight = 10 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "BorrowerName", HeaderText = "借閱者", FillWeight = 15 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "BorrowerPhoneNumber", HeaderText = "借閱者電話", FillWeight = 15 });

                var rentalDateColumn = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "RentalDate",
                    HeaderText = "租借於",
                    FillWeight = 12,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" }
                };
                dgvAvailableComics.Columns.Add(rentalDateColumn);

                var returnDateColumn = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ReturnDate",
                    HeaderText = "到期日",
                    FillWeight = 13,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" }
                };
                dgvAvailableComics.Columns.Add(returnDateColumn);
                StyleModernDataGridView(dgvAvailableComics); 
            }
            else
            {
                _logger!.Log("正在為會員視圖設定 DataGridView (可借閱漫畫)。");
                var imgCol = new DataGridViewImageColumn
                {
                    Name = "CoverImage",
                    HeaderText = "封面",
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Width = 80
                };
                dgvAvailableComics!.Columns.Add(imgCol);
                dgvAvailableComics!.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Title", HeaderText = "書名", FillWeight = 40 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Author", HeaderText = "作者", FillWeight = 30 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Genre", HeaderText = "類型", FillWeight = 20 });
                dgvAvailableComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Isbn", HeaderText = "ISBN", FillWeight = 30 });
                StyleModernDataGridView(dgvAvailableComics);
                dgvAvailableComics.RowTemplate.Height = 100;
                dgvAvailableComics.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            }

            dgvAvailableComics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAvailableComics.MultiSelect = false;
            dgvAvailableComics.ReadOnly = true;
            dgvAvailableComics.AllowUserToAddRows = false;
        }

        private void LoadAvailableComics()
        {
            if (dgvAvailableComics == null) return;
            this._logger?.Log("正在將可借閱漫畫載入到主表單的 DataGridView。 (Existing log for context)");
            try
            {
                int totalComicsBeforeFilter = this._comicService!.GetAllComics().Count;
                this._logger?.Log($"LoadAvailableComics [Before Filter]: Starting with {totalComicsBeforeFilter} comics from _comicService.GetAllComics().");

                var queryResult = this._comicService!.GetAllComics().Where(c => !c.IsRented).ToList();
                this._logger?.Log($"LoadAvailableComics [After Filter]: After Where(c => !c.IsRented), queryResult count is {queryResult.Count}.");

                var availableComics = queryResult ?? new List<Comic>();

                Action updateGrid = () =>
                {
                    var dgv = dgvAvailableComics; // Capture to local variable
                    if (dgv != null)
                    {
                        dgv.DataSource = null;
                        dgv.DataSource = availableComics;
                    }
                };

                if (dgvAvailableComics != null && dgvAvailableComics.IsHandleCreated && !dgvAvailableComics.IsDisposed && this.InvokeRequired)
                    this.Invoke(updateGrid);
                else if (dgvAvailableComics != null && dgvAvailableComics.IsHandleCreated && !dgvAvailableComics.IsDisposed)
                    updateGrid();

                this._logger?.Log($"已成功載入 {availableComics.Count} 本可借閱漫畫。 (Existing log for context, shows final bound count)");
            }
            catch (Exception ex)
            {
                LogErrorActivity("載入可借閱漫畫時發生錯誤。", ex); 
                MessageBox.Show($"載入可用漫畫列表時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadAllComicsStatusForAdminAsync()
        {
            if (dgvAvailableComics == null || _memberService == null || _comicService == null) return;
            _logger?.Log("正在非同步為管理員視圖載入所有漫畫狀態。");

            try
            {
                List<Member>? allMembers = await Task.Run(() => _memberService.GetAllMembers()); // CS8604 addressed by checking null below
                if (allMembers == null)
                {
                    _logger?.LogWarning("LoadAllComicsStatusForAdminAsync: _memberService.GetAllMembers() returned null. Using empty list for statuses.");
                    allMembers = new List<Member>();
                }
                List<AdminComicStatusViewModel> comicStatuses = await Task.Run(() => _comicService.GetAdminComicStatusViewModels(allMembers));

                _allAdminComicStatuses = new List<AdminComicStatusViewModel>(comicStatuses);
                ApplyAdminComicsView(); 

                _logger?.Log($"已成功非同步為管理員視圖載入 {comicStatuses.Count} 本漫畫。");
            }
            catch (Exception ex)
            {
                LogErrorActivity("非同步載入所有漫畫狀態供管理員檢視時發生錯誤。", ex);
                Action showError = () => MessageBox.Show($"載入所有漫畫狀態時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    if (this.InvokeRequired) this.Invoke(showError);
                    else showError();
                }
            }
        }

        private void SetupUIAccessControls()
        {
            bool isAdmin = _currentUser.Role == UserRole.Admin;
            _logger.Log($"正在設定UI控制項。使用者是否為管理員: {isAdmin}");

            if (mainContentPanel == null)
            {
                _logger?.LogError("設定UI控制項存取權限失敗：mainContentPanel 為空。");
                return;
            }

            if (leftNavPanel != null) leftNavPanel.Visible = false;
            if (_adminDashboardControl != null) _adminDashboardControl.Visible = false; // Guarded
            if (memberViewTabControl != null) memberViewTabControl.Visible = false;

            if (lblAvailableComics != null) lblAvailableComics.Visible = false;
            if (cmbAdminComicFilterStatus != null) cmbAdminComicFilterStatus.Visible = false;
            if (dgvAvailableComics != null) dgvAvailableComics.Visible = false;

            if (this.menuStrip2 != null)
            {
                var managementMenuItem = this.menuStrip2.Items
                    .OfType<ToolStripMenuItem>()
                    .FirstOrDefault(item => item.Name == "管理ToolStripMenuItem");
                if (managementMenuItem != null) managementMenuItem.Visible = isAdmin;

                var toolsMenuItem = this.menuStrip2.Items
                    .OfType<ToolStripMenuItem>()
                    .FirstOrDefault(item => item.Name == "工具ToolStripMenuItem");
                if (toolsMenuItem != null) toolsMenuItem.Visible = isAdmin;

                var userRegItem = this.menuStrip2.Items
                    .OfType<ToolStripMenuItem>()
                    .FirstOrDefault(item => item.Name == "使用者註冊ToolStripMenuItem");
                if (userRegItem != null) userRegItem.Visible = isAdmin;
            }
            else
            {
                _logger.LogWarning("找不到 MenuStrip 控制項 'menuStrip2' 或其為空。");
            }

            if (isAdmin)
            {
                if (leftNavPanel != null) leftNavPanel.Visible = true;
                this.Text = "漫畫租借系統 - 管理員";

                if (btnNavDashboard != null)
                {
                    SelectNavButton(btnNavDashboard);
                }
                else if (btnNavComicMgmt != null)
                {
                    SelectNavButton(btnNavComicMgmt);
                }
            }

            else
            {
                this.Text = "漫畫租借系統";
                if (memberViewTabControl != null)
                {
                    _logger?.Log($"Member View: Current memberViewTabControl.Visible: {memberViewTabControl.Visible}. Intending to set to true.");
                    memberViewTabControl.Visible = true;
                    _logger?.Log($"Member View: New memberViewTabControl.Visible: {memberViewTabControl.Visible}.");

                    if (availableComicsTabPage != null)
                    {
                        bool isSelected = memberViewTabControl.SelectedTab == availableComicsTabPage;
                        _logger?.Log($"Member View: availableComicsTabPage is {(isSelected ? "selected" : "not selected")}. Visibility: {availableComicsTabPage.Visible}. memberViewTabControl selectedIndex: {memberViewTabControl.SelectedIndex}");
                    }
                    else
                    {
                        _logger?.LogWarning("Member View: availableComicsTabPage is null during UI setup.");
                    }

                    if (dgvAvailableComics != null)
                    {
                        _logger?.Log($"Member View: Intended dgvAvailableComics.Visible: true (within member view tab). Current: {dgvAvailableComics.Visible}. Will be set by tab visibility.");
                    }
                    else
                    {
                        _logger?.LogWarning("Member View: dgvAvailableComics is null during UI setup for member view.");
                    }

                    if (lblAvailableComics != null)
                    {
                        lblAvailableComics.Text = "可租借漫畫清單：";
                    }
                    if (lblMyRentedComicsHeader != null)
                    {
                        lblMyRentedComicsHeader.Text = "您目前租借的項目：";
                    }
                    
                }
                else
                {
                    _logger?.LogError("Member View Setup Error: memberViewTabControl is null.");
                    if (lblAvailableComics != null)
                    {
                        lblAvailableComics.Visible = true;
                        lblAvailableComics.Text = "可租借漫畫";
                    }
                    if (dgvAvailableComics != null) dgvAvailableComics.Visible = true;
                    if (lblMyRentedComicsHeader != null) lblMyRentedComicsHeader.Visible = true;
                    if (dgvMyRentedComics != null) dgvMyRentedComics.Visible = true;
                }

                if (memberViewTabControl != null && memberViewTabControl.TabPages.Count > 0)
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

            _logger!.Log($"UI 控制項的可見性和文字已根據管理員狀態 ({isAdmin}) 更新。");
        }


        private void UpdateStatusBar()
        {
            // _currentUser and _logger are guaranteed non-null by the constructor used for normal operation.
            // toolStripStatusLabelUser is part of InitializeComponent.
            if (this.toolStripStatusLabelUser != null)
            {
                this.toolStripStatusLabelUser.Text = $"使用者: {_currentUser.Username} | 角色: {_currentUser.Role}";
            }
            else
            {
                this._logger.LogWarning("toolStripStatusLabelUser is null. Cannot update status bar text.");
            }
            // Log status update attempt regardless of label's state, as user info is available.
            this._logger.Log($"Status bar update processed for User: {_currentUser.Username}, Role: {_currentUser.Role}");
        }

        private void SetupMyRentedComicsDataGridView()
        {
            if (dgvMyRentedComics == null) return;
            _logger?.Log("正在為會員已租借的漫畫設定 DataGridView (dgvMyRentedComics)。");
            dgvMyRentedComics.AutoGenerateColumns = false;
            dgvMyRentedComics.Columns.Clear();
            dgvMyRentedComics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvMyRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ComicTitle", HeaderText = "書名", FillWeight = 30 });
            dgvMyRentedComics.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Author", HeaderText = "作者", FillWeight = 20 });

            var rentalDateColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RentalDate",
                HeaderText = "租借日期",
                FillWeight = 18
            };
            rentalDateColumn.DefaultCellStyle ??= new DataGridViewCellStyle();
            rentalDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvMyRentedComics.Columns.Add(rentalDateColumn);

            var returnDateColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ExpectedReturnDate",
                HeaderText = "歸還日期",
                FillWeight = 18
            };
            returnDateColumn.DefaultCellStyle ??= new DataGridViewCellStyle();
            returnDateColumn.DefaultCellStyle.Format = "yyyy-MM-dd";
            dgvMyRentedComics.Columns.Add(returnDateColumn);

            if (!dgvMyRentedComics.Columns.Contains("statusColumn"))
            {
                var statusColumn = new DataGridViewTextBoxColumn
                {
                    Name = "statusColumn",
                    HeaderText = "狀態",
                    FillWeight = 14
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
                _logger?.LogWarning("載入我的租借漫畫失敗：CurrentUser 或關鍵服務為空。正在清除 DGV。");
                ClearDgvMyRentedComics();
                return;
            }
            if (_currentUser.Role != UserRole.Member)
            {
                _logger?.Log("載入我的租借漫畫失敗：使用者不是會員。正在清除 DGV。");
                ClearDgvMyRentedComics();
                return;
            }

            _logger?.LogInformation($"載入我的租借漫畫：正在嘗試為使用者載入租借記錄：'{_currentUser?.Username ?? "未知使用者"}'。");

            try
            {
                Member? currentMember = _memberService!.GetMemberByUsername(_currentUser!.Username);
                if (currentMember == null)
                {
                    _logger?.LogWarning($"載入我的租借漫畫失敗：找不到使用者名稱 '{_currentUser?.Username ?? "未知使用者"}' 的會員資料。未載入任何租借記錄。");
                    ClearDgvMyRentedComics();
                    return;
                }
                _logger?.LogInformation($"載入我的租借漫畫：找到會員：ID={currentMember.Id}，姓名='{currentMember.Name}'，使用者名稱='{currentMember.Username}'。");

                var allComics = _comicService!.GetAllComics();
                _logger?.LogDebug($"載入我的租借漫畫：篩選前從服務取得的漫畫總數：{allComics?.Count ?? 0}。");

                if (allComics == null)
                {
                    _logger?.LogWarning("載入我的租借漫畫失敗：_comicService.GetAllComics() 回傳為空。");
                    ClearDgvMyRentedComics();
                    return;
                }

                _logger?.LogDebug($"載入我的租借漫畫：正在依會員 ID 篩選：{currentMember.Id}。");
                if (allComics != null)
                {
                    _logger?.LogDebug("載入我的租借漫畫：頭幾本漫畫 (篩選前)：");
                    foreach (var comic in allComics.Take(5))
                    {
                        _logger?.LogDebug($"  - ID: {comic.Id}，書名: '{comic.Title}'，是否已租借: {comic.IsRented}，租借會員ID: {comic.RentedToMemberId}");
                    }
                }

                var myRentedComics = allComics!
                    .Where(c => c.IsRented && c.RentedToMemberId == currentMember.Id)
                    .Select(c => new RentalDetailViewModel
                    {
                        ComicId = c.Id,
                        ComicTitle = c.Title,
                        Author = c.Author,
                        RentalDate = c.RentalDate,
                        ExpectedReturnDate = c.ReturnDate
                    })
                    .ToList();

                _logger?.LogInformation($"載入我的租借漫畫：找到會員 ID {currentMember.Id} 的 {myRentedComics.Count} 本已租借漫畫。");

                if (myRentedComics.Any())
                {
                    var comicTitles = string.Join(", ", myRentedComics.Select(c => $"'{c.ComicTitle}'"));
                    _logger?.LogDebug($"載入我的租借漫畫：已租借漫畫：[{comicTitles}]");
                }
                else
                {
                    _logger?.LogInformation($"載入我的租借漫畫：找不到會員 ID {currentMember.Id} 的已租借漫畫。");
                }

                Action updateGrid = () =>
                {
                    dgvMyRentedComics.DataSource = null;
                    dgvMyRentedComics.DataSource = myRentedComics;
                };

                if (dgvMyRentedComics.IsHandleCreated && this.InvokeRequired)
                    this.Invoke(updateGrid);
                else if (dgvMyRentedComics.IsHandleCreated)
                    updateGrid();
            }
            catch (Exception ex)
            {
                _logger?.LogError("載入會員已租借漫畫時發生錯誤。", ex);
                MessageBox.Show($"載入您的租借漫畫列表時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearDgvMyRentedComics();
            }
        }

        private void ClearDgvMyRentedComics()
        {
            if (dgvMyRentedComics == null) return;
            Action clearGrid = () => {
                var dgv = dgvMyRentedComics; // Capture to local variable
                if (dgv != null)
                {
                    dgv.DataSource = null;
                }
            };
            if (dgvMyRentedComics != null && dgvMyRentedComics.IsHandleCreated && !dgvMyRentedComics.IsDisposed && this.InvokeRequired)
                this.Invoke(clearGrid);
            else if (dgvMyRentedComics != null && dgvMyRentedComics.IsHandleCreated && !dgvMyRentedComics.IsDisposed)
                clearGrid();
        }

        private void 離開ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("「離開」選單項目已點擊。");
            Application.Exit();
        }

        private void 漫畫管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("正在開啟漫畫管理表單。");
            ComicManagementForm comicMgmtForm = new ComicManagementForm(
                this._logger!,
                this._comicService,
                this._currentUser
            );
            comicMgmtForm.ShowDialog(this);
        }

        private void 會員管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("正在開啟會員管理表單。");
            if (Program.AppAuthService != null && this._comicService != null)
            {
                MemberManagementForm memberMgmtForm = new MemberManagementForm(
                    this._logger!,
                    this._memberService,
                    Program.AppAuthService,
                    this._comicService,
                    this._currentUser
                );
                memberMgmtForm.ShowDialog(this);
            }
            else
            {
                this._logger?.LogError("AuthenticationService 為空。無法開啟會員管理表單。");
                MessageBox.Show("無法開啟會員管理功能，因為驗證服務未正確初始化。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rentalManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("正在開啟租借表單。");
            try
            {
                RentalForm rentalForm = new RentalForm(
                    this._comicService,
                    this._memberService,
                    this._logger!,
                    this._reloadService
                );
                rentalForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                this._logger?.LogError("開啟租借表單失敗。", ex);
                MessageBox.Show($"開啟租借表單時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 使用者註冊ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log("「使用者註冊」選單項目已點擊。");
            if (_currentUser.Role == UserRole.Admin)
            {
                if (this._logger != null && Program.AppAuthService != null && this._memberService != null)
                {
                    var regForm = new RegistrationForm(
                        this._logger,
                        Program.AppAuthService,
                        this._memberService
                    );
                    regForm.ShowDialog(this);
                }
                else
                {
                    MessageBox.Show("Logger, AuthenticationService, 或 MemberService 未初始化，無法開啟使用者註冊。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this._logger?.LogError("由於 logger、AppAuthService 或 _memberService 為空，無法開啟註冊表單。");
                }
            }
            else
            {
                MessageBox.Show("只有管理員才能註冊新使用者。", "權限不足", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this._logger?.Log($"非管理員使用者 '{_currentUser.Username}' 嘗試開啟註冊表單。");
            }
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log($"使用者 '{_currentUser.Username}' 正在登出。");
            Application.Restart();
        }



        private void 檢視日誌ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._logger?.Log($"使用者 '{_currentUser.Username}' 點擊了「檢視日誌」選單項目。");
            if (_currentUser.Role == UserRole.Admin)
            {
                this._logger?.Log($"管理員使用者 '{_currentUser.Username}' 正在檢視日誌。");
                try
                {
                    string logDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ComicRentalApp", "Logs");
                    if (System.IO.Directory.Exists(logDirectory))
                    {
                        Process.Start(new ProcessStartInfo(logDirectory) { UseShellExecute = true });
                    }
                    else
                    {
                        MessageBox.Show("日誌資料夾尚未建立或找不到。", "資訊", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    this._logger?.LogError("開啟日誌資料夾失敗。", ex);
                    MessageBox.Show($"無法開啟日誌資料夾: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                this._logger?.Log($"使用者 '{_currentUser.Username}' (角色: {_currentUser.Role}) 嘗試檢視日誌。權限不足。");
                MessageBox.Show("權限不足", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void 日誌管理ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_currentUser.Role == UserRole.Admin)
            {
                using var f = new LogManagementForm((FileLogger)_logger!);
                f.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("權限不足", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            this._logger?.Log("主表單正在關閉。正在取消訂閱事件。");
            if (this._comicService != null)
                this._comicService.ComicsChanged -= ComicService_ComicsChanged;
            if (dgvAvailableComics != null)
                dgvAvailableComics.SelectionChanged -= dgvAvailableComics_SelectionChanged;
            base.OnFormClosing(e);
        }

        private void ApplyAdminComicsView()
        {
            if (_allAdminComicStatuses == null || this.dgvAvailableComics == null) return;
            if (!this.dgvAvailableComics.IsHandleCreated || this.dgvAvailableComics.IsDisposed) return;

            IEnumerable<AdminComicStatusViewModel> viewToShow = _allAdminComicStatuses;

            if (this.cmbAdminComicFilterStatus != null && this.cmbAdminComicFilterStatus.SelectedItem != null)
            {
                string? selectedStatus = this.cmbAdminComicFilterStatus.SelectedItem.ToString();
                if (selectedStatus == "已租借")
                {
                    viewToShow = viewToShow.Where(vm => vm.Status == "被借閱");
                }
                else if (selectedStatus == "可租借")
                {
                    viewToShow = viewToShow.Where(vm => vm.Status == "在館中");
                }
            }

            if (!string.IsNullOrEmpty(_currentSortColumnName))
            {
                var prop = typeof(AdminComicStatusViewModel).GetProperty(_currentSortColumnName);
                if (prop != null)
                {
                    if (_currentSortDirection == ListSortDirection.Ascending)
                        viewToShow = viewToShow.OrderBy(vm => prop.GetValue(vm, null));
                    else
                        viewToShow = viewToShow.OrderByDescending(vm => prop.GetValue(vm, null));
                }
            }

            var finalViewList = viewToShow.ToList();

            Action updateGridAction = () =>
            {
                var dgv = dgvAvailableComics; // Capture to local variable
                if (dgv != null)
                {
                    dgv.DataSource = null;
                    dgv.DataSource = finalViewList ?? new List<AdminComicStatusViewModel>();

                    foreach (DataGridViewColumn column in dgv.Columns)
                    {
                        if (column?.HeaderCell != null) // Guard for HeaderCell
                           column.HeaderCell.SortGlyphDirection = SortOrder.None;
                    }

                    if (!string.IsNullOrEmpty(_currentSortColumnName) && dgv.Columns.Contains(_currentSortColumnName))
                    {
                        var column = dgv.Columns[_currentSortColumnName];
                        if (column?.HeaderCell != null) // Guard for CS8602 on HeaderCell
                        {
                            column.HeaderCell.SortGlyphDirection =
                                _currentSortDirection == ListSortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending;
                        }
                    }
                }
            };

            if (this.dgvAvailableComics != null && this.dgvAvailableComics.IsHandleCreated && !this.dgvAvailableComics.IsDisposed)
            {
                if (this.dgvAvailableComics.InvokeRequired) this.dgvAvailableComics.Invoke(updateGridAction);
                else updateGridAction();
            }
        }

        private void dgvAvailableComics_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (_currentUser == null || _currentUser.Role != UserRole.Admin) return;
            if (this.dgvAvailableComics == null || e.ColumnIndex < 0 || e.ColumnIndex >= this.dgvAvailableComics.Columns.Count) return;
            if (this.dgvAvailableComics.Columns[e.ColumnIndex].DataPropertyName == null) return;

            string newSortColumnName = this.dgvAvailableComics.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(newSortColumnName)) return;

            if (_currentSortColumnName == newSortColumnName)
                _currentSortDirection = (_currentSortDirection == ListSortDirection.Ascending)
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
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
        private void SelectNavButton(Button selectedButton)
        {
            if (_currentSelectedNavButton != null)
            {
                _currentSelectedNavButton.BackColor = ModernBaseForm.SecondaryColor;
                _currentSelectedNavButton.ForeColor = ModernBaseForm.TextColor;
                _currentSelectedNavButton.Font = ModernBaseForm.ButtonFont ?? new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            }

            selectedButton.BackColor = ModernBaseForm.PrimaryColor;
            selectedButton.ForeColor = System.Drawing.Color.White;
            var baseFont = ModernBaseForm.ButtonFont ?? new System.Drawing.Font("Segoe UI Semibold", 9.75F);
            selectedButton.Font = new System.Drawing.Font(baseFont, System.Drawing.FontStyle.Bold);
            _currentSelectedNavButton = selectedButton;

            if (_adminDashboardControl != null) _adminDashboardControl.Visible = false; // Guarded
            if (memberViewTabControl != null) memberViewTabControl.Visible = false;

            if (lblAvailableComics != null) lblAvailableComics.Visible = false;
            if (cmbAdminComicFilterStatus != null) cmbAdminComicFilterStatus.Visible = false;
            if (dgvAvailableComics != null) dgvAvailableComics.Visible = false;

            if (selectedButton == btnNavDashboard)
            {
                if (_adminDashboardControl != null) // Guarded
                {
                    _adminDashboardControl.Visible = true; // Guarded
                    _adminDashboardControl.BringToFront();
                    _adminDashboardControl.LoadDashboardData(); // Guarded
                    this.Text = "漫畫租借系統 - 儀表板";
                }
                _logger?.Log("已選取儀表板視圖。");
            }
            else if (selectedButton == btnNavComicMgmt)
            {
                if (lblAvailableComics != null && lblAvailableComics.Parent != mainContentPanel)
                {
                    mainContentPanel.Controls.Add(lblAvailableComics);
                }
                if (cmbAdminComicFilterStatus != null && cmbAdminComicFilterStatus.Parent != mainContentPanel)
                {
                    mainContentPanel.Controls.Add(cmbAdminComicFilterStatus);
                }
                if (dgvAvailableComics != null && dgvAvailableComics.Parent != mainContentPanel)
                {
                    mainContentPanel.Controls.Add(dgvAvailableComics);
                }

                var panelWidth = mainContentPanel.ClientSize.Width;
                var panelHeight = mainContentPanel.ClientSize.Height;
                const int margin = 8;

                if (lblAvailableComics != null)
                {
                    lblAvailableComics.SetBounds(
                        margin,
                        margin,
                        panelWidth - 2 * margin,
                        28
                    );
                    lblAvailableComics.Visible = true;
                }

                if (cmbAdminComicFilterStatus != null)
                {
                    int comboWidth = 120;
                    int comboHeight = 23;
                    int comboX = panelWidth - comboWidth - margin;
                    int comboY = (lblAvailableComics != null ? lblAvailableComics.Bottom : margin) + 8;
                    cmbAdminComicFilterStatus.SetBounds(
                        comboX,
                        comboY,
                        comboWidth,
                        comboHeight
                    );
                    cmbAdminComicFilterStatus.Visible = true;
                }

                if (dgvAvailableComics != null)
                {
                    int dgvY = (cmbAdminComicFilterStatus != null ? cmbAdminComicFilterStatus.Bottom : (lblAvailableComics != null ? lblAvailableComics.Bottom : margin)) + 8;
                    int dgvHeight = panelHeight - dgvY - margin;
                    dgvAvailableComics.SetBounds(
                        margin,
                        dgvY,
                        panelWidth - 2 * margin,
                        dgvHeight
                    );
                    dgvAvailableComics.Visible = true;
                    dgvAvailableComics.BringToFront();
                }

                this.Text = "漫畫租借系統 - 漫畫管理";
                _logger?.Log("已選取漫畫管理視圖。");
            }
            else if (selectedButton == btnNavMemberMgmt)
            {
                this.會員管理ToolStripMenuItem_Click(this, EventArgs.Empty);
                _logger?.Log("會員管理導覽按鈕已點擊。");
            }
            else if (selectedButton == btnNavRentalMgmt)
            {
                this.rentalManagementToolStripMenuItem_Click(this, EventArgs.Empty);
                _logger?.Log("租借管理導覽按鈕已點擊。");
            }
            else if (selectedButton == btnNavUserReg)
            {
                this.使用者註冊ToolStripMenuItem_Click(this, EventArgs.Empty);
                _logger?.Log("使用者註冊導覽按鈕已點擊。");
            }
            else if (selectedButton == btnNavLogs)
            {
                this.日誌管理ToolStripMenuItem_Click(this, EventArgs.Empty);
                _logger?.Log("日誌管理導覽按鈕已點擊。");
            }
        }



        private void btnNavDashboard_Click(object? sender, EventArgs e)
        {
            if (sender is Button cb)
            {
                SelectNavButton(cb);
                _logger?.Log("Dashboard navigation button clicked.");
            }
        }
        private void btnNavComicMgmt_Click(object? sender, EventArgs e)
        {
            if (sender is Button cb)
            {
                SelectNavButton(cb);
                _logger?.Log("Comic Mgmt navigation button clicked.");
            }
        }
        private void btnNavMemberMgmt_Click(object? sender, EventArgs e)
        {
            if (sender is Button cb)
            {
                SelectNavButton(cb);
                _logger?.Log("Member Mgmt navigation button clicked.");
            }
        }
        private void btnNavRentalMgmt_Click(object? sender, EventArgs e)
        {
            if (sender is Button cb)
            {
                SelectNavButton(cb);
                _logger?.Log("Rental Mgmt navigation button clicked.");
            }
        }
        private void btnNavUserReg_Click(object? sender, EventArgs e)
        {
            if (sender is Button cb)
            {
                SelectNavButton(cb);
                _logger?.Log("User Reg navigation button clicked.");
            }
        }
        private void btnNavLogs_Click(object? sender, EventArgs e)
        {
            if (sender is Button cb)
            {
                SelectNavButton(cb);
                _logger?.Log("Logs navigation button clicked.");
            }
        }

        private void dgvAvailableComics_AdminView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (_currentUser == null || _currentUser.Role != UserRole.Admin) return;
            if (dgvAvailableComics == null || e.RowIndex < 0 || e.RowIndex >= dgvAvailableComics.Rows.Count) return;

            DataGridViewRow row = dgvAvailableComics.Rows[e.RowIndex];
            if (row.DataBoundItem is not AdminComicStatusViewModel comicStatus) return;

            if (dgvAvailableComics.Columns[e.ColumnIndex].DataPropertyName == "Status")
            {
                if (e.Value?.ToString() == "被借閱") // CS8602 for e.CellStyle below
                {
                    if (e.CellStyle != null) e.CellStyle.ForeColor = ModernBaseForm.DangerColor;
                }
                else if (e.Value?.ToString() == "在館中") // CS8602 for e.CellStyle below
                {
                    if (e.CellStyle != null) e.CellStyle.ForeColor = ModernBaseForm.SuccessColor;
                }
            }

            if (dgvAvailableComics.Columns[e.ColumnIndex].DataPropertyName == "ReturnDate" ||
                dgvAvailableComics.Columns[e.ColumnIndex].DataPropertyName == "Status")
            {
                // comicStatus is guaranteed non-null here by `is not AdminComicStatusViewModel comicStatus) return;`
                if (comicStatus.Status == "被借閱" && comicStatus.ReturnDate.HasValue) // CS8602 for row.DefaultCellStyle below
                {
                    DateTime returnDate = comicStatus.ReturnDate.Value;
                    if (row.DefaultCellStyle != null) // Guard for DefaultCellStyle
                    {
                        if (returnDate.Date < DateTime.Today)
                        {
                            row.DefaultCellStyle.BackColor = ModernBaseForm.DangerColor;
                            row.DefaultCellStyle.ForeColor = Color.White;
                        }
                        else if (returnDate.Date <= DateTime.Today.AddDays(3))
                        {
                            row.DefaultCellStyle.BackColor = ModernBaseForm.AccentColor;
                            row.DefaultCellStyle.ForeColor = ModernBaseForm.TextColor;
                        }
                    }
                }
            }
        }

        private void dgvAvailableComics_MemberView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (_currentUser == null || _currentUser.Role != UserRole.Member) return;
            if (dgvAvailableComics == null || e.RowIndex < 0 || e.RowIndex >= dgvAvailableComics.Rows.Count) return;

            if (dgvAvailableComics.Columns[e.ColumnIndex].Name == "CoverImage")
            {
                DataGridViewRow row = dgvAvailableComics.Rows[e.RowIndex];
                if (row.DataBoundItem is Comic comic)
                {
                    if (!string.IsNullOrEmpty(comic.CoverImagePath) && File.Exists(comic.CoverImagePath))
                    {
                        try { e.Value = Image.FromFile(comic.CoverImagePath); }
                        catch { e.Value = _placeholderCoverImage; }
                    }
                    else
                    {
                        e.Value = _placeholderCoverImage;
                    }
                }
            }
        }

        private void dgvMyRentedComics_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvMyRentedComics == null || e.RowIndex < 0 || e.RowIndex >= dgvMyRentedComics.Rows.Count)
                return;

            DataGridViewRow row = dgvMyRentedComics.Rows[e.RowIndex];
            if (row.DataBoundItem == null) return;

            DateTime? returnDate = null;
            try
            {
                var returnDateObj = row.DataBoundItem.GetType().GetProperty("ExpectedReturnDate")?.GetValue(row.DataBoundItem, null);
                if (returnDateObj != null && returnDateObj != DBNull.Value)
                    returnDate = Convert.ToDateTime(returnDateObj);
            }
            catch (Exception ex)
            {
                _logger?.LogError($"在 CellFormatting 中透過反映取得 ExpectedReturnDate 時發生錯誤: {ex.Message}");
                return;
            }

            if (returnDate == null) return;

            if (dgvMyRentedComics.Columns[e.ColumnIndex].Name == "statusColumn")
            {
                TimeSpan remainingTime = returnDate.Value.Date - DateTime.Today;
                if (remainingTime.TotalDays < 0)
                {
                    e.Value = $"逾期 {-remainingTime.TotalDays} 天";
                }
                else if (remainingTime.TotalDays == 0)
                {
                    e.Value = "今日到期";
                }
                else
                {
                    e.Value = $"剩餘 {remainingTime.TotalDays} 天";
                }
                e.FormattingApplied = true;
            }

            if (e.CellStyle != null)
            {
                if (returnDate.Value.Date < DateTime.Today)
                {
                    e.CellStyle.BackColor = ModernBaseForm.DangerColor;
                    e.CellStyle.ForeColor = Color.White;
                }
                else if (returnDate.Value.Date <= DateTime.Today.AddDays(3))
                {
                    e.CellStyle.BackColor = ModernBaseForm.AccentColor;
                    e.CellStyle.ForeColor = ModernBaseForm.TextColor;
                }
            }
        }

        private void memberViewTabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (memberViewTabControl == null) return;

            if (memberViewTabControl.SelectedTab == myRentalsTabPage)
            {
                _logger?.Log("已選取「我的租借」標籤頁。正在重新載入已租借漫畫。");
                LoadMyRentedComics();
            }
            else if (memberViewTabControl.SelectedTab == availableComicsTabPage)
            {
                _logger?.Log("已選取「可租借漫畫」標籤頁。正在重新套用篩選器。");
                if (dgvAvailableComics != null)
                {
                    dgvAvailableComics.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    dgvAvailableComics.RowTemplate.Height = 100;
                }
                ApplyAvailableComicsFilter();
                if (dgvAvailableComics != null)
                {
                    dgvAvailableComics.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    dgvAvailableComics.RowTemplate.Height = 100;
                }
            }
        }

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
                _logger?.LogWarning("套用可租借漫畫篩選器：關鍵元件為空，略過篩選。");
                return;
            }

            try
            {
                _logger?.Log("正在套用可租借漫畫篩選器… (ApplyAvailableComicsFilter called)");

                string placeholderText = "依書名/作者搜尋...";
                string actualSearchText = "";

                bool isPlaceholderActive = (txtSearchAvailableComics != null &&
                                            txtSearchAvailableComics.Text == placeholderText &&
                                            txtSearchAvailableComics.ForeColor == Color.Gray);

                if (txtSearchAvailableComics != null &&
                    txtSearchAvailableComics.Text != null &&
                    !isPlaceholderActive &&
                    !string.IsNullOrWhiteSpace(txtSearchAvailableComics.Text))
                {
                    actualSearchText = txtSearchAvailableComics.Text.Trim().ToLowerInvariant();
                }

                var comicsToFilter = _comicService!
                                        .GetAllComics()
                                        .Where(c => !c.IsRented)
                                        .ToList();
                _logger?.Log($"ApplyAvailableComicsFilter: Initial available comics (not rented): {comicsToFilter.Count}");


                if (!string.IsNullOrWhiteSpace(actualSearchText))
                {
                    comicsToFilter = comicsToFilter
                        .Where(c =>
                            (c.Title != null && c.Title.ToLowerInvariant().Contains(actualSearchText)) ||
                            (c.Author != null && c.Author.ToLowerInvariant().Contains(actualSearchText))
                        )
                        .ToList();
                }
                _logger?.Log($"ApplyAvailableComicsFilter: After text search ('{actualSearchText}'), count: {comicsToFilter.Count}");

                string chosenGenre = "所有類型";
                if (cmbGenreFilter != null && cmbGenreFilter.SelectedIndex > 0 && cmbGenreFilter.SelectedItem is string genreStr)
                {
                    if (genreStr != "所有類型") 
                    {
                        chosenGenre = genreStr;
                        comicsToFilter = comicsToFilter
                            .Where(c =>
                                !string.IsNullOrWhiteSpace(c.Genre) &&
                                c.Genre != null && c.Genre.ToUpperInvariant() == chosenGenre.ToUpperInvariant()
                            )
                            .ToList();
                    }
                }
                _logger?.Log($"ApplyAvailableComicsFilter: After genre filter ('{chosenGenre}'), count: {comicsToFilter.Count}");

                Action updateGrid = () => {
                    dgvAvailableComics.DataSource = null;
                    dgvAvailableComics.DataSource = comicsToFilter;
                };

                if (dgvAvailableComics.IsHandleCreated && !dgvAvailableComics.IsDisposed)
                {
                    if (dgvAvailableComics.InvokeRequired)
                    {
                        dgvAvailableComics.Invoke(updateGrid);
                    }
                    else
                    {
                        updateGrid();
                    }
                }

                _logger?.Log($"篩選完成：搜尋「{actualSearchText}」、類型「{chosenGenre}」→ 共 {comicsToFilter.Count} 本。 (Final log in ApplyAvailableComicsFilter)");
            }
            catch (Exception ex)
            {
                _logger?.LogError("套用可租借漫畫篩選器時發生錯誤。", ex);
                if (dgvAvailableComics != null)
                {
                    Action clearGridAction = () => dgvAvailableComics.DataSource = null;
                    if (dgvAvailableComics.IsHandleCreated && !dgvAvailableComics.IsDisposed)
                    {
                        if (dgvAvailableComics.InvokeRequired) dgvAvailableComics.Invoke(clearGridAction); else clearGridAction();
                    }
                }
            }
        }

        private static Image CreatePlaceholderCoverImage()
        {
            Bitmap bmp = new Bitmap(80, 100);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                using Font font = new Font("Arial", 8);
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("暫無封面圖", font, Brushes.Black, new RectangleF(0, 0, bmp.Width, bmp.Height), sf);
            }
            return bmp;
        }
    }
}
