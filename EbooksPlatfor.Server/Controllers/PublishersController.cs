using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.DTOs;
using OnlineBookstore.Models;
using OnlineBookstore.Services;

namespace OnlineBookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublishersController : ControllerBase
    {

        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        // GET: api/publishers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublisherDto>>> GetPublishers()  // ← GET: Use ActionResult<T> (you know the return type)
        {
            try
            {
                var publishers = await _publisherService.GetAllPublishersAsync();

                return Ok(publishers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving publishers", error = ex.Message });
            }
        }

        // GET: api/publishers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PublisherDto>> GetPublisher(int id)
        {
            try
            {
                var publisher = await _publisherService.GetPublisherByIdAsync(id);

                if (publisher == null)
                {
                    return NotFound(new { message = "Publisher not found" });
                }

                return Ok(publisher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the publisher", error = ex.Message });
            }
        }

        // POST: api/publishers
        [HttpPost]
        public async Task<IActionResult> CreatePublisher(CreatePublisherDto createPublisherDto)  // ← POST: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var publisher = await _publisherService.CreatePublisherAsync(createPublisherDto);

                return CreatedAtAction(nameof(GetPublisher), new { id = publisher.Id }, publisher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the publisher", error = ex.Message });
            }
        }

        // PUT: api/publishers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublisher(int id, UpdatePublisherDto updatePublisherDto)  // ← PUT: Use IActionResult (different success responses)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _publisherService.UpdatePublisherAsync(id, updatePublisherDto);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the publisher", error = ex.Message });
            }
        }

        // DELETE: api/publishers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            try
            {
                var result = await _publisherService.DeletePublisherAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "Publisher not found" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the publisher", error = ex.Message });
            }
        }
    }
}