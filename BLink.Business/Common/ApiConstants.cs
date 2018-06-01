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

        public static readonly string InvitePlayerEndpoint = ApiUrl + "/Clubs/{0}/invite-player";

        public static readonly string RespondInvitationEndpoint = ApiUrl + "/Members/{0}/invitations/{1}/respond";

        public static readonly string GetMemberInvitationsEndpoint = ApiUrl + "/Members/{0}/Invitations";

        public static readonly string CreateClubEventEndpoint = ApiUrl + "/ClubEvents";

        public static readonly string GetClubEventsEndpoint = ApiUrl + "/ClubEvents";

        public static readonly string GetMemberPhotoEndpoint = ApiUrl + "/Members/{0}/mainPhoto";

        public static readonly string GetClubPhotoEndpoint = ApiUrl + "/Clubs/{0}/mainPhoto";

        public static readonly string EditMemberDetailsEndpoint = ApiUrl + "/Members/{0}";

        public static readonly string EditClubEndpoint = ApiUrl + "/Clubs/{0}";

        public static readonly string KickPlayerEndpoint = ApiUrl + "/Clubs/{0}/kickPlayer/{1}";

        public static readonly string LeaveClubEndpoint = ApiUrl + "/Members/{0}/leaveClub";

        public static readonly string EditClubEventEndpoint = ApiUrl + "/ClubEvents/{0}";

        public static readonly string RemoveEventEndpoint = ApiUrl + "/ClubEvents/{0}";
    }
}