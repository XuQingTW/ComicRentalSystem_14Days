using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces; // 技術點3: 引用介面命名空間
using ComicRentalSystem_14Days.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComicRentalSystem_14Days.Services
{
    public class MemberService
    {
        private readonly FileHelper _fileHelper;
        private readonly string _memberFileName = "members.csv"; // 會員資料檔名
        private List<Member> _members; // 記憶體中的會員快取
        private readonly ILogger _logger; // 技術點3: 介面 - 儲存 ILogger 實例

        // 技術點5: 委派與事件
        public delegate void MemberDataChangedEventHandler(object? sender, EventArgs e);
        public event MemberDataChangedEventHandler? MembersChanged;

        // 建構函式修改以接收 ILogger
        // 技術點4: 多型 (可以傳入任何實作 ILogger 的物件)
        public MemberService(FileHelper fileHelper, ILogger logger)
        {
            _fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // 初始化 Logger

            _logger.Log("MemberService initializing."); // 技術點3 & 4: 透過介面呼叫 Log()
            LoadMembers();
            _logger.Log($"MemberService initialized. Loaded {_members.Count} members.");
        }

        private void LoadMembers()
        {
            _logger.Log($"Attempting to load members from file: '{_memberFileName}'.");
            try
            {
                // 技術點8: 檔案與資料夾處理 (間接透過 FileHelper)
                _members = _fileHelper.ReadCsvFile<Member>(_memberFileName, Member.FromCsvString);
                _logger.Log($"Successfully loaded {_members.Count} members from '{_memberFileName}'.");
            }
            catch (Exception ex) // 技術點5: 例外處理
            {
                // 技術點4: 過載 & 多型 (使用 LogError 的過載，透過 ILogger 介面呼叫)
                _logger.LogError($"Error loading members from '{_memberFileName}'. Initializing with an empty list.", ex);
                _members = new List<Member>(); // 發生錯誤時，使用空列表以避免程式崩潰
            }
        }

        private void SaveMembers()
        {
            _logger.Log($"Attempting to save {_members.Count} members to file: '{_memberFileName}'.");
            try
            {
                // 技術點8: 檔案與資料夾處理 (間接透過 FileHelper)
                _fileHelper.WriteCsvFile<Member>(_memberFileName, _members, member => member.ToCsvString());
                _logger.Log($"Successfully saved {_members.Count} members to '{_memberFileName}'.");
                OnMembersChanged(); // 觸發資料變更事件
            }
            catch (Exception ex) // 技術點5: 例外處理
            {
                _logger.LogError($"Error saving members to '{_memberFileName}'.", ex);
                throw; // 重新拋出，讓呼叫者（如UI層）知道儲存失敗並可以適當處理
            }
        }

        protected virtual void OnMembersChanged()
        {
            MembersChanged?.Invoke(this, EventArgs.Empty);
            _logger.Log("MembersChanged event invoked.");
        }

        public List<Member> GetAllMembers()
        {
            _logger.Log("GetAllMembers called.");
            return new List<Member>(_members); // 返回複本
        }

        public Member? GetMemberById(int id)
        {
            _logger.Log($"GetMemberById called for ID: {id}.");
            Member? member = _members.FirstOrDefault(m => m.Id == id);
            if (member == null)
            {
                _logger.Log($"Member with ID: {id} not found.");
            }
            else
            {
                _logger.Log($"Member with ID: {id} found: Name='{member.Name}'.");
            }
            return member;
        }

        public void AddMember(Member member)
        {
            if (member == null)
            {
                var ex = new ArgumentNullException(nameof(member));
                _logger.LogError("Attempted to add a null member object.", ex);
                throw ex;
            }

            _logger.Log($"Attempting to add new member: Name='{member.Name}', PhoneNumber='{member.PhoneNumber}'.");

            // 檢查ID是否已存在 (如果ID非0)
            if (member.Id != 0 && _members.Any(m => m.Id == member.Id))
            {
                var ex = new InvalidOperationException($"Member with ID {member.Id} already exists.");
                _logger.LogError($"Failed to add member: ID {member.Id} (Name='{member.Name}') already exists.", ex);
                throw ex;
            }
            // 檢查是否有相同電話號碼的會員 (可選，取決於業務邏輯)
            if (_members.Any(m => m.PhoneNumber == member.PhoneNumber))
            {
                _logger.Log($"Warning: A member with the same phone number '{member.PhoneNumber}' already exists (Name='{_members.First(m => m.PhoneNumber == member.PhoneNumber).Name}').");
                // 根據需求，這裡可以選擇拋出例外或僅記錄警告
            }


            if (member.Id == 0)
            {
                member.Id = GetNextId(); // GetNextId 內部已有日誌
                _logger.Log($"Generated new ID {member.Id} for member '{member.Name}'.");
            }

            _members.Add(member);
            _logger.Log($"Member '{member.Name}' (ID: {member.Id}) added to in-memory list. Total members: {_members.Count}.");
            SaveMembers(); // SaveMembers 內部已有日誌記錄
        }

        public void UpdateMember(Member member)
        {
            if (member == null)
            {
                var ex = new ArgumentNullException(nameof(member));
                _logger.LogError("Attempted to update with a null member object.", ex);
                throw ex;
            }

            _logger.Log($"Attempting to update member with ID: {member.Id} (Name='{member.Name}').");

            Member? existingMember = _members.FirstOrDefault(m => m.Id == member.Id);
            if (existingMember == null)
            {
                var ex = new InvalidOperationException($"Member with ID {member.Id} not found for update.");
                _logger.LogError($"Failed to update member: ID {member.Id} (Name='{member.Name}') not found.", ex);
                throw ex;
            }

            // 更新屬性
            existingMember.Name = member.Name;
            existingMember.PhoneNumber = member.PhoneNumber;
            _logger.Log($"Member properties for ID {member.Id} (Name='{existingMember.Name}') updated in-memory.");

            SaveMembers(); // SaveMembers 內部已有日誌記錄
            _logger.Log($"Member with ID: {member.Id} (Name='{existingMember.Name}') update persisted to file.");
        }

        public void DeleteMember(int id)
        {
            _logger.Log($"Attempting to delete member with ID: {id}.");
            Member? memberToRemove = _members.FirstOrDefault(m => m.Id == id);

            if (memberToRemove == null)
            {
                var ex = new InvalidOperationException($"Member with ID {id} not found for deletion.");
                _logger.LogError($"Failed to delete member: ID {id} not found.", ex);
                throw ex;
            }

            // 之後 Day 9-10: 可以在此處加入檢查該會員是否有未歸還的漫畫
            // _logger.Log($"Checking if member ID {id} (Name='{memberToRemove.Name}') has open rentals before deletion.");
            // if (HasOpenRentals(id)) // 假設未來有此方法
            // {
            //     var rentalEx = new InvalidOperationException($"Member '{memberToRemove.Name}' (ID: {id}) has open rentals and cannot be deleted.");
            //     _logger.LogError(rentalEx.Message, rentalEx);
            //     throw rentalEx;
            // }

            _members.Remove(memberToRemove);
            _logger.Log($"Member '{memberToRemove.Name}' (ID: {id}) removed from in-memory list. Total members: {_members.Count}.");
            SaveMembers(); // SaveMembers 內部已有日誌記錄
        }

        public int GetNextId()
        {
            int nextId = !_members.Any() ? 1 : _members.Max(m => m.Id) + 1;
            _logger.Log($"Next available member ID determined as: {nextId}.");
            return nextId;
        }

        // 技術點4: 過載 - 示範一個服務層的過載方法
        // 根據姓名搜尋會員 (精確符合)
        public Member? GetMemberByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.Log("GetMemberByName called with empty name.");
                return null;
            }
            _logger.Log($"GetMemberByName called for name: '{name}'.");
            Member? member = _members.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (member == null)
            {
                _logger.Log($"Member with name: '{name}' not found.");
            }
            else
            {
                _logger.Log($"Member with name: '{name}' found: ID='{member.Id}'.");
            }
            return member;
        }

        // 過載版本：根據電話號碼搜尋會員
        // 技術點4: 過載
        public Member? GetMemberByPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                _logger.Log("GetMemberByPhoneNumber called with empty phone number.");
                return null;
            }
            _logger.Log($"GetMemberByPhoneNumber called for phone number: '{phoneNumber}'.");
            Member? member = _members.FirstOrDefault(m => m.PhoneNumber.Equals(phoneNumber));
            if (member == null)
            {
                _logger.Log($"Member with phone number: '{phoneNumber}' not found.");
            }
            else
            {
                _logger.Log($"Member with phone number: '{phoneNumber}' found: ID='{member.Id}', Name='{member.Name}'.");
            }
            return member;
        }
    }
}