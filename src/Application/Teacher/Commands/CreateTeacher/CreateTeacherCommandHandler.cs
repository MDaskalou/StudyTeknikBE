using Application.Abstractions;
using Application.Abstractions.IPersistence;
using Application.Common.Results;
using Application.Teacher.Dtos;
using Application.Teacher.Repository;
using Domain.Abstractions.Enum;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Teacher.Commands.CreateTeacher
{
    public sealed class CreateTeacherCommandHandler 
        : IRequestHandler<CreateTeacherCommand, OperationResult<CreateTeacherDto>>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IValidator<CreateTeacherCommand> _validator;
        private readonly IAppDbContext _db;
        
        public CreateTeacherCommandHandler(
            ITeacherRepository teacherRepository,
            IPasswordHasher passwordHasher,
            IValidator<CreateTeacherCommand> validator,
            IAppDbContext db)
        {
            _teacherRepository = teacherRepository;
            _passwordHasher = passwordHasher;
            _validator = validator;
            _db = db;
        }

        public async Task<OperationResult<CreateTeacherDto>> Handle(CreateTeacherCommand request,
            CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var error = Error.Validation("Validation.Error", string.Join(",", validationResult.Errors.Select(e => e.ErrorMessage)));
                return OperationResult<CreateTeacherDto>.Failure(error);
            }
            
            var email = request.Email.Trim().ToLowerInvariant();
            if (await _db.Users.AnyAsync(u => u.Email == email, ct))
            {
                var error = Error.Conflict("Teacher.EmailAlreadyExists", $"En lärare med e-postadressen '{email}' finns redan.");
                return OperationResult<CreateTeacherDto>.Failure(error);
            }
            
            var passwordHash = _passwordHasher.HashPassword(request.Password);
            
            var now = DateTime.UtcNow;

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                SecurityNumber = request.SecurityNumber.Trim(),
                Role = UserRole.Teacher,
                PasswordHash = passwordHash,
                ExternalProvider = "manual",
                ExternalSubject = ""
            };

            _teacherRepository.Add(user);
            await _db.SaveChangesAsync(ct);
            
            var dto = new CreateTeacherDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.SecurityNumber,
                request.Password
            );
            return OperationResult<CreateTeacherDto>.Success(dto);
        }
    }
}