using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using NotificationsExtensions.Toasts; // NotificationsExtensions.Win10
using Microsoft.QueryStringDotNET; // QueryString.NET
using Newtonsoft.Json;
using NotificationsExtensions.Tiles;
using System.Diagnostics;

namespace Libraries
{
    public class Toast
    { 

        public static bool Tile(string line1, string line2, string line3)
        {
            try
            {
                TileContent content = GetTileContent(line1, line2, line3);


                // Create the tile notification
                var notification = new TileNotification(content.GetXml());

                notification.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10);

                // And send the notification
                TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
            
        }
        public static void TileClear()
        {

            try
            {
                TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message.ToString());
            }
        }

        private static TileContent GetTileContent(string line1, string line2, string line3)
        {
            try
            {
                // Construct the tile content
                TileContent content = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        TileMedium = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                Children =
                {
                    new TileText()
                    {
                        Text = line1
                    },

                    new TileText()
                    {
                        Text = line2,
                        Style = TileTextStyle.CaptionSubtle
                    },

                    new TileText()
                    {
                        Text = line3,
                        Style = TileTextStyle.CaptionSubtle
                    }
                }
                            }
                        },

                        TileWide = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                Children =
                {
                    new TileText()
                    {
                        Text = line1,
                        Style = TileTextStyle.Subtitle
                    },

                    new TileText()
                    {
                        Text = line2,
                        Style = TileTextStyle.CaptionSubtle
                    },

                    new TileText()
                    {
                        Text = line3,
                        Style = TileTextStyle.CaptionSubtle
                    }
                }
                            }
                        }
                    }
                };

                return content;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                return null;
                
            }
        }

        private static int conversationId = 384928;
        private static string title = "Boba-pp";
        private static string content = "";
        private static string logo = "";
        public static bool Show(string message, string ok, string nok, Type viewOk, Type viewNok, object paramView, string cb, object data)
        {
            if (ok == null) ok = "Ok";
            if (nok == null) nok = "Ignore";
            if (ok == null && nok == null)
            {
                ok = "Yes"; nok = "No";
            }
            content = message;


            ToastVisual visual = GetVisual();
            ToastActionsCustom actions = GetAction(ok,nok,viewOk,viewNok,paramView,cb, data);


            // Now we can construct the final toast content
            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                Actions = actions,

                // Arguments when the user taps body of toast
                Launch = new QueryString()
                {
                    { "action", "viewBob" },
                    { "message", message.ToString() },
                    { "ok", ok.ToString() },
                    { "nok", nok.ToString() },
                    { "viewOk", JsonConvert.SerializeObject(viewOk) },
                    { "viewNok",JsonConvert.SerializeObject(viewNok) },
                    { "paramView", JsonConvert.SerializeObject(paramView) },
                    { "cb", cb },
                    { "data", JsonConvert.SerializeObject(data) }

                }.ToString(),
               
            };

            // And create the toast notification
            var toast = new ToastNotification(toastContent.GetXml());
            toast.ExpirationTime = DateTime.Now.AddDays(2);
            toast.Tag = "dialog";
            toast.Group = "wallPosts";

            ToastNotificationManager.CreateToastNotifier().Show(toast);

            return true;

        }

        private static ToastActionsCustom GetAction(string ok, string nok, Type viewOk, Type viewNok, object paramView, string cb, object data)
        {
           
            // In a real app, these would be initialized with actual data
            ToastActionsCustom actions;
            if (nok != null)
            {
               actions = new ToastActionsCustom()
                {
                    Buttons =
                {

                    new ToastButton(ok, new QueryString()
                    {
                        { "action", "ok" },
                        { "value", ok.ToString() },
                        { "viewOk", JsonConvert.SerializeObject(viewOk) },
                        { "viewNok",JsonConvert.SerializeObject(viewNok) },
                        { "paramView", JsonConvert.SerializeObject(paramView) },
                        { "cb", cb },
                        { "data", JsonConvert.SerializeObject(data) }

                    }.ToString())
                    {
                        //ActivationType = ToastActivationType.Background
                    },

                    new ToastButton(nok, new QueryString()
                    {
                        { "action", "nok" },
                        { "value", nok.ToString() },
                        { "viewOk", JsonConvert.SerializeObject(viewOk) },
                        { "viewNok",JsonConvert.SerializeObject(viewNok) },
                        { "paramView", JsonConvert.SerializeObject(paramView) },
                        { "cb", cb },
                        { "data", JsonConvert.SerializeObject(data) }

                    }.ToString())
                    {
                        //ActivationType = ToastActivationType.Background
                    }


                }
                };
            }
            else
            {
                actions = new ToastActionsCustom()
                {
                    Buttons =
                {

                    new ToastButton(ok, new QueryString()
                    {
                        { "action", "ok" },
                        { "conversationId", conversationId.ToString() },
                        { "value", ok.ToString() },
                        { "viewOk", JsonConvert.SerializeObject(viewOk) },
                        { "viewNok",JsonConvert.SerializeObject(viewNok) },
                        { "paramView", JsonConvert.SerializeObject(paramView) },
                        { "cb", cb },
                        { "data", JsonConvert.SerializeObject(data) }

                    }.ToString())
                    {
                        //ActivationType = ToastActivationType.Background
                    }

                   

                }
                };
            }

            
            
            return actions;
        }

        private static ToastVisual GetVisual()
        {
            // In a real app, these would be initialized with actual data
           

            // Construct the visuals of the toast
            ToastVisual visual = new ToastVisual()
            {
                TitleText = new ToastText()
                {
                    Text = title
                },

                BodyTextLine1 = new ToastText()
                {
                    Text = content
                },
                

                AppLogoOverride = new ToastAppLogo()
                {
                    Source = new ToastImageSource(logo),
                    Crop = ToastImageCrop.Circle
                }
            };

            return visual;
        }



    }
}
