using AutoMapper;
using CourseLibraryApi.Helpers;
using CourseLibraryApi.Models;
using CourseLibraryApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseLibraryApi.Controllers
{
    [ApiController]
    [Route("/api/authorcollections")]
    public class AuthorCollectionsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorCollectionsController(ICourseLibraryRepository repo, IMapper mapper)
        {
            _courseLibraryRepository = repo;
            _mapper = mapper;
        }

        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public IActionResult GetAuthorCollection(
        [FromRoute][ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var authorEntities = _courseLibraryRepository.GetAuthors(ids);

            if (ids.Count() != authorEntities.Count())
                return NotFound();

            var authorsToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            return Ok(authorsToReturn);
        }

        [HttpPost]
        public ActionResult<IEnumerable<AuthorDto>> CreateAuthors(IEnumerable<AuthorForCreationDto> authorDtos)
        {
            var authorEntities = _mapper.Map<IEnumerable<Entities.Author>>(authorDtos);
            //   List<Guid> authorIds = new List<Guid>();
            foreach (var author in authorEntities)
            {
                //authorIds.Add(author.Id);
                _courseLibraryRepository.AddAuthor(author);
            }
            _courseLibraryRepository.Save();

            #region Where the Authors get created and we harvest their ids

            var authorCollectionToReturn = _mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            var idAsString = string.Join(",", authorCollectionToReturn.Select(a => a.Id));

            #endregion Where the Authors get created and we harvest their ids

            return CreatedAtRoute("GetAuthorCollection", new { ids = idAsString },
                authorCollectionToReturn);
        }
    }
}