using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
using Data;
using UnityEditor.SceneManagement;
using UnityEngine.Networking;

[CustomEditor(typeof(SpaceShipSettings))]
public class SettingsModifyerEditor : Editor
{
    enum SupportedAspects
    {
        Aspect4by3 = 1,
        Aspect5by4 = 2,
        Aspect16by10 = 3,
        Aspect16by9 = 4,
        Aspect4by4 = 5,
    };

    Camera _cam = null;
    RenderTexture _rt;
    Texture2D _tex2d;
    Scene _scene;
    SpaceShipSettings _target;

    private SerializedObject _serializedObject;
    // preview variables
    SupportedAspects _aspectChoiceIdx = SupportedAspects.Aspect16by10;
    float _curAspect;

    float ToFloat(SupportedAspects aspects)
    {
        switch (aspects)
        {
            case SupportedAspects.Aspect16by10:
                return 16 / 10f;
            case SupportedAspects.Aspect16by9:
                return 16 / 9f;
            case SupportedAspects.Aspect4by3:
                return 4 / 3f;
            case SupportedAspects.Aspect5by4:
                return 5 / 4f;
            case SupportedAspects.Aspect4by4:
                return 4 / 4f;
            default:
                throw new ArgumentException();
        }
    }

    void DrawRefScene()
    {
        _rt = new RenderTexture(Mathf.RoundToInt(_curAspect * _target.RenderTextureHeight), _target.RenderTextureHeight, 16);
        _cam.targetTexture = _rt;
        _cam.Render();
        _tex2d = new Texture2D(_rt.width, _rt.height, TextureFormat.RGBA32, false);
        _tex2d.Apply(false);
        Graphics.CopyTexture(_rt, _tex2d);
    }

    Vector2 GetGUIPreviewSize()
    {
        //Vector2 camSizeWorld = new Vector2(_worldScreenHeight * _curAspect, _worldScreenHeight);
        Vector2 camSizeWorld = new Vector2(_target.WorldScreenHeight * _curAspect, _target.WorldScreenHeight);
        float scaleFactor = EditorGUIUtility.currentViewWidth / camSizeWorld.x;
        return new Vector2(EditorGUIUtility.currentViewWidth, scaleFactor * camSizeWorld.y);
    }

    #region Init
    void OnEnable()
    {
        _target = (SpaceShipSettings)target;
        _serializedObject = new SerializedObject(_target);

        void OpenSceneDelay()
        {
            EditorApplication.delayCall -= OpenSceneDelay;
            DrawRefScene();
        }

        _aspectChoiceIdx = SupportedAspects.Aspect16by10;

        _scene = SceneManager.GetActiveScene();

        // PrefabUtility.LoadPrefabContentsIntoPreviewScene("Assets/Prefabs/Demo/DemoBkg.prefab", _scene);
        _cam = _scene.GetRootGameObjects()[0].GetComponentInChildren<Camera>();
        _cam.cameraType = CameraType.Preview;
        _cam.scene = _scene;
        _curAspect = ToFloat(_aspectChoiceIdx);
        _cam.aspect = _curAspect;
        _cam.orthographicSize = _target.WorldScreenHeight;

        EditorApplication.delayCall += OpenSceneDelay;

        var points = FindObjectsOfType<NetworkStartPosition>();
        if(points != null)
        {
            for (int i = 0; i < points.Length; i++)
            {
                _target.AddSpawnPoint(points[i].gameObject);
            }
        }
    }

    void OnDisable()
    {
        //EditorSceneManager.ClosePreviewScene(_scene);
    }
    #endregion

    void OnCamSettingChange()
    {
        _curAspect = ToFloat(_aspectChoiceIdx);
        _cam.aspect = _curAspect;
        _cam.orthographicSize = _target.WorldScreenHeight;
        DrawRefScene();

        _serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(_target);

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    // GUI states
    class GUIControlStates
    {
        public bool foldout = false;
    };
    GUIControlStates _guiStates = new GUIControlStates();
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        using (var scope = new EditorGUI.ChangeCheckScope())
        {
            _aspectChoiceIdx = (SupportedAspects)EditorGUILayout.EnumPopup("Aspect of the window", (Enum)_aspectChoiceIdx);
            if (scope.changed)
            {
                OnCamSettingChange();
            }
        }

        _guiStates.foldout = EditorGUILayout.Foldout(_guiStates.foldout, "Resolution", true);
        if (_guiStates.foldout)
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {   
                _target.SetWorldScreenHeight(EditorGUILayout.FloatField("WorldScreenHeight", _target.WorldScreenHeight));
                _target.SetRenderTextureHeight(EditorGUILayout.IntField("RenderTextureHeight", _target.RenderTextureHeight));

                if (scope.changed)
                {
                    OnCamSettingChange();
                }
            }
        }

        if (_tex2d != null)
        {
            Vector2 sz = GetGUIPreviewSize();
            Rect r = EditorGUILayout.GetControlRect(false, GUILayout.Height(sz.y), GUILayout.ExpandHeight(false));
            EditorGUI.DrawPreviewTexture(r, _tex2d);
        }

        _target.SetPrefab(EditorGUILayout.ObjectField("PrefabPoint", _target.Prefab, typeof(GameObject), true) as GameObject);
        _target.SetPointsHolder(EditorGUILayout.ObjectField("Holder", _target.SpawnPointsHolder, typeof(GameObject), true) as GameObject);

        if (GUILayout.Button("New SpawnPoint"))
        {
            var point = Instantiate(_target.Prefab, _target.SpawnPointsHolder.transform);
            _target.AddSpawnPoint(point);
            OnCamSettingChange();
        }

        //_target.SpawnPoints.Clear();
        using (var scope = new EditorGUI.ChangeCheckScope())
        {
            GUILayout.BeginVertical("box");

            for (int i = 0; i < _target.SpawnPoints.Count; i++)
            {
                if (_target.SpawnPoints[i] == null)
                {
                    _target.SpawnPoints.RemoveAt(i);
                    continue;
                }

                GUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"Точка №{i}", GUILayout.Width(80));

                EditorGUIUtility.labelWidth = 30;

                _target.SetXCoord(i, EditorGUILayout.FloatField("X", _target.SpawnPoints[i].transform.position.x, GUILayout.Width(90)));

                GUILayout.Space(20);

                _target.SetZCoord(i, EditorGUILayout.FloatField("Z", _target.SpawnPoints[i].transform.position.z, GUILayout.Width(90)));

                GUILayout.Space(20);

                if (GUILayout.Button("Delete", GUILayout.Width(50)))
                {
                    DestroyImmediate(_target.SpawnPoints[i]);
                    _target.SpawnPoints.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            if (GUILayout.Button("ClearList"))
            {
                foreach (var item in _target.SpawnPoints)
                {
                    if (item != null) DestroyImmediate(item);
                }

                _target.SpawnPoints.Clear();
            }

            if (scope.changed)
            {
                OnCamSettingChange();
            }
        }          
    }
}
