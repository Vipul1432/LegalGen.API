using LegalGen.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Domain.Interfaces
{
    public interface IResearchBookRepository
    {
        /// <summary>
        /// Get all research books.
        /// </summary>
        /// <returns>A collection of research books.</returns>
        Task<IEnumerable<ResearchBook>> GetAllResearchBooksAsync();

        /// <summary>
        /// Get a research book by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the research book to retrieve.</param>
        /// <returns>The research book with the specified ID.</returns>
        Task<ResearchBook> GetResearchBookByIdAsync(int id);

        /// <summary>
        /// Update an existing research book.
        /// </summary>
        /// <param name="id">The ID of the research book to update.</param>
        /// <param name="researchBook">The updated research book data.</param>
        /// <returns>The updated research book.</returns>
        Task<ResearchBook> UpdateResearchBookAsync(int id, ResearchBook researchBook);

        /// <summary>
        /// Create a new research book.
        /// </summary>
        /// <param name="researchBook">The research book to create.</param>
        /// <returns>The newly created research book.</returns>
        Task<ResearchBook> CreateResearchBookAsync(ResearchBook researchBook);

        /// <summary>
        /// Delete a research book by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the research book to delete.</param>
        /// <returns>True if the research book was successfully deleted; otherwise, false.</returns>
        Task<bool> DeleteResearchBookAsync(int id);

        /// <summary>
        /// Get all research books associated with a user.
        /// </summary>
        /// <param name="userId">The ID of the user whose research books are to be retrieved.</param>
        /// <returns>A collection of research books owned by the user.</returns>
        Task<IEnumerable<ResearchBook>> GetResearchBooksByUserIdAsync(string userId);
    }
}
