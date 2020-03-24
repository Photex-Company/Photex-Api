using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Photex.Core.Contracts.Requests;
using Photex.Core.Exceptions;
using Photex.Core.Interfaces;
using Phtotex.Api.Extensions;

namespace Phtotex.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImagesController(
            IImageService imageService)
        {
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetImages()
        {
            var response = await _imageService.GetImages(
                HttpContext.GetUserId().Value);

            return response == null
                ? (IActionResult)NotFound()
                : Ok(response);
        }

        [HttpGet("{catalogue}")]
        [Authorize]
        public async Task<IActionResult> GetImagesFromCatalogue([FromRoute] string catalogue)
        {
            var response = await _imageService.GetCatalogue(
                HttpContext.GetUserId().Value, catalogue);

            return response == null
                ? (IActionResult)NotFound()
                : Ok(response);
        }

        [HttpGet("catalogues")]
        [Authorize]
        public async Task<IActionResult> GetCatalogues()
        {
            var response = await _imageService.GetCatalogues(
                HttpContext.GetUserId().Value);

            return response == null
                ? (IActionResult)NotFound()
                : Ok(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadImage(
            [FromForm] UploadImageRequest request)
        {
            try
            {
                await _imageService.UploadImageFromStream(
                    HttpContext.GetUserId().Value,
                    request.Catalogue,
                    request.Description,
                    request.Image.OpenReadStream());
            }
            catch (WrongImageFormatException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }

            return NoContent();
        }
    }
}