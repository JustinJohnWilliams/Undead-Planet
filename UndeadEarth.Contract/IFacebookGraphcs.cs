using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IFacebookGraph
    {
        string GetAccessToken(string code, string redirectUri);
        string ProfilePicture(long facebookUserId);
        List<IFacebookFriend> GetFriends(string accessToken);
        IFacebookUser GetUser(string accessToken);
        bool ValidateSignedRequest(string signedRequest);
        IFacebookUser GetUserFromSignedRequest(string accessToken);
        string GetAccessTokenFromSignedRequest(string signedRequest);
        string GetRawJson(string signedRequest);
        bool IsLoggedIn(string signedRequest);
    }
}
