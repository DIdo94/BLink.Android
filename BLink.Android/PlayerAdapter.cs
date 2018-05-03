using Android.App;
using Android.Views;
using Android.Support.V7.Widget;
using BLink.Business.Models;
using BLink.Business.Managers;
using System.Net.Http;
using Android.Widget;

namespace BLink.Droid
{
    public class PlayerAdapter : RecyclerView.Adapter
    {
        private MemberDetails[] _players;
        private Activity _activity;
        private ClubDetails _clubDetails;

        public PlayerAdapter(Activity activity, MemberDetails[] players, ClubDetails clubDetails)
        {
            _players = players;
            _activity = activity;
            _clubDetails = clubDetails;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup and inflate your layout here
            var id = Resource.Layout.PlayerCard;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            return new PlayerAdapterViewHolder(itemView);
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var player = _players[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as PlayerAdapterViewHolder;
            holder.Caption.Text = $"{player.FirstName} {player.LastName}";
            holder.Height.Text = player.Height.HasValue ? player.Height.Value.ToString() : "0";
            holder.Weight.Text = player.Height.HasValue ? player.Weight.Value.ToString() : "0";
            holder.InvitePlayer.Click += (sender, eventArgs) => 
                InvitePlayer_Click(sender, eventArgs, player.Id, _clubDetails.Id);
        }

        private async void InvitePlayer_Click(object sender, System.EventArgs e, int playerId, int clubId)
        {
            HttpResponseMessage invitePlayerHttpResponse = await RestManager.InvitePlayer(playerId, clubId);

            if (invitePlayerHttpResponse.IsSuccessStatusCode)
            {
                Toast.MakeText(_activity, "Играчът е поканен!", ToastLength.Short).Show();
            }
        }

        public override int ItemCount => _players.Length;
    }
}