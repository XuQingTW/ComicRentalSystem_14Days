using ComicRentalSystem_14Days.Models;
using ComicRentalSystem_14Days.Services;
using ComicRentalSystem_14Days.Interfaces; // 技術點3: 引用介面命名空間
using System;
using System.IO; // For IOException
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Forms
{
    public partial class ComicEditForm : ComicRentalSystem_14Days.BaseForm // 技術點3: 繼承 BaseForm
    {
        private readonly ComicService _comicService;
        private Comic? _editableComic; // 要編輯的漫畫物件，如果是 null，則表示是新增模式
        private bool _isEditMode;
        // Logger is inherited from BaseForm (protected ILogger? Logger)

        // 建構函式修改以接收 ILogger 並傳遞給 BaseForm
        // 技術點4: 過載 (這個建構函式本身就是對應不同用途的設計)
        public ComicEditForm(Comic? comicToEdit, ComicService comicService, ILogger logger) : base(logger) // 技術點4: 多型 (傳遞 ILogger 給 BaseForm)
        {
            InitializeComponent();
            // Logger is already set by BaseForm's constructor

            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));
            _editableComic = comicToEdit;
            _isEditMode = (_editableComic != null);

            LogActivity($"ComicEditForm initializing. Mode: {(_isEditMode ? "Edit" : "Add")}" +
                        (_isEditMode && _editableComic != null ? $", ComicID: {_editableComic.Id}" : ""));

            if (_isEditMode && _editableComic != null)
            {
                this.Text = "編輯漫畫";
                LoadComicData();
            }
            else
            {
                this.Text = "新增漫畫";
                // 新增漫畫時，IsRented 應預設為 false 且不可編輯
                chkIsRented.Checked = false;
                chkIsRented.Enabled = false; // 在 UI 設計器中可能已設定，這裡再次確保
            }
            LogActivity("ComicEditForm initialized.");
        }

        private void LoadComicData()
        {
            if (_editableComic == null)
            {
                LogActivity("LoadComicData called but _editableComic is null (should not happen in edit mode).");
                return;
            }

            LogActivity($"Loading data for comic ID: {_editableComic.Id}, Title: '{_editableComic.Title}'.");
            txtTitle.Text = _editableComic.Title;
            txtAuthor.Text = _editableComic.Author;
            txtIsbn.Text = _editableComic.Isbn;
            txtGenre.Text = _editableComic.Genre;

            // 在編輯模式下，IsRented 欄位的啟用狀態取決於漫畫是否已被租借
            // 如果你希望一旦租借就不能在此表單修改租借狀態，可以這樣設定：
            // chkIsRented.Enabled = !_editableComic.IsRented;
            // 但目前Day 9-10的簡化版租借邏輯還未完全整合，暫時保持可見但可能不可編輯的狀態
            chkIsRented.Checked = _editableComic.IsRented;
            chkIsRented.Enabled = false; // 通常租借狀態不由編輯表單直接修改，而是通過租借/歸還流程

            LogActivity("Comic data loaded into form controls.");
        }

        // 技術點 #6: 視窗應用程式的事件處理
        private void btnSave_Click(object sender, EventArgs e)
        {
            LogActivity("Save button clicked.");

            // 1. 簡單的輸入驗證
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                LogActivity("Validation failed: Title is empty.");
                MessageBox.Show("書名不得為空。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                LogActivity("Validation failed: Author is empty.");
                MessageBox.Show("作者不得為空。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAuthor.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtIsbn.Text))
            {
                LogActivity("Validation failed: ISBN is empty.");
                MessageBox.Show("ISBN不得為空。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIsbn.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtGenre.Text))
            {
                LogActivity("Validation failed: Genre is empty.");
                MessageBox.Show("類型不得為空。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGenre.Focus();
                return;
            }
            LogActivity("Input validation passed.");

            try // 技術點 #5: 例外處理
            {
                if (_isEditMode && _editableComic != null) // 編輯模式
                {
                    LogActivity($"Attempting to save changes for existing comic ID: {_editableComic.Id}.");
                    _editableComic.Title = txtTitle.Text.Trim();
                    _editableComic.Author = txtAuthor.Text.Trim();
                    _editableComic.Isbn = txtIsbn.Text.Trim();
                    _editableComic.Genre = txtGenre.Text.Trim();
                    // _editableComic.IsRented and RentedToMemberId are not typically changed here
                    // They would be managed by a rental/return process.

                    _comicService.UpdateComic(_editableComic); // Service 層已有詳細日誌
                    LogActivity($"Comic ID: {_editableComic.Id} updated successfully.");
                    MessageBox.Show("漫畫資料已更新。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else // 新增模式
                {
                    LogActivity("Attempting to add new comic.");
                    Comic newComic = new Comic
                    {
                        // Id is generated by ComicService
                        Title = txtTitle.Text.Trim(),
                        Author = txtAuthor.Text.Trim(),
                        Isbn = txtIsbn.Text.Trim(),
                        Genre = txtGenre.Text.Trim(),
                        IsRented = false, // New comics are not rented by default
                        RentedToMemberId = 0 // New comics are not rented to anyone
                    };
                    _comicService.AddComic(newComic); // Service 層已有詳細日誌
                    LogActivity($"New comic '{newComic.Title}' (ID: {newComic.Id}) added successfully.");
                    MessageBox.Show("漫畫已成功新增。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                LogActivity("ComicEditForm closing with DialogResult.OK.");
                this.Close();
            }
            catch (InvalidOperationException opEx) // 特定業務邏輯錯誤
            {
                LogErrorActivity($"Operation error while saving comic: {opEx.Message}", opEx);
                MessageBox.Show(opEx.Message, "操作錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException formatEx) // 資料格式錯誤 (雖然此表單直接輸入較少發生)
            {
                LogErrorActivity($"Data format error while saving comic: {formatEx.Message}", formatEx);
                MessageBox.Show($"資料格式錯誤: {formatEx.Message}", "格式錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ioEx) // 檔案讀寫錯誤
            {
                LogErrorActivity($"File I/O error while saving comic: {ioEx.Message}", ioEx);
                MessageBox.Show($"儲存漫畫失敗，檔案存取錯誤: {ioEx.Message}", "儲存失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) // 其他未預期錯誤
            {
                LogErrorActivity($"Unexpected error while saving comic: {ex.Message}", ex);
                MessageBox.Show($"儲存漫畫時發生未預期錯誤: {ex.Message}", "嚴重錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            LogActivity("Cancel button clicked. ComicEditForm closing with DialogResult.Cancel.");
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ComicEditForm_Load(object sender, EventArgs e)
        {
            // LogActivity is already called at the end of the constructor.
            // If any specific Load-time logic is added here, log it.
            LogActivity("ComicEditForm finished loading.");
        }
    }
}