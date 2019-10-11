using System;
using Domain;
using HeatingApi.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace HeatingApi.Controllers
{
    [Obsolete]
    [Route("/api/setup")]
    [RequiredPermission(Permission.Configuration_Devices)]
    public class SetupController : BaseController
    {
        private readonly IHeatingControl _heatingControl;

        public SetupController(IHeatingControl heatingControl)
        {
            _heatingControl = heatingControl;
        }

        [HttpGet]
        public Building GetBuildingModel()
        {
            return _heatingControl.State.Model;
        }
    }
}
