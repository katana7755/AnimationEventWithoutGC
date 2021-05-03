using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AnimationEvent.asset", menuName = "Animation Event Without GC/Animation Event")]
public class ModifiedAnimationEvent : ScriptableObject
{
    public float floatParameter;
    public int intParameter;
    public string stringParameter;
    public Object objectReferenceParameter;

#if UNITY_EDITOR
    [UnityEditor.MenuItem("UTK Customized/Convert All Animation Events To Modified Version")]
    public static void ConvertAllAnimationEventToModifiedVersion()
    {
        if (UnityEditor.EditorApplication.isPlaying)
        {
            Debug.LogError("[ModifiedAnimationEvent] please execute the menu when editor is not playing.");

            return;
        }

        var assetPaths = System.IO.Directory.EnumerateDirectories(Application.dataPath);

        foreach (var assetPath in assetPaths)
        {
            var strGUIDs = UnityEditor.AssetDatabase.FindAssets("t:script", new string[] { assetPath.Substring(assetPath.IndexOf("Assets")) });

            foreach (var strGUID in strGUIDs)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(strGUID);
                var code = System.IO.File.ReadAllText(path);

                if (path.Contains("ModifiedAnimationEvent.cs"))
                {
                    continue;
                }

                if (!code.Contains("(AnimationEvent "))
                {
                    continue;
                }

                code = code.Replace("(AnimationEvent ", "(ModifiedAnimationEvent ");
                System.IO.File.WriteAllText(path, code);
                Debug.LogWarning($"[ModifiedAnimationEvent] {path} has been changed!!!");
            }
        }

        var animataionClips = Resources.FindObjectsOfTypeAll<AnimationClip>();
        var animationEventToMoveList = new List<AnimationEventToMove>();
        var animationEventToCreateList = new List<AnimationEventToCreate>();
        var animationEventToRemoveList = new List<string>();

        foreach (var aniClip in animataionClips)
        {
            animationEventToMoveList.Clear();
            animationEventToCreateList.Clear();
            animationEventToRemoveList.Clear();

            var path = UnityEditor.AssetDatabase.GetAssetPath(aniClip);

            if (string.IsNullOrEmpty(path))
            {
                continue;
            }

            var dirName = $"{System.IO.Path.GetDirectoryName(path)}/{System.IO.Path.GetFileNameWithoutExtension(path)}_Modified";
            var aniEvts = UnityEditor.AnimationUtility.GetAnimationEvents(aniClip);

            if (aniEvts == null || aniEvts.Length <= 0)
            {
                System.IO.Directory.Delete(dirName, true);

                continue;
            }

            if (!System.IO.Directory.Exists(dirName))
            {
                System.IO.Directory.CreateDirectory(dirName);
            }

            animationEventToRemoveList.AddRange(System.IO.Directory.GetFiles(dirName));

            for (int i = 0; i < aniEvts.Length; ++i)
            {
                var aniEvt = aniEvts[i];

                if (aniEvt.objectReferenceParameter is ModifiedAnimationEvent)
                {
                    var animationEventToMove = new AnimationEventToMove();
                    animationEventToMove._PathBefore = UnityEditor.AssetDatabase.GetAssetPath(aniEvt.objectReferenceParameter).Replace("\\", "/");
                    animationEventToMove._PathAfter = $"{dirName}/{aniClip.name}_{i}.asset".Replace("\\", "/");
                    animationEventToMoveList.Add(animationEventToMove);
                }
                else
                {
                    var modifiedEvent = ScriptableObject.CreateInstance<ModifiedAnimationEvent>();
                    modifiedEvent.intParameter = aniEvt.intParameter;
                    modifiedEvent.floatParameter = aniEvt.floatParameter;
                    modifiedEvent.stringParameter = aniEvt.stringParameter;
                    modifiedEvent.objectReferenceParameter = aniEvt.objectReferenceParameter;
                    aniEvt.objectReferenceParameter = modifiedEvent;

                    var animationEventToCreate = new AnimationEventToCreate();
                    animationEventToCreate._AniEvt = modifiedEvent;
                    animationEventToCreate._Path = $"{dirName}/{aniClip.name}_{i}.asset".Replace("\\", "/");
                    animationEventToCreateList.Add(animationEventToCreate);
                }
            }

            UnityEditor.AnimationUtility.SetAnimationEvents(aniClip, aniEvts);

            Debug.LogWarning($"[ModifiedAnimationEvent] all animation events in {aniClip.name} has been converted!!!");

            for (int i = 0; i < animationEventToRemoveList.Count; ++i)
            {
                animationEventToRemoveList[i] = animationEventToRemoveList[i].Replace("\\", "/");
            }

            foreach (var animationEventToMove in animationEventToMoveList)
            {
                animationEventToRemoveList.RemoveAll(item => item.Contains(animationEventToMove._PathBefore));
                animationEventToRemoveList.RemoveAll(item => item.Contains(animationEventToMove._PathAfter));

                if (animationEventToMove._PathBefore != animationEventToMove._PathAfter)
                {
                    if (System.IO.File.Exists(animationEventToMove._PathAfter))
                    {
                        UnityEditor.AssetDatabase.DeleteAsset(animationEventToMove._PathAfter);
                    }

                    UnityEditor.AssetDatabase.MoveAsset(animationEventToMove._PathBefore, animationEventToMove._PathAfter);
                }                
            }

            foreach (var animationEventToCreate in animationEventToCreateList)
            {
                if (System.IO.File.Exists(animationEventToCreate._Path))
                {
                    UnityEditor.AssetDatabase.DeleteAsset(animationEventToCreate._Path);
                }

                UnityEditor.AssetDatabase.CreateAsset(animationEventToCreate._AniEvt, animationEventToCreate._Path);
            }

            foreach (var animationEventToRemove in animationEventToRemoveList)
            {
                UnityEditor.AssetDatabase.DeleteAsset(animationEventToRemove);
            }
        }

        UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceSynchronousImport);
        UnityEditor.HandleUtility.Repaint();
        UnityEditor.EditorApplication.RepaintAnimationWindow();
        UnityEditor.EditorApplication.RepaintProjectWindow();
    }

    private class AnimationEventToMove
    {
        public string _PathBefore;
        public string _PathAfter;
    }

    private class AnimationEventToCreate
    {
        public ModifiedAnimationEvent _AniEvt;
        public string _Path;
    }
#endif
}
