using Application.Student.Dtos;
using Application.Student.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Student.Queries.GetAllStudents
{
    public class GetAllStudentsHandler : IRequestHandler<GetAllStudentsQuery, IReadOnlyList <StudentDetailsDto>>
    {
       private readonly IStudentRepository _repo;
       public GetAllStudentsHandler(IStudentRepository repo) => _repo = repo;
       
       public Task<IReadOnlyList <StudentDetailsDto>> Handle(GetAllStudentsQuery request, CancellationToken ct)
       => _repo.GetAllAsync(ct);
    }
}