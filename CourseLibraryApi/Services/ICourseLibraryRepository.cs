﻿using CourseLibraryApi.Entities;
using CourseLibraryApi.Helpers;
using CourseLibraryApi.ResourceParameters;
using System;
using System.Collections.Generic;

namespace CourseLibraryApi.Services
{
    public interface ICourseLibraryRepository
    {
        IEnumerable<Course> GetCourses(Guid authorId);

        Course GetCourse(Guid authorId, Guid courseId);

        void AddCourse(Guid authorId, Course course);

        void UpdateCourse(Course course);

        void DeleteCourse(Course course);

        IEnumerable<Author> GetAuthors();

        Author GetAuthor(Guid authorId);

        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);

        PagedList<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters);

        void AddAuthor(Author author);

        void DeleteAuthor(Author author);

        void UpdateAuthor(Author author);

        bool AuthorExists(Guid authorId);

        bool Save();
    }
}