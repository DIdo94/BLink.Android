using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using BLink.Business.Models;

namespace BLink.Droid
{
    public class ClubEventAdapter : RecyclerView.Adapter
    {
        private ClubEventFilterResult[] _clubEvents;
        private Activity _activity;
        private ClubDetails _clubDetails;

        public ClubEventAdapter(Activity activity, ClubEventFilterResult[] clubEvents, ClubDetails clubDetails)
        {
            _clubEvents = clubEvents;
            _activity = activity;
            _clubDetails = clubDetails;
        }

        public override int ItemCount => _clubEvents.Length;

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var clubEvent = _clubEvents[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as ClubEventAdapterViewHolder;
            holder.EventType.Text = clubEvent.EventType.ToString();
            holder.Title.Text = clubEvent.Title;
            holder.Description.Text = clubEvent.Description;
            holder.StartTime.Text = clubEvent.StartTime.ToString();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.ClubEventCard;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            return new ClubEventAdapterViewHolder(itemView);
        }
    }
}