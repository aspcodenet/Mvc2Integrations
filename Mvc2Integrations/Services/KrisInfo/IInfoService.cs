using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Mvc2Integrations.Services.KrisInfo
{
    public interface IInfoService
    {
        public List<KrisInfoServiceModel> GetKrisInfo();
    }
}