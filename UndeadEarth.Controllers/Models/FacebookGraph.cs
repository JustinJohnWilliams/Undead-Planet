using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UndeadEarth.Contract;
using System.Runtime.Serialization.Json;
using System.Net;
using System.IO;
using System.Text;
using UndeadEarth.Model;
using System.Security.Cryptography;

namespace UndeadEarth.Controllers.Models
{
    public class FacebookGraph : IFacebookGraph
    {
        private string _clientSecret;
        private string _clientId;
        /// <summary>
        /// Initializes a new instance of the FacebookGraph class.
        /// </summary>
        public FacebookGraph()
        {
            //this is your Facebook App ID
            _clientId = "115289678504285";

            //this is your Secret Key
            _clientSecret = "0547b032e5822fa745685d5f6f38c2e7";
        }

        string IFacebookGraph.GetAccessToken(string code, string redirectUri)
        {
            //we have to request an access token from the following Uri
            string url = "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}";

            //Create a webrequest to perform the request against the Uri
            WebRequest request = WebRequest.Create(string.Format(url, _clientId, redirectUri, _clientSecret, code));

            //read out the response as a utf-8 encoding and parse out the access_token
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader streamReader = new StreamReader(stream, encode);
            string accessToken = streamReader.ReadToEnd().Replace("access_token=", "");
            streamReader.Close();
            response.Close();

            return accessToken;
        }

        IFacebookUser IFacebookGraph.GetUser(string accessToken)
        {
            return GetUserForAccessToken(accessToken);
        }

        string IFacebookGraph.ProfilePicture(long facebookUserId)
        {
            string url = string.Format("https://graph.facebook.com/{0}?fields=picture&type=large", facebookUserId.ToString());

            WebRequest request = WebRequest.Create(url);

            //Get the response
            WebResponse response = request.GetResponse();

            //Get the response stream
            Stream stream = response.GetResponseStream();

            //Take our statically typed representation of the JSON User and deserialize the response stream
            //using the DataContractJsonSerializer
            DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(FacebookUser));
            FacebookUser user = new FacebookUser();
            user = dataContractJsonSerializer.ReadObject(stream) as FacebookUser;

            //close the stream
            response.Close();

            return user.picture;
        }

        bool IFacebookGraph.ValidateSignedRequest(string request)
        {
            if(string.IsNullOrEmpty(request))
            {
                return false;
            }

            if(request.Contains(".") == false)
            {
                return false;
            }

            string[] signedRequest = request.Split('.');
            string expectedSignature = signedRequest[0];
            string payload = signedRequest[1];

            // Attempt to get same hash
            var hmac = SignWithHmac(UTF8Encoding.UTF8.GetBytes(payload), UTF8Encoding.UTF8.GetBytes(_clientSecret));
            var hmacBase64 = ToUrlBase64String(hmac);

            return (hmacBase64 == expectedSignature);
        }

        IFacebookUser IFacebookGraph.GetUserFromSignedRequest(string signedRequest)
        {
            return GetUserForAccessToken(GetDecodedSignedRequest(signedRequest).oauth_token);
        }

        List<IFacebookFriend> IFacebookGraph.GetFriends(string accessToken)
        {
            FacebookFriendContainer container = Get<FacebookFriendContainer>("me/friends", accessToken);
            return container.data.Cast<IFacebookFriend>().ToList();
        }

        private string ToUrlBase64String(byte[] Input)
        {
            return Convert.ToBase64String(Input).Replace("=", String.Empty)
                                                .Replace('+', '-')
                                                .Replace('/', '_');
        }

        private byte[] SignWithHmac(byte[] dataToSign, byte[] keyBody)
        {
            using (var hmacAlgorithm = new HMACSHA256(keyBody))
            {
                hmacAlgorithm.ComputeHash(dataToSign);
                return hmacAlgorithm.Hash;
            }
        }

        private IFacebookUser GetUserForAccessToken(string accessToken)
        {
            return Get<FacebookUser>("me", accessToken);
        }

        

        private T Get<T>(string resource, string accessToken) where T : class, new()
        {
            resource = "https://graph.facebook.com/" + resource + "?access_token={0}";
            WebRequest request = WebRequest.Create(string.Format(resource, accessToken));

            //Get the response
            WebResponse response = request.GetResponse();

            //Get the response stream
            Stream stream = response.GetResponseStream();

            //Take our statically typed representation of the JSON User and deserialize the response stream
            //using the DataContractJsonSerializer
            DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));

            T result = dataContractJsonSerializer.ReadObject(stream) as T;

            //close the stream
            response.Close();

            return result;
        }

        string IFacebookGraph.GetAccessTokenFromSignedRequest(string signedRequest)
        {
            return GetDecodedSignedRequest(signedRequest).oauth_token;
        }

        private FacebookSignedRequest GetDecodedSignedRequest(string signedRequest)
        {
            string json = GetJsonString(signedRequest);

            DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(FacebookSignedRequest));
            FacebookSignedRequest facebookSignedRequest = new FacebookSignedRequest();
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(facebookSignedRequest.GetType());
                facebookSignedRequest = (FacebookSignedRequest)serializer.ReadObject(ms); // <== Your missing line
            }

            return facebookSignedRequest;
        }

        string IFacebookGraph.GetRawJson(string signedRequest)
        {
            return GetJsonString(signedRequest);
        }

        private string GetJsonString(string signedRequest)
        {
            string payload = signedRequest.Split('.').Last();
            UTF8Encoding encoding = new UTF8Encoding();
            string decodedJson = payload.Replace("=", string.Empty).Replace('-', '+').Replace('_', '/');
            byte[] base64JsonArray = Convert.FromBase64String(decodedJson.PadRight(decodedJson.Length + (4 - decodedJson.Length % 4) % 4, '='));
            string json = encoding.GetString(base64JsonArray);
            return json;
        }


        bool IFacebookGraph.IsLoggedIn(string signedRequest)
        {
            return !string.IsNullOrEmpty(GetDecodedSignedRequest(signedRequest).oauth_token);
        }
    }
}