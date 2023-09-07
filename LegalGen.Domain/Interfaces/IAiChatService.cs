using LegalGen.Domain.Dtos;
using LegalGen.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalGen.Domain.Interfaces
{
    public interface IAiChatService
    {
        /// <summary>
        /// Searches for relevant research books and legal information based on a user's input message.
        /// </summary>
        /// <param name="message">The user's input message to search for.</param>
        /// <param name="userId">The unique identifier of the user initiating the search.</param>
        /// <returns>An asynchronous task that represents the operation and returns the search results.</returns>
        Task<AiChatResponse> SearchAsync(string message, string userId);

    }
}
