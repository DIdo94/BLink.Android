using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace BLink.Droid
{
    public class PlayerAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public ImageView Image { get; private set; }
        public TextView Caption { get; private set; }

        public PlayerAdapterViewHolder(View itemView) : base(itemView)
        {
            //Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            Caption = itemView.FindViewById<TextView>(Resource.Id.textView);
        }
    }
}