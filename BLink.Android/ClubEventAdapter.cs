using Android.App;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using BLink.Business.Models;
using Newtonsoft.Json;

namespace BLink.Droid
{
    public class ClubEventAdapter : RecyclerView.Adapter
    {
        private ClubEventFilterResult[] _clubEvents;
        private ClubEventFilterResult _clubEvent;
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
            _clubEvent = _clubEvents[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as ClubEventAdapterViewHolder;
            holder.EventType.Text = _clubEvent.EventType.ToString();
            holder.Title.Text = _clubEvent.Title;
            holder.Description.Text = _clubEvent.Description;
            holder.StartTime.Text = _clubEvent.StartTime.ToString();
            holder.ViewLocation.Click += ViewLocation_Click;
        }

        private void ViewLocation_Click(object sender, System.EventArgs e)
        {
            var content = JsonConvert.SerializeObject(_clubEvent.Coordinates);
            var intent = new Intent(_activity, typeof(GoogleMapActivity));
            intent.PutExtra("place", content);
            _activity.BaseContext.StartActivity(intent);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.ClubEventCard;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            return new ClubEventAdapterViewHolder(itemView);
        }
    }
}