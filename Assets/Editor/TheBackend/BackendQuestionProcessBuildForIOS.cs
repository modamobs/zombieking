#if UNITY_EDITOR && UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#endif

# if UNITY_IOS
public static class BackendQuestionProcessBuildForIOS
{
    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            {
                string prjPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

                PBXProject pbxProject = new PBXProject();
                pbxProject.ReadFromFile(prjPath);

                string unityFrameworkTarget = pbxProject.GetUnityFrameworkTargetGuid();

                pbxProject.AddFrameworkToProject(unityFrameworkTarget, "WebKit.framework", false);

                pbxProject.WriteToFile(prjPath);
            }
            {
                string infoPlistPath = path + "/Info.plist";
                PlistDocument plistDocument = new PlistDocument();
                plistDocument.ReadFromFile(infoPlistPath);
                if (plistDocument.root != null)
                {
                    string cameraMessage = "첨부파일을 추가하시려면 카메라 접근 권한이 필요합니다"; // <-- 카메라 접근 시 문구
                    string videoMessage = "첨부파일을 추가하시려면 비디오 접근 권한이 필요합니다"; // <-- 카메라(동영상) 접근 시 문구

                    plistDocument.root.SetString("NSCameraUsageDescription", cameraMessage);
                    plistDocument.root.SetString("NSMicrophoneUsageDescription", videoMessage);
                    plistDocument.WriteToFile(infoPlistPath);

                }
                else
                {
                    Debug.LogError("ERROR: Can't open " + infoPlistPath);
                }
            }

        }
    }

}
#endif