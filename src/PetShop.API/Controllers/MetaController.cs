﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace PetShop.API.Controllers
{
    public class MetaController : BaseApiController
    {
        [HttpGet("/info")]
        public ActionResult<string> Info()
        {
            var assembly = typeof(Program).Assembly;
            var lastUpdate = System.IO.File.GetLastWriteTime(assembly.Location);
            var version = FileVersionInfo.GetVersionInfo(assembly.Location);

            return Ok($"Version: {version}, Last Updated: {lastUpdate}");
        }
    }
}
