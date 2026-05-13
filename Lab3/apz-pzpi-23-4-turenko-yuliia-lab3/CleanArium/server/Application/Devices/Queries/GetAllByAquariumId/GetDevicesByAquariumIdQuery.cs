using Application.DTOs.Devices;
using MediatR;

namespace Application.Devices.Queries.GetAllByAquariumId;

public record GetDevicesByAquariumIdQuery(long UserId, long AquariumId) : IRequest<List<DeviceDto>>;