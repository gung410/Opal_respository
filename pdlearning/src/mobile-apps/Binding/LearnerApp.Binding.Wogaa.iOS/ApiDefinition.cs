#pragma warning disable

using System;

using ObjCRuntime;
using Foundation;
using UIKit;

namespace LearnerApp.Binding.Wogaa.iOS
{
	// @interface ReachabilityBridge : NSObject
	[BaseType(typeof(NSObject), Name = "_TtC7Tracker18ReachabilityBridge")]
	[Protocol]
	interface ReachabilityBridge
	{
		// +(NSString * _Nonnull)connectionType __attribute__((warn_unused_result("")));
		[Static]
		[Export("connectionType")]
		string ConnectionType { get; }

		// +(BOOL)isOnline __attribute__((warn_unused_result("")));
		[Static]
		[Export("isOnline")]
		bool IsOnline { get; }
	}

	// @interface Tracker : NSObject
	[BaseType(typeof(NSObject), Name = "_TtC7Tracker7Tracker")]
	[Protocol]
	[DisableDefaultCtor]
	interface Tracker
	{
		// +(void)start;
		[Static]
		[Export("start")]
		void Start();

		// +(void)startWith:(enum Environment)environment;
		[Static]
		[Export("startWith:")]
		void StartWith(Environment environment);

		// +(void)setGeoLocationEnabledWithEnabled:(BOOL)enabled;
		[Static]
		[Export("setGeoLocationEnabledWithEnabled:")]
		void SetGeoLocationEnabledWithEnabled(bool enabled);

		// +(void)setAutotrackScreenViewsWithEnabled:(BOOL)enabled;
		[Static]
		[Export("setAutotrackScreenViewsWithEnabled:")]
		void SetAutotrackScreenViewsWithEnabled(bool enabled);
	}

	// @interface TrackerConfig : NSObject
	[BaseType(typeof(NSObject), Name = "_TtC7Tracker13TrackerConfig")]
	[Protocol]
	interface TrackerConfig
	{
	}
}

