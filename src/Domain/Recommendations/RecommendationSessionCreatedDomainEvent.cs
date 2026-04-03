using SharedKernel;

namespace Domain.Recommendations;

public sealed record RecommendationSessionCreatedDomainEvent(Guid SessionId)
    : IDomainEvent;
