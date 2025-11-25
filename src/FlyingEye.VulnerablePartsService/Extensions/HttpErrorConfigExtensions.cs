using System.Net;
using FlyingEye.Exceptions;
using Volo.Abp.AspNetCore.ExceptionHandling;

namespace FlyingEye.Extensions
{
    public static class HttpErrorConfigExtensions
    {
        public static IServiceCollection ConfigExceptionToHttpErrors(this IServiceCollection services)
        {
            services.Configure<AbpExceptionHttpStatusCodeOptions>(options =>
            {
                // HttpError:BadRequest 相关错误码映射到 400
                options.Map(HttpErrorCodes.BadRequest, HttpStatusCode.BadRequest);
                options.Map(HttpErrorCodes.InvalidInput, HttpStatusCode.BadRequest);
                options.Map(HttpErrorCodes.MissingRequiredField, HttpStatusCode.BadRequest);
                options.Map(HttpErrorCodes.ValidationFailed, HttpStatusCode.BadRequest);
                options.Map(HttpErrorCodes.InvalidFormat, HttpStatusCode.BadRequest);

                // HttpError:Unauthorized 相关错误码映射到 401
                options.Map(HttpErrorCodes.Unauthorized, HttpStatusCode.Unauthorized);
                options.Map(HttpErrorCodes.TokenExpired, HttpStatusCode.Unauthorized);
                options.Map(HttpErrorCodes.TokenInvalid, HttpStatusCode.Unauthorized);
                options.Map(HttpErrorCodes.LoginRequired, HttpStatusCode.Unauthorized);

                // HttpError:Forbidden 相关错误码映射到 403
                options.Map(HttpErrorCodes.Forbidden, HttpStatusCode.Forbidden);
                options.Map(HttpErrorCodes.InsufficientPermissions, HttpStatusCode.Forbidden);
                options.Map(HttpErrorCodes.AccessDenied, HttpStatusCode.Forbidden);
                options.Map(HttpErrorCodes.AccountDisabled, HttpStatusCode.Forbidden);

                // HttpError:NotFound 相关错误码映射到 404
                options.Map(HttpErrorCodes.NotFound, HttpStatusCode.NotFound);
                options.Map(HttpErrorCodes.ResourceNotFound, HttpStatusCode.NotFound);
                options.Map(HttpErrorCodes.UserNotFound, HttpStatusCode.NotFound);
                options.Map(HttpErrorCodes.FileNotFound, HttpStatusCode.NotFound);

                // HttpError:Conflict 相关错误码映射到 409
                options.Map(HttpErrorCodes.Conflict, HttpStatusCode.Conflict);
                options.Map(HttpErrorCodes.ResourceExists, HttpStatusCode.Conflict);
                options.Map(HttpErrorCodes.ConcurrentModification, HttpStatusCode.Conflict);
                options.Map(HttpErrorCodes.VersionConflict, HttpStatusCode.Conflict);

                // HttpError:UnprocessableEntity 相关错误码映射到 422
                options.Map(HttpErrorCodes.UnprocessableEntity, HttpStatusCode.UnprocessableEntity);
                options.Map(HttpErrorCodes.BusinessRuleViolation, HttpStatusCode.UnprocessableEntity);
                options.Map(HttpErrorCodes.InvalidState, HttpStatusCode.UnprocessableEntity);
                options.Map(HttpErrorCodes.PreconditionFailed, HttpStatusCode.UnprocessableEntity);

                // HttpError:TooManyRequests 相关错误码映射到 429
                options.Map(HttpErrorCodes.TooManyRequests, HttpStatusCode.TooManyRequests);
                options.Map(HttpErrorCodes.RateLimitExceeded, HttpStatusCode.TooManyRequests);
                options.Map(HttpErrorCodes.QuotaExceeded, HttpStatusCode.TooManyRequests);

                // HttpError:InternalServerError 相关错误码映射到 500
                options.Map(HttpErrorCodes.InternalServerError, HttpStatusCode.InternalServerError);
                options.Map(HttpErrorCodes.ServiceUnavailable, HttpStatusCode.InternalServerError);
                options.Map(HttpErrorCodes.DatabaseError, HttpStatusCode.InternalServerError);
                options.Map(HttpErrorCodes.ExternalServiceError, HttpStatusCode.InternalServerError);
            });

            return services;
        }
    }
}
