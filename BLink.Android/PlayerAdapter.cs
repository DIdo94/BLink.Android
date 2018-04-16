using Android.App;
using Android.Views;
using Android.Support.V7.Widget;
using BLink.Business.Models;

namespace BLink.Droid
{
    public class PlayerAdapter : RecyclerView.Adapter
    {

        MemberDetails[] players;
        Activity _activity;

        public PlayerAdapter(Activity activity, MemberDetails[] images)
        {
            players = images;
            _activity = activity;
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
            var item = players[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as PlayerAdapterViewHolder;
            holder.Caption.Text = $"{item.FirstName} {item.LastName}";
        }

        public override int ItemCount => players.Length;
    }
}