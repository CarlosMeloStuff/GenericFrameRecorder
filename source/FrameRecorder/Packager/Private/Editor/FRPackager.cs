﻿using System;
using System.IO;
using UnityEngine;

namespace UnityEditor.Recorder
{
    static class FRPackager
    {
        const string k_PackageName = "Recorder";

        public static string GetFrameRecorderRootPath()
        {
            return Application.dataPath + "/Recorder/";
        }

        [MenuItem("Tools/Recorder/Asset Store/Increment Version", false, 100)]
        static void IncrementVersion()
        {
            UpdateVersion();
        }

        [MenuItem("Tools/Recorder/Asset Store/Generate Assetstore package", false, 100)]
        static void GenerateAssetStorePackage()
        {
            var rootPath = FRPackagerPaths.GetRecorderRootPath();
            var type = Type.GetType("UnityEditor.Recorder.MovieRecorderPackager");
            if (type != null)
            {
                var method = type.GetMethod("GeneratePackage");
                method.Invoke(null, null);
                AssetDatabase.Refresh();
            }

            UpdateVersion();

            var files = new []
            {
                Path.Combine(rootPath, "Recorder_install.pdf" ),
                Path.Combine(rootPath, "Framework.meta" ),
                Path.Combine(rootPath, "Framework/Core" ),
                Path.Combine(rootPath, "Framework/Inputs" ),
                Path.Combine(rootPath, "Framework/Recorders" ),
                Path.Combine(rootPath, "Framework/Packager/Editor" ),
                Path.Combine(rootPath, "Extensions/UTJ" ),
                Path.Combine(rootPath, "Extensions/FCIntegration" ),
                Path.Combine(rootPath, "Extensions/MovieRecorder/Packaging" ),
            };
            var destFile = k_PackageName + " " + RecorderVersion.Stage + " v"+ RecorderVersion.Tag +  ".unitypackage";
            AssetDatabase.ExportPackage(files, destFile, ExportPackageOptions.Recurse);
            Debug.Log("Generated package: " + destFile);
        }

        static void UpdateVersion()
        {
            var path = FRPackagerPaths.GetRecorderVersionFilePath();
            var script = File.ReadAllText(path);

            var tag = "public static int BuildNumber";
            var startOffset = script.IndexOf(tag);

            var endOffset = script.IndexOf(";", startOffset);
            var pattern = script.Substring(startOffset, endOffset - startOffset);

            RecorderVersion.BuildNumber++;
            script = script.Replace(pattern, string.Format("public static int BuildNumber = {0}", RecorderVersion.BuildNumber));
            File.WriteAllText(path, script);
            AssetDatabase.Refresh();

            Debug.Log( "Version Tag set to: "+RecorderVersion.Tag);
        }

    }
}
