---
title: Mobile Application with OPAL2.0
author: Tom
description: A short tutorial that builds and runs a basic mobile-app in OPAL2.0.
uid: mobile-app
---

## Well come to Learner Mobile Application

## Policy

**Where is place we will working on?**

We will working on opal20-platform repository [opal20-platform](https://dev.azure.com/orientsoftwaredevelopment/OPAL20/_git/opal20-platform). Because this is the mono repository so we need some convention when we working on it:

Branch Name: mobiles/xxx_features (eg: mobile-app/OP-2649 Report Vulnerability)

Pull Request Name: [OP-2649] Add report vulnerability for mobile app (OP-2649 the jira ticket)

## Mobile technical requirement

| **Concern**                         | **Analysis Report**                |
|-------------------------------------|------------------------------------|
| Screen size and resolution          | I think it's full support because I don't see a specific number of the screen size in the tender requirement.
| Memory and storage space of the app | Low memory use, because the current app doesn't have the feature to download courses for offline viewing, but we could use some 50-200 MB cache storage for cache lesson content at a time.
| CPU performance                     | It has low CPU need. Exception maybe when playing video lesson.
| Specific hard ware                  | In later phase, we may however optionally use at least camera. but not for MVP.
| OS version should support           | In the tender requirement, it says "supports at least the latest and last two versions of Apple iOS and Google Android – this would be confirmed at the Requirements Gathering phase", but  Vile think "iOS that's version 12 and above and Android 7 above".

## First Build and Run
TODO
