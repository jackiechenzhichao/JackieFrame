using UnityEngine;
using UnityEditor;

public class BMFontEditor : EditorWindow
{
    [MenuItem("Tools/BMFont Maker", false, 200)]
    static public void OpenBMFontMaker()
    {
        EditorWindow.GetWindow<BMFontEditor>(false, "BMFont Maker", true).Show();
    }

    [SerializeField]
    private Font targetFont;
    [SerializeField]
    private TextAsset fntData;
    [SerializeField]
    private Material fontMaterial;
    [SerializeField]
    private Texture2D fontTexture;
    private BMFont bmFont = new BMFont();

    void OnGUI()
    {
        //targetFont = EditorGUILayout.ObjectField("Target Font", targetFont, typeof(Font), false) as Font;
        fntData = EditorGUILayout.ObjectField("Fnt Data", fntData, typeof(TextAsset), false) as TextAsset;
        //fontMaterial = EditorGUILayout.ObjectField("Font Material", fontMaterial, typeof(Material), false) as Material;
        fontTexture = EditorGUILayout.ObjectField("Font Texture", fontTexture, typeof(Texture2D), false) as Texture2D;

        if (GUILayout.Button("Create BMFont"))
        {
            if (fntData == null)
            {
                ShowNotification(new GUIContent("Font Fnt 为空"));
                return;
            }

            if (fontTexture == null)
            {
                ShowNotification(new GUIContent("Font Texture 为空"));
                return;
            }

            string fontTextureUrl = AssetDatabase.GetAssetPath(fontTexture);
            int index = fontTextureUrl.LastIndexOf('/');
            string fontPrefix = fontTextureUrl.Substring(0, index);

            BMFontReader.Load(bmFont, fntData.name, fntData.bytes); // 借用NGUI封装的读取类
            CharacterInfo[] characterInfo = new CharacterInfo[bmFont.glyphs.Count];
            for (int i = 0; i < bmFont.glyphs.Count; i++)
            {
                BMGlyph bmInfo = bmFont.glyphs[i];
                CharacterInfo info = new CharacterInfo();
                info.index = bmInfo.index; 
                //info.uv.x = (float)bmInfo.x / (float)bmFont.texWidth;
                //info.uv.y = 1 - (float)bmInfo.y / (float)bmFont.texHeight;
                //info.uv.width = (float)bmInfo.width / (float)bmFont.texWidth;
                //info.uv.height = -1f * (float)bmInfo.height / (float)bmFont.texHeight;
                //info.vert.x = 0;
                //info.vert.y = -(float)bmInfo.height;
                //info.vert.width = (float)bmInfo.width;
                //info.vert.height = (float)bmInfo.height;
                //info.width = (float)bmInfo.advance;


                //这里注意下UV坐标系和从BMFont里得到的信息的坐标系是不一样的哦，前者左下角为（0,0），
                //右上角为（1,1）。而后者则是左上角上角为（0,0），右下角为（图宽，图高）
                info.uvTopLeft = new Vector2((float)bmInfo.x / bmFont.texWidth, 1 - (float)bmInfo.y / bmFont.texHeight);
                info.uvTopRight = new Vector2((float)(bmInfo.x + bmInfo.width) / bmFont.texWidth, 1 - (float)bmInfo.y / bmFont.texHeight);
                info.uvBottomLeft = new Vector2((float)bmInfo.x / bmFont.texWidth, 1 - (float)(bmInfo.y + bmInfo.height) / bmFont.texHeight);
                info.uvBottomRight = new Vector2((float)(bmInfo.x + bmInfo.width) / bmFont.texWidth, 1 - (float)(bmInfo.y + bmInfo.height) / bmFont.texHeight);

                info.minX = 0;
                info.minY = -bmInfo.height;
                info.maxX = bmInfo.width;
                info.maxY = 0;

                info.advance = bmInfo.advance;
                characterInfo[i] = info;
            }

            //Material
            string fontMaterialUrl = fontPrefix + "/" + fontTexture.name + ".mat";
            fontMaterial = AssetDatabase.LoadAssetAtPath(fontMaterialUrl, typeof(Material)) as Material;

            if (fontMaterial == null)
            {
                fontMaterial = new Material(Shader.Find("UI/Unlit/Text"));
                AssetDatabase.CreateAsset(fontMaterial, fontMaterialUrl);
                AssetDatabase.Refresh();
            }
            fontMaterial.shader = Shader.Find("UI/Unlit/Text");//这一行很关键，如果用standard的shader，放到Android手机上，第一次加载会很慢
            fontMaterial.SetTexture("_MainTex", fontTexture);
            EditorUtility.SetDirty(fontMaterial);


            //Font
            string fontUrl = fontPrefix + "/" + fontTexture.name + ".fontsettings";
            targetFont = AssetDatabase.LoadAssetAtPath(fontUrl, typeof(Font)) as Font;
            if (targetFont == null)
            {
                targetFont = new Font();
                AssetDatabase.CreateAsset(targetFont, fontUrl);
                AssetDatabase.Refresh();
            }
            targetFont.material = fontMaterial;
            targetFont.characterInfo = characterInfo;
            EditorUtility.SetDirty(targetFont);
            AssetDatabase.SaveAssets();

           

            Debug.Log("create font <" + targetFont.name + "> success");
            ShowNotification(new GUIContent("create font <" + targetFont.name + "> success"));
            //Close();
        }
    }
}