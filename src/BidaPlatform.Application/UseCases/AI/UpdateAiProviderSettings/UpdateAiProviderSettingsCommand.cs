using MediatR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.AI;

namespace BidaPlatform.Application.UseCases.AI.UpdateAiProviderSettings;

public record UpdateAiProviderSettingsCommand(
    Guid ActorUserId,
    UpdateAiProviderSettingsRequest Request
) : IRequest<AiProviderSettingsResponse>;

public class UpdateAiProviderSettingsHandler : IRequestHandler<UpdateAiProviderSettingsCommand, AiProviderSettingsResponse>
{
    private readonly IAiProviderSettingsService _service;

    public UpdateAiProviderSettingsHandler(IAiProviderSettingsService service)
    {
        _service = service;
    }

    public async Task<AiProviderSettingsResponse> Handle(UpdateAiProviderSettingsCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Request.ApiKey))
            throw new ArgumentException("API key không được để trống.");

        return await _service.UpdateSettingsAsync(request.ActorUserId, request.Request, ct);
    }
}
