﻿using System;
using System.Linq;

namespace BooksRentalSystem.Admin.Services
{
    public class ServiceEndpoints
    {
        public string Identity { get; private set; }

        public string Statistics { get; private set; }

        public string Publishers { get; private set; }

        public string this[string service]
            => GetType()
                   .GetProperties()
                   .Where(pr => string.Equals(pr.Name, service, StringComparison.CurrentCultureIgnoreCase))
                   .Select(pr => (string) pr.GetValue(this))
                   .FirstOrDefault()
               ?? throw new InvalidOperationException(
                   $"External service with name '{service}' does not exists.");
    }
}
