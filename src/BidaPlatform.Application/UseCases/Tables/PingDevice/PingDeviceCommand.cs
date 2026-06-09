using MediatR;

namespace BidaPlatform.Application.UseCases.Tables.PingDevice;

public record PingDeviceCommand(Guid TableId) : IRequest<PingDeviceResult>;

public record PingDeviceResult(bool IsOnline, string IpAddress);
