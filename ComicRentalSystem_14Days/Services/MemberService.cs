// ComicRentalSystem_14Days/Services/MemberService.cs
using ComicRentalSystem_14Days.Helpers;
using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComicRentalSystem_14Days.Services
{
    public class MemberService
    {
        private readonly FileHelper _fileHelper;
        private readonly string _memberFileName = "members.csv";
        private List<Member> _members = new List<Member> { };
        private readonly ILogger _logger; // 保持 non-nullable

        public delegate void MemberDataChangedEventHandler(object? sender, EventArgs e);
        public event MemberDataChangedEventHandler? MembersChanged;

        // 將 ILogger logger 改為 ILogger? logger
        public MemberService(FileHelper fileHelper, ILogger? logger)
        {
            _fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null for MemberService.");

            #pragma warning disable CS8602 // 暫時禁用「可能 null 參考的取值」警告 (因為我們知道 _logger 在此處非 null)
            _logger.Log("MemberService initializing."); // 這是您收到警告的那一行 (請確認行號)
            #pragma warning restore CS8602 // 恢復警告檢查

            LoadMembers();
            _logger.Log($"MemberService initialized. Loaded {_members.Count} members.");
        }
        private void LoadMembers()
        {
            _logger.Log($"Attempting to load members from file: '{_memberFileName}'.");
            try
            {
                _members = _fileHelper.ReadCsvFile<Member>(_memberFileName, Member.FromCsvString);
                _logger.Log($"Successfully loaded {_members.Count} members from '{_memberFileName}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading members from '{_memberFileName}'. Initializing with an empty list.", ex);
                _members = new List<Member>();
            }
        }

        private void SaveMembers()
        {
            _logger.Log($"Attempting to save {_members.Count} members to file: '{_memberFileName}'.");
            try
            {
                _fileHelper.WriteCsvFile<Member>(_memberFileName, _members, member => member.ToCsvString());
                _logger.Log($"Successfully saved {_members.Count} members to '{_memberFileName}'.");
                OnMembersChanged();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving members to '{_memberFileName}'.", ex);
                throw;
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
            return new List<Member>(_members);
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

            if (member.Id != 0 && _members.Any(m => m.Id == member.Id))
            {
                var ex = new InvalidOperationException($"Member with ID {member.Id} already exists.");
                _logger.LogError($"Failed to add member: ID {member.Id} (Name='{member.Name}') already exists.", ex);
                throw ex;
            }
            if (_members.Any(m => m.PhoneNumber == member.PhoneNumber))
            {
                _logger.Log($"Warning: A member with the same phone number '{member.PhoneNumber}' already exists (Name='{_members.First(m => m.PhoneNumber == member.PhoneNumber).Name}').");
            }

            if (member.Id == 0)
            {
                member.Id = GetNextId();
                _logger.Log($"Generated new ID {member.Id} for member '{member.Name}'.");
            }

            _members.Add(member);
            _logger.Log($"Member '{member.Name}' (ID: {member.Id}) added to in-memory list. Total members: {_members.Count}.");
            SaveMembers();
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

            existingMember.Name = member.Name;
            existingMember.PhoneNumber = member.PhoneNumber;
            _logger.Log($"Member properties for ID {member.Id} (Name='{existingMember.Name}') updated in-memory.");

            SaveMembers();
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

            _members.Remove(memberToRemove);
            _logger.Log($"Member '{memberToRemove.Name}' (ID: {id}) removed from in-memory list. Total members: {_members.Count}.");
            SaveMembers();
        }

        public int GetNextId()
        {
            int nextId = !_members.Any() ? 1 : _members.Max(m => m.Id) + 1;
            _logger.Log($"Next available member ID determined as: {nextId}.");
            return nextId;
        }

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