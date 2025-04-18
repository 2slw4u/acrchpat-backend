﻿using CoreService.Middlewares.Exceptions;
using CoreService.Middlewares.Authorization;

namespace CoreService.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static void UseExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddlewareService>();
        }

        public static void UseAuthorizationMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<AuthorizationMiddlewareService>();
        }
    }
}
