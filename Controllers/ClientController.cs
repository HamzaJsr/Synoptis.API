// Controllers/ClientController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientController : ControllerBase
{
    private readonly IClientService _svc;
    public ClientController(IClientService svc) => _svc = svc;

    [HttpGet("search")]
    public async Task<ActionResult<List<ClientRechercheDTO>>> Search([FromQuery] string q, [FromQuery] int take = 10)
        => Ok(await _svc.SearchAsync(q, take));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClientResponseDTO>> Get(Guid id)
        => (await _svc.GetAsync(id)) is { } dto ? Ok(dto) : NotFound();

    [HttpPost]
    public async Task<ActionResult<ClientResponseDTO>> Create([FromBody] ClientCreateDTO dto)
    {
        try
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
