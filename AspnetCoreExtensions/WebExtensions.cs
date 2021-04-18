using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace AspnetCoreExtensions
{
    public static class WebExtensions
    {
        // clean version
        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        /// 
        /// <returns>
        /// true if the specified HTTP request is an AJAX request; otherwise, false.
        /// </returns>
        /// <param name="request">The HTTP request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
        public static bool IsAjaxRequest(this HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            var request = httpContext.Request;

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }

        //public static string GetFullName(this IIdentity identity)
        //{
        //    var claimsIdentity = identity as ClaimsIdentity;
        //    if (claimsIdentity == null)
        //        return null;

        //    var nameClaim = claimsIdentity.
        //    var claim = (identity as ClaimsIdentity).Claims.FirstOrDefault(x => x.Type == ClaimTypes);
        //    if (claim != null)
        //        return claim.Value;
        //    else
        //        return string.Empty;
        //}

        public static int GetUserId(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity == null)
                return -1;

            var userIdClaim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return -1;

            return System.Convert.ToInt32(userIdClaim.Value);
        }


        public static byte[] ToByteArray(this IFormFile file)
        {
            byte[] buffer = null;

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                buffer = ms.ToArray();
            }

            return buffer;
        }
    }
}
