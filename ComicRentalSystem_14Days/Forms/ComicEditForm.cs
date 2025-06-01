using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Interfaces;
using System;
using System.IO;
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class ComicEditForm : ComicRentalSystem_14Days.BaseForm
    {
        // 將欄位宣告為可為 Null
        private readonly ComicService? _comicService;
        private Comic? _editableComic;
        private bool _isEditMode;
        private readonly User? _currentUser;
        private System.Windows.Forms.ErrorProvider errorProvider1;

        public ComicEditForm() : base()
        {
            InitializeComponent();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider();
            if (this.DesignMode) 
            {
                chkIsRented.Enabled = false;
            }
        }

        public ComicEditForm(Comic? comicToEdit, ComicService comicService, ILogger logger, User? currentUser = null) : this()
        {
            // InitializeComponent(); // 由無參數建構函式透過 : this() 呼叫
            base.SetLogger(logger);

            // InitializeComponent 執行後套用現代樣式
            if (btnSave != null) StyleModernButton(btnSave);
            if (btnCancel != null) StyleSecondaryButton(btnCancel);

            // 假設 gbComicDetails 和 gbStatus 是 Designer.cs 中的欄位且可在此處存取
            // 設計工具應將它們設為部分類別中的欄位。
            // 如果它們是正確的欄位，我們需要透過 'this' 存取它們。
            Control[] topLevelControls = this.Controls.Find("gbComicDetails", true);
            if (topLevelControls.Length > 0 && topLevelControls[0] is GroupBox gbDetails) StyleModernGroupBox(gbDetails);

            topLevelControls = this.Controls.Find("gbStatus", true);
            if (topLevelControls.Length > 0 && topLevelControls[0] is GroupBox gbStat) StyleModernGroupBox(gbStat);
            // 如果欄位由設計工具正確產生，則有更直接的方法：
            // if (this.gbComicDetails != null) StyleModernGroupBox(this.gbComicDetails);
            // if (this.gbStatus != null) StyleModernGroupBox(this.gbStatus);


            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            _editableComic = comicToEdit;
            _isEditMode = (_editableComic != null);
            _currentUser = currentUser;

            LogActivity($"漫畫編輯表單初始化中。模式: {(_isEditMode ? "編輯" : "新增")}" +
                        (_isEditMode && _editableComic != null ? $", 漫畫ID: {_editableComic.Id}" : "") +
                        (_currentUser != null ? $", 使用者: {_currentUser.Username} ({_currentUser.Role})" : ", 無使用者資訊"));

            if (_isEditMode && _editableComic != null)
            {
                this.Text = "編輯漫畫";
                LoadComicData();
            }
            else
            {
                this.Text = "新增漫畫";
                chkIsRented.Checked = false;
                chkIsRented.Enabled = false;
            }
            LogActivity("漫畫編輯表單已使用服務初始化。");
        }

        private void LoadComicData()
        {
            if (_editableComic == null)
            {
                LogActivity("LoadComicData 已呼叫，但 _editableComic 為空 (在編輯模式下不應發生)。");
                return;
            }

            LogActivity($"正在載入漫畫ID: {_editableComic.Id}, 書名: '{_editableComic.Title}' 的資料。");
            txtTitle.Text = _editableComic.Title;
            txtAuthor.Text = _editableComic.Author;
            txtIsbn.Text = _editableComic.Isbn;
            txtGenre.Text = _editableComic.Genre;
            chkIsRented.Checked = _editableComic.IsRented;
            // chkIsRented.Enabled = false; // 由以下邏輯取代

            if (_currentUser != null && _currentUser.Role == UserRole.Admin)
            {
                chkIsRented.Enabled = true;
            }
            else
            {
                chkIsRented.Enabled = false;
            }

            LogActivity("漫畫資料已載入表單控制項。");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_comicService == null)
            {
                MessageBox.Show("服務未初始化，無法儲存。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity("儲存按鈕已點擊，但 _comicService 為空。");
                return;
            }

            LogActivity("儲存按鈕已點擊。");

        if (!this.ValidateChildren()) {
            MessageBox.Show("請修正標示的錯誤。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning); // 請修正標示的錯誤。 | 驗證錯誤
            LogActivity("驗證失敗。請修正醒目提示的錯誤。");
            return;
            }

            try
            {
                if (_isEditMode && _editableComic != null)
                {
                    LogActivity($"正在嘗試儲存現有漫畫ID: {_editableComic.Id} 的變更。");

                    if (_currentUser != null && _currentUser.Role == UserRole.Admin && chkIsRented.Enabled)
                    {
                        bool originalIsRented = _editableComic.IsRented; // 在變更 _editableComic 之前儲存原始狀態
                        bool currentChkIsRented = chkIsRented.Checked;

                        if (originalIsRented && !currentChkIsRented) // 先前已租借，現在管理員取消勾選 (處理歸還程序)
                        {
                            _editableComic.IsRented = false;
                            _editableComic.RentedToMemberId = 0;
                            _editableComic.RentalDate = null;
                            _editableComic.ReturnDate = null;
                            _editableComic.ActualReturnTime = DateTime.Now;
                            LogActivity($"管理員已透過編輯表單手動將漫畫 ID: {_editableComic.Id} 標記為已歸還。");
                        }
                        else if (!originalIsRented && currentChkIsRented) // 先前未租借，管理員勾選
                        {
                            MessageBox.Show("若要將漫畫標記為已租借，請使用「租借管理」表單以確保輸入所有必要的租借詳細資料（如會員ID和歸還日期）。\n此變更不會將漫畫標記為已租借。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // 復原核取方塊的勾選，因為我們不允許管理員透過此核取方塊租借
                            chkIsRented.Checked = false;
                            // _editableComic.IsRented 保持為 false (其原始狀態)
                            LogActivity($"管理員嘗試透過編輯表單將漫畫 ID: {_editableComic.Id} 標記為已租借。已引導至租借表單。變更已復原。");
                        }
                    }

                    _editableComic.Title = txtTitle.Text.Trim();
                    _editableComic.Author = txtAuthor.Text.Trim();
                    _editableComic.Isbn = txtIsbn.Text.Trim();
                    _editableComic.Genre = txtGenre.Text.Trim();
                    // 注意：如果適用，_editableComic.IsRented 現在由上述的管理員邏輯處理，
                    // 或者如果管理員沒有/無法變更它，則保持其原始載入值。
                    _comicService.UpdateComic(_editableComic);
                    LogActivity($"漫畫ID: {_editableComic.Id} 已成功更新。");
                    MessageBox.Show("漫畫資料已更新。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    LogActivity("正在嘗試新增漫畫。");
                    Comic newComic = new Comic
                    {
                        Title = txtTitle.Text.Trim(),
                        Author = txtAuthor.Text.Trim(),
                        Isbn = txtIsbn.Text.Trim(),
                        Genre = txtGenre.Text.Trim(),
                        IsRented = false,
                        RentedToMemberId = 0
                    };
                    _comicService.AddComic(newComic);
                    LogActivity($"新漫畫 '{newComic.Title}' (ID: {newComic.Id}) 已成功新增。");
                    MessageBox.Show("漫畫已成功新增。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                LogActivity("漫畫編輯表單正在以 DialogResult.OK 關閉。");
                this.Close();
            }
            catch (Exception ex)
            {
                LogErrorActivity($"儲存漫畫時發生錯誤: {ex.Message}", ex);
                MessageBox.Show($"儲存漫畫時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            LogActivity("取消按鈕已點擊。漫畫編輯表單正在以 DialogResult.Cancel 關閉。");
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ComicEditForm_Load(object sender, EventArgs e)
        {
            LogActivity("漫畫編輯表單已完成載入。");
        }

        // 驗證事件處理常式
        private void txtTitle_Validating(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is TextBox txt && string.IsNullOrWhiteSpace(txt.Text))
            {
                errorProvider1?.SetError(txt, "書名不能為空。"); // 書名不能為空。
                e.Cancel = true;
            }
            else if (sender is TextBox txtBox)
            {
                errorProvider1?.SetError(txtBox, ""); // 清除錯誤
            }
        }

        private void txtAuthor_Validating(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is TextBox txt && string.IsNullOrWhiteSpace(txt.Text))
            {
                errorProvider1?.SetError(txt, "作者不能為空。"); // 作者不能為空。
                e.Cancel = true;
            }
            else if (sender is TextBox txtBox)
            {
                errorProvider1?.SetError(txtBox, ""); // 清除錯誤
            }
        }

        private void txtIsbn_Validating(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is TextBox txt && string.IsNullOrWhiteSpace(txt.Text))
            {
                errorProvider1?.SetError(txt, "ISBN 不能為空。"); // ISBN 不能為空。
                e.Cancel = true;
            }
            else if (sender is TextBox txtBox)
            {
                errorProvider1?.SetError(txtBox, ""); // 清除錯誤
            }
        }

        private void txtGenre_Validating(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is TextBox txt && string.IsNullOrWhiteSpace(txt.Text))
            {
                errorProvider1?.SetError(txt, "類型不能為空。"); // 類型不能為空。
                e.Cancel = true;
            }
            else if (sender is TextBox txtBox)
            {
                errorProvider1?.SetError(txtBox, ""); // 清除錯誤
            }
        }
    }
}