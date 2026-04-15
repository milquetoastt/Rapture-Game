using UnityEngine;
using UnityEditor;

public class BalanceTool : EditorWindow
{
    private GloveSpells gloveSpells;
    private MantleSpells mantleSpells;
    private HatSpells hatSpells;
    private BootSpells bootSpells;

    private bool showGloveSpells = true;
    private bool showMantleSpells = true;
    private bool showHatSpells = true;
    private bool showBootSpells = true;

    private Vector2 scrollPos;

    [MenuItem("Tools/Spell Balance Tool")]
    public static void ShowWindow()
    {
        GetWindow<BalanceTool>("Spell Balance Tool");
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        DrawGloveSection();
        EditorGUILayout.Space();

        DrawMantleSection();
        EditorGUILayout.Space();

        DrawHatSection();
        EditorGUILayout.Space();

        DrawBootSection();

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            if (gloveSpells != null) EditorUtility.SetDirty(gloveSpells);
            if (mantleSpells != null) EditorUtility.SetDirty(mantleSpells);
            if (hatSpells != null) EditorUtility.SetDirty(hatSpells);
            if (bootSpells != null) EditorUtility.SetDirty(bootSpells);
        }
    }

    private void DrawGloveSection()
    {
        gloveSpells = (GloveSpells)EditorGUILayout.ObjectField(
            "Glove Spells",
            gloveSpells,
            typeof(GloveSpells),
            true
        );

        if (gloveSpells == null) return;

        showGloveSpells = EditorGUILayout.Foldout(
            showGloveSpells,
            "Glove Spells",
            true,
            EditorStyles.foldoutHeader
        );

        if (!showGloveSpells) return;

        for (int i = 0; i < gloveSpells.spells.Length; i++)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Glove Spell {i}");

            gloveSpells.spells[i].fireCooldown =
                EditorGUILayout.FloatField("Cooldown", gloveSpells.spells[i].fireCooldown);

            gloveSpells.spells[i].recoil =
                EditorGUILayout.FloatField("Recoil", gloveSpells.spells[i].recoil);

            gloveSpells.spells[i].projectilePrefab =
                (GameObject)EditorGUILayout.ObjectField(
                    "Projectile",
                    gloveSpells.spells[i].projectilePrefab,
                    typeof(GameObject),
                    false
                );

            gloveSpells.spells[i].windUp =
                EditorGUILayout.FloatField("Windup", gloveSpells.spells[i].windUp);

            SpellStats stats = gloveSpells.spells[i].projectilePrefab.GetComponent<SpellStats>();
            DrawSpellStats(stats, "Projectile Stats");

            //droplets
            if (i == 3)
            {
                BubbleBall ball = gloveSpells.spells[i].projectilePrefab.GetComponent<BubbleBall>();
                GameObject dropletsPrefab = ball.dropletsPrefab;
                SpellStats dropletStats = dropletsPrefab.GetComponent<SpellStats>();

                DrawSpellStats(dropletStats, "Droplets Stats");
            }

            EditorGUILayout.EndVertical();
        }
    }

    private void DrawMantleSection()
    {
        mantleSpells = (MantleSpells)EditorGUILayout.ObjectField(
            "Mantle Spells",
            mantleSpells,
            typeof(MantleSpells),
            true
        );

        if (mantleSpells == null) return;
        showMantleSpells = EditorGUILayout.Foldout(
            showMantleSpells,
            "Mantle Spells",
            true,
            EditorStyles.foldoutHeader
        );

        if (!showMantleSpells) return;
        for (int i = 0; i < mantleSpells.spells.Length; i++)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Mantle Spell {i}");

            mantleSpells.spells[i].fireCooldown =
                EditorGUILayout.FloatField("Cooldown", mantleSpells.spells[i].fireCooldown);

            mantleSpells.spells[i].spellPrefab =
                (GameObject)EditorGUILayout.ObjectField(
                    "Projectile",
                    mantleSpells.spells[i].spellPrefab,
                    typeof(GameObject),
                    false
                );

            mantleSpells.spells[i].windUp =
                EditorGUILayout.FloatField("Windup", mantleSpells.spells[i].windUp);

            SpellStats stats = mantleSpells.spells[i].spellPrefab.GetComponent<SpellStats>();
            DrawSpellStats(stats, "Projectile Stats");

            EditorGUILayout.EndVertical();
        }
    }

    private void DrawHatSection()
    {
        hatSpells = (HatSpells)EditorGUILayout.ObjectField(
            "Hat Spells",
            hatSpells,
            typeof(HatSpells),
            true
        );

        if (hatSpells == null) return;

        showHatSpells = EditorGUILayout.Foldout(
            showHatSpells,
            "Hat Spells",
            true,
            EditorStyles.foldoutHeader
        );

        if (!showHatSpells) return;
        for (int i = 0; i < hatSpells.spells.Length; i++)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Hat Spell {i}");

            hatSpells.spells[i].fireCooldown =
                EditorGUILayout.FloatField("Cooldown", hatSpells.spells[i].fireCooldown);

            hatSpells.spells[i].spellPrefab =
                (GameObject)EditorGUILayout.ObjectField(
                    "Projectile",
                    hatSpells.spells[i].spellPrefab,
                    typeof(GameObject),
                    false
                );

            hatSpells.spells[i].windUp =
                EditorGUILayout.FloatField("Windup", hatSpells.spells[i].windUp);

            SpellStats stats = hatSpells.spells[i].spellPrefab.GetComponent<SpellStats>();
            DrawSpellStats(stats, "Projectile Stats");

            EditorGUILayout.EndVertical();
        }
    }

    private void DrawBootSection()
    {
        bootSpells = (BootSpells)EditorGUILayout.ObjectField(
            "Boot Spells",
            bootSpells,
            typeof(BootSpells),
            true
        );

        if (bootSpells == null) return;

        showBootSpells = EditorGUILayout.Foldout(
            showBootSpells,
            "Boot Spells",
            true,
            EditorStyles.foldoutHeader
        );

        if (!showBootSpells) return;
        for (int i = 0; i < bootSpells.spells.Length; i++)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Boot Spell {i}");

            bootSpells.spells[i].fireCooldown =
                EditorGUILayout.FloatField("Cooldown", bootSpells.spells[i].fireCooldown);


            bootSpells.spells[i].spellPrefab =
                (GameObject)EditorGUILayout.ObjectField(
                    "Projectile",
                    bootSpells.spells[i].spellPrefab,
                    typeof(GameObject),
                    false
                );

            bootSpells.spells[i].windUp =
                EditorGUILayout.FloatField("Windup", bootSpells.spells[i].windUp);

            SpellStats stats = bootSpells.spells[i].spellPrefab.GetComponent<SpellStats>();
            DrawSpellStats(stats, "Projectile Stats");

            if (i == 0)
            {
                CloudCart cart = bootSpells.spells[i].spellPrefab.GetComponent<CloudCart>();
                GameObject rain = cart.rainPrefab;
                SpellStats rainStats = rain.GetComponent<SpellStats>();

                DrawSpellStats(rainStats, "Rain Stats");
            }
            if (i == 2)
            {
                OhmsTresspass ohms = bootSpells.spells[i].spellPrefab.GetComponent<OhmsTresspass>();
                GameObject bolt = ohms.hitboxPrefab;
                SpellStats boltStats = bolt.GetComponent<SpellStats>();

                DrawSpellStats(boltStats, "Bolt Stats");
            }

            EditorGUILayout.EndVertical();
        }
    }

    private void DrawSpellStats(SpellStats stats, string header)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(header);

        SerializedObject serializedStats = new SerializedObject(stats);
        serializedStats.Update();

        SerializedProperty property = serializedStats.GetIterator();
        bool enterChildren = true;

        while (property.NextVisible(enterChildren))
        {
            if (property.name != "m_Script")
            {
                EditorGUILayout.PropertyField(property, true);
            }

            enterChildren = false;
        }

        serializedStats.ApplyModifiedProperties();
    }
}