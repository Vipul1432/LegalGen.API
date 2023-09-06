using AutoMapper;
using LegalGen.Domain.DTOs;
using LegalGen.Domain.Interfaces;
using LegalGen.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LegalGen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResearchBooksController : ControllerBase
    {
        private readonly IResearchBookRepository _researchBookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ResearchBooksController> _logger;
        private readonly IResearchBookService _researchBookService;

        public ResearchBooksController(IResearchBookRepository researchBookRepository, IMapper mapper, ILogger<ResearchBooksController> logger, IResearchBookService researchBookService)
        {
            _researchBookRepository = researchBookRepository;
            _mapper = mapper;
            _logger = logger;
            _researchBookService = researchBookService;
        }

        #region ResearchBook Operations
        /// <summary>
        /// Retrieves a list of research books.
        /// </summary>
        /// <returns>
        /// A response containing the list of research books, a success message, and a status code.
        /// </returns>
        /// <response code="200">Returns the list of research books.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ResearchBookDto>>>> GetResearchBooks()
        {
            try
            {
                var researchBooks = await _researchBookRepository.GetAllResearchBooksAsync();
                var response = new ApiResponse<IEnumerable<ResearchBookDto>>
                {
                    Message = "Successfully retrieved research books.",
                    Data = _mapper.Map<IEnumerable<ResearchBookDto>>(researchBooks),
                    StatusCode = 200
                };
                _logger.LogInformation("List of Research Books Accessed!");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting research books.");
                var response = new ApiResponse<IEnumerable<ResearchBookDto>>
                {
                    Message = "An error occurred while processing your request.",
                    Data = null,
                    StatusCode = 500 // Internal Server Error
                };

                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Retrieves a specific research book by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the research book to retrieve.</param>
        /// <returns>
        /// A response containing the research book, a success message, and a status code.
        /// </returns>
        /// <response code="200">Returns the requested research book.</response>
        /// <response code="404">If the research book with the given ID is not found.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ResearchBookDto>>> GetResearchBook(int id)
        {
            try
            {
                var researchBook = await _researchBookRepository.GetResearchBookByIdAsync(id);

                if (researchBook == null)
                {
                    return NotFound();
                }

                var response = new ApiResponse<ResearchBookDto>
                {
                    Message = "Successfully retrieved research book.",
                    Data = _mapper.Map<ResearchBookDto>(researchBook),
                    StatusCode = 200 // OK
                };
                _logger.LogInformation($"Successfully retrieved research book by {id}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting a research book.");
                var response = new ApiResponse<ResearchBookDto>
                {
                    Message = "An error occurred while processing your request.",
                    Data = null,
                    StatusCode = 500 // Internal Server Error
                };

                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Updates an existing research book with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the research book to update.</param>
        /// <param name="researchBookDto">The updated research book data.</param>
        /// <returns>
        /// A response containing the updated research book, a success message, and a status code.
        /// </returns>
        /// <response code="200">Returns the updated research book.</response>
        /// <response code="400">If the provided ID does not match the research book data.</response>
        /// <response code="404">If the research book with the given ID is not found.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ResearchBookDto>>> PutResearchBook(int id, ResearchBookDto researchBookDto)
        {
            try
            {
                if (id != researchBookDto.Id)
                {
                    return BadRequest();
                }

                var researchBook = _mapper.Map<ResearchBook>(researchBookDto);

                var updatedResearchBook = await _researchBookRepository.UpdateResearchBookAsync(id, researchBook);

                if (updatedResearchBook == null)
                {
                    return NotFound();
                }
                _logger.LogInformation("Successfully updated research book!");
                var response = new ApiResponse<ResearchBookDto>
                {
                    Message = "Successfully updated research book.",
                    Data = _mapper.Map<ResearchBookDto>(updatedResearchBook),
                    StatusCode = 200 // OK
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating a research book.");
                var response = new ApiResponse<ResearchBookDto>
                {
                    Message = "An error occurred while processing your request.",
                    Data = null,
                    StatusCode = 500 // Internal Server Error
                };

                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Creates a new research book with the provided data.
        /// </summary>
        /// <param name="researchBookCreateDto">The data for the research book to create.</param>
        /// <returns>
        /// A response containing the newly created research book, a success message, and a status code.
        /// </returns>
        /// <response code="201">Returns the newly created research book.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ResearchBookDto>>> PostResearchBook(ResearchBookDto researchBookCreateDto)
        {
            try
            {
                var researchBook = _mapper.Map<ResearchBook>(researchBookCreateDto);
                var createdResearchBook = await _researchBookRepository.CreateResearchBookAsync(researchBook);

                var response = new ApiResponse<ResearchBookDto>
                {
                    Message = "Successfully created research book.",
                    Data = _mapper.Map<ResearchBookDto>(createdResearchBook),
                    StatusCode = 201 // Created
                };
                _logger.LogInformation("Successfully created research book!");
                return CreatedAtAction("GetResearchBook", new { id = createdResearchBook.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a research book.");
                var response = new ApiResponse<ResearchBookDto>
                {
                    Message = "An error occurred while processing your request.",
                    Data = null,
                    StatusCode = 500 // Internal Server Error
                };

                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Deletes a research book with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the research book to delete.</param>
        /// <returns>
        /// A response indicating the success or failure of the deletion, a success message, and a status code.
        /// </returns>
        /// <response code="204">If the research book is successfully deleted.</response>
        /// <response code="404">If the research book with the given ID is not found.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteResearchBook(int id)
        {
            try
            {
                var deleted = await _researchBookRepository.DeleteResearchBookAsync(id);

                if (!deleted)
                {
                    return NotFound();
                }
                _logger.LogInformation("Successfully deleted research book!");
                var response = new ApiResponse<object>
                {
                    Message = "Successfully deleted research book.",
                    Data = null,
                    StatusCode = 204 // No Content
                };

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting a research book.");
                var response = new ApiResponse<object>
                {
                    Message = "An error occurred while processing your request.",
                    Data = null,
                    StatusCode = 500 // Internal Server Error
                };

                return StatusCode(500, response);
            }
        }

        #endregion ResearchBook Operations

        #region LegalInformation Operations

        /// <summary>
        /// Retrieves all legal information associated with a research book.
        /// </summary>
        /// <param name="researchBookId">The ID of the research book.</param>
        /// <returns>
        /// A response containing a list of legal information DTOs, a success message, and a status code.
        /// </returns>
        /// <response code="200">If legal information is successfully retrieved.</response>
        /// <response code="404">If the research book is not found or has no legal information.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet("{researchBookId}/legalinformation")]
        public async Task<IActionResult> GetAllLegalInformationInResearchBookAsync(int researchBookId)
        {
            try
            {
                var legalInformation = await _researchBookService.GetAllLegalInformationsInResearchBookAsync(researchBookId);

                if (legalInformation == null)
                {
                    _logger.LogWarning("ResearchBook not found or has no legal information");
                    return NotFound(new ApiResponse<IEnumerable<LegalInformationDto>>
                    {
                        Message = "ResearchBook not found or has no legal information.",
                        Data = null,
                        StatusCode = 404 // Not Found
                    });
                }
                _logger.LogInformation("Successfully retrieved legal information in ResearchBook");
                return Ok(new ApiResponse<IEnumerable<LegalInformationDto>>
                {
                    Message = "Successfully retrieved legal information in ResearchBook.",
                    Data = legalInformation,
                    StatusCode = 200 // OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting legal information in ResearchBook.");
                return StatusCode(500, new ApiResponse<IEnumerable<LegalInformationDto>>
                {
                    Message = "An error occurred while processing your request.",
                    Data = null,
                    StatusCode = 500 // Internal Server Error
                });
            }
        }

        /// <summary>
        /// Adds legal information to a research book.
        /// </summary>
        /// <param name="researchBookId">The ID of the research book.</param>
        /// <param name="legalInformationDto">The legal information DTO to be added.</param>
        /// <returns>
        /// A response indicating the success or failure of adding legal information to the research book.
        /// </returns>
        /// <response code="200">If legal information is added successfully.</response>
        /// <response code="404">If the research book is not found or legal information could not be added.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpPost("addlegalinformation/{researchBookId}")]
        public async Task<IActionResult> AddLegalInformationToResearchBook(int researchBookId, [FromBody] LegalInformationDto legalInformationDto)
        {
            try
            {
                bool result = await _researchBookService.AddLegalInformationToResearchBookAsync(researchBookId, legalInformationDto);

                if (result)
                {
                    _logger.LogInformation("Legal information added to research book successfully!");
                    return Ok(new ApiResponse<bool>
                    {
                        Message = "Legal information added to research book successfully.",
                        Data = true,
                        StatusCode = 200
                    });
                }
                else
                {
                    _logger.LogInformation("Research book not found or legal information could not be added!");
                    return NotFound(new ApiResponse<bool>
                    {
                        Message = "Research book not found or legal information could not be added.",
                        Data = false,
                        StatusCode = 404
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting a research book.");
                return StatusCode(500, new ApiResponse<bool>
                {
                    Message = "An error occurred while adding legal information to research book.",
                    Data = false,
                    StatusCode = 500
                });
            }
        }

        /// <summary>
        /// Updates legal information associated with a research book.
        /// </summary>
        /// <param name="researchBookId">The ID of the research book.</param>
        /// <param name="legalInformationId">The ID of the legal information to be updated.</param>
        /// <param name="legalInformationDto">The legal information DTO containing updated information.</param>
        /// <returns>
        /// A response indicating the success or failure of updating legal information.
        /// </returns>
        /// <response code="200">If legal information is updated successfully.</response>
        /// <response code="404">If the legal information or research book is not found.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpPut("{researchBookId}/legalinformation/{legalInformationId}")]
        public async Task<IActionResult> UpdateLegalInformationOfResearchBook(int researchBookId, int legalInformationId, [FromBody] LegalInformationDto legalInformationDto)
        {
            try
            {
                var updated = await _researchBookService.UpdateLegalInformationByResearchBookIdAsync(researchBookId, legalInformationId, legalInformationDto);

                if (!updated)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Message = "LegalInformation or ResearchBook not found.",
                        Data = null,
                        StatusCode = 404 // Not Found
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Message = "LegalInformation updated successfully.",
                    Data = null,
                    StatusCode = 200 // OK
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, new ApiResponse<object>
                {
                    Message = "An error occurred while updating LegalInformation.",
                    Data = null,
                    StatusCode = 500 // Internal Server Error
                });
            }
        }

        /// <summary>
        /// Deletes legal information associated with a research book.
        /// </summary>
        /// <param name="researchBookId">The ID of the research book.</param>
        /// <param name="legalInformationId">The ID of the legal information to be deleted.</param>
        /// <returns>
        /// A response indicating the success or failure of deleting legal information.
        /// </returns>
        /// <response code="204">If legal information is deleted successfully.</response>
        /// <response code="404">If the legal information or research book is not found.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpDelete("{researchBookId}/legalinformation/{legalInformationId}")]
        public async Task<IActionResult> DeleteLegalInformationByResearchBookIdAsync(int researchBookId, int legalInformationId)
        {
            try
            {
                var deleted = await _researchBookService.DeleteLegalInformationByResearchBookIdAsync(
                    researchBookId,
                    legalInformationId);

                if (!deleted)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Message = "LegalInformation or ResearchBook not found.",
                        Data = null,
                        StatusCode = 404 // Not Found
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, new ApiResponse<object>
                {
                    Message = "An error occurred while deleting LegalInformation.",
                    Data = null,
                    StatusCode = 500 // Internal Server Error
                });
            }
        }
        
        #endregion LegalInformation Operations

    }
}
