using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

namespace TilkiOyunu.Foundation.Editor
{
    public static class Sprint5Validation
    {
        private const string MixerPath = "Assets/_Game/Audio/Mixers/TilkiAudioMixer.mixer";

        [MenuItem("Tilki Oyunu/Sprint 5/Validate")]
        public static void ValidateFromMenu()
        {
            if (ValidateProject(out List<string> errors))
            {
                Debug.Log("Sprint 5 validation passed.");
                return;
            }

            foreach (string error in errors)
            {
                Debug.LogError(error);
            }
        }

        public static void ValidateFromCommandLine()
        {
            if (ValidateProject(out List<string> errors))
            {
                Debug.Log("Sprint 5 validation passed.");
                EditorApplication.Exit(0);
                return;
            }

            foreach (string error in errors)
            {
                Debug.LogError(error);
            }

            EditorApplication.Exit(1);
        }

        public static bool ValidateProject(out List<string> errors)
        {
            errors = new List<string>();
            ValidateImportedAssets(errors);
            ValidatePlayerPrefab(errors);
            ValidateForestScene(errors);
            ValidateDocumentation(errors);
            return errors.Count == 0;
        }

        private static void ValidateImportedAssets(List<string> errors)
        {
            RequireAsset<GameObject>("Assets/ThirdParty/Quaternius/UltimateAnimatedAnimals/Fox/Fox.fbx", "Quaternius fox FBX", errors);
            RequireAsset<RuntimeAnimatorController>("Assets/_Game/Art/Characters/FoxAnimatorController.controller", "Fox animator controller", errors);
            RequireClip(errors, "idle", "idle");
            RequireClip(errors, "walk", "walk");
            RequireClip(errors, "run/gallop", "run", "gallop");
            RequireClip(errors, "jump", "jump");

            string[] nature =
            {
                "BirchTree_1.fbx", "MapleTree_1.fbx", "NormalTree_1.fbx", "PineTree_1.fbx",
                "Bush.fbx", "Grass_Small.fbx", "Flower_1_Clump.fbx", "Rock_1.fbx"
            };
            for (int i = 0; i < nature.Length; i++)
            {
                RequireAsset<GameObject>($"Assets/ThirdParty/Quaternius/UltimateStylizedNature/{nature[i]}", nature[i], errors);
            }

            RequireAsset<AudioClip>("Assets/ThirdParty/OpenGameArt/Music/SunsetWalk.ogg", "background music", errors);
            RequireAsset<AudioClip>("Assets/ThirdParty/OpenGameArt/Ambience/Forest_Ambience.mp3", "forest ambience", errors);
            RequireAsset<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Footsteps/leaves01.ogg", "footstep SFX", errors);
            RequireAsset<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Chimes/bell_ding1.wav", "memory/light chime", errors);
            RequireAsset<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Cards/contact1.wav", "card contact SFX", errors);
            RequireAsset<AudioClip>("Assets/ThirdParty/OpenGameArt/SFX/Campfire/fire.wav", "campfire loop", errors);
            RequireAsset<AudioClip>("Assets/ThirdParty/Kenney/InterfaceSounds/click_001.ogg", "UI click", errors);
            ValidateAudioMixer(errors);
            RequireAsset<GameUITheme>("Assets/_Game/Art/UI/GameUITheme.asset", "UI theme", errors);
            RequireAsset<FinalMessageDefinition>("Assets/_Game/Data/Config/FinalMessage.asset", "final message", errors);
        }

        private static void ValidatePlayerPrefab(List<string> errors)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/Characters/PlayerFox.prefab");
            if (prefab == null)
            {
                errors.Add("PlayerFox prefab is missing.");
                return;
            }

            if (prefab.transform.Find("VisualRoot/QuaterniusFox/FoxModel") == null)
            {
                errors.Add("PlayerFox prefab must contain VisualRoot/QuaterniusFox/FoxModel.");
            }

            if (prefab.GetComponentInChildren<FoxAnimationDriver>(true) == null)
            {
                errors.Add("PlayerFox prefab is missing FoxAnimationDriver.");
            }

            Animator animator = prefab.GetComponentInChildren<Animator>(true);
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                errors.Add("PlayerFox prefab is missing a bound AnimatorController.");
            }
            else if (animator.applyRootMotion)
            {
                errors.Add("Fox Animator must have root motion disabled.");
            }

            if (prefab.GetComponent<FootstepAudio>() == null)
            {
                errors.Add("PlayerFox prefab is missing FootstepAudio.");
            }

