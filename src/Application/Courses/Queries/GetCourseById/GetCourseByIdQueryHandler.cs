using Application.Common.Results;
using Application.Courses.DTOs.Course;
using Application.Courses.Repository;
using Application.StudentProfiles.IRepository;
using Application.Courses.Mappers;
using Domain.Common;
using Domain.Models.StudentProfiles;
using FluentValidation;
using MediatR;

namespace Application.Courses.Queries.GetCourseById
{
    public sealed class GetCourseByIdQueryHandler 
        : IRequestHandler<GetCourseByIdQuery, OperationResult<CourseDto>>
    {
        private readonly Application.Courses.Repository.ICourseRepository _courseRepository;
        private readonly IStudentProfileRepository _studentProfileRepository;
        private readonly IValidator<GetCourseByIdQuery> _validator;

        public GetCourseByIdQueryHandler(
            ICourseRepository courseRepository,
            IStudentProfileRepository studentProfileRepository,
            IValidator<GetCourseByIdQuery> validator)
        {
            _courseRepository = courseRepository;
            _studentProfileRepository = studentProfileRepository;
            _validator = validator;
        }

        public async Task<OperationResult<CourseDto>> Handle(
            GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            // STEP 1: Validate
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return OperationResult<CourseDto>.Failure(
                    Error.Validation(ErrorCodes.General.Validation, errorMessages));
            }

            // STEP 2: Verify StudentProfile exists
            var studentProfile = await _studentProfileRepository.GetByIdAsync(request.StudentProfileId, cancellationToken);
            if (studentProfile == null)
            {
                return OperationResult<CourseDto>.Failure(
                    Error.NotFound(ErrorCodes.General.NotFound, "StudentProfile not found"));
            }

            // STEP 3: Get course
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
            {
                return OperationResult<CourseDto>.Failure(
                    Error.NotFound(ErrorCodes.General.NotFound, "Course not found"));
            }

            // STEP 4: Verify course belongs to profile
            var profileCourses = await _courseRepository.GetByStudentProfileIdAsync(request.StudentProfileId, cancellationToken);
            if (!profileCourses.Any(c => c.Id == request.CourseId))
            {
                return OperationResult<CourseDto>.Failure(
                    Error.Forbidden(ErrorCodes.General.Forbidden, "Course does not belong to this profile"));
            }

            // STEP 5: Map and return
            var dto = course.ToDto();
            return OperationResult<CourseDto>.Success(dto);
        }
    }
}

