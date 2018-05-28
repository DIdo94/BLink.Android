using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace BLink.Droid
{
    public class InvitationAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView Description { get; set; }

        public TextView Header { get; set; }

        public Button AcceptInvitation { get; set; }

        public Button RefuseInvitation { get; set; }

        public InvitationAdapterViewHolder(View itemView) : base(itemView)
        {
            Description = itemView.FindViewById<TextView>(Resource.Id.tv_ic_description);
            Header = itemView.FindViewById<TextView>(Resource.Id.tv_ic_header);
            AcceptInvitation = itemView.FindViewById<Button>(Resource.Id.btn_ic_acceptInvitation);
            RefuseInvitation = itemView.FindViewById<Button>(Resource.Id.btn_ic_refuseInvitation);
        }
    }
}