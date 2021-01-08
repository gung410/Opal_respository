# Learner App

There are some technical highlights for this project:

- We have a problem with deep nested folder: https://docs.microsoft.com/en-us/appcenter/build/xamarin/android/index#32-project-and-configuration
- That's why we need to keep the project at 4 levels: "For best performance, the analysis is currently limited to four directory levels including the root of your repository.".
- UIWebView Deprecation and Xamarin.Forms:
+ Add this flag in the additional mtouch arguments field: --optimize=experimental-xforms-product-type
+ Linker Behavior set to SDK Only or All
