using Application.Recommendations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Web.Api.Endpoints;

public static class RecommendationsEndpoints
{
    public static void MapRecommendationsEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/recommendations")
            .WithTags("Recommendations");

        group.MapPost("/session", async (
                [FromBody] CreateRecommendationSessionCommand command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                Result<RecommendationSessionResponse> result = await mediator.Send(command, cancellationToken);

                return result.IsSuccess 
                    ? Results.Ok(result.Value) 
                    : Results.BadRequest(result.Error);
            })
            .Produces<RecommendationSessionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithName("CreateRecommendationSession");
    }
}
