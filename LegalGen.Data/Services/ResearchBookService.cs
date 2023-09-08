using AutoMapper;
using LegalGen.Data.Context;
using LegalGen.Domain.DTOs;
using LegalGen.Domain.Interfaces;
using LegalGen.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LegalGen.Data.Services
{
    public class ResearchBookService : IResearchBookService
    {
        private readonly LegalGenDbContext _context;
        private readonly IMapper _mapper;

        public ResearchBookService(LegalGenDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all legal information within a specific research book.
        /// </summary>
        /// <param name="researchBookId">The ID of the research book to retrieve legal information from.</param>
        /// <returns>A collection of legal information DTOs.</returns>
        public async Task<IEnumerable<LegalInformationDto>> GetAllLegalInformationsInResearchBookAsync(int researchBookId)
        {
            var researchBook = await _context.ResearchBooks.Include(rb => rb.LegalInformation).FirstOrDefaultAsync(rb => rb.Id == researchBookId);

            if (researchBook == null)
            {
                return null;
            }

            // Map the LegalInformation entities to DTOs
            var legalInformationDtos = _mapper.Map<IEnumerable<LegalInformationDto>>(researchBook.LegalInformation);

            return legalInformationDtos;
        }

        /// <summary>
        /// Add legal information to a specific research book.
        /// </summary>
        /// <param name="researchBookId">The ID of the research book to add legal information to.</param>
        /// <param name="legalInformationDto">The legal information DTO to add.</param>
        /// <returns>True if the legal information was added successfully; otherwise, false.</returns>
        public async Task<bool> AddLegalInformationToResearchBookAsync(int researchBookId, LegalInformationDto legalInformationDto)
        {
            var researchBook = await _context.ResearchBooks.Include(rb => rb.LegalInformation).FirstOrDefaultAsync(rb => rb.Id == researchBookId);

            if (researchBook == null)
            {
                return false;
            }
            var legalInformation = _mapper.Map<LegalInformation>(legalInformationDto);

            // Add the legal information to the research book's LegalInformation collection
            researchBook.LegalInformation.Add(legalInformation);

            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Update legal information within a research book.
        /// </summary>
        /// <param name="researchBookId">The ID of the research book containing the legal information.</param>
        /// <param name="legalInformationId">The ID of the legal information to update.</param>
        /// <param name="legalInformationDto">The legal information DTO containing updated information.</param>
        /// <returns>True if the legal information was updated successfully; otherwise, false.</returns>
        public async Task<bool> UpdateLegalInformationByResearchBookIdAsync(int reserchBookId, int legalInformationId, LegalInformationDto legalInformationDto)
        {
            var researchBook = await _context.ResearchBooks.Include(rb => rb.LegalInformation).FirstOrDefaultAsync(rb => rb.Id == reserchBookId);
            if (researchBook == null)
            {
                return false;
            }
            var legalInformation = researchBook.LegalInformation.FirstOrDefault(li => li.Id == legalInformationId);
            if (legalInformation == null)
            {
                return false;
            }
            var updatedLegalInformation = _mapper.Map<LegalInformation>(legalInformationDto);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Delete legal information from a research book.
        /// </summary>
        /// <param name="researchBookId">The ID of the research book containing the legal information.</param>
        /// <param name="legalInformationId">The ID of the legal information to delete.</param>
        /// <returns>True if the legal information was deleted successfully; otherwise, false.</returns>
        public async Task<bool> DeleteLegalInformationByResearchBookIdAsync(int researchBookId, int legalInformationId)
        {
            var researchBook = await _context.ResearchBooks.Include(rb => rb.LegalInformation).FirstOrDefaultAsync(rb => rb.Id == researchBookId);
            if (researchBook == null)
            {
                return false;
            }
            var legalInformation = researchBook.LegalInformation.FirstOrDefault(li => li.Id == legalInformationId);
            if (legalInformation == null)
            {
                return false;
            }

            researchBook.LegalInformation.Remove(legalInformation);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Searches for legal information within research books based on specified search criteria.
        /// </summary>
        /// <param name="criteria">The search criteria including document type, title, and date.</param>
        /// <returns>An asynchronous task that represents the operation and returns a collection of matching legal information.</returns>
        public async Task<IEnumerable<LegalInformation>> SearchResearchBooksAsync(SearchCriteria criteria)
        {
            var query = _context.ResearchBooks.SelectMany(book => book.LegalInformation).AsQueryable();

            if (!string.IsNullOrEmpty(criteria?.DocumentType))
            {
                query = query.Where(li => li.Document == criteria.DocumentType);
            }

            if (!string.IsNullOrEmpty(criteria?.Title))
            {
                query = query.Where(li => li.Title == criteria.Title);
            }

            if (criteria?.Date != null)
            {
                DateTime criteriaDate = criteria.Date.Value;
                query = query.Where(li => li.DateAdded.Date == criteriaDate.Date);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Shares a research book with a list of users identified by their user IDs.
        /// </summary>
        /// <param name="bookId">The ID of the research book to be shared.</param>
        /// <param name="userIds">A list of user IDs representing the users with whom the book will be shared.</param>
        /// <returns>An asynchronous task representing the operation.</returns>
        public async Task ShareBookWithUsersAsync(int bookId, List<string> userIds)
        {
            var book = await _context.ResearchBooks.FindAsync(bookId);

            if (book == null)
            {
                // Handle the case where the book is not found.
                return;
            }

            foreach (var userId in userIds)
            {
                var user = await _context.Users.FindAsync(userId);

                if (user != null)
                {
                    // Create a UserResearchBookAssignment to share the book with the user.
                    var assignment = new ResearchBookShare
                    {
                        ResearchBook = book,
                        User = user
                    };

                    _context.ResearchBookShares.Add(assignment);
                }
                // Handle the case where the user is not found.
            }

            await _context.SaveChangesAsync();
        }
    }
}
