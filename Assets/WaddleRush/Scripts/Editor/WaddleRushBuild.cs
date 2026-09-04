#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using WaddleRush.Core;

namespace WaddleRush.Editor
{
    public static class WaddleRushBuild
    {
        const string AndroidDirectory="Builds/Android"; const string IOSDirectory="Builds/iOS/WaddleRushXcode";
        [MenuItem("Tools/WaddleRush/Generate Prototype Content")]
        public static void GeneratePrototypeContent() { Directory.CreateDirectory("Assets/Generated"); AssetDatabase.Refresh(); Debug.Log("Prototype content is generated at runtime from primitives; Generated folder prepared."); }
        [MenuItem("Tools/WaddleRush/Build Android Debug APK")]
        public static void BuildAndroidDebugAPK() => Android("WaddleRush-debug.apk",false,false);
        [MenuItem("Tools/WaddleRush/Build Android Release APK")]
        public static void BuildAndroidReleaseAPK() => Android("WaddleRush-release.apk",false,true);
        [MenuItem("Tools/WaddleRush/Build Android AAB")]
        public static void BuildAndroidAAB() => Android("WaddleRush.aab",true,true);
        static void Android(string file,bool bundle,bool release)
        {
            Configure(); Directory.CreateDirectory(AndroidDirectory); EditorUserBuildSettings.buildAppBundle=bundle;
            var options=new BuildPlayerOptions{scenes=new[]{EnsureBootScene()},locationPathName=Path.Combine(AndroidDirectory,file),target=BuildTarget.Android,options=release?BuildOptions.None:BuildOptions.Development}; Verify(BuildPipeline.BuildPlayer(options),Path.Combine(AndroidDirectory,file));
        }
        [MenuItem("Tools/WaddleRush/Build iOS Xcode Project")]
        public static void BuildIOS()
        { Configure(); Directory.CreateDirectory(IOSDirectory); var o=new BuildPlayerOptions{scenes=new[]{EnsureBootScene()},locationPathName=IOSDirectory,target=BuildTarget.iOS,options=BuildOptions.None}; Verify(BuildPipeline.BuildPlayer(o),IOSDirectory); }
        static string EnsureBootScene()
        { const string path="Assets/WaddleRush/Scenes/Boot.unity"; if(!File.Exists(path)) throw new BuildFailedException("Required entry scene is missing: "+path); return path; }
        static void Configure()
        { PlayerSettings.productName=GameConstants.GameName; PlayerSettings.bundleVersion=GameConstants.Version; PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,GameConstants.PackageId); PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64; PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait; }
        static void Verify(BuildReport report,string path)
        { if(report.summary.result!=BuildResult.Succeeded || !File.Exists(path) && !Directory.Exists(path)) throw new BuildFailedException("Build artifact was not created: "+path); Debug.Log("Verified build artifact: "+Path.GetFullPath(path)); }
    }
}
#endif
