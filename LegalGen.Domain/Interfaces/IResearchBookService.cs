using LegalGen.Domain.DTOs;
using LegalGen.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Domain.Interfaces
{
    public interface IResearchBookService
    {
        /// <summary>
        /// Get all legal information within a specific research book.
        /// </summary>
        /// <param name="researchBookId">The ID of the research book to retrieve legal information from.</param>
        /// <returns>A collection of legal information DTOs.</returns>
        Task<IEnumerable<LegalInformationDto>> GetAllLegalInformationsInResearchBookAsync(int researchBookId);

        /// <summary>
        /// Add legal information to a specific research book.
        /// </summary>
        /// <param name="researchBookId">The ID of the research book to add legal information to.</param>
        /// <param name="legalInformationDto">The legal information DTO to add.</param>
        /// <returns>True if the legal information was added successfully; otherwise, false.</returns>
        Task<bool> AddLegalInformationToResearchBookAsync(int researchBookId, LegalInformationDto legalInformationDto);

        /// <summary>
        /// Update legal information within a research book.
        /// </summary>
        /// <param name="researchBookId">The ID of the research book containing the legal information.</param>
        /// <param name="legalInformationId">The ID of the legal information to update.</param>
        /// <param name="legalInformationDto">The legal information DTO containing updated information.</param>
        /// <returns>True if the legal information was updated successfully; otherwise, false.</returns>
        Task<bool> UpdateLegalInformationByResearchBookIdAsync(int researchBookId, int legalInformationId, LegalInformationDto legalInformationDto);

        /// <summary>
        /// Delete legal information from a research book.
        /// </summary>
        /// <param name="researchBookId">The ID of the research book containing the legal information.</param>
        /// <param name="legalInformationId">The ID of the legal information to delete.</param>
        /// <returns>True if the legal information was deleted successfully; otherwise, false.</returns>
        Task<bool> DeleteLegalInformationByResearchBookIdAsync(int researchBookId, int legalInformationId);

        /// <summary>
        /// Searches for legal information within research books based on specified search criteria.
        /// </summary>
        /// <param name="criteria">The search criteria including document type, title, and date.</param>
        /// <returns>An asynchronous task that represents the operation and returns a collection of matching legal information.</returns>
        Task<IEnumerable<LegalInformation>> SearchResearchBooksAsync(SearchCriteria criteria);

        /// <summary>
        /// Shares a research book with a list of users identified by their user IDs.
        /// </summary>
        /// <param name="bookId">The ID of the research book to be shared.</param>
        /// <param name="userIds">A list of user IDs representing the users with whom the book will be shared.</param>
        /// <returns>An asynchronous task representing the operation.</returns>
        Task ShareBookWithUsersAsync(int bookId, List<string> userIds);
    }
}
