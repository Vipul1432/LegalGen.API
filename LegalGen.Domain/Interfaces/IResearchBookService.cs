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


        Task<IEnumerable<LegalInformation>> SearchResearchBooksAsync(SearchCriteria criteria);
    }
}
