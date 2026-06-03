using Application.DTOs.Auth;
using MediatR;

namespace Application.Auth.Commands.Register;

public record RegisterCommand(string UserName, string Email, string Password) : IRequest<AuthResponseDto>;