using Shared.Security.Configuration;
using Shared.Security.Interfaces;
using Lancamentos.Api.Contracts.Requests;
using Lancamentos.Api.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Lancamentos.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly JwtOptions _jwtOptions;

    public AuthController(
        IJwtTokenGenerator jwtTokenGenerator,
        IOptions<JwtOptions> jwtOptions)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _jwtOptions = jwtOptions.Value;
    }

    [AllowAnonymous]
    [HttpPost("token")]
    public IActionResult Token([FromBody] AuthTokenRequest request)
    {
        if (request.UserName != "admin" || request.Password != "123456")
            return Unauthorized(new { message = "Usuário ou senha inválidos." });

        var token = _jwtTokenGenerator.GenerateToken(request.UserName, "Admin");

        return Ok(new AuthTokenResponse
        {
            AccessToken = token,
            ExpiresIn = _jwtOptions.ExpirationMinutes * 60
        });
    }
}