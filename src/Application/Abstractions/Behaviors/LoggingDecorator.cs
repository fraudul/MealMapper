using Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel;

namespace Application.Abstractions.Behaviors;

internal static partial class LoggingDecorator
{
    // Command with response
    internal sealed partial class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>                    // ← исправлено
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            LogProcessingCommand(logger, commandName);

            Result<TResponse> result = await innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                LogCompletedCommand(logger, commandName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    LogCompletedCommandWithError(logger, commandName);
                }
            }

            return result;
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Processing command {CommandName}")]
        private static partial void LogProcessingCommand(ILogger logger, string commandName);

        [LoggerMessage(Level = LogLevel.Information, Message = "Completed command {CommandName}")]
        private static partial void LogCompletedCommand(ILogger logger, string commandName);

        [LoggerMessage(Level = LogLevel.Error, Message = "Completed command {CommandName} with error")]
        private static partial void LogCompletedCommandWithError(ILogger logger, string commandName);
    }

    // Command without response (void-style)
    internal sealed partial class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,                 // ← исправлено
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand>
        where TCommand : ICommand                               // ← исправлено
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            LogProcessingCommand(logger, commandName);

            Result result = await innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                LogCompletedCommand(logger, commandName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    LogCompletedCommandWithError(logger, commandName);
                }
            }

            return result;
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Processing command {CommandName}")]
        private static partial void LogProcessingCommand(ILogger logger, string commandName);

        [LoggerMessage(Level = LogLevel.Information, Message = "Completed command {CommandName}")]
        private static partial void LogCompletedCommand(ILogger logger, string commandName);

        [LoggerMessage(Level = LogLevel.Error, Message = "Completed command {CommandName} with error")]
        private static partial void LogCompletedCommandWithError(ILogger logger, string commandName);
    }

    // Query
    internal sealed partial class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>                        // ← исправлено
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            string queryName = typeof(TQuery).Name;

            LogProcessingQuery(logger, queryName);

            Result<TResponse> result = await innerHandler.Handle(query, cancellationToken);

            if (result.IsSuccess)
            {
                LogCompletedQuery(logger, queryName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    LogCompletedQueryWithError(logger, queryName);
                }
            }

            return result;
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Processing query {QueryName}")]
        private static partial void LogProcessingQuery(ILogger logger, string queryName);

        [LoggerMessage(Level = LogLevel.Information, Message = "Completed query {QueryName}")]
        private static partial void LogCompletedQuery(ILogger logger, string queryName);

        [LoggerMessage(Level = LogLevel.Error, Message = "Completed query {QueryName} with error")]
        private static partial void LogCompletedQueryWithError(ILogger logger, string queryName);
    }
}
