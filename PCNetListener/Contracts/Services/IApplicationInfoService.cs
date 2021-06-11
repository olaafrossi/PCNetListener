using System;

namespace PCNetListener.Contracts.Services
{
    public interface IApplicationInfoService
    {
        Version GetVersion();
    }
}
