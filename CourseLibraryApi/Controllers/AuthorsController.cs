using CourseLibraryApi.Services;
using CourseLibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibraryApi.ResourceParameters;
using CourseLibraryApi.Helpers;
using System.Text.Json;
using Microsoft.AspNetCore.Cors;

namespace CourseLibraryApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepo;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository repository,
            IMapper mapper)
        {
            _courseLibraryRepo = repository ?? throw
                new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(Name = "GetAuthors")]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors(
           [FromQuery] AuthorsResourceParameters authorsResourceParameters)
        {
            var authorsFromRepo = _courseLibraryRepo.GetAuthors(authorsResourceParameters);

            var previousPageLink = authorsFromRepo.HasPrevious ?
                CreateAuthorsResourceUri(authorsResourceParameters,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = authorsFromRepo.HasNext ?
               CreateAuthorsResourceUri(authorsResourceParameters,
               ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize = authorsFromRepo.PageSize,
                currentPage = authorsFromRepo.CurrentPage,
                totalPages = authorsFromRepo.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepo.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
        }

        [HttpPost]
        public IActionResult CreateAuthor(AuthorForCreationDto authorDto)
        {
            var authorEntity = _mapper.Map<Entities.Author>(authorDto);
            _courseLibraryRepo.AddAuthor(authorEntity);
            _courseLibraryRepo.Save();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtAction("GetAuthor", new { authorId = authorToReturn.Id },
                authorToReturn);
        }

        [HttpDelete("{authorId}")]
        public ActionResult DeleteAuthor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepo.GetAuthor(authorId);
            if (authorFromRepo == null)
            {
                return NotFound();
            }
            _courseLibraryRepo.DeleteAuthor(authorFromRepo);
            _courseLibraryRepo.Save();
            return NoContent();
        }

        private string CreateAuthorsResourceUri(
           AuthorsResourceParameters authorsResourceParameters,
           ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAuthors",
                      new
                      {
                          pageNumber = authorsResourceParameters.PageNumber - 1,
                          pageSize = authorsResourceParameters.PageSize,
                          mainCategory = authorsResourceParameters.MainCategory,
                          searchQuery = authorsResourceParameters.SearchQuery
                      });

                case ResourceUriType.NextPage:
                    return Url.Link("GetAuthors",
                      new
                      {
                          pageNumber = authorsResourceParameters.PageNumber + 1,
                          pageSize = authorsResourceParameters.PageSize,
                          mainCategory = authorsResourceParameters.MainCategory,
                          searchQuery = authorsResourceParameters.SearchQuery
                      });

                default:
                    return Url.Link("GetAuthors",
                    new
                    {
                        pageNumber = authorsResourceParameters.PageNumber,
                        pageSize = authorsResourceParameters.PageSize,
                        mainCategory = authorsResourceParameters.MainCategory,
                        searchQuery = authorsResourceParameters.SearchQuery
                    });
            }
        }
    }
}