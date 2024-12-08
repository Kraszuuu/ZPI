using UnityEngine;
using TMPro;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class CircularTextMeshPro : MonoBehaviour
{
    private TextMeshProUGUI m_TextComponent;

    [SerializeField]
    [Tooltip("The radius of the text circle arc")]
    private float m_radius = 10.0f;

    public float Radius
    {
        get => m_radius;
        set
        {
            m_radius = value;
            OnCurvePropertyChanged();
        }
    }

    private void Awake()
    {
        m_TextComponent = gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        m_TextComponent.OnPreRenderText += UpdateTextCurve;
        OnCurvePropertyChanged();
    }

    private void OnDisable()
    {
        m_TextComponent.OnPreRenderText -= UpdateTextCurve;
    }

    protected void OnCurvePropertyChanged()
    {
        UpdateTextCurve(m_TextComponent.textInfo);
        m_TextComponent.ForceMeshUpdate();
    }

    protected void UpdateTextCurve(TMP_TextInfo textInfo)
    {
        Vector3[] vertices;
        Matrix4x4 matrix;

        for (int i = 0; i < textInfo.characterInfo.Length; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            vertices = textInfo.meshInfo[materialIndex].vertices;

            Vector3 charMidBaselinePos = new Vector2(
                (vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2,
                textInfo.characterInfo[i].baseLine);

            vertices[vertexIndex + 0] += -charMidBaselinePos;
            vertices[vertexIndex + 1] += -charMidBaselinePos;
            vertices[vertexIndex + 2] += -charMidBaselinePos;
            vertices[vertexIndex + 3] += -charMidBaselinePos;

            matrix = ComputeTransformationMatrix(charMidBaselinePos, textInfo, i);

            vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
            vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
            vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
            vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
        }
    }

    protected Matrix4x4 ComputeTransformationMatrix(Vector3 charMidBaselinePos, TMP_TextInfo textInfo, int charIdx)
    {
        float radiusForThisLine = m_radius + textInfo.lineInfo[textInfo.characterInfo[charIdx].lineNumber].baseline;
        float circumference = 2 * radiusForThisLine * Mathf.PI;
        float angle = ((charMidBaselinePos.x / circumference - 0.5f) * 360 + 90) * Mathf.Deg2Rad;

        float x0 = Mathf.Cos(angle);
        float y0 = Mathf.Sin(angle);
        Vector2 newMidBaselinePos = new Vector2(x0 * radiusForThisLine, -y0 * radiusForThisLine);

        float rotationAngle = -Mathf.Atan2(y0, x0) * Mathf.Rad2Deg - 90;

        return Matrix4x4.TRS(
            new Vector3(newMidBaselinePos.x, newMidBaselinePos.y, 0),
            Quaternion.AngleAxis(rotationAngle, Vector3.forward),
            Vector3.one
        );
    }
}
