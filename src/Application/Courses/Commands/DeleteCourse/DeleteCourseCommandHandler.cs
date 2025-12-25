using Application.Common.Results;
using Application.Courses.Repository;
using Application.StudentProfiles.IRepository;
using Domain.Common;
using Domain.Models.StudentProfiles;
using FluentValidation;
using MediatR;

namespace Application.Courses.Commands.DeleteCourse
{
    public sealed class DeleteCourseCommandHandler 
        : IRequestHandler<DeleteCourseCommand, OperationResult>
    {
        private readonly Application.Courses.Repository.ICourseRepository _courseRepository;
        private readonly IStudentProfileRepository _studentProfileRepository;
        private readonly IValidator<DeleteCourseCommand> _validator;

        public DeleteCourseCommandHandler(
            ICourseRepository courseRepository,
            IStudentProfileRepository studentProfileRepository,
            IValidator<DeleteCourseCommand> validator)
        {
            _courseRepository = courseRepository;
            _studentProfileRepository = studentProfileRepository;
            _validator = validator;
        }

        public async Task<OperationResult> Handle(
            DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            // STEP 1: Validate
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return OperationResult.Failure(
                    Error.Validation(ErrorCodes.General.Validation, errorMessages));
            }

            // STEP 2: Verify StudentProfile exists
            var studentProfile = await _studentProfileRepository.GetByIdAsync(request.StudentProfileId, cancellationToken);
            if (studentProfile == null)
            {
                return OperationResult.Failure(
                    Error.NotFound(ErrorCodes.General.NotFound, "StudentProfile not found"));
            }

            // STEP 3: Get course
            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course == null)
            {
                return OperationResult.Failure(
                    Error.NotFound(ErrorCodes.General.NotFound, "Course not found"));
            }

            // STEP 4: Verify course belongs to profile
            var profileCourses = await _courseRepository.GetByStudentProfileIdAsync(request.StudentProfileId, cancellationToken);
            if (!profileCourses.Any(c => c.Id == request.CourseId))
            {
                return OperationResult.Failure(
                    Error.Forbidden(ErrorCodes.General.Forbidden, "Course does not belong to this profile"));
            }

            // STEP 5: Delete
            await _courseRepository.DeleteAsync(request.CourseId, cancellationToken);

            return OperationResult.Success();
        }
    }
}

