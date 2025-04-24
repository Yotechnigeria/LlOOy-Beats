using System;
using System.IO;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections;
using Firebase.Database;
using Email.Net;
using ZPF.Media;
using Plugin.LocalNotification;
using static BeatsAppClient.Home;
using System.Collections.ObjectModel;
using Plugin.LocalNotification.EventArgs;
using Microsoft.Maui.Devices;
using Plugin.LocalNotification.AndroidOption;
using CSharpShellCore;
using Rave;
using Rave.NET.Models.Card;
using Rave.NET.Models.Charge;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace BeatsAppClient
{
    public partial class Home : ContentPage
    {
        public class Beat
        {
            public string BeatTitle { get; set; }
            public string Art { get; set; }
            public string Audio { get; set; }
            public string Genre { get; set; }
            public int BPM { get; set; }
            public string ID { get; set; }
        }
        public class Licence
        {
            public string Type { get; set; }
            public decimal Price { get; set; }
            public int AudioStreams { get; set; }
            public int VideoStreams { get; set; }
            public int Song { get; set; }
            public int MusicVideo { get; set; }
            public bool Performance { get; set; }
            public bool RadioBroadcasting { get; set; }
            public int Royalties { get; set; }

        }
        public Home()
        {
            InitializeComponent();

            ////
            var fb_db_options = new FirebaseOptions();
            fb_db_options.SyncPeriod = TimeSpan.FromSeconds(05);
            var beat_fb_db = new FirebaseClient("", fb_db_options).Child("Beats");
            var beat = new Beat();
            var beat_list = new ObservableCollection<Beat>();
            var licence = new Licence();
            var licence_list = new ObservableCollection<Licence>();
            var mail = EmailMessage.Compose();
            mail.From("yotechnigeria@gmail.com");
            mail.ReplyTo("producerllooy@gmail.com");
            mail.WithNormalPriority();
            var payment_config = new RaveConfig("FLWPUBK-3e02152bc2527f96b74efbdde8307de6-X", "FLWSECK-9099c158186d3b6062818cd0261020e8-1965eb41b6cvt-X", false);
            var beat_player = ZeMediaPlayer.Current;
            var notification = new NotificationRequest();
            notification.Image.ResourceName = "llooybeat.png";
            notification.Silent = false;
            notification.BadgeNumber = 1;
            notification.CategoryType = NotificationCategoryType.Alarm;
            int int_ = 0;
            ////

            ////
            Application.Current.PageAppearing += (s, e) =>
            {
                beat_player.Init();
                licence_list = new ObservableCollection<Licence>();
                licence = new Licence();
                licence.Song = 1;
                licence.Type = "Basic";
                licence.Price = 5;
                licence.Royalties = 0;
                licence.MusicVideo = 0;
                licence.Performance = false;
                licence.AudioStreams = 100000;
                licence.VideoStreams = 70000;
                licence.RadioBroadcasting = true;
                licence_list.Add(licence);
                licence = new Licence();
                licence.Song = 1;
                licence.Type = "Gold";
                licence.Price = 10;
                licence.Royalties = 0;
                licence.MusicVideo = 0;
                licence.Performance = false;
                licence.AudioStreams = 300000;
                licence.VideoStreams = 100000;
                licence.RadioBroadcasting = true;
                licence_list.Add(licence);
                licence = new Licence();
                licence.Song = 1;
                licence.Type = "Premium";
                licence.Price = 300;
                licence.Royalties = 0;
                licence.MusicVideo = 0;
                licence.Performance = false;
                licence.AudioStreams = 300000;
                licence.VideoStreams = 100000;
                licence.RadioBroadcasting = true;
                licence_list.Add(licence);
                licence = new Licence();
                licence.Song = 1;
                licence.Type = "Unlimited";
                licence.Price = 500;
                licence.Royalties = 25;
                licence.MusicVideo = 1;
                licence.Performance = true;
                licence.AudioStreams = 0;
                licence.VideoStreams = 0;
                licence.RadioBroadcasting = true;
                licence_list.Add(licence);
                beat_fb_db.AsObservable<Beat>().Subscribe(data =>
                {
                    if (data.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                    {
                        if (beat_list.Contains(data.Object))
                        {
                            int_ = beat_list.IndexOf(data.Object);
                            beat_list.RemoveAt(int_);
                            beat_list.Insert(int_, data.Object);
                        }
                        else
                        {
                            beat_list.Add(data.Object);
                        }
                    }
                    if (data.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
                    {
                        int_ = beat_list.IndexOf(data.Object);
                        beat_list.RemoveAt(int_);
                    }
                    BeatsList.ItemsSource = beat_list;
                });
            };
            Application.Current.PageDisappearing += (s, e) =>
            {
                if (beat_player.State == MediaPlayerState.Playing)
                {
                    beat_player.Pause();
                    beat_player.Stop();
                }

            };
            Window.Backgrounding += (s, e) =>
            {
                if (beat_player.State == MediaPlayerState.Playing)
                {
                    notification.Title = beat.BeatTitle + " is playing";
                    notification.Description = "Genre: " + beat.Genre;
                    if (DeviceInfo.Platform == DevicePlatform.Android)
                    {
                        notification.Android.ProgressBar.IsIndeterminate = false;
                        notification.Android.ProgressBar.Max = Int32.Parse(beat_player.Position.TotalMilliseconds.ToString());
                        notification.Android.ProgressBar.Progress = beat_player.Position.Milliseconds;
                        notification.Android.VisibilityType = Plugin.LocalNotification.AndroidOption.AndroidVisibilityType.Public;
                        notification.Android.Ongoing = true;
                        notification.Android.LaunchAppWhenTapped = true;
                        notification.Android.AutoCancel = false;
                    }
                    if (DeviceInfo.Platform == DevicePlatform.iOS)
                    {
                        notification.iOS.HideForegroundAlert = false;
                        notification.iOS.ShowInNotificationCenter = true;
                        notification.iOS.SummaryArgument = "Genre of Beat Playing: " + beat.Genre;
                    }
                    notification.CategoryType = NotificationCategoryType.Progress;
                    notification.Geofence.NotifyOn = NotificationRequestGeofence.GeofenceNotifyOn.OnEntry;
                    notification.Geofence.Center.Latitude = 10;
                    notification.Geofence.Center.Longitude = 08;
                }
                beat_fb_db.AsObservable<Beat>().Subscribe(data =>
                {
                    if (data.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                    {
                        if (beat_list.Contains(data.Object))
                        {
                            int_ = beat_list.IndexOf(data.Object);
                            beat_list.RemoveAt(int_);
                            beat_list.Insert(int_, data.Object);
                            notification.Title = "Beat Update";
                            notification.Description = beat.Genre + " is updated";
                        }
                        else
                        {
                            beat_list.Add(data.Object);
                            notification.Title = "New Beat";
                            notification.Description = "A New " + beat.Genre + " Beat is available";
                        }
                    }
                    if (data.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
                    {
                        int_ = beat_list.IndexOf(data.Object);
                        beat_list.RemoveAt(int_);
                    }
                    BeatsList.ItemsSource = beat_list;
                });
            };
            BeatsList.ItemSelected += (s, e) =>
            {
                if (beat_player.State == MediaPlayerState.Playing)
                {
                    beat_player.Pause();
                    beat_player.Stop();
                }
                beat_player.Play(beat.Audio);
                Art___.Source = beat.Art;
                BeatProgressBar.Progress = beat_player.Position.Milliseconds;
                Title___.Text = beat.BeatTitle;
            };
            Purchase_Button.Clicked += (s, e) =>
            {
                LicenceList.IsVisible = true;
            };
            LicenceList.ItemTapped += (s, e) =>
            {
                DebitCardPlate.IsVisible = true;
            };
            Done_Button.Clicked += (s, e) =>
            {
                var payment_card = new Card(CardNo.Text, ExpirationMonth.Text, ExpirationYear.Text, CVV_.Text);
                var payment_payload = new CardParams("FLWPUBK-3e02152bc2527f96b74efbdde8307de6-X", "FLWSECK-9099c158186d3b6062818cd0261020e8-1965eb41b6cvt-X", Preferences.Get("name", "unknown"), "Customer", Preferences.Get("email", "Unknown"), licence.Price, "USD", payment_card);
                payment_payload.SuggestedAuth = "PIN";
                payment_payload.Pin = CardPin.Text;
                var payment_payload_charge = new ChargeCard(payment_config);
                var response = payment_payload_charge.Charge(payment_payload).GetAwaiter();
                response.OnCompleted(() =>
                {
                    if (response.GetResult().Data.SuggestedAuth == "OTP")
                    {
                        OTP_.Completed += (s, e) =>
                        {
                            payment_payload.SuggestedAuth = "OTP";
                            payment_payload.Otp = OTP_.Text;
                            payment_payload_charge.Charge(payment_payload, false);
                            var response_ = payment_payload_charge.Charge(payment_payload).GetAwaiter();
                            if (response_.GetResult().Status == "success")
                            {
                                Document.Create(container =>
                                {
                                    container.Page(page =>
                                    {
                                        page.Size(PageSizes.A4);
                                        page.Header().Text("Receipt for " + beat.BeatTitle);
                                        page.Foreground().Text("YoBeats Receipt").Bold();
                                        page.Content().Column(column =>
                                        {
                                            column.Item().Text("Your Purchase of " + beat.BeatTitle);
                                            column.Item().Text("Beat Title: " + beat.BeatTitle + ". Genre: " + beat.Genre + ". Licence Type: " + licence.Type);
                                        });
                                    });

                                }).GeneratePdf("Receipt from Yobeats.pdf");
                            }
                            else
                            {
                                Toast.MakeText("There was an error. Please try again", CSharpShellCore.Toast.ToastLength.Long).Show();
                            };
                        };
                    }
                    if (response.GetResult().Status == "success")
                    {
                        Document.Create(container =>
                        {
                            container.Page(page =>
                            {
                                page.Size(PageSizes.A4);
                                page.Header();
                                page.Foreground().Text("YoBeats Receipt").Bold();
                                page.Content().Column(column =>
                                {
                                    column.Item().Text("Your Purchase of " + beat.BeatTitle);
                                    column.Item().Text("Beat Title: " + beat.BeatTitle + ". Genre: " + beat.Genre + ". Licence Type: " + licence.Type + ". Costs is " + licence.Price.ToString());
                                });
                            });
                        }).GeneratePdf("Receipt from Yobeats.pdf");
                    }
                    else
                    {
                        Toast.MakeText("There was an error. Please try again", CSharpShellCore.Toast.ToastLength.Long).Show();
                    }
                });
            };
        }
    }
}