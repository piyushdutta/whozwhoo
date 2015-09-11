using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.IO;
using Android.Provider;
using Path = System.IO.Path;
using Uri = Android.Net.Uri;
using Environment = Android.OS.Environment;
using Android.Graphics;
using System.Linq;

namespace camera
{
	[Activity (Label = "camera", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int CAMERA_CAPTURE_IMAGE_REQUEST_CODE = 100;
		string  IMAGE_DIRECTORY_NAME = "Mypics";
		private Uri fileuri;


		private ImageView image;
		private ImageView imgPreview;
		private Button mybutton;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			mybutton = FindViewById<Button> (Resource.Id.mybutton);
			image = FindViewById<ImageView> (Resource.Id.image);
			mybutton.Click += (s, args) => {

				captureimage ();
			};

		}
			
		void captureimage()
		{
			Intent intent = new Intent (MediaStore.ActionImageCapture);
			fileuri = getOutputMediaFile (this.ApplicationContext, IMAGE_DIRECTORY_NAME, String.Empty);
			intent.PutExtra (MediaStore.ExtraOutput, fileuri);

			StartActivityForResult (intent, CAMERA_CAPTURE_IMAGE_REQUEST_CODE);
		}
		Uri getOutputMediaFile(Context context, String subdir, string name)
		{
			subdir = subdir ?? string.Empty;
			if (string.IsNullOrWhiteSpace (name)) {
				string timestamp = DateTime.Now.ToString ("yy-MMM-dd ddd");
				name = "IMG" + timestamp + ".jpg";
			}
			string mediaType = Environment.DirectoryPictures;
			using (Java.IO.File mediaStorageDir = new Java.IO.File (Environment.GetExternalStoragePublicDirectory (mediaType), subdir)) {
			
				if (!mediaStorageDir.Exists ()) 
				{
					if(!mediaStorageDir.Mkdirs()){
						throw new IOException ("No directory available. Please clear your archives for the app");
												}
				}
				return Uri.FromFile (new Java.IO.File (GetUniquePath (mediaStorageDir.Path, name)));
					
					
			}
		

	}
		string GetUniquePath(string path, string name)
		{
			string ext = Path.GetExtension (name);
			if (ext == string.Empty)
				ext = ".jpg";
			name = Path.GetFileNameWithoutExtension (name);

			String nname = name + ext;
			int i = 1;
			while (File.Exists (Path.Combine (path, nname))) 
				nname = name + "_" + (i++) + ext;

				return Path.Combine (path, nname);
			
		}

		protected override void OnActivityResult (int requestCode,Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
			if (requestCode == CAMERA_CAPTURE_IMAGE_REQUEST_CODE) {
				if (resultCode == Result.Ok) {
					previewCaptureImage ();
				} else if (resultCode == Result.Canceled) {
					Toast.MakeText (this.ApplicationContext, "The image capture canceled",ToastLength.Short).Show ();
				} else {
					Toast.MakeText (this.ApplicationContext, "the image is captured", ToastLength.Short).Show ();
				}
			}
		}
		void previewCaptureImage()
		{
			BitmapFactory.Options options = new BitmapFactory.Options ();
			options.InSampleSize = 8;
			Bitmap bitmap = BitmapFactory.DecodeFile (fileuri.Path, options);
			imgPreview.SetImageBitmap (bitmap);
		}
	}
}






