﻿using System.Security.Claims;
using System.Text;

namespace StudyTeknik.Extensions
{
    /// <summary>
    /// Extension methods for extracting user identity from claims
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        private const string InternalUserIdClaimType = "InternalUserId";

        /// <summary>
        /// Extract the User ID from claims with multiple fallback strategies
        /// 
        /// Priority order:
        /// 1. "InternalUserId" claim (set by UserProvisioningMiddleware after finding/creating user in DB)
        /// 2. "sub" claim (direct from IdP)
        /// 3. ClaimTypes.NameIdentifier (mapped by ASP.NET Core)
        /// 4. "oid" claim (Azure AD)
        /// 
        /// For string-based IDs (like from Logto), converts to GUID using deterministic hash
        /// </summary>
        public static Guid? GetUserIdAsGuid(this ClaimsPrincipal user)
        {
            if (user == null)
                return null;

            // 🔐 STRATEGY 1: Look for InternalUserId claim (set by UserProvisioningMiddleware)
            // This is the most reliable as it comes from the database
            var internalIdClaim = user.FindFirstValue(InternalUserIdClaimType);
            if (!string.IsNullOrEmpty(internalIdClaim))
            {
                if (Guid.TryParse(internalIdClaim, out var internalGuid))
                    return internalGuid;
            }

            // Strategy 2: Try "sub" claim (most common in JWT from IdP)
            var subClaim = user.FindFirstValue("sub");
            if (!string.IsNullOrEmpty(subClaim))
            {
                if (Guid.TryParse(subClaim, out var subGuid))
                    return subGuid;
                else
                    return ConvertStringToGuid(subClaim);
            }

            // Strategy 3: Try ClaimTypes.NameIdentifier (alternate mapping)
            var nameIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(nameIdClaim))
            {
                if (Guid.TryParse(nameIdClaim, out var nameIdGuid))
                    return nameIdGuid;
                else
                    return ConvertStringToGuid(nameIdClaim);
            }

            // Strategy 4: Try "oid" claim (common in Azure AD)
            var oidClaim = user.FindFirstValue("oid");
            if (!string.IsNullOrEmpty(oidClaim) && Guid.TryParse(oidClaim, out var oidGuid))
                return oidGuid;

            // Strategy 5: Try "http://schemas.microsoft.com/identity/claims/objectidentifier" (Azure AD alt format)
            var azureOidClaim = user.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
            if (!string.IsNullOrEmpty(azureOidClaim) && Guid.TryParse(azureOidClaim, out var azureGuid))
                return azureGuid;

            // No claim found
            return null;
        }

        /// <summary>
        /// Extract the User ID from claims as string
        /// </summary>
        public static string? GetUserIdAsString(this ClaimsPrincipal user)
        {
            if (user == null)
                return null;

            // Strategy 1: Try "sub" claim
            var subClaim = user.FindFirstValue("sub");
            if (!string.IsNullOrEmpty(subClaim))
                return subClaim;

            // Strategy 2: Try ClaimTypes.NameIdentifier
            var nameIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(nameIdClaim))
                return nameIdClaim;

            // Strategy 3: Try "oid" claim
            var oidClaim = user.FindFirstValue("oid");
            if (!string.IsNullOrEmpty(oidClaim))
                return oidClaim;

            return null;
        }

        /// <summary>
        /// Debug method: Get all claims for troubleshooting
        /// </summary>
        public static Dictionary<string, string> GetAllClaims(this ClaimsPrincipal user)
        {
            var claims = new Dictionary<string, string>();
            if (user?.Claims == null)
                return claims;

            foreach (var claim in user.Claims)
            {
                if (!claims.ContainsKey(claim.Type))
                    claims[claim.Type] = claim.Value;
            }

            return claims;
        }

        /// <summary>
        /// Convert a string to a deterministic GUID using SHA-256 hash
        /// This is useful for IdPs like Logto that provide string-based user IDs
        /// 
        /// Example: "rxr7ymm4581s" → "5c7a3f2e-1234-5678-9abc-def012345678"
        /// Same input always produces same output (deterministic)
        /// </summary>
        /// <param name="input">The string to convert (e.g., "rxr7ymm4581s")</param>
        /// <returns>A deterministic GUID based on the string</returns>
        private static Guid ConvertStringToGuid(string input)
        {
            if (string.IsNullOrEmpty(input))
                return Guid.Empty;

            // Use SHA-256 to hash the string
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                
                // Take first 16 bytes of SHA-256 hash to create GUID
                // This ensures the same string always produces the same GUID
                var guidBytes = new byte[16];
                Array.Copy(hashBytes, guidBytes, 16);
                
                return new Guid(guidBytes);
            }
        }
    }
}


