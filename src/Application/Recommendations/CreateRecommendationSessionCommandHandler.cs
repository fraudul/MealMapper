using Application.Abstractions;
using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Domain.Recommendations;
using MediatR;
using SharedKernel;

namespace Application.Recommendations;

/*internal sealed class CreateRecommendationSessionCommandHandler 
    : ICommandHandler<CreateRecommendationSessionCommand, Result<RecommendationSessionResponse>>
{
    private readonly IRecommendationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateRecommendationSessionCommandHandler(IRecommendationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Result<RecommendationSessionResponse>>> Handle(
        CreateRecommendationSessionCommand request, 
        CancellationToken cancellationToken)
    {
        // 1. Создаём Value Objects
        var userLocation = new GeoLocation(request.Latitude, request.Longitude);
        userLocation.Validate();

        var budget = new BudgetRange(request.BudgetMin, request.BudgetMax);
        budget.Validate();

        var preferences = new UserPreferences(
            request.ActionType,
            request.IsFastFoodAllowed,
            request.DistancePref,
            request.CuisinePreference,
            request.OnlyNearby);

        // 2. Создаём Aggregate
        var session = new RecommendationSession(userLocation, budget, preferences);

        // 3. Получаем рекомендации из репозитория
        List<RecommendationItem> recommendations;

        if (preferences.ActionType == FoodActionType.CookAtHome)
        {
            List<Recipe> matchingRecipes = await _repository.FindMatchingRecipesAsync(
                budget, preferences, 10, cancellationToken);

            recommendations = matchingRecipes.Select(r => new RecommendationItem(
                r.Id,
                r.Name,
                $"{r.EstimatedCost} BYN • {r.CookingTimeMinutes} мин",
                "Recipe",
                0.85, // временный скоринг
                "Подходит под ваш бюджет и время"
            )).ToList();
        }
        else
        {
            List<FoodPlace> matchingPlaces = await _repository.FindMatchingFoodPlacesAsync(
                userLocation, budget, preferences, 10, cancellationToken);

            recommendations = matchingPlaces.Select(p => new RecommendationItem(
                p.Id,
                p.Name,
                $"{p.AveragePricePerPerson} BYN • {Math.Round(p.Location.DistanceTo(userLocation)/1000, 1)} км",
                "FoodPlace",
                p.Rating / 5.0,
                p.IsOpenNow ? "Открыто сейчас" : "Закрыто"
            )).ToList();
        }

        // 4. Добавляем в Aggregate и сохраняем
        session.AddRecommendations(recommendations);

        await _repository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Возвращаем ответ
        var response = new RecommendationSessionResponse(session.Id, recommendations.Take(3).ToList());

        return Result.Success(response);
        // 3. Генерируем рекомендации (Mock) — основной фокус сейчас
        var recommendations = GenerateMockRecommendations(request, userLocation, budget, preferences);

        // 4. Добавляем рекомендации в сессию
        session.AddRecommendations(recommendations);

        // 5. Сохраняем в БД
        await _repository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Возвращаем только топ-3
        var response = new RecommendationSessionResponse(
            session.Id,
            recommendations.Take(3).ToList());

        return Result.Success(response);
    }

    private List<RecommendationItem> GenerateMockRecommendations(
        CreateRecommendationSessionCommand request,
        GeoLocation userLocation,
        BudgetRange budget,
        UserPreferences preferences)
    {
        var items = new List<RecommendationItem>();

        // Пример 1: Ресторан / Кафе
        items.Add(new RecommendationItem(
            Guid.NewGuid(),
            "Burger King - Немига",
            $"15–28 BYN • ~0.8 км • Открыто до 23:00",
            "FoodPlace",
            CalculateScore(budget, preferences, 0.8, true),
            "Отличный фастфуд в пределах бюджета, рядом с тобой"
        ));

        // Пример 2: Доставка / Ресторан
        items.Add(new RecommendationItem(
            Guid.NewGuid(),
            "Пицца Папа Джонс (доставка)",
            $"18–35 BYN • Доставка 25–40 мин",
            "FoodPlace",
            CalculateScore(budget, preferences, 3.5, false),
            "Хорошая доставка, подходит под твой бюджет"
        ));

        // Пример 3: Рецепт (если выбрано приготовить дома)
        if (preferences.ActionType == FoodActionType.CookAtHome)
        {
            items.Add(new RecommendationItem(
                Guid.NewGuid(),
                "Курица с овощами в духовке",
                "~12–18 BYN • 35 минут приготовления",
                "Recipe",
                CalculateScore(budget, preferences, 0, true),
                "Простой и быстрый рецепт под твой бюджет"
            ));
        }
        else
        {
            // Пример 3: Супермаркет / Кафе
            items.Add(new RecommendationItem(
                Guid.NewGuid(),
                "Суши Весло - Октябрьская",
                $"22–40 BYN • ~2.1 км",
                "FoodPlace",
                CalculateScore(budget, preferences, 2.1, true),
                "Популярное место с хорошим рейтингом"
            ));
        }

        // Сортируем по скорингу
        return items.OrderByDescending(x => x.Score).ToList();
    }

    private double CalculateScore(
        BudgetRange budget,
        UserPreferences preferences,
        double distanceKm,
        bool isFastFoodOrSuitable)
    {
        double score = 0.0;

        // 1. Budget Match (40%)
        double budgetMatch = budget.IsInRange(20m) ? 1.0 : 0.7; // упрощённо
        score += budgetMatch * 0.40;

        // 2. Action Match (30%)
        double actionMatch = preferences.ActionType switch
        {
            FoodActionType.EatOut => 0.95,
            FoodActionType.OrderDelivery => 0.85,
            FoodActionType.CookAtHome => 0.90,
            _ => 0.7
        };
        score += actionMatch * 0.30;

        // 3. Distance Match (20%)
        double distanceMatch = distanceKm switch
        {
            < 1.0 => 1.0,
            < 3.0 => 0.85,
            < 7.0 => 0.65,
            _ => 0.4
        };
        score += distanceMatch * 0.20;

        // 4. FastFood / Preferences Match (10%)
        double fastFoodMatch = (preferences.IsFastFoodAllowed && isFastFoodOrSuitable) ? 1.0 : 0.75;
        score += fastFoodMatch * 0.10;

        return Math.Min(1.0, Math.Max(0.65, score)); // минимум 65% для красоты
    }
    }*/
