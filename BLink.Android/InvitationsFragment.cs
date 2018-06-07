﻿using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Views;
using Xamarin.Auth;
using Android.Support.V7.Widget;
using BLink.Business.Models;
using BLink.Business.Managers;
using System.Net.Http;
using Newtonsoft.Json;
using Android.Widget;
using BLink.Business.Enums;
using BLink.Business.Common;

namespace BLink.Droid
{
    public class InvitationsFragment : Android.Support.V4.App.Fragment
    {
        private RecyclerView _recyclerView;
        private RecyclerView.LayoutManager _layoutManager;
        private InvitationAdapter _adapter;
        private IEnumerable<InvitationResponse> _invitationResponses;
        private Account _account;
        private TextView _header;
        private TextView _noInvitations;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.Invitations, null);
            return view;
        }

        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            _account = AccountStore
              .Create(Context)
              .FindAccountsForService(GetString(Resource.String.app_name))
              .FirstOrDefault();
            HttpResponseMessage httpResponse = await RestManager.GetMemberInvitations(_account.Username);
            _header = View.FindViewById<TextView>(Resource.Id.tv_invitations_header);
            _noInvitations = View.FindViewById<TextView>(Resource.Id.tv_invitations_noInvitations);
            if (_account.Properties["roles"].Contains(Role.Coach.ToString()))
            {
                _header.Text = Literals.InvitationsToPlayers;
            }
            else
            {
                _header.Text = Literals.InvitationsFromClubs;
            }

            if (httpResponse.IsSuccessStatusCode)
            {
                string response = await httpResponse.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(response) && response != "null")
                {
                    _invitationResponses = JsonConvert.DeserializeObject<IEnumerable<InvitationResponse>>(response);
                    _adapter = new InvitationAdapter(Activity, _invitationResponses.ToArray(), _account);
                    _recyclerView = View.FindViewById<RecyclerView>(Resource.Id.rv_invitations_invitations);
                    _recyclerView.SetAdapter(_adapter);
                    _layoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false);
                    _recyclerView.SetLayoutManager(_layoutManager);
                    if (!_invitationResponses.Any())
                    {
                        _noInvitations.Visibility = ViewStates.Visible;
                    }
                }
            }
        }
    }
}