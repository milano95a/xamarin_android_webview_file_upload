using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace FileUpload
{
    public class FileChooserWebChromeClient : WebChromeClient
    {
        private static int filechooser = 1;
        private IValueCallback message;
        private MainActivity activity = null;

        Action<IValueCallback, Java.Lang.String, Java.Lang.String> callback;

        public FileChooserWebChromeClient(Action<IValueCallback, Java.Lang.String, Java.Lang.String> callback, MainActivity context)
        {
            this.activity = context;
            this.callback = callback;
        }

        //For Android 4.1
        [Java.Interop.Export]
        public void openFileChooser(IValueCallback uploadMsg, Java.Lang.String acceptType, Java.Lang.String capture)
        {
            callback(uploadMsg, acceptType, capture);
        }

        public override bool OnShowFileChooser(WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
        {
            this.message = filePathCallback;
            Intent chooserIntent = fileChooserParams.CreateIntent();
            chooserIntent.AddCategory(Intent.CategoryOpenable);
            this.activity.StartActivity(Intent.CreateChooser(chooserIntent, "File Chooser"), filechooser, this.OnActivityResult);
            return true;
        }

        private void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (data != null)
            {
                if (requestCode == filechooser)
                {
                    if (null == this.message)
                    {
                        Console.WriteLine("message is null");
                        return;
                    }

                    this.message.OnReceiveValue(WebChromeClient.FileChooserParams.ParseResult((int)resultCode, data));
                    this.message = null;
                }
            }
        }

    }
}