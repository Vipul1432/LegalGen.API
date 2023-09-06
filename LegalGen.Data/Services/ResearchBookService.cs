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
    }
}
