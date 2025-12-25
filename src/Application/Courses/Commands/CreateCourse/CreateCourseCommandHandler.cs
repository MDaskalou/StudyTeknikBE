using Application.Common.Results;
using Application.Courses.DTOs.Course;
using Application.Courses.Repository;
using Application.StudentProfiles.IRepository;
using Application.Courses.Mappers;
using Domain.Common;
using Domain.Models.StudentProfiles;
using FluentValidation;
using MediatR;

namespace Application.Courses.Commands.CreateCourse
{
    public sealed class CreateCourseCommandHandler 
        : IRequestHandler<CreateCourseCommand, OperationResult<CourseDto>>
    {
        private readonly Application.Courses.Repository.ICourseRepository _courseRepository;
        private readonly IStudentProfileRepository _studentProfileRepository;
        private readonly IValidator<CreateCourseCommand> _validator;

        public CreateCourseCommandHandler(
            IStudentProfileRepository studentProfileRepository,
            ICourseRepository courseRepository,
            IValidator<CreateCourseCommand> validator)
        {
            _studentProfileRepository = studentProfileRepository;
            _courseRepository = courseRepository;
            _validator = validator;
        }

        public async Task<OperationResult<CourseDto>> Handle(
            CreateCourseCommand request, CancellationToken cancellationToken)
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

            // STEP 3: Check uniqueness (name must be unique per profile)
            var isNameUnique = await _courseRepository.IsNameUniquePerProfileAsync(
                request.StudentProfileId, 
                request.Name,
                null,
                cancellationToken);

            if (!isNameUnique)
            {
                return OperationResult<CourseDto>.Failure(
                    Error.Conflict(ErrorCodes.General.Conflict, 
                        "A course with this name already exists for this profile"));
            }

            // STEP 4: Create domain entity
            var courseResult = Course.Create(
                request.StudentProfileId,
                request.Name, 
                request.Description, 
                request.Difficulty);
            if (courseResult.IsFailure)
            {
                return OperationResult<CourseDto>.Failure(
                    Error.Validation(ErrorCodes.General.Validation, courseResult.Error));
            }

            var course = courseResult.Value;

            // STEP 5: Save
            await _courseRepository.AddAsync(course, cancellationToken);

            // STEP 6: Map and return
            var dto = course.ToDto();
            return OperationResult<CourseDto>.Success(dto);
        }
    }
}

