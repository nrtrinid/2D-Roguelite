#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponDefinition))]
public class WeaponDefinitionEditor : Editor
{
    SerializedProperty displayName, weaponSprite;
    SerializedProperty bulletPrefab, bulletSpeed, damage, bulletLife, hitMask;
    SerializedProperty fireMode, fireCooldown;
    SerializedProperty burstCount, burstInterval;
    SerializedProperty pellets, spreadDegrees;
    SerializedProperty allowAuto, autoOnByDefault;

    void OnEnable()
    {
        displayName     = serializedObject.FindProperty("displayName");
        weaponSprite    = serializedObject.FindProperty("weaponSprite");

        bulletPrefab    = serializedObject.FindProperty("bulletPrefab");
        bulletSpeed     = serializedObject.FindProperty("bulletSpeed");
        damage          = serializedObject.FindProperty("damage");
        bulletLife      = serializedObject.FindProperty("bulletLife");
        hitMask         = serializedObject.FindProperty("hitMask");

        fireMode        = serializedObject.FindProperty("fireMode");
        fireCooldown    = serializedObject.FindProperty("fireCooldown");

        burstCount      = serializedObject.FindProperty("burstCount");
        burstInterval   = serializedObject.FindProperty("burstInterval");

        pellets         = serializedObject.FindProperty("pellets");
        spreadDegrees   = serializedObject.FindProperty("spreadDegrees");

        allowAuto       = serializedObject.FindProperty("allowAuto");
        autoOnByDefault = serializedObject.FindProperty("autoOnByDefault");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Meta", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(displayName);
        EditorGUILayout.PropertyField(weaponSprite);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Projectile", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(bulletPrefab);
        EditorGUILayout.PropertyField(bulletSpeed);
        EditorGUILayout.PropertyField(damage);
        EditorGUILayout.PropertyField(bulletLife);
        EditorGUILayout.PropertyField(hitMask);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Firing", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(fireMode);
        EditorGUILayout.PropertyField(fireCooldown);

        // Mode-specific
        var mode = (FireMode)fireMode.enumValueIndex;
        using (new EditorGUI.IndentLevelScope())
        {
            switch (mode)
            {
                case FireMode.Burst:
                    EditorGUILayout.PropertyField(burstCount,     new GUIContent("Burst Count"));
                    EditorGUILayout.PropertyField(burstInterval,  new GUIContent("Burst Interval (s)"));
                    EditorGUILayout.PropertyField(spreadDegrees,  new GUIContent("Spread (deg)"));
                    break;

                case FireMode.Shotgun:
                    EditorGUILayout.PropertyField(pellets,        new GUIContent("Pellets"));
                    EditorGUILayout.PropertyField(spreadDegrees,  new GUIContent("Spread (deg)"));
                    break;

                case FireMode.Semi:
                    // nothing extra
                    break;
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Auto Fire (per-weapon)", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(allowAuto, new GUIContent("Allow Auto"));
        using (new EditorGUI.IndentLevelScope())
        using (new EditorGUI.DisabledScope(!allowAuto.boolValue))

        EditorGUILayout.Space();
        DrawInfo(mode);

        serializedObject.ApplyModifiedProperties();
    }

    void DrawInfo(FireMode mode)
    {
        string tip = mode switch
        {
            FireMode.Semi    => "Semi: One shot per cycle (cooldown).",
            FireMode.Burst   => "Burst: Fires Burst Count shots spaced by Burst Interval.",
            FireMode.Shotgun => "Shotgun: Fires Pellets with Spread each cycle.",
            _ => ""
        };

        EditorGUILayout.HelpBox(
            tip + "\n\nAuto-fire is a per-weapon option. If 'Allow Auto' is enabled on the weapon, you can toggle it at runtime via WeaponShooter.ToggleAuto().",
            MessageType.Info
        );
    }
}
#endif
