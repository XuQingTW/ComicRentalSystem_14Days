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

        public ComicEditForm() : base()
        {
            InitializeComponent();
            if (this.DesignMode) 
            {
                chkIsRented.Enabled = false;
            }
        }

        public ComicEditForm(Comic? comicToEdit, ComicService comicService, ILogger logger, User? currentUser = null) : this()
        {
            base.SetLogger(logger);

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
            // chkIsRented.Enabled = false; // Replaced by below logic

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

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                LogActivity("驗證失敗: 書名為空。");
                MessageBox.Show("書名不得為空。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                LogActivity("驗證失敗: 作者為空。");
                MessageBox.Show("作者不得為空。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAuthor.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtGenre.Text))
            {
                LogActivity("驗證失敗: 類型為空。");
                MessageBox.Show("類型不得為空。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGenre.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtIsbn.Text))
            {
                LogActivity("驗證失敗: ISBN為空。");
                MessageBox.Show("ISBN不得為空。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIsbn.Focus();
                return;
            }

            try
            {
                if (_isEditMode && _editableComic != null)
                {
                    LogActivity($"正在嘗試儲存現有漫畫ID: {_editableComic.Id} 的變更。");

                    if (_currentUser != null && _currentUser.Role == UserRole.Admin && chkIsRented.Enabled)
                    {
                        bool originalIsRented = _editableComic.IsRented; // Store original state before changing _editableComic
                        bool currentChkIsRented = chkIsRented.Checked;

                        if (originalIsRented && !currentChkIsRented) // Was rented, now admin unchecks it (processes a return)
                        {
                            _editableComic.IsRented = false;
                            _editableComic.RentedToMemberId = 0;
                            _editableComic.RentalDate = null;
                            _editableComic.ReturnDate = null;
                            _editableComic.ActualReturnTime = DateTime.Now;
                            LogActivity($"Admin manually marked comic ID: {_editableComic.Id} as returned via EditForm.");
                        }
                        else if (!originalIsRented && currentChkIsRented) // Was not rented, admin checks it
                        {
                            MessageBox.Show("若要將漫畫標記為已租借，請使用「租借管理」表單以確保輸入所有必要的租借詳細資料（如會員ID和歸還日期）。\n此變更不會將漫畫標記為已租借。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Revert the checkbox because we are not allowing admin to rent via this checkbox
                            chkIsRented.Checked = false;
                            // _editableComic.IsRented remains false (its original state)
                            LogActivity($"Admin attempted to mark comic ID: {_editableComic.Id} as rented via EditForm. Guided to RentalForm. Change reverted.");
                        }
                    }

                    _editableComic.Title = txtTitle.Text.Trim();
                    _editableComic.Author = txtAuthor.Text.Trim();
                    _editableComic.Isbn = txtIsbn.Text.Trim();
                    _editableComic.Genre = txtGenre.Text.Trim();
                    // Note: _editableComic.IsRented is now handled by the admin logic above if applicable,
                    // or remains its original loaded value if admin didn't/couldn't change it.
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
    }
}