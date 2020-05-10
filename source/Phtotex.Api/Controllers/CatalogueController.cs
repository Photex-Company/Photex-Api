using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Photex.Core.Contracts.Requests;
using Photex.Core.Interfaces;
using Phtotex.Api.Extensions;

namespace Phtotex.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogueController : ControllerBase
    {
        private readonly ICatalogueService _catalogueService;

        public CatalogueController(
            ICatalogueService catalogueService)
        {
            _catalogueService = catalogueService ?? throw new ArgumentNullException(nameof(catalogueService));
        }

        [HttpDelete("{catalogueId}")]
        [Authorize]
        public async Task<IActionResult> DeleteCatalogue(
            [FromRoute] long catalogueId)
        {
            await _catalogueService.DeleteCatalogue(
                HttpContext.GetUserId().Value,
                catalogueId);

            return NoContent();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCatalogue(
            [FromBody] AddCatalogueRequest request)
        {
            await _catalogueService.AddCatalogue(
                HttpContext.GetUserId().Value,
                request.Name);

            return NoContent();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCatalogues()
        {
            var response = await _catalogueService.GetCatalogues(
                HttpContext.GetUserId().Value);

            return response == null
                ? (IActionResult)NotFound()
                : Ok(response);
        }

        [HttpGet("{catalogueId}")]
        [Authorize]
        public async Task<IActionResult> GetImagesFromCatalogue(
            [FromRoute] long catalogueId,
            [FromServices] IImageService imageService)
        {
            var response = await imageService.GetImagesFromCatalogue(
                HttpContext.GetUserId().Value, catalogueId);

            return response == null
                ? (IActionResult)NotFound()
                : Ok(response);
        }
    }
}