using AutoMapper;
using CourseLibraryApi.Entities;
using CourseLibraryApi.Models;
using CourseLibraryApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CourseLibraryApi.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository repository,
            IMapper mapper)
        {
            _courseLibraryRepository = repository ??
            throw new ArgumentNullException(nameof(repository));

            _mapper = mapper ??
            throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var coursesForAuthorFromRepo = _courseLibraryRepository.GetCourses(authorId);
            return Ok(_mapper.Map<IEnumerable<CourseDto>>(coursesForAuthorFromRepo));
        }

        [HttpGet("{courseId}", Name = "GetCourseForAuthor")]
        public ActionResult<IEnumerable<CourseDto>> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo =
                _courseLibraryRepository.GetCourse(authorId, courseId);
            if (courseForAuthorFromRepo == null)
                return NotFound();
            return Ok(_mapper.Map<CourseDto>(courseForAuthorFromRepo));
        }

        [HttpPost]
        public ActionResult<Course> CreateCourseForAuthor(Guid authorId, CourseForCreationDto course)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseEntity = _mapper.Map<Entities.Course>(course);
            _courseLibraryRepository.AddCourse(authorId, courseEntity);
            _courseLibraryRepository.Save();

            var courseToReturn = _mapper.Map<Models.CourseDto>(courseEntity);

            return CreatedAtRoute("GetCourseForAuthor",
                new { authorId = authorId, courseId = courseToReturn.Id }, courseToReturn);
        }
    }
}