// Application/Recommendations/CreateRecommendationSessionCommandHandler.cs
internal sealed class CreateRecommendationSessionCommandHandler 
    : ICommandHandler<CreateRecommendationSessionCommand, Result<RecommendationSessionResponse>>
{
    private readonly IRecommendationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRecommendationSessionCommandHandler(
        IRecommendationRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Result<RecommendationSessionResponse>>> Handle(
        CreateRecommendationSessionCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Создаём Value Objects
        var userLocation = new GeoLocation(request.Latitude, request.Longitude);
        var budget = new BudgetRange(request.BudgetMin, request.BudgetMax);
        var preferences = new UserPreferences(
            request.ActionType,
            request.IsFastFoodAllowed,
            request.DistancePref,
            request.CuisinePreference,
            request.OnlyNearby);

        // 2. Создаём Aggregate Root
        var session = RecommendationSession.Create(userLocation, budget, preferences);

        // 3. Генерируем рекомендации (Mock)
        List<RecommendationItem> recommendations = GenerateMockRecommendations(request, budget, preferences);

        // 4. Добавляем рекомендации в сессию
        session.AddRecommendations(recommendations);

        // 5. Сохраняем в БД
        await _repository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Возвращаем только топ-3
        var response = new RecommendationSessionResponse(
            session.Id,
            recommendations.Take(3).ToList());

        return Result.Success(response);
    }

    private List<RecommendationItem> GenerateMockRecommendations(
        CreateRecommendationSessionCommand request,
        //GeoLocation userLocation,
        BudgetRange budget,
        UserPreferences preferences)
    {
        var items = new List<RecommendationItem>
        {
            // Рекомендация 1
            new RecommendationItem(
                Guid.NewGuid(),
                "Burger King - Немига",
                "15–28 BYN • ~0.8 км • Открыто до 23:00",
                "FoodPlace",
                CalculateScore(budget, preferences, 0.8, true),
                "Отличный фастфуд в пределах бюджета, рядом с тобой"
            ),
            // Рекомендация 2
            new RecommendationItem(
                Guid.NewGuid(),
                "Пицца Папа Джонс (доставка)",
                "18–35 BYN • Доставка 25–40 мин",
                "FoodPlace",
                CalculateScore(budget, preferences, 3.5, false),
                "Хорошая доставка, подходит под твой бюджет"
            )
        };

        // Рекомендация 3
        if (request.ActionType == FoodActionType.CookAtHome)
        {
            items.Add(new RecommendationItem(
                Guid.NewGuid(),
                "Курица с овощами в духовке",
                "~12–18 BYN • 35 минут приготовления",
                "Recipe",
                CalculateScore(budget, preferences, 0, true),
                "Простой и быстрый рецепт под твой бюджет"
            ));
        }
        else
        {
            items.Add(new RecommendationItem(
                Guid.NewGuid(),
                "Суши Весло - Октябрьская",
                "22–40 BYN • ~2.1 км",
                "FoodPlace",
                CalculateScore(budget, preferences, 2.1, true),
                "Популярное место с хорошим рейтингом"
            ));
        }

        // Сортируем по скорингу
        return items.OrderByDescending(x => x.Score).ToList();
    }

    private double CalculateScore(
        BudgetRange budget,
        UserPreferences preferences,
        double distanceKm,
        bool isFastFoodOrSuitable)
    {
        double score = 0.0;

        // 1. Budget Match (40%)
        double budgetMatch = budget.Min <= 25 && budget.Max >= 15 ? 1.0 : 0.75;
        score += budgetMatch * 0.40;

        // 2. Action Match (30%)
        double actionMatch = preferences.ActionType switch
        {
            FoodActionType.Buy => 0.95,
            FoodActionType.Order => 0.85,
            FoodActionType.CookAtHome => 0.90,
            _ => 0.70
        };
        score += actionMatch * 0.30;

        // 3. Distance Match (20%)
        double distanceMatch = distanceKm switch
        {
            < 1.0 => 1.0,
            < 3.0 => 0.85,
            < 7.0 => 0.65,
            _ => 0.40
        };
        score += distanceMatch * 0.20;

        // 4. FastFood Match (10%)
        double fastFoodMatch = preferences.IsFastFoodAllowed && isFastFoodOrSuitable ? 1.0 : 0.75;
        score += fastFoodMatch * 0.10;

        return Math.Clamp(score, 0.65, 1.0); // минимум 65% для красоты
    }
}
