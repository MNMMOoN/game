# Unity Build Automation — Android APK

## Repository

- Remote: `game` — `https://github.com/MNMMOoN/game.git`
- Branch: `main`

## Unity Project Path

`/` (repository root; leave Project Subfolder Path empty)

## Unity Version

`2022.3.50f1`

## Target

- Platform: Android
- Desired output: APK
- Build app bundles (`.aab`) instead of APK: **OFF**
- Architecture: ARM64
- Build type: Development/debug
- Signing: Unity/cloud debug signing; no custom release keystore
- Android SDK: choose the highest SDK Unity Build Automation offers for Unity
  `2022.3.50f1`.

## Custom build method

Enable **Run custom build script** and use:

`WaddleRush.Editor.WaddleRushBuild.BuildAndroidDebugAPK`

The method writes `Builds/Android/WaddleRush-debug.apk`, checks that it exists,
and fails the build if Unity did not produce it. Build Automation exposes the
APK as the build artifact; its downloadable location is controlled by Unity.
