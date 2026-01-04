using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(NamedArrayAttribute))]
public class NamedArrayDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 배열 내부의 요소에 대해서만 처리
        // PropertyDrawer는 배열 자체가 아니라 배열의 각 요소마다 호출될 수 있음 (하지만 여기서는 배열 전체에 속성을 걸었으므로 주의)
        // Unity에서 배열에 속성을 걸면, 배열의 각 요소마다 Drawer가 호출되는 것이 아니라, 배열 자체에는 Drawer가 안 먹히고 요소에 먹히는 경우가 있음.
        // 하지만 여기서는 List<AudioClip>에 걸었으므로, List 자체에 대한 Drawer가 아니라 요소에 대한 Drawer가 필요할 수 있음.
        // 그러나 우리는 "배열의 인덱스"에 따라 이름을 바꾸고 싶음.
        
        // NamedArrayAttribute는 배열 필드 위에 선언됨.
        // Unity의 PropertyDrawer는 배열의 'Element'를 그릴 때 호출됨.
        
        NamedArrayAttribute namedAttribute = attribute as NamedArrayAttribute;
        
        // 현재 그려지고 있는 프로퍼티의 경로에서 인덱스를 추출
        // 예: _sfxList.Array.data[0]
        int index = GetIndexFromPath(property.propertyPath);

        if (index >= 0 && index < namedAttribute.Names.Length)
        {
            label.text = namedAttribute.Names[index];
        }
        else if (index >= namedAttribute.Names.Length)
        {
            label.text = $"Element {index}";
        }

        EditorGUI.PropertyField(position, property, label, true);
    }

    private int GetIndexFromPath(string path)
    {
        // 정규식이나 문자열 파싱으로 인덱스 추출
        // "variableName.Array.data[0]" 형태
        int startIndex = path.LastIndexOf("[");
        int endIndex = path.LastIndexOf("]");

        if (startIndex > 0 && endIndex > startIndex)
        {
            string indexStr = path.Substring(startIndex + 1, endIndex - startIndex - 1);
            if (int.TryParse(indexStr, out int index))
            {
                return index;
            }
        }
        return -1;
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
