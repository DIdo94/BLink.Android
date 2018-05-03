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

namespace BLink.Droid
{
    public class InvitationAdapter : RecyclerView.Adapter
    {
        private InvitationResponse[] _invitationResponses;
        private Account _account;
        private Activity _activity;
        public InvitationAdapter(Activity activity, InvitationResponse[] invitationResponses, Account account)
        {
            _invitationResponses = invitationResponses;
            _activity = activity;
            _account = account;
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
            holder.ClubName.Text = invitation.ClubName;
            holder.Description.Text = invitation.Description;
            holder.AcceptInvitation.Click += (sender, eventArgs) =>
                RespondInvitation_Click(sender, eventArgs, invitation.Id, _account.Username, InvitationStatus.Accepted);
            holder.RefuseInvitation.Click += (sender, eventArgs) =>
                RespondInvitation_Click(sender, eventArgs, invitation.Id, _account.Username, InvitationStatus.Refused);
        }

        private async void RespondInvitation_Click(object sender, EventArgs eventArgs, int invitationId, string email, InvitationStatus invitationStatus)
        {
            HttpResponseMessage respondInvitationHttpResponse = await RestManager.RespondInvitation(email, invitationId, invitationStatus);

            if (respondInvitationHttpResponse.IsSuccessStatusCode)
            {
                Toast.MakeText(_activity, "Успешен отговор на поканата!", ToastLength.Short).Show();
            }
        }

        public override int ItemCount => _invitationResponses.Length;
    }
}