﻿using AndOS.Common.Interfaces;
using AndOS.Core.Schemas;
using AndOS.Domain.Exceptions.UserExceptions;
using System.Text.RegularExpressions;

namespace AndOS.Domain.Entities;

public interface IUser : IAggregateRoot, IEntity
{
    string UserName { get; set; }
    string Email { get; set; }
    bool UserNameUpdated { get; set; }
    Folder Folder { get; set; }

    List<Account> Accounts { get; set; }

    void UpdateUserName(string userName)
    {
        if (UserNameUpdated)
            throw new UserNameAlreadyUpdatedException();

        ValidateUserName(userName);
        this.UserName = userName;
        UserNameUpdated = true;
    }

    static bool ValidateUserName(string userName)
    {
        if (userName is { Length: < UserSchema.MinLenghtUserName or > UserSchema.MaxLenghtUserName })
            throw new InvalidUserNameLengthException();

        if (!Regex.IsMatch(userName, UserSchema.RegexUserName))
            throw new InvalidUserNameCharacters();

        return true;
    }
}
