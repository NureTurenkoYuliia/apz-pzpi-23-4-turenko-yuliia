using Application.DTOs.Auth;
using MediatR;

namespace Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;