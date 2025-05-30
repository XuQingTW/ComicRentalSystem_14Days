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

        public ComicEditForm() : base()
        {
            InitializeComponent();
            if (this.DesignMode) 
            {
                chkIsRented.Enabled = false;
            }
        }

        public ComicEditForm(Comic? comicToEdit, ComicService comicService, ILogger logger) : this()
        {
            base.SetLogger(logger);

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
                chkIsRented.Checked = false;
                chkIsRented.Enabled = false;
            }
            LogActivity("ComicEditForm initialized with services.");
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
            chkIsRented.Checked = _editableComic.IsRented;
            chkIsRented.Enabled = false;

            LogActivity("Comic data loaded into form controls.");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_comicService == null)
            {
                MessageBox.Show("服務未初始化，無法儲存。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogErrorActivity("Save button clicked, but _comicService is null.");
                return;
            }

            LogActivity("Save button clicked.");

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                LogActivity("Validation failed: Title is empty.");
                MessageBox.Show("書名不得為空。", "驗證錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            try
            {
                if (_isEditMode && _editableComic != null)
                {
                    LogActivity($"Attempting to save changes for existing comic ID: {_editableComic.Id}.");
                    _editableComic.Title = txtTitle.Text.Trim();
                    _editableComic.Author = txtAuthor.Text.Trim();
                    _editableComic.Isbn = txtIsbn.Text.Trim();
                    _editableComic.Genre = txtGenre.Text.Trim();
                    _comicService.UpdateComic(_editableComic);
                    LogActivity($"Comic ID: {_editableComic.Id} updated successfully.");
                    MessageBox.Show("漫畫資料已更新。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    LogActivity("Attempting to add new comic.");
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
                    LogActivity($"New comic '{newComic.Title}' (ID: {newComic.Id}) added successfully.");
                    MessageBox.Show("漫畫已成功新增。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                LogActivity("ComicEditForm closing with DialogResult.OK.");
                this.Close();
            }
            catch (Exception ex)
            {
                LogErrorActivity($"Error while saving comic: {ex.Message}", ex);
                MessageBox.Show($"儲存漫畫時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            LogActivity("ComicEditForm finished loading.");
        }
    }
}