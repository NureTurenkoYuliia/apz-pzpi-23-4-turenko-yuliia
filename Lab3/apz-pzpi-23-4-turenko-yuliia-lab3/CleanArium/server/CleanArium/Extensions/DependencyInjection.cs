using Application.Abstractions;
using Application.AlarmRules.Commands.Create;
using Application.AlarmRules.Commands.Deactivate;
using Application.AlarmRules.Commands.Delete;
using Application.AlarmRules.Commands.Update;
using Application.AlarmRules.Queries.GetAllByDeviceId;
using Application.AlarmRules.Queries.GetById;
using Application.Aquariums.Commands.Create;
using Application.Aquariums.Commands.Delete;
using Application.Aquariums.Commands.Import;
using Application.Aquariums.Commands.Update;
using Application.Aquariums.Queries.ExportAquariumsCsv;
using Application.Aquariums.Queries.ExportAquariumsJson;
using Application.Aquariums.Queries.ExportAquariumsPdf;
using Application.Aquariums.Queries.GetAllByUserId;
using Application.Auth.Commands.ForgotPassword;
using Application.Auth.Commands.Login;
using Application.Auth.Commands.Logout;
using Application.Auth.Commands.RefreshToken;
using Application.Auth.Commands.Register;
using Application.Auth.Commands.ResetPassword;
using Application.Devices.Commands.Create;
using Application.Devices.Commands.Delete;
using Application.Devices.Commands.Update;
using Application.Devices.Queries.GetAllByAquariumId;
using Application.DTOs.AlarmRules;
using Application.DTOs.Aquariums;
using Application.DTOs.ScheduledCommands;
using Application.ExecutedCommands.Commands.Create;
using Application.ExecutedCommands.Queries.GetAllByDeviceId;
using Application.Notifications.Commands.MarkAsRead;
using Application.Notifications.Queries.GetAllByUserId;
using Application.ScheduledCommands.Commands.Create;
using Application.ScheduledCommands.Commands.Deactivate;
using Application.ScheduledCommands.Commands.Delete;
using Application.ScheduledCommands.Commands.Update;
using Application.ScheduledCommands.Queries.GetAllByDeviceId;
using Application.ScheduledCommands.Queries.GetById;
using Application.SensorData.Commands.Create;
using Application.SupportMessages.Commands.Create;
using Application.SupportMessages.Commands.Delete;
using Application.SupportMessages.Commands.Reply;
using Application.SupportMessages.Commands.Update;
using Application.SupportMessages.Queries.GetHistory;
using Application.SystemSettings.Commands.Update;
using Application.Users.Commands.AssignModerator;
using Application.Users.Commands.Block;
using Application.Users.Commands.Delete;
using Application.Users.Commands.RemoveModerator;
using Application.Users.Commands.Unblock;
using Application.Users.Queries.GetById;
using FluentValidation;
using Infrastructure.AdminSeed;
using Infrastructure.Authentication;
using Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Application;
using Persistence.Repositories;
using Persistence.Services;
using Persistence.Services.Token;

