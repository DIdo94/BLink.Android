using Android.App;
using Android.Views;
using Android.Support.V7.Widget;
using BLink.Business.Models;
using BLink.Business.Managers;
using System.Net.Http;
using Android.Widget;
using BLink.Business.Common;
using Android.Graphics;
using Xamarin.Auth;
using System.Collections.Generic;
using BLink.Business.Enums;
using Android.Util;

namespace BLink.Droid
{
    public class PlayerAdapter : RecyclerView.Adapter
    {
        private MemberDetails[] _players;
        private Activity _activity;
        private ClubDetails _clubDetails;
        private Account _account;

        public PlayerAdapter(Activity activity, MemberDetails[] players, ClubDetails clubDetails, Account account)
        {
            _players = players;
            _activity = activity;
            _clubDetails = clubDetails;
            _account = account;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup and inflate your layout here
            var id = Resource.Layout.PlayerCard;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            return new PlayerAdapterViewHolder(itemView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var player = _players[position];
            Typeface tf = Typeface.CreateFromAsset(_activity.Assets, "fonts/Font Awesome 5 Free-Solid-900.otf");
            // Replace the contents of the view with that element
            var holder = viewHolder as PlayerAdapterViewHolder;
            holder.Caption.Text = $"{player.FirstName} {player.LastName}";

            byte[] imageAsBytes = Base64.Decode(player.Thumbnail, Base64Flags.Default);
            var img = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
            holder.Image.SetImageBitmap(img);

            holder.Height.Text = player.Height.HasValue ?
                string.Format(Literals.HeightCmFormat, _activity.GetString(Resource.String.iconRuler), player.Height.Value.ToString()) :
                "0";
            holder.Height.Typeface = tf;

            holder.Weight.Text = player.Height.HasValue ?
                string.Format(Literals.WeightKgFormat, _activity.GetString(Resource.String.iconWeight), player.Weight.Value.ToString()) :
                "0";
            holder.Weight.Typeface = tf;

            holder.PlayerPosition.Text = player.PreferedPosition.HasValue ?
                $"{_activity.GetString(Resource.String.iconMale)} {Literals.ResourceManager.GetString(player.PreferedPosition.Value.ToString())}" :
                string.Empty;
            holder.PlayerPosition.Typeface = tf;

            if (!_account.Properties["roles"].Contains(Role.Coach.ToString()))
            {
                holder.KickPlayer.Visibility = ViewStates.Gone;
                holder.InvitePlayer.Visibility = ViewStates.Gone;
            }
            else
            {
                if (!player.ClubId.HasValue)
                {
                    holder.InvitePlayer.Click += (sender, eventArgs) =>
                        InvitePlayer_Click(sender, eventArgs, player.Id, _clubDetails.Id);
                    holder.KickPlayer.Visibility = ViewStates.Gone;
                }
                else
                {
                    holder.KickPlayer.Click += (sender, eventArgs) =>
                        KickPlayer_Click(sender, eventArgs, player, position);
                    holder.InvitePlayer.Visibility = ViewStates.Gone;
                }
            }
        }

        private void KickPlayer_Click(object sender, System.EventArgs e, MemberDetails member, int position)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(_activity);
            alert.SetTitle("Премахване на играч");
            alert.SetMessage("Сигурни ли сте, че искате да премахнете играча от отбора?");
            alert.SetPositiveButton("Да", async (senderAlert, args) =>
            {
                var response = await RestManager.KickPlayer(new KickPlayerRequest
                {
                    ClubId = member.ClubId.Value,
                    PlayerId = member.Id
                });
                if (response.IsSuccessStatusCode)
                {
                    _players = RemoveItem(position);
                    NotifyItemRemoved(position);
                    if (ItemCount > 0)
                    {
                        NotifyItemRangeChanged(position, ItemCount);
                    }

                    Toast.MakeText(_activity, "Успешно премахнат играч", ToastLength.Long).Show();
                }
            });

            alert.SetNegativeButton("Не", (senderAlert, args) =>
            {
                return;
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void InvitePlayer_Click(object sender, System.EventArgs e, int playerId, int clubId)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(_activity);
            EditText description = new EditText(_activity);
            LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, 
                LinearLayout.LayoutParams.WrapContent);

            lp.LeftMargin = 60;
            lp.RightMargin = 60;
            description.LayoutParameters = lp;
            alert.SetView(description);
            alert.SetTitle("Покана на играч");
            alert.SetMessage("Добавете описание към поканата");
            alert.SetPositiveButton("Покани", async (senderAlert, args) =>
            {
                InvitePlayerRequest invitePlayer = new InvitePlayerRequest
                {
                    PlayerId = playerId,
                    ClubId = clubId,
                    Description = description.Text
                };

                HttpResponseMessage invitePlayerHttpResponse = await RestManager.InvitePlayer(invitePlayer);

                if (invitePlayerHttpResponse.IsSuccessStatusCode)
                {
                    Toast.MakeText(_activity, "Играчът е поканен!", ToastLength.Short).Show();
                }
            });

            alert.SetNegativeButton("Отказ", (senderAlert, args) =>
            {
                return;
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        public override int ItemCount => _players.Length;

        private MemberDetails[] RemoveItem(int itemPosition)
        {
            var list = new List<MemberDetails>(_players);
            list.RemoveAt(itemPosition);
            return list.ToArray();
        }
    }
}