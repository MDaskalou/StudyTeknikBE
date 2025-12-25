using Application.Common.Results;
using Application.Courses.DTOs.Course;
using Application.Courses.Repository;
using Application.StudentProfiles.IRepository;
using Application.Courses.Mappers;
using Domain.Common;
using Domain.Models.StudentProfiles;
using FluentValidation;
using MediatR;

namespace Application.Courses.Queries.GetCoursesForProfile
{
    public sealed class GetCoursesForProfileQueryHandler 
        : IRequestHandler<GetCoursesForProfileQuery, OperationResult<List<CourseDto>>>
    {
        private readonly Application.Courses.Repository.ICourseRepository _courseRepository;
        private readonly IStudentProfileRepository _studentProfileRepository;
        private readonly IValidator<GetCoursesForProfileQuery> _validator;

        public GetCoursesForProfileQueryHandler(
            ICourseRepository courseRepository,
            IStudentProfileRepository studentProfileRepository,
            IValidator<GetCoursesForProfileQuery> validator)
        {
            _courseRepository = courseRepository;
            _studentProfileRepository = studentProfileRepository;
            _validator = validator;
        }

        public async Task<OperationResult<List<CourseDto>>> Handle(
            GetCoursesForProfileQuery request, CancellationToken cancellationToken)
        {
            // STEP 1: Validate
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return OperationResult<List<CourseDto>>.Failure(
                    Error.Validation(ErrorCodes.General.Validation, errorMessages));
            }

            // STEP 2: Verify StudentProfile exists
            var studentProfile = await _studentProfileRepository.GetByIdAsync(request.StudentProfileId, cancellationToken);
            if (studentProfile == null)
            {
                return OperationResult<List<CourseDto>>.Failure(
                    Error.NotFound(ErrorCodes.General.NotFound, "StudentProfile not found"));
            }

            // STEP 3: Get all courses for profile
            var courses = await _courseRepository.GetByStudentProfileIdAsync(request.StudentProfileId, cancellationToken);

            // STEP 4: Map and return
            var dtos = courses.Select(c => c.ToDto()).ToList();
            return OperationResult<List<CourseDto>>.Success(dtos);
        }
    }
}

