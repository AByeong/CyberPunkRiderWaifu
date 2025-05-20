
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.InteropServices;







namespace AMAZ3DUnity
{
    public class Amaz3DWindow : EditorWindow
    {

        [DllImport("AMAZ3D_unity_interface_lite.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr createManager();

        [DllImport("AMAZ3D_unity_interface_lite")]
        private static extern IntPtr setup(IntPtr man, string input, string output_filepath,
        string metadata_filepath, double face_reduction,
        double normals_scaling, int minimum_face_number,
        double preserve_boundary_edges, bool preserve_hard_edges,
        bool preserve_smooth_edges, bool project_normals,
        int normals_weighting, double contrast, int retexture, bool merge_duplicated_uv,
        bool remove_isolated_vertices, bool remove_non_manifold_faces, bool remove_duplicated_faces,
        bool remove_duplicated_boundary_vertices, bool remove_degenerate_faces, int max_triangle_threshold,
        bool use_vertex_mask, int resize_images, bool joined_simplification, int remove_mesh_by_count,
        double remove_mesh_by_size, bool feature_importance, double feature_angle, double feature_area,
        bool remove_hidden_objects, bool join_fbx_meshes, bool join_gltf_meshes, int gltf_format, int target_poly);


        [DllImport("AMAZ3D_unity_interface_lite")]
        private static extern IntPtr optimization(IntPtr man, string algo);

        [DllImport("AMAZ3D_unity_interface_lite")]
        private static extern void freeManager(IntPtr man);


        private UnityEngine.Object selectedFBX;
        private string selectedAlgorithm = "simple";
        private float faceReduction = 0.5f;
        private bool featureImportance = false;
        private float featureAngle = 50f;
        private double featureAreaDouble = 0.00011777898623242495;
        private int preserveBoundaryEdges = 1;
        private bool preserveHardEdges = true;
        private bool preserveSmoothEdges = true;
        private int retexture = 2;
        private bool mergeDuplicatedUV = true;

        private bool joinFbxMeshes = false;
        private bool joinGltfMeshes = false;
        private bool removeIsolatedVertices = true;
        private bool removeNonManifoldFaces = false;
        private bool removeDuplicatedFaces = true;
        private bool removeDuplicatedBoundaryVertices = true;
        private bool removeDegenerateFaces = true;
        private bool removeHiddenObjects = false;
        private bool projectNormals = false;
        private bool useVertexMask = false;
        private int resizeImages = 0;
        private int normalsWeighting = 1;
        private double contrastDouble = 0.5;
        private bool joinedSimplification = true;
        private float normalsScaling = 0f;
        private float removeMeshesBySize = 0f;
        private int removeMeshesByCount = 0;
        private int minimumFaceNumber = 0;
        private int target_poly = -1;

        private string commandResult;
      

        private string optionalSuffix = "_OPT";
        private string optionalDirectory = "AMAZ3D";
        private bool customDirectory = false;
        private bool choosePolygonCount = false;

        private bool showOptimizerParameters = false;


        private int resizeImagesIndex;
        private readonly int[] presetValues = { 0, 8192, 4096, 2048, 1024, 512, 256, 128 };
        private readonly string[] presetOptions = { "Off", "8192x8192", "4096x4096", "2048x2048", "1024x1024", "512x512", "256x256", "128x128" };
        private int idxAlgorithm;
        private readonly string[] presetAlgorithmValues = { "simple","amaz3d" };
        private readonly string[] presetAlgorithmsOptions = { "Basic optimization algorithm", "Advanced optimization algorithm"};

        [MenuItem("Tools/AMAZ3D/Contact us")]
        private static void ContactUs()
        {
            Application.OpenURL("https://adapta.studio/contact/");
        }

        [MenuItem("Tools/AMAZ3D/Tutorial")]
        private static void Tutorial()
        {
            Application.OpenURL("https://www.youtube.com/watch?v=75ZcPmwLgjA");
        }

        [MenuItem("Tools/AMAZ3D/Buy pro Plugin")]
        private static void BuyPro()
        {
            // insert Pro Plugin link
        }



        [MenuItem("Tools/AMAZ3D/AMAZ3D")]
        public static void ShowWindow()
        {
            GetWindow<Amaz3DWindow>("AMAZ3D");
        }

        void OnGUI()
        {
            GUILayout.Label("Mesh Optimization Settings", EditorStyles.boldLabel);

            selectedFBX = EditorGUILayout.ObjectField("Select FBX", selectedFBX, typeof(UnityEngine.Object), false);
            GUIContent textOptimizationAlgorithm= new GUIContent("Optimization algorithm", "This toogle will let you choose between the different optimization algorithms developed by ADAPTA studio. Keep in min that the results of the simple optimization algorithm are not same ase the advanced ones. The first one will let you optimizae asset up to 25k polygons, whereas the advanced one has no limit on the actual number of polygons.");
            idxAlgorithm = EditorGUILayout.Popup(textOptimizationAlgorithm, idxAlgorithm, presetAlgorithmsOptions);
            selectedAlgorithm = presetAlgorithmValues[idxAlgorithm];
   




            GUILayout.Space(10);
            EditorGUIUtility.labelWidth = 250;
            GUILayout.Label("Optimizer Parameters", EditorStyles.boldLabel);
            showOptimizerParameters = EditorGUILayout.Foldout(showOptimizerParameters, "Optimizer Parameters");
            if (showOptimizerParameters)
            {
                EditorGUI.indentLevel++;

                choosePolygonCount = EditorGUILayout.Toggle("Choose target polygon count", choosePolygonCount);
                if (choosePolygonCount)
                {
                    GUIContent textTargetPoly = new GUIContent("Target # of polygons", "The target polygon count you need to reach");
                    target_poly = EditorGUILayout.IntField(textTargetPoly, target_poly);
                }
                else
                {
                    target_poly = -1;
                    GUIContent textFaceReduction = new GUIContent("Face Reduction (% of face to remove)", "Polygon reduction can be tuned using a percentage ranging from 100% (inclusive) to 0.1% (inclusive)." +
                        "Example: value 10% will result in a reduced mesh constituted by 10 polygons when the original mesh consists of 100 polygons.\r\n" +
                        "REMARK: the original mesh is always transformed into a triangular tessellation before optimization takes place.");
                    faceReduction = EditorGUILayout.Slider(textFaceReduction, faceReduction * 100f, 0f, 100f) / 100f;
                }

                GUIContent textResizeImages = new GUIContent("Resize textures (px)", "Defines the new resolution all textures will be resized to" +
                    "(if a texture resolution is smaller or equal to the specified value no resizing is made for that specific texture)." +
                    "Assume square textures. Set the value to Orig. or 0 to avoid resizing textures. This parameter is not available if the project contains 16-bit textures.");
                resizeImagesIndex = EditorGUILayout.Popup(textResizeImages, resizeImagesIndex, presetOptions);
                resizeImages = presetValues[resizeImagesIndex];
                EditorGUI.indentLevel--;

                GUILayout.Label("Mesh merging", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                GUIContent textjoinFbxMeshes = new GUIContent("Join meshes (by material)", "When this option is enabled, AMAZ3D joins meshes composing a scene in file. The scene in the output file will contain only the joined mesh. Animations are not preserved in the joined mesh. For GLTF and GLB files meshes or primitives are joined only if they have thesame material.");
                joinFbxMeshes = EditorGUILayout.Toggle(textjoinFbxMeshes, joinFbxMeshes);
                EditorGUI.indentLevel--;

                GUILayout.Label("Boundary preservation", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                GUIContent textBoundaryPreserve = new GUIContent("Preserve Boundary Edges", "Manages boundary preservation at four different levels of detail of increasing power");
                preserveBoundaryEdges = EditorGUILayout.IntSlider(textBoundaryPreserve, preserveBoundaryEdges, 0, 3);
                EditorGUI.indentLevel--;


                GUILayout.Label("Vertex mask", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                GUIContent textVertexMask = new GUIContent("Use Vertex Mask", "When this option is selected, AMAZ3D will use the first vertex color layer as a mask for performing the optimization. The mask is defined in grayscale. In absolute black areas, AMAZ3D will preserve topology without performing any optimization. In absolute white areas, AMAZ3D will perform the optimization on the base of set parameters. It is possible to define grayscale to control how optimization influences specific areas. This option is available only if the mask is defined as grayscale vertex paint layer as detailed in the video.\r\nLink: https://youtu.be/J5jvXFgmJMo");
                useVertexMask = EditorGUILayout.Toggle(textVertexMask, useVertexMask);
                EditorGUI.indentLevel--;


                GUILayout.Label("Normals Computation", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                GUIContent textScaling = new GUIContent("Normals Scaling", "It handles normals preservation.\r\nHigher values lead to more accurate normals in the optimized object");
                normalsScaling = EditorGUILayout.Slider(textScaling, normalsScaling, 0f, 1f);
                if (normalsScaling == 0f)
                {
                    GUIContent textNormalsWeighting = new GUIContent("Normals Weighting", "Defines the strategy for calculating the weighted average of face normals used in the calculation of each vertex normals.\r\n" +
                        "Uniform (0): face normals are uniformly weighted.\r\nArea (1): face normals are weighted by face area.\r\nAngle (2): face normals are weighted by the angle between the two edges incident to each vertex.\r\n" +
                        "Mixed (3): face normals are weighted using both “Area” and “Angle” strategies. If shading importance is enabled this parameter won't have any effect.");
                    normalsWeighting = EditorGUILayout.IntSlider(textNormalsWeighting, normalsWeighting, 0, 3);
                    GUIContent textContrast = new GUIContent("Contrast", "Defines the strategy for calculating the weighted average of face normals used in the calculation of each vertex normal.\r\nUniform (0): face normals are uniformly weighted.\r\nArea (1): face normals are weighted by face area.\r\nAngle (2): face normals are weighted by the angle between the two edges incident to each vertex.\r\nMixed (3): face normals are weighted using both “Area” and “Angle” strategies.\r\nIf shading importance is enabled this parameter won't have any effect.");
                    contrastDouble = EditorGUILayout.Slider(textContrast, (float)contrastDouble, 0f, 1f);
                }
                GUIContent textPreserveHard = new GUIContent("Preserve Hard Edges", "When this option is selected, AMAZ3D preserves shading for hard edges during the optimization process.\r\nVice versa the software may convert some hard edges into smooth edges.");
                preserveHardEdges = EditorGUILayout.Toggle(textPreserveHard, preserveHardEdges);
                GUIContent textPreserveSmooth = new GUIContent("Preserve Smooth Edges", "When this option is selected, AMAZ3D preserves shading for smooth edges.\r\nVice versa the software may convert some smooth edges into hard edges.");
                preserveSmoothEdges = EditorGUILayout.Toggle(textPreserveSmooth, preserveSmoothEdges);
                GUIContent textProjectNormals = new GUIContent("Project Normals", "You may try to enable project normals to obtain better normals in your results. By enabling this option your result could get some artefacts.If you are optimizing an hard surface or a geometrical object please set an higher value of Feature Importance in order to improve your result.");
                projectNormals = EditorGUILayout.Toggle(textProjectNormals, projectNormals);

                EditorGUI.indentLevel--;



                GUILayout.Label("Mesh Cleanup", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                GUIContent textMerge = new GUIContent("Merge Duplicated UV", "When this option is selected, AMAZ3D merges duplicated UV coordinates.");
                mergeDuplicatedUV = EditorGUILayout.Toggle(textMerge, mergeDuplicatedUV);
                GUIContent textRemoveIsolated = new GUIContent("Remove Isolated Vertice", "When this option is selected, AMAZ3D removes vertices that do not belong to any triangle.");
                removeIsolatedVertices = EditorGUILayout.Toggle(textRemoveIsolated, removeIsolatedVertices);
                GUIContent textRemoveNonManifold = new GUIContent("Remove Non-Manifold Faces", "When this option is selected, AMAZ3D recursively deletes triangles adjacent to a non-manifold edge (i.e., an edge that is part of more than two triangles) until a 2-manifold edge (i.e., an edge that is part of two triangles only) is obtained.");
                removeNonManifoldFaces = EditorGUILayout.Toggle(textRemoveNonManifold, removeNonManifoldFaces);
                GUIContent textRemoveDups = new GUIContent("Remove Non-Manifold Faces", "When this option is selected, AMAZ3D merges triangles with identical vertices.");
                removeDuplicatedFaces = EditorGUILayout.Toggle(textRemoveDups, removeDuplicatedFaces);
                GUIContent textRemoveDupsBound = new GUIContent("Remove Non-Manifold Faces", "When this option is selected, AMAZ3D merges boundary vertices with identical coordinates.");
                removeDuplicatedBoundaryVertices = EditorGUILayout.Toggle(textRemoveDupsBound, removeDuplicatedBoundaryVertices);
                GUIContent textRemoveDegens = new GUIContent("Remove Degenerate Faces", "When this option is selected, AMAZ3D removes triangular faces with a null area.");
                removeDegenerateFaces = EditorGUILayout.Toggle(textRemoveDegens, removeDegenerateFaces);


                EditorGUI.indentLevel--;

                GUILayout.Label("Remove meshes", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                GUIContent textRemoveHidden = new GUIContent("Remove Hidden Objects", "When this option is enabled, AMAZ3D removes components that are not visibile by a set of uniformly distributed cameras along the bounding sphere of the scene.");
                removeHiddenObjects = EditorGUILayout.Toggle(textRemoveHidden, removeHiddenObjects);
                GUIContent textMeshBySize = new GUIContent("Remove Meshes By Size", "Removes meshes whose mesh to scene bounding box volume ratio is lower than the value selected");
                removeMeshesBySize = EditorGUILayout.FloatField(textMeshBySize, removeMeshesBySize);
                GUIContent textMeshByCount = new GUIContent("Remove Meshes By Count", "Removes meshes whose mesh to scene bounding box volume ratio is lower than the value selected");
                removeMeshesByCount = EditorGUILayout.IntField(textMeshByCount, removeMeshesByCount);
                EditorGUI.indentLevel--;

                GUILayout.Label("Geometric Feature importance", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                GUIContent textFeatureImportance = new GUIContent("Feature Importance", "Geometrical features importance allows controlling the preservation of geometric details during a polygon mesh reduction operation.\r\n Setting it to True may result in a larger number of polygons than originally intended.");
                featureImportance = EditorGUILayout.Toggle(textFeatureImportance, featureImportance);
                if (featureImportance)
                {
                    GUIContent textFeatureAngle = new GUIContent("Feature Angle", "Ignore areas delimited by geometrical features with angles smaller than this parameter");
                    featureAngle = EditorGUILayout.FloatField(textFeatureAngle, featureAngle);
                    GUIContent textFeatureArea = new GUIContent("Feature Area", "Ignore areas delimited by geometrical features smaller than this parameter. The feature area parameter describes the percentage between the ignored areas and the total area. This parameter won't have any effect if feature importance is set to Low.");
                    featureAreaDouble = EditorGUILayout.DoubleField(textFeatureArea, featureAreaDouble);

                }
                EditorGUI.indentLevel--;
            }

            optionalSuffix = EditorGUILayout.TextField("Optional Suffix", optionalSuffix);
            customDirectory = EditorGUILayout.Toggle("Custom directory", customDirectory);
            if (customDirectory)
            {

                optionalDirectory = EditorGUILayout.TextField("Optional Directory in Assets (Asset/)", optionalDirectory);
            }

            if (GUILayout.Button("Optimize"))
            {
                RunOptimizationCommand();
            }



            if (!string.IsNullOrEmpty(commandResult))
            {
                EditorGUILayout.LabelField("Command Result:");
                EditorGUILayout.TextArea(commandResult, GUILayout.Height(100));
            }

            EditorGUILayout.TextArea("This is a lite version of this plugin which uses a basic algorithm for 3D access optimization. \n " +
                "You can only optimize asset with polygon count up to 25k due to technical limitations of the basic algorithm\n " +
                "You can buy buy the pro plugin on the Unity Asset store to access optimization at any level");
        }



        private string ConstructCommand()
        {
            string rec;
            //string inputFilePath = Path.Combine(dataPath, Path.GetFileName(AssetDatabase.GetAssetPath(selectedFBX)));

            if (selectedFBX != null)
            {

                string dataPath = Application.dataPath;
                dataPath = dataPath.Replace("Assets", "");


                string inputFilePath = Path.Combine(dataPath, AssetDatabase.GetAssetPath(selectedFBX));

                string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                string relativePath = Path.GetDirectoryName(inputFilePath);
                string extension = Path.GetExtension(inputFilePath);
                string importedFileName = fileName + optionalSuffix + extension;
                string outputFilePath;
                string metadataFilePath;

                if (!customDirectory)
                {
                    outputFilePath = Path.Combine(relativePath, importedFileName);
                    metadataFilePath = Path.Combine(relativePath, "meta.log");
                }
                else
                {
                    outputFilePath = Path.Combine(Application.dataPath, optionalDirectory, importedFileName);
                    string importSubfolderPath = string.IsNullOrEmpty(optionalDirectory) ? "Assets" : Path.Combine("Assets", optionalDirectory);


                    if (!AssetDatabase.IsValidFolder(importSubfolderPath) && !string.IsNullOrEmpty(optionalDirectory))
                    {
                        string parentFolder = "Assets";
                        string[] folders = optionalDirectory.Split('/');
                        foreach (string folder in folders)
                        {
                            string newFolderPath = Path.Combine(parentFolder, folder);
                            if (!AssetDatabase.IsValidFolder(newFolderPath))
                            {
                                AssetDatabase.CreateFolder(parentFolder, folder);
                            }
                            parentFolder = newFolderPath;
                        }
                    }

                    metadataFilePath = Path.Combine(Application.dataPath, optionalDirectory, "meta.log");
                }



                IntPtr manager = createManager();


                IntPtr options = setup(manager,
                                       inputFilePath,
                                       outputFilePath,
                                       metadataFilePath,
                                       faceReduction,
                                       normalsScaling,
                                       minimumFaceNumber,
                                       preserveBoundaryEdges,
                                       preserveHardEdges,
                                       preserveSmoothEdges,
                                       projectNormals,
                                       normalsWeighting,
                                       contrastDouble,
                                       retexture,
                                       mergeDuplicatedUV,
                                       removeIsolatedVertices,
                                       removeNonManifoldFaces,
                                       removeDuplicatedFaces,
                                       removeDuplicatedBoundaryVertices,
                                       removeDegenerateFaces,
                                       10000000,
                                       useVertexMask,
                                       resizeImages,
                                       joinedSimplification,
                                       removeMeshesByCount,
                                       removeMeshesBySize,
                                       featureImportance,
                                       featureAngle,
                                       featureAreaDouble,
                                       removeHiddenObjects,
                                       joinFbxMeshes,
                                       joinGltfMeshes,
                                       0,
                                       target_poly);

                // TBD Marshal.FreeCoTaskMem(tt);

                IntPtr tt = optimization(manager, selectedAlgorithm);

                rec = Marshal.PtrToStringAnsi(tt);
               

                AssetDatabase.Refresh();


                if (!File.Exists(outputFilePath))
                {
                    Debug.LogError("Optimized FBX file not found: " + outputFilePath);

                }
                else
                {
                    // Aggiusta il percorso per essere relativo alla cartella 'Assets'
                    string relativePathForFbx = "Assets/" + Path.Combine(optionalDirectory, importedFileName).Replace("\\", "/");
                    if (resizeImages != 0)
                    {

                        string texturesFolderPath = Path.Combine("Assets", optionalDirectory, "textures" + optionalSuffix);
                        ExtractTextures(relativePathForFbx, texturesFolderPath);
                    }

                    var importer = AssetImporter.GetAtPath(relativePathForFbx) as ModelImporter;
                    if (extension == "fbx")
                    {
                        AssetDatabase.ImportAsset(importer.assetPath, ImportAssetOptions.ForceUpdate);
                    }
                }
                string deleteMeta = metadataFilePath;
                string deleteMetaMeta = metadataFilePath + ".meta";
                FileUtil.DeleteFileOrDirectory(deleteMeta);
                FileUtil.DeleteFileOrDirectory(deleteMetaMeta);
                AssetDatabase.Refresh();

                //freeManager(manager);
            }
            else
            {
                Debug.LogError("No input file!");
                rec = "No input file!";
            }



            return rec;
        }


        private void RunOptimizationCommand()
        {
            string command = ConstructCommand();

            commandResult = command;

        }



        private void ExtractTextures(string pathToFbx, string destinationPath)
        {
            var importer = AssetImporter.GetAtPath(pathToFbx) as ModelImporter;



            AssetDatabase.Refresh();


        }

    }
}

