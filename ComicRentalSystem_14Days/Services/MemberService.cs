using ComicRentalSystem_14Days.Interfaces;
using ComicRentalSystem_14Days.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicRentalSystem_14Days.Services
{
    public class MemberService
    {
        private readonly ComicRentalDbContext _context;
        private readonly ILogger _logger;
        private readonly IComicService _comicService;

        public delegate void MemberDataChangedEventHandler(object? sender, EventArgs e);
        public event MemberDataChangedEventHandler? MembersChanged;

        public MemberService(ComicRentalDbContext context, ILogger logger, IComicService comicService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "MemberService logger cannot be null.");
            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));

            _logger.Log("MemberService initialized with ComicRentalDbContext.");
        }

        public async Task ReloadAsync()
        {
            _logger.Log("MemberService ReloadAsync requested. Data is now live from DB.");
            OnMembersChanged(); // Notify listeners
            await Task.CompletedTask; // Placeholder
        }

        protected virtual void OnMembersChanged()
        {
            MembersChanged?.Invoke(this, EventArgs.Empty);
            _logger.Log("MembersChanged event triggered.");
        }

        public List<Member> GetAllMembers()
        {
            _logger.Log("GetAllMembers called.");
            return _context.Members.OrderBy(m => m.Name).ToList();
        }

        public Member? GetMemberById(int id)
        {
            _logger.Log($"GetMemberById called for ID: {id}.");
            var member = _context.Members.Find(id);
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

            if (string.IsNullOrWhiteSpace(member.Name) || string.IsNullOrWhiteSpace(member.PhoneNumber))
            {
                var ex = new ArgumentException("Member Name and PhoneNumber cannot be empty.", nameof(member));
                _logger.LogError("AddMember failed: Name or PhoneNumber is empty.", ex);
                throw ex;
            }

            _logger.Log($"Attempting to add member: Name='{member.Name}'. ID will be DB-generated if {member.Id} is 0.");
            if (_context.Members.Any(m => m.PhoneNumber == member.PhoneNumber))
            {
                _logger.LogWarning($"AddMember: Member with phone number '{member.PhoneNumber}' already exists.");
                // Original logic allowed adding, so we maintain that.
                // For a stricter system, one might throw new InvalidOperationException($"Member with phone number '{member.PhoneNumber}' already exists.");
            }
            _context.Members.Add(member);
            try
            {
                _context.SaveChanges();
                _logger.Log($"Member '{member.Name}' (ID: {member.Id}) added to database.");
                OnMembersChanged();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error adding member '{member.Name}' to database.", ex);
                throw new InvalidOperationException($"Could not add member. Possible constraint violation (e.g., duplicate ID if {member.Id} was non-zero and existed).", ex);
            }
        }

        public void UpdateMember(Member member)
        {
            if (member == null)
            {
                var ex = new ArgumentNullException(nameof(member));
                _logger.LogError("Attempted to update with a null member object.", ex);
                throw ex;
            }

            _logger.Log($"Attempting to update member ID: {member.Id} (Name='{member.Name}').");
            var existingMember = _context.Members.Find(member.Id);
            if (existingMember == null)
            {
                var ex = new InvalidOperationException($"Cannot update: Member with ID {member.Id} not found.");
                _logger.LogError(ex.Message, ex);
                throw ex;
            }

            if (_context.Members.Any(m => m.PhoneNumber == member.PhoneNumber && m.Id != member.Id))
            {
                 _logger.LogWarning($"UpdateMember: Another member with phone number '{member.PhoneNumber}' already exists.");
                // Depending on business rules, might throw or just log.
            }

            _context.Entry(existingMember).CurrentValues.SetValues(member);
            try
            {
                _context.SaveChanges();
                _logger.Log($"Member ID: {existingMember.Id} updated in database.");
                OnMembersChanged();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error updating member ID: {existingMember.Id} in database.", ex);
                throw new InvalidOperationException($"Could not update member. Possible constraint violation.", ex);
            }
        }

        public void DeleteMember(int id)
        {
            _logger.Log($"Attempting to delete member with ID: {id}.");
            var memberToRemove = _context.Members.Find(id);
            if (memberToRemove == null)
            {
                var ex = new InvalidOperationException($"Cannot delete: Member with ID {id} not found.");
                _logger.LogWarning(ex.Message);
                throw ex;
            }

            var allComics = _comicService.GetAllComics();
            if (allComics.Any(c => c.IsRented && c.RentedToMemberId == id))
            {
                _logger.LogWarning($"Attempt to delete member ID {id} ('{memberToRemove.Name}') with active rentals was blocked.");
                throw new InvalidOperationException("Cannot delete member: Member has active comic rentals.");
            }

            _context.Members.Remove(memberToRemove);
            try
            {
                _context.SaveChanges();
                _logger.Log($"Member ID: {id} deleted from database.");
                OnMembersChanged();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error deleting member ID: {id} from database.", ex);
                throw new InvalidOperationException($"Could not delete member. It might be in use or a DB error occurred.", ex);
            }
        }

        public Member? GetMemberByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.Log("GetMemberByName called with empty name.");
                return null;
            }
            _logger.Log($"已為姓名: '{name}' 呼叫 GetMemberByName。");
            Member? member = _context.Members.FirstOrDefault(m => m.Name.ToUpperInvariant() == name.ToUpperInvariant());
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
            _logger.Log($"GetMemberByPhoneNumber called for phone: '{phoneNumber}'.");
            var member = _context.Members.FirstOrDefault(m => m.PhoneNumber.Equals(phoneNumber));
            if (member == null)
            {
                _logger.Log($"Member with phone: '{phoneNumber}' not found.");
            }
            else
            {
                _logger.Log($"Member with phone: '{phoneNumber}' found: ID='{member.Id}', Name='{member.Name}'.");
            }
            return member;
        }

        public Member? GetMemberByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogWarning("GetMemberByUsername called with empty or whitespace username.");
                return null;
            }
            var allMembers = GetAllMembers();

            if (allMembers == null || !allMembers.Any())
            {
                _logger.LogWarning("GetMemberByUsername: 無可用會員進行搜尋。");
                return null;
            }

            string userUpper = username.ToUpperInvariant();
            Member? foundMember = allMembers.FirstOrDefault(m =>
                m.Username != null && m.Username.ToUpperInvariant() == userUpper
            );

            if (foundMember != null)
            {
                _logger.Log($"Member with username: '{username}' found: ID='{foundMember.Id}'.");
            }
            else
            {
                _logger.Log($"Member with username: '{username}' not found.");
            }
            return foundMember;
        }

        public List<Member> SearchMembers(string searchTerm)
        {
            _logger.Log($"SearchMembers called with searchTerm: '{searchTerm}'.");
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.Log("SearchTerm is empty, returning all members ordered by Name.");
                return _context.Members.OrderBy(m => m.Name).ToList();
            }

            var lowerSearchTerm = searchTerm.ToLowerInvariant();
            var results = _context.Members.Where(m =>
                (m.Name != null && m.Name.ToLowerInvariant().Contains(lowerSearchTerm)) ||
                (m.PhoneNumber != null && m.PhoneNumber.Contains(searchTerm)) || // Phone is exact search here
                (m.Id.ToString().Equals(searchTerm)) ||
                (m.Username != null && m.Username.ToLowerInvariant().Contains(lowerSearchTerm))
            ).OrderBy(m => m.Name).ToList();

            _logger.Log($"SearchMembers found {results.Count} matching members.");
            return results;
        }
    }
}