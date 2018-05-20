using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace BLink.Droid
{
    public class ClubEventAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView EventType { get; set; }

        public TextView Title { get; set; }

        public TextView Description { get; set; }

        public TextView StartTime { get; set; }

        public Button ViewLocation { get; set; }

        public ClubEventAdapterViewHolder(View itemView) : base(itemView)
        {
            EventType = itemView.FindViewById<TextView>(Resource.Id.tv_cec_clubEventType);
            Title = itemView.FindViewById<TextView>(Resource.Id.tv_cec_clubEventTitle);
            Description = itemView.FindViewById<TextView>(Resource.Id.tv_cec_clubEventDescription);
            StartTime = itemView.FindViewById<TextView>(Resource.Id.tv_cec_clubEventStartTime);
            ViewLocation = itemView.FindViewById<Button>(Resource.Id.btn_cec_viewOnMap);
        }
    }
}