namespace CleanArium.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CleanAriumDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Persistence")
            ));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterCommandHandler).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddHttpContextAccessor();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IAquariumRepository, AquariumRepository>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IAlarmRuleRepository, AlarmRuleRepository>();
        services.AddScoped<IScheduledCommandRepository, ScheduledCommandRepository>();
        services.AddScoped<ISupportMessageRepository, SupportMessageRepository>();
        services.AddScoped<ISystemSettingsRepository, SystemSettingsRepository>();
        services.AddScoped<IExecutedCommandRepository, ExecutedCommandRepository>();
        services.AddScoped<ISensorDataRepository, SensorDataRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<AdminSeeder>();

        services.AddHttpContextAccessor();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddHostedService<RefreshTokenCleanupService>();
        services.AddScoped<IJsonExportService, JsonExportService>();
        services.AddScoped<IPdfExportService, PdfExportService>();
        services.AddScoped<IUserActivityService, UserActivityService>();
        services.AddScoped<IAlarmRuleProcessor, AlarmRuleProcessor>();
        services.AddScoped<ISensorDataService, SensorDataService>();
        services.AddScoped<IAlarmRuleAnalyticsService, AlarmRuleAnalyticsService>();
        services.AddScoped<ICommandAlarmAnalyticsService, CommandAlarmAnalyticsService>();

        services.AddScoped<IValidator<RegisterCommand>, RegisterCommandValidator>();
        services.AddScoped<IValidator<LoginCommand>, LoginCommandValidator>();
        services.AddScoped<IValidator<LogoutCommand>, LogoutCommandValidator>();
        services.AddScoped<IValidator<ForgotPasswordCommand>, ForgotPasswordCommandValidator>();
        services.AddScoped<IValidator<ResetPasswordCommand>, ResetPasswordCommandValidator>();
        services.AddScoped<IValidator<RefreshTokenCommand>, RefreshTokenCommandValidator>();

        services.AddScoped<IValidator<CreateAquariumCommand>, CreateAquariumCommandValidator>();
        services.AddScoped<IValidator<UpdateAquariumCommand>, UpdateAquariumCommandValidator>();
        services.AddScoped<IValidator<DeleteAquariumCommand>, DeleteAquariumCommandValidator>();
        services.AddScoped<IValidator<GetAquariumsByUserIdQuery>, GetAquariumsByUserIdQueryValidator>();
        services.AddScoped<IValidator<ImportAquariumsWithDevicesCommand>, ImportAquariumsWithDevicesCommandValidator>();
        services.AddScoped<IValidator<ExportAquariumsCsvQuery>, ExportAquariumsCsvQueryValidator>();
        services.AddScoped<IValidator<ExportAquariumsJsonQuery>, ExportAquariumsJsonQueryValidator>();
        services.AddScoped<IValidator<ExportAquariumsPdfQuery>, ExportAquariumsPdfQueryValidator>();

        services.AddScoped<IValidator<CreateDeviceCommand>, CreateDeviceCommandValidator>();
        services.AddScoped<IValidator<UpdateDeviceCommand>, UpdateDeviceCommandValidator>();
        services.AddScoped<IValidator<DeleteDeviceCommand>, DeleteDeviceCommandValidator>();
        services.AddScoped<IValidator<GetDevicesByAquariumIdQuery>, GetDevicesByAquariumIdQueryValidator>();

        services.AddScoped<IValidator<CreateAlarmRuleCommand>, CreateAlarmRuleCommandValidator>();
        services.AddScoped<IValidator<UpdateAlarmRuleCommand>, UpdateAlarmRuleCommandValidator>();
        services.AddScoped<IValidator<DeleteAlarmRuleCommand>, DeleteAlarmRuleCommandValidator>();
        services.AddScoped<IValidator<GetAlarmRulesByDeviceIdQuery>, GetAlarmRulesByDeviceIdQueryValidator>();
        services.AddScoped<IValidator<GetAlarmRuleByIdQuery>, GetAlarmRuleByIdQueryValidator>();
        services.AddScoped<IValidator<DeactivateAlarmRuleCommand>, DeactivateAlarmRuleCommandValidator>();

        services.AddScoped<IValidator<CreateScheduledCommand>, CreateScheduledCommandValidator>();
        services.AddScoped<IValidator<UpdateScheduledCommand>, UpdateScheduledCommandValidator>();
        services.AddScoped<IValidator<DeleteScheduledCommand>, DeleteScheduledCommandValidator>();
        services.AddScoped<IValidator<GetScheduledCommandsByDeviceIdQuery>, GetScheduledCommandsByDeviceIdQueryValidator>();
        services.AddScoped<IValidator<GetScheduledCommandByIdQuery>, GetScheduledCommandByIdQueryValidator>();
        services.AddScoped<IValidator<DeactivateScheduledCommand>, DeactivateScheduledCommandValidator>();

        services.AddScoped<IValidator<GetExecutedCommandsByDeviceIdQuery>, GetExecutedCommandsByDeviceIdQueryValidator>();
        services.AddScoped<IValidator<CreateExecutedCommand>, CreateExecutedCommandValidator>();
        services.AddScoped<IValidator<CreateSensorDataCommand>, CreateSensorDataCommandValidator>();

        services.AddScoped<IValidator<MarkAsReadCommand>, MarkAsReadCommandValidator>();
        services.AddScoped<IValidator<GetNotificationsQuery>, GetNotificationsQueryValidator>();

        services.AddScoped<IValidator<BlockUserCommand>, BlockUserCommandValidator>();
        services.AddScoped<IValidator<UnblockUserCommand>, UnblockUserCommandValidator>();
        services.AddScoped<IValidator<DeleteUserCommand>, DeleteUserCommandValidator>();
        services.AddScoped<IValidator<GetUserByIdQuery>, GetUserByIdQueryValidator>();
        services.AddScoped<IValidator<RemoveModeratorCommand>, RemoveModeratorCommandValidator>();
        services.AddScoped<IValidator<AssignModeratorCommand>, AssignModeratorCommandValidator>();

        services.AddScoped<IImportParser<ScheduledCommandImportDto>, ScheduledCommandImportParser>();
        services.AddScoped<IImportParser<AlarmRuleImportDto>, AlarmRuleImportParser>();
        services.AddScoped<IImportParser<AquariumImportDto>, AquariumImportParser>();

        services.AddScoped<IValidator<CreateMessageCommand>, CreateMessageCommandValidator>();
        services.AddScoped<IValidator<ReplyToMessageCommand>, ReplyToMessageCommandValidator>();
        services.AddScoped<IValidator<UpdateMessageCommand>, UpdateMessageCommandValidator>();
        services.AddScoped<IValidator<DeleteMessagesCommand>, DeleteMessagesCommandValidator>();
        services.AddScoped<IValidator<GetHistoryOfMessagesQuery>, GetHistoryOfMessagesQueryValidator>();

        services.AddScoped<IValidator<UpdateSystemSettingsCommand>, UpdateSystemSettingsCommandValidator>();

        return services;
    }
}
