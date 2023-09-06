using AutoMapper;
using LegalGen.Data.Context;
using LegalGen.Domain.DTOs;
using LegalGen.Domain.Interfaces;
using LegalGen.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Data.Repository
{
    public class ResearchBookRepository : IResearchBookRepository
    {
        private readonly LegalGenDbContext _context;
        private readonly IMapper _mapper;

        public ResearchBookRepository(LegalGenDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all research books.
        /// </summary>
        /// <returns>A collection of research books.</returns>
        public async Task<IEnumerable<ResearchBook>> GetAllResearchBooksAsync()
        {
            return await _context.ResearchBooks.Include(rb => rb.LegalInformation).ToListAsync();
        }

        /// <summary>
        /// Get a research book by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the research book to retrieve.</param>
        /// <returns>The research book with the specified ID.</returns>
        public async Task<ResearchBook> GetResearchBookByIdAsync(int id)
        {
            return await _context.ResearchBooks.FindAsync(id);
        }

        /// <summary>
        /// Update an existing research book.
        /// </summary>
        /// <param name="id">The ID of the research book to update.</param>
        /// <param name="researchBook">The updated research book data.</param>
        /// <returns>The updated research book.</returns>
        public async Task<ResearchBook> UpdateResearchBookAsync(int id, ResearchBook researchBook)
        {
            var book = await _context.ResearchBooks.FirstOrDefaultAsync(rb => rb.Id == id);
            _mapper.Map<ResearchBook>(researchBook);
            book.LastModified = DateTime.Now;
            

            await _context.SaveChangesAsync();
            return researchBook;
        }

        /// <summary>
        /// Create a new research book.
        /// </summary>
        /// <param name="researchBook">The research book to create.</param>
        /// <returns>The newly created research book.</returns>
        public async Task<ResearchBook> CreateResearchBookAsync(ResearchBook researchBook)
        {
            _context.ResearchBooks.Add(researchBook);
            await _context.SaveChangesAsync();
            return researchBook;
        }

        /// <summary>
        /// Delete a research book by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the research book to delete.</param>
        /// <returns>True if the research book was successfully deleted; otherwise, false.</returns>
        public async Task<bool> DeleteResearchBookAsync(int id)
        {
            var researchBook = await _context.ResearchBooks.FindAsync(id);

            if (researchBook == null)
            {
                return false;
            }

            _context.ResearchBooks.Remove(researchBook);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Get all research books associated with a user.
        /// </summary>
        /// <param name="userId">The ID of the user whose research books are to be retrieved.</param>
        /// <returns>A collection of research books owned by the user.</returns>
        public async Task<IEnumerable<ResearchBook>> GetResearchBooksByUserIdAsync(string userId)
        {
            return await _context.ResearchBooks.Where(rb => rb.UserId == userId).ToListAsync();
        }
    }
}
