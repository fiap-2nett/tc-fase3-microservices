using System;
using HelpDesk.Core.Domain.Abstractions;
using HelpDesk.Core.Domain.Enumerations;
using HelpDesk.Core.Domain.Errors;
using HelpDesk.Core.Domain.Exceptions;
using HelpDesk.Core.Domain.Extensions;
using HelpDesk.Core.Domain.Primitives;
using HelpDesk.Core.Domain.Utility;
using HelpDesk.Core.Domain.ValueObjects;


namespace HelpDesk.Core.Domain.Entities
{
    public class User : AggregateRoot<int>, IAuditableEntity, ISoftDeletableEntity
    {
        #region Private Fields

        private string _passwordHash;

        #endregion

        #region Properties

        public byte IdRole { get; private set; }

        public string Name { get; private set; }
        public string Surname { get; private set; }
        public Email Email { get; private set; }

        public bool IsDeleted { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }

        #endregion

        #region Constructors

        private User()
        { }

        public User(string name, string surname, Email email, UserRoles userRole, string passwordHash)
        {
            Ensure.NotEmpty(name, "The name is required.", nameof(name));
            Ensure.NotEmpty(surname, "The surname is required.", nameof(surname));
            Ensure.NotEmpty(email, "The email is required.", nameof(email));
            Ensure.NotEmpty(passwordHash, "The password hash is required", nameof(passwordHash));

            Name = name;
            Surname = surname;
            Email = email;
            IdRole = (byte)userRole;
            _passwordHash = passwordHash;
        }

        #endregion

        #region Methods

        public bool VerifyPasswordHash(string password, IPasswordHashChecker passwordHashChecker)
            => !password.IsNullOrWhiteSpace() && passwordHashChecker.HashesMatch(_passwordHash, password);

        public void ChangePassword(string passwordHash)
        {
            if (passwordHash == _passwordHash)
                throw new DomainException(DomainErrors.User.CannotChangePassword);

            _passwordHash = passwordHash;
        }

        public void ChangeName(string name, string surname)
        {
            if (name.IsNullOrWhiteSpace())
                throw new DomainException(DomainErrors.User.NameIsRequired);

            if (surname.IsNullOrWhiteSpace())
                throw new DomainException(DomainErrors.User.SurnameIsRequired);

            Name = name;
            Surname = surname;
        }

        #endregion
    }
}
