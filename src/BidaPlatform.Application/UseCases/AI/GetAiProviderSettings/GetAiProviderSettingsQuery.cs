using MediatR;
using BidaPlatform.Application.Interfaces;
using BidaPlatform.Application.Models.AI;

namespace BidaPlatform.Application.UseCases.AI.GetAiProviderSettings;

public record GetAiProviderSettingsQuery : IRequest<AiProviderSettingsResponse>;

public class GetAiProviderSettingsHandler : IRequestHandler<GetAiProviderSettingsQuery, AiProviderSettingsResponse>
{
    private readonly IAiProviderSettingsService _service;

    public GetAiProviderSettingsHandler(IAiProviderSettingsService service)
    {
        _service = service;
    }

    public async Task<AiProviderSettingsResponse> Handle(GetAiProviderSettingsQuery request, CancellationToken ct)
    {
        return await _service.GetSettingsAsync(ct);
    }
}
