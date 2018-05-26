using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace BLink.Droid
{
    public class PlayerAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public ImageView Image { get; private set; }
        public TextView Caption { get; private set; }

        public TextView Weight { get; private set; }

        public TextView Height { get; private set; }

        public TextView PlayerPosition { get; set; }

        public Button InvitePlayer { get; set; }

        public Button KickPlayer { get; set; }

        public PlayerAdapterViewHolder(View itemView) : base(itemView)
        {
            //Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            Caption = itemView.FindViewById<TextView>(Resource.Id.tv_pc_name);
            Weight = itemView.FindViewById<TextView>(Resource.Id.tv_pc_weight);
            Height = itemView.FindViewById<TextView>(Resource.Id.tv_pc_height);
            PlayerPosition = itemView.FindViewById<TextView>(Resource.Id.tv_pc_position);
            InvitePlayer = itemView.FindViewById<Button>(Resource.Id.btn_pc_invitePlayer);
            KickPlayer = itemView.FindViewById<Button>(Resource.Id.btn_pc_kickPlayer);
        }
    }
}