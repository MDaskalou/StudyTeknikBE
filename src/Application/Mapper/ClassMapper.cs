using Domain.Entities;
using Domain.Models.Classes;

namespace Application.Mapper
{
    // TODO: Håll mapping här (statisk) istället för AutoMapper

    public static class ClassMapper
    {
        public static Class ToModel(this ClassEntity entity) =>
        Class.Rehydrate(
            entity.Id,
            entity.CreatedAtUtc,
            entity.UpdatedAtUtc,
            entity.SchoolName,
            entity.Year,
            entity.ClassName
        );
    }
}