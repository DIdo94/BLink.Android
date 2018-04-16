namespace BLink.Business.Common
{
    public static class ApiConstants
    {
        public const string ApiUrl = "http://10.0.2.2:5000/api";

        public static readonly string LoginEndpoint = $"{ApiUrl}/Accounts/Login";

        public static readonly string RegisterEndpoint = $"{ApiUrl}/Accounts/Register";

        public static readonly string CreateClubEndpoint = $"{ApiUrl}/Clubs";

        public static readonly string GetMembersEndpoint = $"{ApiUrl}/Members";

        public static readonly string GetMemberClubEndpoint = ApiUrl + "/Members/{0}/Club";

        public static readonly string GetClubPlayersEndpoint = ApiUrl + "/Clubs/{0}/Players";

        public static readonly string GetAvailablePlayersEndpoint = $"{ApiUrl}/Players";
    }
}