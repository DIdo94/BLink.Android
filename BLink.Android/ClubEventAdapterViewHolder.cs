using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace BLink.Droid
{
    public class ClubEventAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView Heading { get; set; }

        public TextView Description { get; set; }

        public TextView DescriptionIcon { get; set; }

        public TextView StartTime { get; set; }

        public TextView StartTimeIcon { get; set; }

        public Button ViewLocation { get; set; }

        public LinearLayout ClubEventActions { get; set; }

        public Button EditClubEvent { get; set; }

        public Button RemoveClubEvent { get; set; }

        public ClubEventAdapterViewHolder(View itemView) : base(itemView)
        {
            Heading = itemView.FindViewById<TextView>(Resource.Id.tv_cec_clubEventHeading);
            Description = itemView.FindViewById<TextView>(Resource.Id.tv_cec_clubEventDescription);
            DescriptionIcon = itemView.FindViewById<TextView>(Resource.Id.tv_cec_clubEventDescriptionIcon);
            StartTime = itemView.FindViewById<TextView>(Resource.Id.tv_cec_clubEventStartTime);
            StartTimeIcon = itemView.FindViewById<TextView>(Resource.Id.tv_cec_clubEventStartTimeIcon);
            ViewLocation = itemView.FindViewById<Button>(Resource.Id.btn_cec_viewOnMap);
            ClubEventActions = itemView.FindViewById<LinearLayout>(Resource.Id.ll_cec_clubEventActions);
            EditClubEvent = itemView.FindViewById<Button>(Resource.Id.btn_cec_editClubEvent);
            RemoveClubEvent = itemView.FindViewById<Button>(Resource.Id.btn_cec_removeClubEvent);
        }
    }
}