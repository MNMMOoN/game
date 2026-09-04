# Android build readiness

## Unity version

`2022.3.50f1`

## Unity project root

Repository root (`/`).

## Git remote

`game` — `https://github.com/MNMMOoN/game.git`

## Branch

`main`

## Scenes

`Assets/WaddleRush/Scenes/Boot.unity` is enabled in
`ProjectSettings/EditorBuildSettings.asset`. Runtime bootstrap presents menu and
match content from this entry scene.

## Android configuration

`WaddleRush.Editor.WaddleRushBuild.BuildAndroidDebugAPK` builds an ARM64,
portrait, development APK called `WaddleRush-debug.apk`, with package identifier
`com.example.waddlerush`; no custom keystore is required.

## Offline configuration

No Photon, Firebase, PlayFab, Mirror, Netcode, WebSocket, HTTP, matchmaking,
authentication, or remote-config references are in project source or packages.

## Static checks performed

Project layout and JSON manifest validated; every tracked Unity asset has a
`.meta`; build scene is committed; local-path and conflict-marker scans passed;
git diff whitespace check passed.

## Known potential cloud-build blockers

Unity compilation cannot be run locally because Unity is unavailable. The first
cloud build must use the custom method below, then its real log is the authority
for any compiler/package issue.

## Exact Unity Build Automation settings

Android; branch `main`; Project Subfolder Path empty; auto-detect Unity `2022.3.50f1`; custom method `WaddleRush.Editor.WaddleRushBuild.BuildAndroidDebugAPK`; Build app bundles OFF; ARM64; development/debug; Unity-provided signing; highest Unity-compatible SDK.
