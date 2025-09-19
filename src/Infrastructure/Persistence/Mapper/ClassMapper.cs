// Infrastructure/Persistence/Mappers/ClassMapper.cs
using Domain.Entities;
using Domain.Models.Classes;

namespace Infrastructure.Persistence.Mapper
{
    public static class ClassMapper
    {
        public static Class ToModel(this ClassEntity e)
            => Class.Rehydrate(e.Id, e.CreatedAtUtc, e.UpdatedAtUtc, e.SchoolName, e.Year, e.ClassName);
    }
}