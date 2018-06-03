using BLink.Business.Common;
using BLink.Business.Enums;
using BLink.Business.Models;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
            var content = new MultipartFormDataContent();
            HttpContent fileStreamContent = new StreamContent(registerUser.File);
            fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "userImage", FileName = "person-placeholder.jpg" };
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/jpeg");
            content.Add(fileStreamContent, "userImage");
            AddStringContent(content, registerUser);

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
            var content = new MultipartFormDataContent();
            HttpContent fileStreamContent = new StreamContent(createClub.ClubPhoto);
            fileStreamContent.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("form-data") { Name = "clubImage", FileName = "club-main-photo.jpg" };
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/jpeg");
            content.Add(fileStreamContent, "clubImage");
            AddStringContent(content, createClub);

            return await _httpClient.PostAsync(ApiConstants.CreateClubEndpoint, content);
        }

        public static async Task<HttpResponseMessage> EditClub(EditClub editClub)
        {
            var content = new MultipartFormDataContent();
            if (editClub.ClubPhoto != null)
            {
                HttpContent fileStreamContent = new StreamContent(editClub.ClubPhoto);
                fileStreamContent.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("form-data") { Name = "clubImage", FileName = "club-main-photo.jpg" };
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/jpeg");
                content.Add(fileStreamContent, "clubImage");
            }

            AddStringContent(content, editClub);

            return await _httpClient.PostAsync(string.Format(ApiConstants.EditClubEndpoint, editClub.ClubId), content);
        }

        public static async Task<HttpResponseMessage> RemoveEvent(int eventId)
        {
            return await _httpClient.DeleteAsync(string.Format(ApiConstants.RemoveEventEndpoint, eventId));
        }

        public static async Task<HttpResponseMessage> KickPlayer(KickPlayerRequest kickPlayerRequest)
        {
            return await _httpClient.PostAsync(
                string.Format(ApiConstants.KickPlayerEndpoint, kickPlayerRequest.ClubId, kickPlayerRequest.PlayerId),
                new StringContent(string.Empty));
        }

        public static async Task<string> GetClubPhoto(int cludId)
        {
            var url = new Uri(string.Format(ApiConstants.GetClubPhotoEndpoint, cludId));
            var imageStream = await _httpClient.GetStreamAsync(url);
            var imagePath = Path.Combine(AppConstants.UserImagesPath, AppConstants.MainClubPhotoFormat);
            using (var streamWriter = new FileStream(imagePath, FileMode.Create))
            {
                imageStream.CopyTo(streamWriter);
            }

            return imagePath;
        }

        public static async Task<HttpResponseMessage> GetMemberDetails(string email)
        {
            var builder = new UriBuilder(ApiConstants.GetMembersEndpoint)
            {
                Query = "email=" + email
            };

            return await _httpClient.GetAsync(builder.ToString());
        }

        public static async Task<HttpResponseMessage> GetClubPlayers(SearchPlayersCritera searchPlayersCritera)
        {
            var query = BuildQueryString(searchPlayersCritera);
            var builder = new UriBuilder(string.Format(ApiConstants.GetClubPlayersEndpoint, searchPlayersCritera.ClubId.Value))
            {
                Query = query
            };

            return await _httpClient.GetAsync(builder.ToString());
        }

        public static async Task<HttpResponseMessage> GetAvailablePlayers(SearchPlayersCritera searchPlayersCritera)
        {
            var query = BuildQueryString(searchPlayersCritera);
            var builder = new UriBuilder(ApiConstants.GetAvailablePlayersEndpoint)
            {
                Query = query
            };

            return await _httpClient.GetAsync(builder.ToString());
        }

        public static async Task<HttpResponseMessage> InvitePlayer(InvitePlayerRequest invitePlayer)
        {
            var jsonObject = JsonConvert.SerializeObject(invitePlayer);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(
                string.Format(ApiConstants.InvitePlayerEndpoint, invitePlayer.ClubId),
                content);
        }

        public static async Task<HttpResponseMessage> LeaveClub(string email)
        {
            return await _httpClient.PostAsync(
                string.Format(ApiConstants.LeaveClubEndpoint, email),
                new StringContent(string.Empty));
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

        public static async Task<HttpResponseMessage> EditClubEvent(ClubEventCreateRequest clubEventCreateRequest)
        {
            var jsonObject = JsonConvert.SerializeObject(clubEventCreateRequest);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(
                string.Format(ApiConstants.EditClubEventEndpoint, clubEventCreateRequest.EventId),
                content);
        }

        public static async Task<HttpResponseMessage> GetClubEvents(ClubEventFilterRequest clubEventFilterRequest)
        {
            var builder = new UriBuilder(ApiConstants.GetClubEventsEndpoint)
            {
                Query = BuildQueryString(clubEventFilterRequest)
            };

            return await _httpClient.GetAsync(builder.ToString());
        }

        public static async Task<string> GetMemberPhoto(string email)
        {
            var url = new Uri(string.Format(ApiConstants.GetMemberPhotoEndpoint, email));
            var imageStream = await _httpClient.GetStreamAsync(url);
            var imagePath = Path.Combine(AppConstants.UserImagesPath, AppConstants.MainPhotoFormat);
            using (var streamWriter = new FileStream(imagePath, FileMode.Create))
            {
                imageStream.CopyTo(streamWriter);
            }

            return imagePath;
        }

        public static async Task<HttpResponseMessage> EditMemberDetails(EditMemberDetails editMemberDetails)
        {
            var content = new MultipartFormDataContent();
            if (editMemberDetails.File != null)
            {
                HttpContent fileStreamContent = new StreamContent(editMemberDetails.File);
                fileStreamContent.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("form-data") { Name = "userImage", FileName = "person-placeholder.jpg" };
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/jpeg");
                content.Add(fileStreamContent, "userImage");
            }
            AddStringContent(content, editMemberDetails);

            return await _httpClient.PostAsync(
                string.Format(ApiConstants.EditMemberDetailsEndpoint, editMemberDetails.Email),
                content);
        }

        private static void AddStringContent<T>(MultipartFormDataContent content, T data)
        {
            var props = data.GetType().GetProperties();
            foreach (var prop in props)
            {
                string value = null;
                if (prop.PropertyType == typeof(DateTime?))
                {
                    var date = DateTime.Parse(prop.GetValue(data).ToString());
                    value = date.ToString("MM/dd/yyyy");
                }
                else if (prop.PropertyType != typeof(Stream))
                {
                    value = prop.GetValue(data)?.ToString();
                }

                if (value != null)
                {
                    var propName = prop.Name;
                    content.Add(new StringContent(value), propName);
                }
            }
        }

        private static string BuildQueryString<T>(T data)
        {
            var queryString = new StringBuilder();
            var props = data.GetType().GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(data);
                
                if (value != null)
                {
                    if (prop.PropertyType == typeof(double))
                    {
                        var s = Convert.ToInt32(value);
                        value = s.ToString();
                    }
                    queryString.Append($"{prop.Name}={value.ToString()}&");
                }
            }

            return queryString.ToString();
        }
    }
}