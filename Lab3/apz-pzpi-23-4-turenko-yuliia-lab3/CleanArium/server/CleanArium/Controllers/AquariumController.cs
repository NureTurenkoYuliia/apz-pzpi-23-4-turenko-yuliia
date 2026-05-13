using Application.Abstractions;
using Application.Aquariums.Commands.Create;
using Application.Aquariums.Commands.Delete;
using Application.Aquariums.Commands.Import;
using Application.Aquariums.Commands.Update;
using Application.Aquariums.Queries.ExportAquariumsCsv;
using Application.Aquariums.Queries.ExportAquariumsJson;
using Application.Aquariums.Queries.ExportAquariumsPdf;
using Application.Aquariums.Queries.GetAllByUserId;
using CleanArium.Contracts.Aquariums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArium.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User")]
public class AquariumController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserService _userService;

    public AquariumController(IMediator mediator, IUserService userService)
    {
        _mediator = mediator;
        _userService = userService;
    }

    /// <summary>
    /// Creates a new aquarium for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// This endpoint validates the provided data and sends a command to create a new aquarium entity
    /// associated with the user's ID.
    /// </remarks>
    /// <param name="request">The request body containing the new aquarium's Name and Location.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A 200 OK response with the ID of the newly created aquarium if the operation is successful.
    /// </returns>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateAquariumRequest request, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new CreateAquariumCommand(
            UserId: userId,
            Name: request.Name,
            Location: request.Location
        );

        var result = await _mediator.Send(command, ct);

        return Ok(result);
    }

    /// <summary>
    /// Updates an existing aquarium belonging to the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// The user must be the owner of the aquarium to perform the update.
    /// </remarks>
    /// <param name="request">The request body containing the AquariumId, new Name, and new Location.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns> A 200 OK response if the update operation is successful.</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateAquariumRequest request, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new UpdateAquariumCommand(
            UserId: userId,
            AquariumId: request.AquariumId,
            Name: request.Name,
            Location: request.Location
        );

        await _mediator.Send(command, ct);

        return Ok();
    }

    /// <summary>
    /// Deletes an existing aquarium belonging to the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// The user must be the owner of the aquarium to perform the deletion.
    /// </remarks>
    /// <param name="aquariumId">The ID of the aquarium to be deleted, passed in the request body.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns> A 200 OK response if the deletion operation is successful.</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] long aquariumId, CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var command = new DeleteAquariumCommand(
            UserId: userId,
            AquariumId: aquariumId
        );

        await _mediator.Send(command, ct);

        return Ok();
    }

    /// <summary>
    /// Retrieves a list of all aquariums of the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// This endpoint queries for all aquarium records where for authenticated userId
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A 200 OK response containing a list of aquariums (DTOs).
    /// </returns>
    [HttpGet("get-all-by-user")]
    public async Task<IActionResult> GetAllByUser(CancellationToken ct)
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var query = new GetAquariumsByUserIdQuery(userId);
        var result = await _mediator.Send(query, ct);

        return Ok(result);
    }

    /// <summary>
    /// Exports the authenticated user's complete list of aquariums to a CSV file.
    /// </summary>
    /// <remarks>
    /// The data includes key information about each aquarium owned by the user.
    /// The file is named 'aquariums.csv'.
    /// </remarks>
    /// <returns>
    /// A File response containing the CSV data stream (Content-Type: text/csv).
    /// </returns>
    [HttpGet("export-csv")]
    public async Task<IActionResult> ExportCsv()
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var bytes = await _mediator.Send(new ExportAquariumsCsvQuery(userId));

        return File(bytes, "text/csv", $"aquariums_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv");
    }

    /// <summary>
    /// Exports the authenticated user's complete list of aquariums to a JSON file.
    /// </summary>
    /// <remarks>
    /// The resulting file contains structured JSON data representing the user's aquariums.
    /// The file is named 'aquariums.json'.
    /// </remarks>
    /// <returns>
    /// A File response containing the JSON data stream (Content-Type: application/json).
    /// </returns>
    [HttpGet("export/json")]
    public async Task<IActionResult> ExportJson()
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var bytes = await _mediator.Send(new ExportAquariumsJsonQuery(userId));

        return File(bytes, "application/json", $"aquariums_{DateTime.UtcNow:yyyyMMdd_HHmm}.json");
    }

    /// <summary>
    /// Exports the authenticated user's complete list of aquariums to a PDF document.
    /// </summary>
    /// <remarks>
    /// This action typically formats the aquarium data into a readable PDF report.
    /// The file is named 'aquariums.pdf'.
    /// </remarks>
    /// <returns>
    /// A File response containing the PDF data stream (Content-Type: application/pdf).
    /// </returns>
    [HttpGet("export/pdf")]
    public async Task<IActionResult> ExportPdf()
    {
        var userId = _userService.GetApplicationUserId()!.Value;

        var bytes = await _mediator.Send(new ExportAquariumsPdfQuery(userId));

        return File(bytes, "application/pdf", $"aquariums_{DateTime.UtcNow:yyyyMMdd_HHmm}.pdf");
    }

    /// <summary>
    /// Imports a collection of aquariums and their associated devices from an uploaded file.
    /// </summary>
    /// <remarks>
    /// The file format (e.g., CSV, JSON) is determined by the underlying Command logic.
    /// Records from the file are validated and linked to the authenticated user's account upon successful import.
    /// </remarks>
    /// <param name="file">The uploaded file containing the aquarium and device data.</param>
    /// <returns>
    /// A 200 OK response with a result object summarizing the import process, or 400 Bad Request if the file is empty.
    /// </returns>
    [HttpPost("import")]
    public async Task<IActionResult> ImportAquariums(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        var userId = _userService.GetApplicationUserId()!.Value;

        var result = await _mediator.Send(new ImportAquariumsWithDevicesCommand(userId, file));

        return Ok(result);
    }
}
