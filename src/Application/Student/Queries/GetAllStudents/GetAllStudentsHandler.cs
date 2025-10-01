using Application.Common.Results;
using Application.Student.Dtos;
using Application.Student.Repository;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Student.Queries.GetAllStudents
{
    // FIX 1: Uppdatera returtypen i interfacet
    public sealed class GetAllStudentsHandler : IRequestHandler<GetAllStudentsQuery, OperationResult<IReadOnlyList<StudentDetailsDto>>>
    {
        private readonly IStudentRepository _repo;
        public GetAllStudentsHandler(IStudentRepository repo) => _repo = repo;
       
        // FIX 2: Uppdatera metodens signatur och logik
        public async Task<OperationResult<IReadOnlyList<StudentDetailsDto>>> Handle(GetAllStudentsQuery request, CancellationToken ct)
        {
            var students = await _repo.GetAllAsync(ct);
           
            // Slå in listan i ett lyckat OperationResult
            return OperationResult<IReadOnlyList<StudentDetailsDto>>.Success(students);
        }
    }
}