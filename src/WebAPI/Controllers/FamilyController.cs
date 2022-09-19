using Application.Dto;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FamilyController : ControllerBase
{
    private readonly IFamilyService _familyService;
    private readonly ILogger<FamilyController> _logger;

    public FamilyController(ILogger<FamilyController> logger, IFamilyService familyService)
    {
        _logger = logger;
        _familyService = familyService;
    }

    [SwaggerOperation(Summary = "Register new family")]
    [HttpPost("Add", Name = nameof(AddNewFamily))]
    [ProducesResponseType(typeof(AddNewFamilyReturnDto), 200)]
    public async Task<IActionResult> AddNewFamily(AddNewFamilyDto dto)
    {
        try
        {
            var returnDto = await _familyService.AddNewFamily(dto).ConfigureAwait(false);
            return Ok(returnDto);
        }
        catch (UserException)
        {
            _logger.LogError("Could not find user with id: {Id}. Process terminated.", dto.FounderId);
            return NotFound("Could not create a family with non-existing user");
        }
        catch (FamilyException ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
    }
    
    [SwaggerOperation(Summary = "Delete specified family")]
    [HttpDelete("Delete", Name = nameof(DeleteFamily))]
    public async Task<IActionResult> DeleteFamily(ulong id)
    {
        var deleteSuccessful = await _familyService.DeleteFamily(id).ConfigureAwait(false);

        if (deleteSuccessful)
        {
            return Ok();
        }
        
        _logger.LogError("Could not found family with ID: {Id}", id);
        return NotFound();
    }
}