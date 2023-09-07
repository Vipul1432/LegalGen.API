using LegalGen.Data.Context;
using LegalGen.Domain.Dtos;
using LegalGen.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using LegalGen.Domain.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using LegalGen.Domain.Models;

namespace LegalGen.Data.Services
{
    public class AiChatService : IAiChatService
    {
        private readonly LegalGenDbContext _context;
        public AiChatService(LegalGenDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Searches for relevant research books and legal information based on a user's input message.
        /// </summary>
        /// <param name="message">The user's input message to search for.</param>
        /// <param name="userId">The unique identifier of the user initiating the search.</param>
        /// <returns>An asynchronous task that represents the operation and returns the search results.</returns>
        public async Task<AiChatResponse> SearchAsync(string message, string userId)
        {
            var words = message.Split(' ');

            // Fetch ResearchBook data first
            var researchBooks = await _context.ResearchBooks.ToListAsync();

            var researchBookResults = researchBooks
                .Where(rb => words.Any(word => rb.Name.Contains(word)))
                .Select(rb => new ResearchBookAiChatDto
                {
                    Id = rb.Id,
                    Name = rb.Name,
                })
                .ToList();

            // Fetch LegalInformation data first
            var legalInformations = await _context.ResearchBooks
                .SelectMany(rb => rb.LegalInformation)
                .ToListAsync();

            var legalInformationResults = legalInformations
                .Where(li => words.Any(word =>
                    li.Type.Contains(word) ||
                    li.Title.Contains(word) ||
                    li.Description.Contains(word) ||
                    (li.Document != null && li.Document.Contains(word))
                ))
                .Select(li => new LegalInformationAiChatDto
                {
                    Id = li.Id,
                    Type = li.Type,
                    Title = li.Title,
                    Description = li.Description,
                })
                .ToList();

            var results = new AiChatResponse()
            {
                ResearchBooks = researchBookResults,
                LegalInformations = legalInformationResults
            };
            await AddChatMessage(message, userId);
            return results;
        }

        /// <summary>
        /// Adds a chat message to the database.
        /// </summary>
        /// <param name="message">The content of the chat message to be added.</param>
        /// <param name="userId">The unique identifier of the user who sent the chat message.</param>
        /// <returns>An asynchronous task representing the operation.</returns>
        private async Task AddChatMessage(string message, string userId)
        {
            var aiChat = new AiChat()
            {
                UserId = userId,
                Message = message,
                DateTime = DateTime.Now,
            };
            await _context.AiChats.AddAsync(aiChat);
            await _context.SaveChangesAsync();
        }
    }
}
