using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace BlazorAppWithEntraIDJWT.Models
{
    public class AuthenticatedUser
    {
        public string Access_Token { get; set; }
        public string UserName { get; set; }

    }
}
