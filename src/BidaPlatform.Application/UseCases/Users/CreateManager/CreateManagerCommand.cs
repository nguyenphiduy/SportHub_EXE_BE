using MediatR;

namespace BidaPlatform.Application.UseCases.Users.CreateManager;

public record CreateManagerCommand(string Email, string FullName) : IRequest;
