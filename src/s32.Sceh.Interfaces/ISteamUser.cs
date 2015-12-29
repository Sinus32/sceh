using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Interfaces
{
    public interface ISteamUser
    {
        Guid Id { get; }

        string Login { get; }

        long SteamId { get; }

        string UserUrl { get; }
    }
}
