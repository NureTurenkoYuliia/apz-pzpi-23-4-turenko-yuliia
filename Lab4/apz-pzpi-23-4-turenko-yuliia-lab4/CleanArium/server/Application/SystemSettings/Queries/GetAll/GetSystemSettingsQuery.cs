using Application.DTOs.SystemSettings;
using MediatR;

namespace Application.SystemSettings.Queries.GetAll;

public record GetSystemSettingsQuery() : IRequest<SystemSettingsDto>;
