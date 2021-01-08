using Foundation;
using Tracker;

namespace Binding
{
	// @interface ReachabilityBridge : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC7Tracker18ReachabilityBridge")]
	interface ReachabilityBridge
	{
		// +(NSString * _Nonnull)connectionType __attribute__((warn_unused_result("")));
		[Static]
		[Export ("connectionType")]
		[Verify (MethodToProperty)]
		string ConnectionType { get; }

		// +(BOOL)isOnline __attribute__((warn_unused_result("")));
		[Static]
		[Export ("isOnline")]
		[Verify (MethodToProperty)]
		bool IsOnline { get; }
	}

	// @interface Tracker : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC7Tracker7Tracker")]
	[DisableDefaultCtor]
	interface Tracker
	{
		// +(void)start;
		[Static]
		[Export ("start")]
		void Start ();

		// +(void)startWith:(enum Environment)environment;
		[Static]
		[Export ("startWith:")]
		void StartWith (Environment environment);

		// +(void)setGeoLocationEnabledWithEnabled:(BOOL)enabled;
		[Static]
		[Export ("setGeoLocationEnabledWithEnabled:")]
		void SetGeoLocationEnabledWithEnabled (bool enabled);

		// +(void)setAutotrackScreenViewsWithEnabled:(BOOL)enabled;
		[Static]
		[Export ("setAutotrackScreenViewsWithEnabled:")]
		void SetAutotrackScreenViewsWithEnabled (bool enabled);
	}

	// @interface TrackerConfig : NSObject
	[BaseType (typeof(NSObject), Name = "_TtC7Tracker13TrackerConfig")]
	interface TrackerConfig
	{
	}
}
