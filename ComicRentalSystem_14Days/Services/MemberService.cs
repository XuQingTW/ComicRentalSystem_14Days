using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;
using System;
using System.Collections.Generic;
using System.IO; 
using System.Linq;
using System.Threading.Tasks; 
using System.Windows.Forms;

namespace ComicRentalSystem_14Days.Services
{
    public class MemberService
    {
        private readonly IFileHelper _fileHelper; 
        private readonly string _memberFileName = "members.csv";
        private List<Member> _members = new List<Member> { };
        private readonly ILogger _logger;
        private readonly IComicService _comicService;

        public delegate void MemberDataChangedEventHandler(object? sender, EventArgs e);
        public event MemberDataChangedEventHandler? MembersChanged;

        public MemberService(IFileHelper fileHelper, ILogger? logger, IComicService comicService)
        {
            _fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "MemberService 的記錄器不可為空。");
            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService)); 

            _logger.Log("MemberService 初始化中。");

            LoadMembersFromFile();
            _logger.Log($"MemberService 初始化完成。已載入 {_members.Count} 位會員。");
        }

        public async Task ReloadAsync() 
        {
            _logger.Log("MemberService 已呼叫 ReloadAsync。");
            _members = await LoadMembersAsync();
            OnMembersChanged();
            _logger.Log($"MemberService 已非同步重新載入。已載入 {_members.Count} 位會員。");
        }

        private void LoadMembersFromFile() 
        {
            _logger.Log($"正在嘗試從檔案載入會員 (同步): '{_memberFileName}'。");
            try
            {
                _members = _fileHelper.ReadFile<Member>(_memberFileName, Member.FromCsvString);
                _logger.Log($"成功從 '{_memberFileName}' (同步) 載入 {_members.Count} 位會員。");
            }
            catch (Exception ex) when (ex is FormatException || ex is IOException)
            {
                _logger.LogError($"嚴重錯誤: 會員資料檔案 '{_memberFileName}' (同步) 已損壞或無法讀取。詳細資訊: {ex.Message}", ex);
                throw new ApplicationException($"無法從 '{_memberFileName}' (同步) 載入會員資料。應用程式可能無法正常運作。", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"從 '{_memberFileName}' (同步) 載入會員時發生未預期的錯誤。詳細資訊: {ex.Message}", ex);
                throw new ApplicationException("載入會員資料期間 (同步) 發生未預期錯誤。", ex);
            }
        }

        private async Task<List<Member>> LoadMembersAsync()
        {
            _logger.Log($"正在嘗試從檔案非同步載入會員: '{_memberFileName}'。");
            try
            {
                string csvData = await _fileHelper.ReadFileAsync(_memberFileName);
                if (string.IsNullOrWhiteSpace(csvData))
                {
                    _logger.LogWarning($"會員檔案 '{_memberFileName}' (非同步) 為空或找不到。");
                    return new List<Member>();
                }

                var membersList = new List<Member>();
                var lines = csvData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines.Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    try
                    {
                        membersList.Add(Member.FromCsvString(line));
                    }
                    catch (FormatException formatEx)
                    {
                        _logger.LogError($"解析會員 CSV 行失敗 (非同步): '{line}'. 錯誤: {formatEx.Message}", formatEx);
                    }
                }
                _logger.Log($"成功從 '{_memberFileName}' (非同步) 載入並解析 {membersList.Count} 位會員。");
                return membersList;
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning($"會員檔案 '{_memberFileName}' (非同步) 找不到。返回空列表。");
                return new List<Member>();
            }
            catch (IOException ioEx)
            {
                _logger.LogError($"讀取會員檔案 '{_memberFileName}' (非同步) 時發生IO錯誤: {ioEx.Message}", ioEx);
                return new List<Member>(); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"從 '{_memberFileName}' (非同步) 載入會員時發生未預期的錯誤: {ex.Message}", ex);
                return new List<Member>(); 
            }
        }

        private void SaveMembers()
        {
            _logger.Log($"正在嘗試將 {_members.Count} 位會員儲存到檔案: '{_memberFileName}'。");
            try
            {
                _fileHelper.WriteFile<Member>(_memberFileName, _members, member => member.ToCsvString());
                _logger.Log($"已成功將 {_members.Count} 位會員儲存到 '{_memberFileName}'。");
                OnMembersChanged();
            }
            catch (Exception ex)
            {
                _logger.LogError($"將會員儲存到 '{_memberFileName}' 時發生錯誤。", ex);
                throw;
            }
        }

        protected virtual void OnMembersChanged()
        {
            MembersChanged?.Invoke(this, EventArgs.Empty);
            _logger.Log("已觸發 MembersChanged 事件。");
        }

        public List<Member> GetAllMembers()
        {
            _logger.Log("已呼叫 GetAllMembers。");
            return new List<Member>(_members);
        }

        public Member? GetMemberById(int id)
        {
            _logger.Log($"已為ID: {id} 呼叫 GetMemberById。");
            Member? member = _members.FirstOrDefault(m => m.Id == id);
            if (member == null)
            {
                _logger.Log($"找不到ID為: {id} 的會員。");
            }
            else
            {
                _logger.Log($"找到ID為: {id} 的會員: 姓名='{member.Name}'。");
            }
            return member;
        }

        public void AddMember(Member member)
        {
            if (member == null)
            {
                var ex = new ArgumentNullException(nameof(member));
                _logger.LogError("嘗試新增空的會員物件。", ex);
                throw ex;
            }

            _logger.Log($"正在嘗試新增會員: 姓名='{member.Name}', 電話號碼='{member.PhoneNumber}'。");

            if (member.Id != 0 && _members.Any(m => m.Id == member.Id))
            {
                var ex = new InvalidOperationException($"ID為 {member.Id} 的會員已存在。");
                _logger.LogError($"新增會員失敗: ID {member.Id} (姓名='{member.Name}') 已存在。", ex);
                throw ex;
            }
            if (_members.Any(m => m.PhoneNumber == member.PhoneNumber))
            {
                _logger.LogWarning($"電話號碼為 '{member.PhoneNumber}' 的會員已存在 (姓名='{_members.First(m => m.PhoneNumber == member.PhoneNumber).Name}')。繼續新增。");
            }

            if (member.Id == 0)
            {
                member.Id = GetNextId();
                _logger.Log($"已為會員 '{member.Name}' 產生新的ID {member.Id}。");
            }

            _members.Add(member);
            _logger.Log($"會員 '{member.Name}' (ID: {member.Id}) 已新增至記憶體列表。會員總數: {_members.Count}。");
            SaveMembers();
        }

        public void UpdateMember(Member member)
        {
            if (member == null)
            {
                var ex = new ArgumentNullException(nameof(member));
                _logger.LogError("嘗試使用空的會員物件進行更新。", ex);
                throw ex;
            }

            _logger.Log($"正在嘗試更新ID為: {member.Id} (姓名='{member.Name}') 的會員。");

            Member? existingMember = _members.FirstOrDefault(m => m.Id == member.Id);
            if (existingMember == null)
            {
                var ex = new InvalidOperationException($"找不到ID為 {member.Id} 的會員進行更新。");
                _logger.LogError($"更新會員失敗: 找不到ID {member.Id} (姓名='{member.Name}')。", ex);
                throw ex;
            }

            existingMember.Name = member.Name;
            existingMember.PhoneNumber = member.PhoneNumber;
            _logger.Log($"ID {member.Id} (姓名='{existingMember.Name}') 的會員屬性已在記憶體中更新。");

            SaveMembers();
            _logger.Log($"ID為: {member.Id} (姓名='{existingMember.Name}') 的會員更新已保存到檔案。");
        }

        public void DeleteMember(int id)
        {
            _logger.Log($"正在嘗試刪除ID為: {id} 的會員。");
            Member? memberToRemove = _members.FirstOrDefault(m => m.Id == id);

            if (memberToRemove == null)
            {
                var ex = new InvalidOperationException($"找不到ID為 {id} 的會員進行刪除。");
                _logger.LogError($"刪除會員失敗: 找不到ID {id}。", ex);
                throw ex;
            }

            var allComics = _comicService.GetAllComics();
            if (allComics.Any(c => c.IsRented && c.RentedToMemberId == id))
            {
                _logger.LogWarning($"已阻止刪除擁有有效租借紀錄的會員ID {id} ('{memberToRemove.Name}')。");
                throw new InvalidOperationException("無法刪除會員: 會員擁有有效的漫畫租借紀錄。");
            }

            _members.Remove(memberToRemove);
            _logger.Log($"會員 '{memberToRemove.Name}' (ID: {id}) 已從記憶體列表移除。會員總數: {_members.Count}。");
            SaveMembers();
        }

        public int GetNextId()
        {
            int nextId = !_members.Any() ? 1 : _members.Max(m => m.Id) + 1;
            _logger.Log($"下一個可用的會員ID已確定為: {nextId}。");
            return nextId;
        }

        public Member? GetMemberByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.Log("已呼叫 GetMemberByName，姓名為空。");
                return null;
            }
            _logger.Log($"已為姓名: '{name}' 呼叫 GetMemberByName。");
            Member? member = _members.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (member == null)
            {
                _logger.Log($"找不到姓名為: '{name}' 的會員。");
            }
            else
            {
                _logger.Log($"找到姓名為: '{name}' 的會員: ID='{member.Id}'。");
            }
            return member;
        }

        public Member? GetMemberByPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                _logger.Log("已呼叫 GetMemberByPhoneNumber，電話號碼為空。");
                return null;
            }
            _logger.Log($"已為電話號碼: '{phoneNumber}' 呼叫 GetMemberByPhoneNumber。");
            Member? member = _members.FirstOrDefault(m => m.PhoneNumber.Equals(phoneNumber));
            if (member == null)
            {
                _logger.Log($"找不到電話號碼為: '{phoneNumber}' 的會員。");
            }
            else
            {
                _logger.Log($"找到電話號碼為: '{phoneNumber}' 的會員: ID='{member.Id}', 姓名='{member.Name}'。");
            }
            return member;
        }

        public Member? GetMemberByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogWarning("已呼叫 GetMemberByUsername，使用者名稱為空或空白。");
                return null;
            }

            var allMembers = GetAllMembers();

            if (allMembers == null || !allMembers.Any())
            {
                _logger.LogWarning("GetMemberByUsername: 無可用會員進行搜尋。");
                return null;
            }

            Member? foundMember = allMembers.FirstOrDefault(m =>
                string.Equals(m.Username, username, StringComparison.OrdinalIgnoreCase)
            );

            if (foundMember != null)
            {
                _logger.Log($"GetMemberByUsername: 找到ID為 {foundMember.Id} 且使用者名稱為 '{username}' 的會員。");
            }
            else
            {
                _logger.Log($"GetMemberByUsername: 找不到使用者名稱為 '{username}' 的會員。");
            }

            return foundMember;
        }

        public List<Member> SearchMembers(string searchTerm)
        {
            _logger.Log($"已呼叫 SearchMembers，搜尋詞: '{searchTerm}'。");
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<Member>(_members); 
            }

            var lowerSearchTerm = searchTerm.ToLowerInvariant();
            List<Member> results = _members.Where(m =>
                (m.Name != null && m.Name.ToLowerInvariant().Contains(lowerSearchTerm)) ||
                (m.PhoneNumber != null && m.PhoneNumber.Contains(searchTerm)) ||
                (m.Id.ToString().Equals(searchTerm)) ||
                (m.Username != null && m.Username.ToLowerInvariant().Contains(lowerSearchTerm)) 
            ).ToList();

            _logger.Log($"SearchMembers 找到 {results.Count} 位符合條件的會員。");
            return results;
        }
    }
}