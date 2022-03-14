using CourseLibraryApi.Services;
using CourseLibraryApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseLibraryApi.Helpers;
using AutoMapper;
using CourseLibraryApi.ResourceParameters;

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

        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors(
           [FromQuery] AuthorsResourceParameters authorsResourceParameters)
        {
            var authorsFromRepo = _courseLibraryRepo.GetAuthors(authorsResourceParameters);
            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepo.GetAuthor(authorId);
            //return !(authorFromRepo == null) ?
            //    Ok(authorFromRepo) : (IActionResult)NotFound();

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
    }
}