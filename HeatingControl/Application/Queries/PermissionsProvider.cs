using Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeatingControl.Application.Queries
{
    public interface IPermissionsProvider
    {
        PermissionsProviderResult Provide();
    }

    public class PermissionsProviderResult
    {
        public ICollection<PermissionDescriptor> Permissions { get; set; }

        public class PermissionDescriptor
        {
            public int Identifier { get; set; }
            public string Name { get; set; }
        }
    }

    public class PermissionsProvider : IPermissionsProvider
    {
        public PermissionsProviderResult Provide()
        {
            var permissions = Enum.GetValues(typeof(Permission))
                                  .Cast<Permission>()
                                  .Select(x => new PermissionsProviderResult.PermissionDescriptor
                                  {
                                      Identifier = (int)x,
                                      Name = x.ToString()
                                  })
                                  .ToArray();

            return new PermissionsProviderResult { Permissions = permissions };
        }
    }
}
