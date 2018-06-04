using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using BLink.Business.Common;
using BLink.Business.Enums;
using BLink.Business.Managers;
using BLink.Business.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Auth;

namespace BLink.Droid
{
    public class ClubEventAdapter : RecyclerView.Adapter
    {
        private ClubEventFilterResult[] _clubEvents;
        private Activity _activity;
        private ClubDetails _clubDetails;
        private Account _account;

        public ClubEventAdapter(
            Activity activity,
            ClubEventFilterResult[] clubEvents,
            ClubDetails clubDetails,
            Account account)
        {
            _clubEvents = clubEvents;
            _activity = activity;
            _clubDetails = clubDetails;
            _account = account;
        }

        public override int ItemCount => _clubEvents.Length;

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var clubEvent = _clubEvents[position];
            Typeface tf = Typeface.CreateFromAsset(_activity.Assets, "fonts/Font Awesome 5 Free-Solid-900.otf");

            // Replace the contents of the view with that element
            var holder = viewHolder as ClubEventAdapterViewHolder;
            var eventType = Literals.ResourceManager.GetString(clubEvent.EventType.ToString());
            holder.Heading.Text = $"{eventType}- {clubEvent.Title}";

            holder.DescriptionIcon.Text = _activity.GetString(Resource.String.iconDescription);
            holder.DescriptionIcon.Typeface = tf;

            holder.Description.Text = clubEvent.Description;

            holder.StartTimeIcon.Text = _activity.GetString(Resource.String.iconClock);
            holder.StartTimeIcon.Typeface = tf;

            holder.StartTime.Text = clubEvent.StartTime.ToString();

            holder.ViewLocation.Click += (sender, e) => ViewLocation_Click(sender, e, clubEvent);
            if (_account.Properties["roles"].Contains(Role.Coach.ToString()))
            {
                holder.ClubEventActions.Visibility = ViewStates.Visible;
                holder.EditClubEvent.Click += (sender, e) => EditClubEvent_Click(sender, e, clubEvent);

                holder.RemoveClubEvent.Click += (sender, e) => RemoveClubEvent_Click(sender, e, clubEvent.Id, position);
            }
        }

        private void RemoveClubEvent_Click(object sender, System.EventArgs e, int eventId, int position)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(_activity);
            alert.SetTitle("Премахване на събитие");
            alert.SetMessage("Сигурни ли сте, че искате да премахнете това събитие?");
            alert.SetPositiveButton("Да", async (senderAlert, args) =>
            {
                HttpResponseMessage respondInvitationHttpResponse = await RestManager.RemoveEvent(eventId);

                if (respondInvitationHttpResponse.IsSuccessStatusCode)
                {
                    _clubEvents = RemoveItem(position);
                    NotifyItemRemoved(position);
                    if (ItemCount > 0)
                    {
                        NotifyItemRangeChanged(position, ItemCount);
                    }

                    Toast.MakeText(_activity, "Успешно премахване на събитие!", ToastLength.Long).Show();
                }
            });

            alert.SetNegativeButton("Не", (senderAlert, args) =>
            {
                return;
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void EditClubEvent_Click(object sender, System.EventArgs e, ClubEventFilterResult clubEvent)
        {
            var content = JsonConvert.SerializeObject(clubEvent);
            var intent = new Intent(_activity, typeof(EditClubEventActivity));
            intent.PutExtra("clubEvent", content);
            _activity.BaseContext.StartActivity(intent);
        }

        private void ViewLocation_Click(object sender, System.EventArgs e, ClubEventFilterResult clubEvent)
        {
            var content = JsonConvert.SerializeObject(clubEvent.Coordinates);
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

        private ClubEventFilterResult[] RemoveItem(int itemPosition)
        {
            var list = new List<ClubEventFilterResult>(_clubEvents);
            list.RemoveAt(itemPosition);
            return list.ToArray();
        }
    }
}