            AudioSource source = prefab.GetComponent<AudioSource>();
            if (source == null || source.outputAudioMixerGroup == null || source.outputAudioMixerGroup.name != "SFX")
            {
                errors.Add("PlayerFox footstep AudioSource must route to the SFX mixer group.");
            }
        }

        private static void ValidateForestScene(List<string> errors)
        {
            EditorSceneManager.OpenScene(SceneIds.ForestPath, OpenSceneMode.Single);
            RequireSceneObject("Environment_Visuals", errors);
            RequireSceneObject("Sprint 5 Scene Audio", errors);
            RequireSceneObject("Final Message Panel", errors);

            if (Object.FindFirstObjectByType<SceneLoopAudio>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Forest scene is missing SceneLoopAudio.");
            }

            RequireAudioRoute("Music Loop", "Music", errors);
            RequireAudioRoute("Forest Ambience Loop", "Ambience", errors);
            RequireAudioRoute("Campfire Loop", "Ambience", errors);

            if (Object.FindFirstObjectByType<MemoryAudioFeedback>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Forest memories are missing MemoryAudioFeedback.");
            }

            if (Object.FindFirstObjectByType<LightPathAudioFeedback>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Light Path is missing LightPathAudioFeedback.");
            }

            if (Object.FindFirstObjectByType<CardMatchingAudioFeedback>(FindObjectsInactive.Include) == null)
            {
                errors.Add("Card Matching is missing CardMatchingAudioFeedback.");
            }

            FinalCampController camp = Object.FindFirstObjectByType<FinalCampController>(FindObjectsInactive.Include);
            if (camp == null)
            {
                errors.Add("Forest scene is missing FinalCampController.");
            }
            else if (camp.GetComponent<FinalSequenceController>() == null)
            {
                errors.Add("Final camp is missing FinalSequenceController.");
            }

            foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(root);
                if (missingCount > 0)
                {
                    errors.Add($"Scene root '{root.name}' has {missingCount} missing script reference(s).");
                }
            }
        }

        private static void ValidateDocumentation(List<string> errors)
        {
            if (!File.Exists("ASSET_NOTES.md"))
            {
                errors.Add("ASSET_NOTES.md is missing.");
            }
        }

        private static void ValidateAudioMixer(List<string> errors)
        {
            AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MixerPath);
            if (mixer == null)
            {
                errors.Add($"Audio mixer is missing at {MixerPath}.");
                return;
            }

            RequireMixerGroup(mixer, "Master", errors);
            RequireMixerGroup(mixer, "Music", errors);
            RequireMixerGroup(mixer, "Ambience", errors);
            RequireMixerGroup(mixer, "SFX", errors);

            string mixerText = File.ReadAllText(MixerPath);
            RequireMixerParameter(mixerText, "MasterVolume", errors);
            RequireMixerParameter(mixerText, "MusicVolume", errors);
            RequireMixerParameter(mixerText, "AmbienceVolume", errors);
            RequireMixerParameter(mixerText, "SFXVolume", errors);
        }

        private static void RequireMixerGroup(AudioMixer mixer, string groupName, List<string> errors)
        {
            if (mixer.FindMatchingGroups(groupName).Length == 0)
            {
                errors.Add($"Audio mixer is missing the {groupName} group.");
            }
        }

        private static void RequireMixerParameter(string mixerText, string parameterName, List<string> errors)
        {
            if (!mixerText.Contains($"name: {parameterName}"))
            {
                errors.Add($"Audio mixer is missing exposed parameter {parameterName}.");
            }
        }

        private static void RequireAsset<T>(string path, string label, List<string> errors) where T : Object
        {
            if (AssetDatabase.LoadAssetAtPath<T>(path) == null)
            {
                errors.Add($"{label} is missing at {path}.");
            }
        }

        private static void RequireClip(List<string> errors, string label, params string[] tokens)
        {
            Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath("Assets/ThirdParty/Quaternius/UltimateAnimatedAnimals/Fox/Fox.fbx");
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is not AnimationClip clip)
                {
                    continue;
                }

                string clipName = clip.name.ToLowerInvariant();
                for (int tokenIndex = 0; tokenIndex < tokens.Length; tokenIndex++)
                {
                    if (clipName.Contains(tokens[tokenIndex]))
                    {
                        return;
                    }
                }
            }

            errors.Add($"Fox FBX is missing an animation clip containing '{label}'.");
        }

        private static void RequireSceneObject(string name, List<string> errors)
        {
            if (GameObject.Find(name) == null)
            {
                errors.Add($"Forest scene is missing {name}.");
            }
        }

        private static void RequireAudioRoute(string objectName, string groupName, List<string> errors)
        {
            GameObject gameObject = GameObject.Find(objectName);
            AudioSource source = gameObject == null ? null : gameObject.GetComponent<AudioSource>();
            if (source == null || source.outputAudioMixerGroup == null || source.outputAudioMixerGroup.name != groupName)
            {
                errors.Add($"{objectName} must route to the {groupName} mixer group.");
            }
        }
    }
}
