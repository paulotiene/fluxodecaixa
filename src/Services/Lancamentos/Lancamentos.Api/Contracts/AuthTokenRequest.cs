using System.ComponentModel.DataAnnotations;

namespace Lancamentos.Api.Contracts.Requests;

public sealed class AuthTokenRequest
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}