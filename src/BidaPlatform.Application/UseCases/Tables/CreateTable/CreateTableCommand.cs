using MediatR;
using BidaPlatform.Application.Models.Tables.CreateTable;

namespace BidaPlatform.Application.UseCases.Tables.CreateTable;

public record CreateTableCommand(
    Guid ActorUserId,
    Guid VenueId,
    string Name,
    string Type,
    decimal PricePerHour,
    string? DeviceIpAddress,
    string? DeviceName
) : IRequest;
