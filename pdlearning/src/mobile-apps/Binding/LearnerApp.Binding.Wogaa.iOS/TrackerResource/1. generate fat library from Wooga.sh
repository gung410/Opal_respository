cp -R "Tracker.xcframework/ios-armv7_arm64/" "Release fat/"
cp -R "Tracker.xcframework/ios-i386_x86_64-simulator/Tracker.framework/Modules/Tracker.swiftmodule/" "Release fat/Tracker.framework/Modules/Tracker.swiftmodule/"

lipo -create -output "Release fat/Tracker.framework/Tracker" "Tracker.xcframework/ios-armv7_arm64/Tracker.framework/Tracker" "Tracker.xcframework/ios-i386_x86_64-simulator/Tracker.framework/Tracker"

lipo -info "Release fat/Tracker.framework/Tracker"

otool -l "Release fat/Tracker.framework/Tracker" | grep __LLVM
