using BLink.Business.Common;
using BLink.Business.Enums;
using BLink.Business.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BLink.Business.Managers
{
    public static class RestManager
    {
        private static HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri(ApiConstants.ApiUrl)
        };

        private static string _accessToken;

        public static void SetAccessToken(string accessToken)
        {
            if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            }
        }

        public static async Task<HttpResponseMessage> RegisterUser(RegisterUser registerUser)
        {
            var jsonObject = JsonConvert.SerializeObject(registerUser);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(ApiConstants.RegisterEndpoint, content);
        }

        public static async Task<HttpResponseMessage> LoginUser(LoginUser loginUser)
        {
            var jsonObject = JsonConvert.SerializeObject(loginUser);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(ApiConstants.LoginEndpoint, content);
        }

        public static async Task<HttpResponseMessage> GetMemberClub(string email)
        {
            var fullEndpint = string.Format(ApiConstants.GetMemberClubEndpoint, email);
            return await _httpClient.GetAsync(fullEndpint);
        }

        public static async Task<HttpResponseMessage> CreateClub(CreateClub createClub)
        {
            var jsonObject = JsonConvert.SerializeObject(createClub);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(ApiConstants.CreateClubEndpoint, content);
        }

        public static async Task<HttpResponseMessage> GetMemberDetails(string email)
        {
            var builder = new UriBuilder(ApiConstants.GetMembersEndpoint)
            {
                Query = "email=" + email
            };

            return await _httpClient.GetAsync(builder.ToString());
        }

        public static async Task<HttpResponseMessage> GetClubPlayers(int clubId)
        {
            return await _httpClient.GetAsync(string.Format(ApiConstants.GetClubPlayersEndpoint, clubId));
        }

        public static async Task<HttpResponseMessage> GetAvailablePlayers()
        {
            return await _httpClient.GetAsync(ApiConstants.GetAvailablePlayersEndpoint);
        }

        public static async Task<HttpResponseMessage> InvitePlayer(int playerId, int clubId)
        {
            var jsonObject = JsonConvert.SerializeObject(new { playerId = playerId, Description = "Hello" }); // TODO Description from coach input
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(
                string.Format(ApiConstants.InvitePlayerEndpoint, clubId), 
                content);
        }

        public static async Task<HttpResponseMessage> RespondInvitation(
            string email, 
            int invitationId, 
            InvitationStatus invitationStatus)
        {
            var jsonObject = JsonConvert.SerializeObject(new { invitationStatus = invitationStatus });
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(
                string.Format(ApiConstants.RespondInvitationEndpoint, email, invitationId),
                content);
        }

        public static async Task<HttpResponseMessage> GetMemberInvitations(string email)
        {
            var fullEndpoint = string.Format(ApiConstants.GetMemberInvitationsEndpoint, email);
            return await _httpClient.GetAsync(fullEndpoint);
        }

        public static async Task<HttpResponseMessage> CreateClubEvent(
           ClubEventCreateRequest clubEventCreateRequest)
        {
            var jsonObject = JsonConvert.SerializeObject(clubEventCreateRequest);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(
                ApiConstants.CreateClubEventEndpoint,
                content);
        }

        public static async Task<HttpResponseMessage> GetClubEvents(ClubEventFilterRequest clubEventFilterRequest)
        {
            var builder = new UriBuilder(ApiConstants.GetClubEventsEndpoint)
            {
                Query = $"clubId={clubEventFilterRequest.ClubId}&memberId={clubEventFilterRequest.MemberId}"
            };

            return await _httpClient.GetAsync(builder.ToString());
        }
    }
}