using LegalGen.Domain.Dtos;
using LegalGen.Domain.DTOs;
using LegalGen.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LegalGen.API.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]
    public class AiChatController : ControllerBase
    {
        private readonly IAiChatService _aiChatService;
        private readonly ILogger<ResearchBooksController> _logger;

        public AiChatController(IAiChatService aiChatService, ILogger<ResearchBooksController> logger)
        {
            _aiChatService = aiChatService;
            _logger = logger;
        }

        /// <summary>
        /// Searches for relevant chat messages and returns search results.
        /// </summary>
        /// <param name="message">The user's input message to search for in chat messages.</param>
        /// <returns>An asynchronous task that represents the operation and returns search results.</returns>
        [HttpGet("AiChat")]
        public async Task<IActionResult> Search(string message)
        {
            try
            {
                // Find user id from claims
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User ID claim not found in the claims!");
                    return BadRequest(new ApiResponse<string>
                    {
                        Message = "User ID claim not found in the claims.",
                        Data = null,
                        StatusCode = 400
                    });
                }
                var results = await _aiChatService.SearchAsync(message, userId);

                var apiResponse = new ApiResponse<AiChatResponse>
                {
                    Message = "Search results retrieved successfully.",
                    Data = results,
                    StatusCode = 200
                };
                _logger.LogInformation("Search results retrieved successfully!");
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return StatusCode(500, new ApiResponse<string>
                {
                    Message = $"An error occurred: {ex.Message}",
                    Data = null,
                    StatusCode = 500
                });
            }
        }
    }
}
