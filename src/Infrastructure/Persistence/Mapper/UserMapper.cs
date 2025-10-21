using Domain.Entities;
using Domain.Common;
using Domain.Models.Users;
using System;

namespace Infrastructure.Persistence.Mapper
{
    public static class UserMapper
    {
        // Metod 1: Från Databas (UserEntity) -> Domänmodell (User)
        public static User ToModel(this UserEntity entity)
        {
            // Säker konvertering av enum
            var role = Enum.IsDefined(typeof(Role), entity.Role)
                ? (Role)entity.Role
                : Role.Student; // Fallback

            // Återskapa Value Object
            var consent = StudentConsent.Load(entity.ConsentGiven, entity.ConsentSetBy, entity.ConsentGivenAtUtc);


            // Anropa den kompletta fabriksmetoden
            return User.Load(
                entity.Id,
                entity.FirstName,
                entity.LastName,
                entity.Email,
                role,
                entity.SecurityNumber,
                entity.CreatedAtUtc,
                entity.UpdatedAtUtc,
                entity.ExternalProvider,
                entity.ExternalSubject,
                consent
            );
        } // <-- DETTA TECKEN SAKNADES!

        // Metod 2: Från Domänmodell (User) -> Databas (UserEntity)
        public static UserEntity ToEntity(this User model)
        {
            return new UserEntity
            {
                Id = model.Id,
                CreatedAtUtc = model.CreatedAtUtc,
                UpdatedAtUtc = model.UpdatedAtUtc,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                SecurityNumber = model.SecurityNumber,
                Role = model.Role, 
                ExternalProvider = model.ExternalProvider,
                ExternalSubject = model.ExternalSubject,
                
                ConsentGiven = model.Consent.Given,
                ConsentGivenAtUtc = model.Consent.GivenAtUtc,
                ConsentSetBy = model.Consent.SetBy // Kompilerar nu med rätt stavning
            };
        }
    }
}