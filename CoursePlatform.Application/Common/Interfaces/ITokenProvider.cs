using System.Security.Claims;
using CoursePlatform.Application.Common.Models;

namespace CoursePlatform.Application.Common.Interfaces;

/// <summary>
/// Issues access tokens for authenticated identities.
/// </summary>
public interface ITokenProvider
{
    JwtTokenResultDto GenerateAccessToken(IEnumerable<Claim> claims);
}
