using System;
using System.Collections.Generic;
using System.Text;

namespace HeatingControl.Application.Queries
{
    public interface IPinAllowedUserListProvider
    {
        PinAllowedUserListProviderResult Provide();
    }

    public class PinAllowedUserListProviderResult
    {
        public IList<string> Logins { get; set; }
    }

    public class PinAllowedUserListProvider : IPinAllowedUserListProvider
    {
        public PinAllowedUserListProvider()
        {

        }

        public PinAllowedUserListProviderResult Provide()
        {
            throw new NotImplementedException();
        }
    }
}
