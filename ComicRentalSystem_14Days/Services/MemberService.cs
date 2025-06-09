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
        private readonly ILogger _logger;
        private readonly IComicService _comicService;

        public delegate void MemberDataChangedEventHandler(object? sender, EventArgs e);
        public event MemberDataChangedEventHandler? MembersChanged;

        public MemberService(ILogger logger, IComicService comicService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "MemberService logger cannot be null.");
            _comicService = comicService ?? throw new ArgumentNullException(nameof(comicService));

            _logger.Log("MemberService initialized (DbContext will be created per-operation).");
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

        [Obsolete("Use asynchronous version GetAllMembersAsync instead.")]
        public List<Member> GetAllMembers()
        {
            _logger.Log("GetAllMembers called (obsolete).");
            using (var context = new ComicRentalDbContext())
            {
                return context.Members.OrderBy(m => m.Name).ToList();
            }
        }

        public async Task<List<Member>> GetAllMembersAsync()
        {
            _logger.Log("GetAllMembersAsync called.");
            using (var context = new ComicRentalDbContext())
            {
                return await context.Members.AsNoTracking().OrderBy(m => m.Name).ToListAsync();
            }
        }

        [Obsolete("Use asynchronous version GetMemberByIdAsync instead.")]
        public Member? GetMemberById(int id)
        {
            _logger.Log($"GetMemberById (obsolete) called for ID: {id}.");
            using (var context = new ComicRentalDbContext())
            {
                var member = context.Members.Find(id);
                if (member == null)
                {
                _logger.Log($"GetMemberById (obsolete): Member with ID: {id} not found.");
            }
            else
            {
                _logger.Log($"GetMemberById (obsolete): Member with ID: {id} found: Name='{member.Name}'.");
            }
            return member;
            }
        }

        public async Task<Member?> GetMemberByIdAsync(int id)
        {
            _logger.Log($"GetMemberByIdAsync called for ID: {id}.");
            using (var context = new ComicRentalDbContext())
            {
                var member = await context.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
                if (member == null)
                {
                _logger.Log($"GetMemberByIdAsync: Member with ID: {id} not found.");
            }
            else
            {
                _logger.Log($"GetMemberByIdAsync: Member with ID: {id} found: Name='{member.Name}'.");
            }
            return member;
            }
        }

        [Obsolete("Use asynchronous version AddMemberAsync instead.")]
        public void AddMember(Member member)
        {
            if (member == null)
            {
                var ex = new ArgumentNullException(nameof(member));
                _logger.LogError("AddMember (obsolete): Attempted to add a null member object.", ex);
                throw ex;
            }

            if (string.IsNullOrWhiteSpace(member.Name) || string.IsNullOrWhiteSpace(member.PhoneNumber))
            {
                var ex = new ArgumentException("AddMember (obsolete): Member Name and PhoneNumber cannot be empty.", nameof(member));
                _logger.LogError("AddMember (obsolete): Name or PhoneNumber is empty.", ex);
                throw ex;
            }

            _logger.Log($"AddMember (obsolete): Attempting to add member: Name='{member.Name}'. ID will be DB-generated if {member.Id} is 0.");
            using (var context = new ComicRentalDbContext())
            {
                if (context.Members.Any(m => m.PhoneNumber == member.PhoneNumber))
                {
                    _logger.LogWarning($"AddMember (obsolete): Member with phone number '{member.PhoneNumber}' already exists.");
                }
                context.Members.Add(member);
                try
                {
                    context.SaveChanges();
                    _logger.Log($"AddMember (obsolete): Member '{member.Name}' (ID: {member.Id}) added to database.");
                    OnMembersChanged();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError($"AddMember (obsolete): Error adding member '{member.Name}' to database.", ex);
                    throw new InvalidOperationException($"AddMember (obsolete): Could not add member. Possible constraint violation.", ex);
                }
            }
        }

        public async Task AddMemberAsync(Member member)
        {
            if (member == null)
            {
                var ex = new ArgumentNullException(nameof(member));
                _logger.LogError("AddMemberAsync: Attempted to add a null member object.", ex);
                throw ex;
            }

            if (string.IsNullOrWhiteSpace(member.Name) || string.IsNullOrWhiteSpace(member.PhoneNumber))
            {
                var ex = new ArgumentException("AddMemberAsync: Member Name and PhoneNumber cannot be empty.", nameof(member));
                _logger.LogError("AddMemberAsync: Name or PhoneNumber is empty.", ex);
                throw ex;
            }

            _logger.Log($"AddMemberAsync: Attempting to add member: Name='{member.Name}'.");
            using (var context = new ComicRentalDbContext())
            {
                // Consider async check for phone number if it's a critical validation before Add.
                // For now, keeping behavior similar to original regarding existing phone numbers (log warning, allow add).
                if (await context.Members.AnyAsync(m => m.PhoneNumber == member.PhoneNumber))
                {
                    _logger.LogWarning($"AddMemberAsync: Member with phone number '{member.PhoneNumber}' already exists.");
                }
                context.Members.Add(member);
                try
                {
                    await context.SaveChangesAsync();
                    _logger.Log($"AddMemberAsync: Member '{member.Name}' (ID: {member.Id}) added to database.");
                    OnMembersChanged();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError($"AddMemberAsync: Error adding member '{member.Name}' to database.", ex);
                    throw new InvalidOperationException($"AddMemberAsync: Could not add member. Possible constraint violation.", ex);
                }
            }
        }

        [Obsolete("Use asynchronous version UpdateMemberAsync instead.")]
        public void UpdateMember(Member member)
        {
            if (member == null)
            {
                var ex = new ArgumentNullException(nameof(member));
                _logger.LogError("UpdateMember (obsolete): Attempted to update with a null member object.", ex);
                throw ex;
            }

            _logger.Log($"UpdateMember (obsolete): Attempting to update member ID: {member.Id} (Name='{member.Name}').");
            using (var context = new ComicRentalDbContext())
            {
                var existingMember = context.Members.Find(member.Id);
                if (existingMember == null)
                {
                    var ex = new InvalidOperationException($"UpdateMember (obsolete): Cannot update: Member with ID {member.Id} not found.");
                    _logger.LogError(ex.Message, ex);
                    throw ex;
                }

                if (context.Members.Any(m => m.PhoneNumber == member.PhoneNumber && m.Id != member.Id))
                {
                    _logger.LogWarning($"UpdateMember (obsolete): Another member with phone number '{member.PhoneNumber}' already exists.");
                }

                context.Entry(existingMember).CurrentValues.SetValues(member);
                try
                {
                    context.SaveChanges();
                    _logger.Log($"UpdateMember (obsolete): Member ID: {existingMember.Id} updated in database.");
                    OnMembersChanged();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError($"UpdateMember (obsolete): Error updating member ID: {existingMember.Id} in database.", ex);
                    throw new InvalidOperationException($"UpdateMember (obsolete): Could not update member. Possible constraint violation.", ex);
                }
            }
        }

        public async Task UpdateMemberAsync(Member member)
        {
            if (member == null)
            {
                var ex = new ArgumentNullException(nameof(member));
                _logger.LogError("UpdateMemberAsync: Attempted to update with a null member object.", ex);
                throw ex;
            }

            _logger.Log($"UpdateMemberAsync: Attempting to update member ID: {member.Id} (Name='{member.Name}').");
            using (var context = new ComicRentalDbContext())
            {
                var existingMember = await context.Members.FindAsync(member.Id);
                if (existingMember == null)
                {
                    var ex = new InvalidOperationException($"UpdateMemberAsync: Cannot update: Member with ID {member.Id} not found.");
                    _logger.LogError(ex.Message, ex);
                    throw ex;
                }

                // Async check for phone number uniqueness
                if (await context.Members.AnyAsync(m => m.PhoneNumber == member.PhoneNumber && m.Id != member.Id))
                {
                    _logger.LogWarning($"UpdateMemberAsync: Another member with phone number '{member.PhoneNumber}' already exists.");
                }

                context.Entry(existingMember).CurrentValues.SetValues(member);
                try
                {
                    await context.SaveChangesAsync();
                    _logger.Log($"UpdateMemberAsync: Member ID: {existingMember.Id} updated in database.");
                    OnMembersChanged();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError($"UpdateMemberAsync: Error updating member ID: {existingMember.Id} in database.", ex);
                    throw new InvalidOperationException($"UpdateMemberAsync: Could not update member. Possible constraint violation.", ex);
                }
            }
        }

        [Obsolete("Use asynchronous version DeleteMemberAsync instead.")]
        public void DeleteMember(int id)
        {
            _logger.Log($"DeleteMember (obsolete): Attempting to delete member with ID: {id}.");
            using (var context = new ComicRentalDbContext())
            {
                var memberToRemove = context.Members.Find(id);
                if (memberToRemove == null)
                {
                    var ex = new InvalidOperationException($"DeleteMember (obsolete): Cannot delete: Member with ID {id} not found.");
                _logger.LogWarning(ex.Message);
                throw ex;
            }

            // This still uses the synchronous _comicService.GetAllComics() which might be an issue
            // if ComicService itself is being made fully async and that method becomes obsolete.
            // For this step, we are focusing on MemberService, but this is a dependency to note.
            var allComics = _comicService.GetAllComics();
            if (allComics.Any(c => c.IsRented && c.RentedToMemberId == id))
            {
                _logger.LogWarning($"DeleteMember (obsolete): Attempt to delete member ID {id} ('{memberToRemove.Name}') with active rentals was blocked.");
                throw new InvalidOperationException("DeleteMember (obsolete): Cannot delete member: Member has active comic rentals.");
            }

            context.Members.Remove(memberToRemove);
            try
            {
                context.SaveChanges();
                _logger.Log($"DeleteMember (obsolete): Member ID: {id} deleted from database.");
                OnMembersChanged();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"DeleteMember (obsolete): Error deleting member ID: {id} from database.", ex);
                throw new InvalidOperationException($"DeleteMember (obsolete): Could not delete member. It might be in use or a DB error occurred.", ex);
            }
            }
        }

        public async Task DeleteMemberAsync(int id)
        {
            _logger.Log($"DeleteMemberAsync: Attempting to delete member with ID: {id}.");
            using (var context = new ComicRentalDbContext())
            {
                var memberToRemove = await context.Members.FindAsync(id);
                if (memberToRemove == null)
                {
                    var ex = new InvalidOperationException($"DeleteMemberAsync: Cannot delete: Member with ID {id} not found.");
                    _logger.LogWarning(ex.Message);
                    throw ex;
                }

                // Asynchronous check for active rentals directly using the context
                if (await context.Comics.AnyAsync(c => c.IsRented && c.RentedToMemberId == id))
                {
                    _logger.LogWarning($"DeleteMemberAsync: Attempt to delete member ID {id} ('{memberToRemove.Name}') with active rentals was blocked.");
                    throw new InvalidOperationException("DeleteMemberAsync: Cannot delete member: Member has active comic rentals.");
                }

                context.Members.Remove(memberToRemove);
                try
                {
                    await context.SaveChangesAsync();
                    _logger.Log($"DeleteMemberAsync: Member ID: {id} deleted from database.");
                    OnMembersChanged();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError($"DeleteMemberAsync: Error deleting member ID: {id} from database.", ex);
                    throw new InvalidOperationException($"DeleteMemberAsync: Could not delete member. It might be in use or a DB error occurred.", ex);
                }
            }
        }

        [Obsolete("Use asynchronous version GetMemberByNameAsync instead.")]
        public Member? GetMemberByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.Log("GetMemberByName (obsolete) called with empty name.");
                return null;
            }
            _logger.Log($"GetMemberByName (obsolete) called for name: '{name}'.");
            using (var context = new ComicRentalDbContext())
            {
                Member? member = context.Members.FirstOrDefault(m => m.Name.ToUpperInvariant() == name.ToUpperInvariant());
                if (member == null)
                {
                _logger.Log($"GetMemberByName (obsolete): Member with name: '{name}' not found.");
            }
            else
            {
                _logger.Log($"GetMemberByName (obsolete): Member with name: '{name}' found: ID='{member.Id}'.");
            }
            return member;
            }
        }

        public async Task<Member?> GetMemberByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.Log("GetMemberByNameAsync called with empty name.");
                return null;
            }
            _logger.Log($"GetMemberByNameAsync called for name: '{name}'.");
            using (var context = new ComicRentalDbContext())
            {
                Member? member = await context.Members.AsNoTracking()
                                         .FirstOrDefaultAsync(m => m.Name.ToUpperInvariant() == name.ToUpperInvariant());
                if (member == null)
                {
                _logger.Log($"GetMemberByNameAsync: Member with name: '{name}' not found.");
            }
            else
            {
                _logger.Log($"GetMemberByNameAsync: Member with name: '{name}' found: ID='{member.Id}'.");
            }
            return member;
            }
        }

        [Obsolete("Use asynchronous version GetMemberByPhoneNumberAsync instead.")]
        public Member? GetMemberByPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                _logger.Log("GetMemberByPhoneNumber (obsolete) called with empty phone number.");
                return null;
            }
            _logger.Log($"GetMemberByPhoneNumber (obsolete) called for phone: '{phoneNumber}'.");
            using (var context = new ComicRentalDbContext())
            {
                var member = context.Members.FirstOrDefault(m => m.PhoneNumber.Equals(phoneNumber));
                if (member == null)
                {
                _logger.Log($"GetMemberByPhoneNumber (obsolete): Member with phone: '{phoneNumber}' not found.");
            }
            else
            {
                _logger.Log($"GetMemberByPhoneNumber (obsolete): Member with phone: '{phoneNumber}' found: ID='{member.Id}', Name='{member.Name}'.");
            }
            return member;
            }
        }

        public async Task<Member?> GetMemberByPhoneNumberAsync(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                _logger.Log("GetMemberByPhoneNumberAsync called with empty phone number.");
                return null;
            }
            _logger.Log($"GetMemberByPhoneNumberAsync called for phone: '{phoneNumber}'.");
            using (var context = new ComicRentalDbContext())
            {
                var member = await context.Members.AsNoTracking()
                                             .FirstOrDefaultAsync(m => m.PhoneNumber.Equals(phoneNumber));
                if (member == null)
                {
                _logger.Log($"GetMemberByPhoneNumberAsync: Member with phone: '{phoneNumber}' not found.");
            }
            else
            {
                _logger.Log($"GetMemberByPhoneNumberAsync: Member with phone: '{phoneNumber}' found: ID='{member.Id}', Name='{member.Name}'.");
            }
            return member;
            }
        }

        [Obsolete("Use asynchronous version GetMemberByUsernameAsync instead.")]
        public Member? GetMemberByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogWarning("GetMemberByUsername (obsolete) called with empty or whitespace username.");
                return null;
            }
            _logger.Log($"GetMemberByUsername (obsolete) called for username: '{username}'.");
            // The original GetAllMembers() call here is inefficient as it fetches all members.
            // This method will now use its own context.
            using (var context = new ComicRentalDbContext())
            {
                // Replicating the inefficient GetAllMembers() call within the new context for this obsolete method.
                var allMembers = context.Members.ToList(); // Simplified, original GetAllMembers had OrderBy

                if (allMembers == null || !allMembers.Any())
                {
                    _logger.LogWarning("GetMemberByUsername (obsolete): No members available for search.");
                    return null;
                }

                string userUpper = username.ToUpperInvariant();
                Member? foundMember = allMembers.FirstOrDefault(m =>
                    m.Username != null && m.Username.ToUpperInvariant() == userUpper
                );

                if (foundMember != null)
                {
                    _logger.Log($"GetMemberByUsername (obsolete): Member with username: '{username}' found: ID='{foundMember.Id}'.");
                }
                else
                {
                    _logger.Log($"GetMemberByUsername (obsolete): Member with username: '{username}' not found.");
                }
                return foundMember;
            }
        }

        public async Task<Member?> GetMemberByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogWarning("GetMemberByUsernameAsync called with empty or whitespace username.");
                return null;
            }
            _logger.Log($"GetMemberByUsernameAsync called for username: '{username}'.");
            using (var context = new ComicRentalDbContext())
            {
                string userUpper = username.ToUpperInvariant();
                Member? foundMember = await context.Members.AsNoTracking()
                                              .FirstOrDefaultAsync(m => m.Username != null && m.Username.ToUpperInvariant() == userUpper);

                if (foundMember != null)
                {
                    _logger.Log($"GetMemberByUsernameAsync: Member with username: '{username}' found: ID='{foundMember.Id}'.");
                }
                else
                {
                    _logger.Log($"GetMemberByUsernameAsync: Member with username: '{username}' not found.");
                }
                return foundMember;
            }
        }

        [Obsolete("Use asynchronous version SearchMembersAsync instead.")]
        public List<Member> SearchMembers(string searchTerm)
        {
            _logger.Log($"SearchMembers (obsolete) called with searchTerm: '{searchTerm}'.");
            using (var context = new ComicRentalDbContext())
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    _logger.Log("SearchMembers (obsolete): SearchTerm is empty, returning all members ordered by Name.");
                    return context.Members.OrderBy(m => m.Name).ToList();
                }

                var lowerSearchTerm = searchTerm.ToLowerInvariant();
                var query = context.Members.Where(m =>
                    (m.Name != null && m.Name.ToLowerInvariant().Contains(lowerSearchTerm)) ||
                    (m.PhoneNumber != null && m.PhoneNumber.Contains(searchTerm)) ||
                    (m.Id.ToString().Equals(searchTerm)) ||
                    (m.Username != null && m.Username.ToLowerInvariant().Contains(lowerSearchTerm))
                );

                var results = query.OrderBy(m => m.Name).ToList();
                _logger.Log($"SearchMembers (obsolete): Found {results.Count} matching members.");
                return results;
            }
        }

        public async Task<List<Member>> SearchMembersAsync(string searchTerm)
        {
            _logger.Log($"SearchMembersAsync called with searchTerm: '{searchTerm}'.");
            using (var context = new ComicRentalDbContext())
            {
                var query = context.Members.AsQueryable();

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    _logger.Log("SearchMembersAsync: SearchTerm is empty, returning all members ordered by Name (AsNoTracking).");
                    return await query.AsNoTracking().OrderBy(m => m.Name).ToListAsync();
                }

                var lowerSearchTerm = searchTerm.ToLowerInvariant();
                // Apply AsNoTracking after Where predicates
                query = query.Where(m =>
                    (m.Name != null && m.Name.ToLowerInvariant().Contains(lowerSearchTerm)) ||
                    (m.PhoneNumber != null && m.PhoneNumber.Contains(searchTerm)) || // Phone is typically exact match, Contains might be too broad or slow if not indexed for it.
                    (m.Id.ToString().Equals(searchTerm)) || // Similar to ComicService, ID search on string conversion can be slow.
                    (m.Username != null && m.Username.ToLowerInvariant().Contains(lowerSearchTerm))
                ).AsNoTracking();

                var results = await query.OrderBy(m => m.Name).ToListAsync();
                _logger.Log($"SearchMembersAsync: Found {results.Count} matching members.");
                return results;
            }
        }
    }
}