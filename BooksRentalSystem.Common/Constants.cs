﻿namespace BooksRentalSystem.Common
{
    public class Constants
    {
        public const string AdministratorRoleName = "Administrator";
    }

    public class DataSeederConstants
    {
        public const string DefaultUserId = "6be0140d-91db-46e3-adc0-bba8839239c6";
        public const string DefaultUserPassword = "coolbooks123!";
    }

    public class InfrastructureConstants
    {
        public const string AuthenticationCookieName = "Authentication";
        public const string AuthorizationHeaderName = "Authorization";
        public const string AuthorizationHeaderValuePrefix = "Bearer";
    }

    public class UserConstants
    {
        public const int MinPhoneNumberLength = 5;
        public const int MaxPhoneNumberLength = 20;
        public const string PhoneNumberRegularExpression = @"\+[0-9]*";
    }
}
