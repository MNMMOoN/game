# WaddleRush repository guide

Portrait-first, offline Unity/URP mobile arena game. Runtime code is in
`Assets/WaddleRush/Scripts`; pure rules live in `Simulation`, Unity views in
`Presentation`, local opponents in `AI`, and JSON persistence in `Persistence`.

## Commands

- Open with Unity 2022.3 LTS or newer.
- EditMode tests: Unity `-batchmode -projectPath . -runTests -testPlatform EditMode -testResults Builds/editmode-results.xml -quit`.
- Debug APK: invoke `WaddleRush.Editor.WaddleRushBuild.BuildAndroidDebugAPK`.
- Release APK/AAB: invoke the corresponding methods in `WaddleRushBuild`.
- iOS project: invoke `WaddleRush.Editor.WaddleRushBuild.BuildIOS`.
- In the editor use `Tools > WaddleRush > Generate Prototype Content`.

## Critical invariants

1. Game must work offline.
2. No networking dependency.
3. Crop does not normally cause immediate death.
4. Fusion preserves mass.
5. Followers use shared trail reconstruction.
6. AI uses the same fundamental movement/collision rules as the player.
7. Normal gameplay uses pooling.
8. Core gameplay is testable.
9. Android/iOS remain buildable.
10. Never claim a build succeeded unless its artifact exists.

Never commit `Library`, `Temp`, build output, signing credentials, or generated
Xcode data. Keep `STATUS.md` current and below 80 lines.
