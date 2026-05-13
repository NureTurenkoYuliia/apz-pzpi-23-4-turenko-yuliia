using Application.DTOs.Result;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Aquariums.Commands.Import;

public record ImportAquariumsWithDevicesCommand(long UserId, IFormFile File) : IRequest<ImportResult>;