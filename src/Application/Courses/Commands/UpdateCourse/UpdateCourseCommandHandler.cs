using Application.Common.Results;
using Application.Courses.DTOs.Course;
using Application.Courses.Repository;
using Application.StudentProfiles.IRepository;
using Application.Courses.Mappers;
using Domain.Common;
using Domain.Models.StudentProfiles;
using FluentValidation;
using MediatR;

namespace Application.Courses.Commands.UpdateCourse
{
    public sealed class UpdateCourseCommandHandler 
        : IRequestHandler<UpdateCourseCommand, OperationResult<CourseDto>>
    {
        private readonly Application.Courses.Repository.ICourseRepository _courseRepository;
        private readonly IStudentProfileRepository _studentProfileRepository;
        private readonly IValidator<UpdateCourseCommand> _validator;

        public UpdateCourseCommandHandler(
            ICourseRepository courseRepository,
            IStudentProfileRepository studentProfileRepository,
            IValidator<UpdateCourseCommand> validator)
        {
            _courseRepository = courseRepository;
            _studentProfileRepository = studentProfileRepository;
            _validator = validator;
        }

        public async Task<OperationResult<CourseDto>> Handle(
            UpdateCourseCommand request, CancellationToken cancellationToken)
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

            // STEP 5: Check uniqueness if name changed
            if (!course.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
            {
                var isNameUnique = await _courseRepository.IsNameUniquePerProfileAsync(
                    request.StudentProfileId,
                    request.Name,
                    request.CourseId,
                    cancellationToken);

                if (!isNameUnique)
                {
                    return OperationResult<CourseDto>.Failure(
                        Error.Conflict(ErrorCodes.General.Conflict, 
                            "A course with this name already exists for this profile"));
                }
            }

            // STEP 6: Update course using domain method
            course.UpdateDetails(request.Name, request.Description, request.Difficulty);

            // STEP 7: Save
            await _courseRepository.UpdateAsync(course, cancellationToken);

            // STEP 8: Map and return
            var dto = course.ToDto();
            return OperationResult<CourseDto>.Success(dto);
        }
    }
}

