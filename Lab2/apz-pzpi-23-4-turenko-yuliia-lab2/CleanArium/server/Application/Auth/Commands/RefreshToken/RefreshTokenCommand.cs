using Application.DTOs.Auth;
using MediatR;

namespace Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;
