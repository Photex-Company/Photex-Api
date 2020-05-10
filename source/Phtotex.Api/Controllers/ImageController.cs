using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Photex.Core.Contracts.Metadata;
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

        [HttpGet("metadata/editable")]
        public IActionResult GetEditableMetadata()
        {
            return Ok(
                Enum.GetValues(typeof(MetadataInfo.MetadataName))
                .OfType<MetadataInfo.MetadataName>()
                .Select(name => new
                {
                    Name = name,
                    Description = MetadataInfo.MetadataDescriptions[name],
                    Value = MetadataInfo.MetadataValues[name]
                }));
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

        [HttpGet("{imageId}")]
        [Authorize]
        public async Task<IActionResult> GetImages(
            [FromRoute] long imageId)
        {
            var response = await _imageService.GetImageFromUser(
                HttpContext.GetUserId().Value,
                imageId);

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
                    request.CatalogueId,
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

        [HttpPut("{imageId}")]
        [Authorize]
        public async Task<IActionResult> UpdateImage(
            [FromBody] UpdateImageRequest request,
            [FromRoute] long imageId)
        {
            await _imageService.UpdateImage(
                HttpContext.GetUserId().Value,
                imageId,
                request);

            return NoContent();
        }

        [HttpPatch("{imageId}/metadata")]
        [Authorize]
        public async Task<IActionResult> UpdateMetadata(
            [FromBody] UpdateMetadataRequest request,
            [FromRoute] long imageId)
        {
            await _imageService.UpdateMetadata(
                HttpContext.GetUserId().Value,
                imageId,
                request);

            return NoContent();
        }

        [HttpDelete("{imageId}")]
        [Authorize]
        public async Task<IActionResult> RemoveImage(
            [FromRoute] long imageId)
        {
            await _imageService.DeleteImage(
                HttpContext.GetUserId().Value,
                imageId);

            return NoContent();
        }
    }
}