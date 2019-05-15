using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace FileUpload
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private static readonly int FILE_CHOOSER_RESULT_CODE = 1;
        private static readonly int REQUEST_WRITE_EXTERNAL_STORAGE = 2;

        private IValueCallback _mUploadMessage;
        private Action<int, Result, Intent> _resultCallbackValue;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            ActivityCompat.RequestPermissions(this,
                new String[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage },
                REQUEST_WRITE_EXTERNAL_STORAGE);

            var chrome = new FileChooserWebChromeClient((uploadMsg, acceptType, capture) =>
            {
                _mUploadMessage = uploadMsg;
                var i = new Intent(Intent.ActionGetContent);
                i.AddCategory(Intent.CategoryOpenable);
                i.SetType("image/*");
                StartActivityForResult(Intent.CreateChooser(i, "File Chooser"), FILE_CHOOSER_RESULT_CODE);
            }, this);

            var wv = this.FindViewById<WebView>(Resource.Id.wv);
            wv.SetWebViewClient(new WebViewClient());
            wv.SetWebChromeClient(chrome);
            wv.Settings.JavaScriptEnabled = true;
            wv.LoadUrl("http://963c5cbc.ngrok.io/upload.html");
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            if (this._resultCallbackValue != null)
            {
                this._resultCallbackValue(requestCode, resultCode, intent);
                this._resultCallbackValue = null;
            }

            if (requestCode == FILE_CHOOSER_RESULT_CODE)
            {
                if (null == _mUploadMessage)
                    return;
                Java.Lang.Object result = intent == null || resultCode != Result.Ok
                    ? null
                    : intent.Data;
                _mUploadMessage.OnReceiveValue(result);
                _mUploadMessage = null;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            if (requestCode == REQUEST_WRITE_EXTERNAL_STORAGE)
            {
                if ((grantResults.Length > 0) && (grantResults[0] == Permission.Granted))
                {
                    Console.WriteLine("permission granted");
                }
                else
                {
                    Console.WriteLine("permission denied");
                }
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void StartActivity(Intent intent, int requestCode, Action<int, Result, Intent> resultCallback)
        {
            this._resultCallbackValue = resultCallback;
            StartActivityForResult(intent, requestCode);
        }

    }
}