
using System;
using CoreLocation;
using Foundation;
using UIKit;

namespace TestFence
{
	public partial class MainViewController : UIViewController
	{
		UILabel locLabel;
		UILabel locLabel1;
		UILabel statusLabel;
		public CLLocationManager locManager;

		public MainViewController () : base ("MainViewController", null)
		{

		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			CLCircularRegion region = new CLCircularRegion(new CLLocationCoordinate2D(10.009176843388, 76.3615990792536), 20.0, "Jubin's Desk");

			locLabel = (UILabel)this.View.ViewWithTag(2004);
			locLabel1 = (UILabel)this.View.ViewWithTag(2005);
			statusLabel = (UILabel)this.View.ViewWithTag(2006);
			locLabel.Text = "Location...";
			locManager = new CLLocationManager();
			locManager.RequestAlwaysAuthorization ();
			//locManager.RequestWhenInUseAuthorization ();
			locManager.DesiredAccuracy = CLLocation.AccuracyBest;
			//locManager.DistanceFilter = 5.0;
			locManager.UpdatedLocation+=(object sender, CLLocationUpdatedEventArgs e) => {
				locLabel.Text = "Longitude: "+e.NewLocation.Coordinate.Longitude+" Lattitude: "+e.NewLocation.Coordinate.Latitude;
				System.Diagnostics.Debug.WriteLine("Longitude: "+e.NewLocation.Coordinate.Longitude+" Lattitude: "+e.NewLocation.Coordinate.Latitude);
			};
			/* this gets fired and works fine */
			locManager.LocationsUpdated+=(object sender, CLLocationsUpdatedEventArgs e) => {
				foreach (CLLocation aLocation in e.Locations)
				{
					locLabel1.Text = "Longitude: "+aLocation.Coordinate.Longitude.ToString()+" Lattitude: "+aLocation.Coordinate.Latitude.ToString();
					if (region.ContainsCoordinate(new CLLocationCoordinate2D(aLocation.Coordinate.Latitude, aLocation.Coordinate.Longitude)))
					{
						statusLabel.Text = "Normal location update: cordinates inside the region";
					}
					else
					{
						statusLabel.Text = "Normal location update: cordinates outside the region";
					}
				}
			};
			locManager.StartUpdatingLocation();

			if (CLLocationManager.IsMonitoringAvailable (typeof(CLCircularRegion))) 
			{
				/* This doesn't get fired */
				locManager.DidDetermineState += (object sender, CLRegionStateDeterminedEventArgs e) => {
					switch(e.State)
					{
						case CLRegionState.Inside:
						InvokeOnMainThread(()=>{
							locLabel.Text = "Iniside...";
							UIAlertView testAlert = new UIAlertView ();
							testAlert.AddButton ("OK");
							testAlert.Title = "Entered";
							testAlert.Message = "Region "+e.Region.ToString();
							testAlert.Show();
						});
						break;

						case CLRegionState.Outside:
						InvokeOnMainThread(()=>{
							locLabel.Text = "Outside...";
							UIAlertView testAlert1 = new UIAlertView ();
							testAlert1.AddButton ("OK");
							testAlert1.Title = "Exit";
							testAlert1.Message = "Region "+e.Region.ToString();
							testAlert1.Show();
						});
						break;

						case CLRegionState.Unknown:
						InvokeOnMainThread(()=>{
							locLabel.Text = "Unknown state for region...";
						});
						break;
						default:
						break;
					}
				};
				/* This works and gets fired */
				locManager.DidStartMonitoringForRegion += (o, e) => {
					InvokeOnMainThread(()=>{
						locManager.RequestState(e.Region);
						locLabel.Text = "Now monitoring region : "+ e.Region.ToString ();
					});
				};
				/* This doesn't get fired */
				locManager.DidVisit+=(object sender, CLVisitedEventArgs e) => {
					InvokeOnMainThread(()=>{
						locLabel.Text = "Did visit region...";
						UIAlertView testAlert = new UIAlertView ();
						testAlert.AddButton ("OK");
						testAlert.Title = "Visited";
						testAlert.Message = "Region "+e.Visit.Coordinate.Latitude.ToString()+" | "+e.Visit.Coordinate.Longitude.ToString();
						testAlert.Show();						
					});
				};
				/* This doesn't get fired */
				locManager.RegionEntered += (o, e) => {
					InvokeOnMainThread(()=>{
						UIAlertView testAlert = new UIAlertView ();
						testAlert.AddButton ("OK");
						testAlert.Title = "Entered";
						testAlert.Message = "Region "+e.Region.ToString();
						testAlert.Show();
						locLabel.Text = "Entered: "+e.Region.ToString();
					});
				};
				/* This doesn't get fired */
				locManager.RegionLeft += (o, e) => {
					InvokeOnMainThread(()=>{
						UIAlertView testAlert = new UIAlertView ();
						testAlert.AddButton ("OK");
						testAlert.Title = "Exit";
						testAlert.Message = "Region "+e.Region.ToString();
						testAlert.Show();
						locLabel.Text = "Left: "+e.Region.ToString();
					});
				};
				locManager.StartMonitoring (region);
			} 
			else 
			{
			}
			// Perform any additional setup after loading the view, typically from a nib.
		}
	}
}

