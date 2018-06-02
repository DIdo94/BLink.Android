using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using BLink.Business.Models;
using Xamarin.Auth;
using BLink.Business.Enums;
using System.Net.Http;
using BLink.Business.Managers;
using Android.Graphics;

namespace BLink.Droid
{
    public class InvitationAdapter : RecyclerView.Adapter
    {
        private InvitationResponse[] _invitationResponses;
        private Account _account;
        private Activity _activity;
        private bool _isCoach;

        public InvitationAdapter(Activity activity, InvitationResponse[] invitationResponses, Account account)
        {
            _invitationResponses = invitationResponses;
            _activity = activity;
            _account = account;
            _isCoach = _account.Properties["roles"].Contains(Role.Coach.ToString());
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.InvitationCard;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            return new InvitationAdapterViewHolder(itemView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var invitation = _invitationResponses[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as InvitationAdapterViewHolder;
            holder.Header.Text = _isCoach ? invitation.PlayerName : invitation.ClubName;

            var img = BitmapFactory.DecodeByteArray(invitation.Thumbnail, 0, invitation.Thumbnail.Length);
            holder.Thumbnail.SetImageBitmap(img);

            holder.Description.Text = invitation.Description;
            if (_isCoach)
            {
                holder.AcceptInvitation.Visibility = ViewStates.Gone;
            }
            else
            {
                holder.AcceptInvitation.Click += (sender, eventArgs) =>
                    RespondInvitation_Click(sender, eventArgs, invitation.Id, _account.Username, InvitationStatus.Accepted, position);
            }

            var refusedStatus = _isCoach ? InvitationStatus.RefusedFromClub : InvitationStatus.RefusedFromPlayer;
            holder.RefuseInvitation.Click += (sender, eventArgs) =>
                RespondInvitation_Click(sender, eventArgs, invitation.Id, _account.Username, refusedStatus, position);
        }

        private void RespondInvitation_Click(
            object sender,
            EventArgs eventArgs,
            int invitationId,
            string email,
            InvitationStatus invitationStatus,
            int position)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(_activity);
            alert.SetTitle("Отговор на покана");
            alert.SetMessage("Сигурни ли сте, че искате да извършите това действие?");
            alert.SetPositiveButton("Да", async (senderAlert, args) =>
            {
                HttpResponseMessage respondInvitationHttpResponse = await RestManager.RespondInvitation(email, invitationId, invitationStatus);

                if (respondInvitationHttpResponse.IsSuccessStatusCode)
                {
                    if (invitationStatus == InvitationStatus.Accepted)
                    {
                        Intent intent = new Intent(_activity, typeof(UserProfileActivity));
                        _activity.StartActivity(intent);
                    }
                    else
                    {
                        _invitationResponses = RemoveItem(position);
                        NotifyItemRemoved(position);
                        if (ItemCount > 0)
                        {
                            NotifyItemRangeChanged(position, ItemCount);
                        }

                        Toast.MakeText(_activity, "Успешен отговор на поканата!", ToastLength.Short).Show();
                    }
                }
            });

            alert.SetNegativeButton("Не", (senderAlert, args) =>
            {
                return;
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private InvitationResponse[] RemoveItem(int itemPosition)
        {
            var list = new List<InvitationResponse>(_invitationResponses);
            list.RemoveAt(itemPosition);
            return list.ToArray();
        }

        public override int ItemCount => _invitationResponses.Length;
    }
}