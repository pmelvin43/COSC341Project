using UnityEngine;
using TMPro;

public class TextSelection : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    private int startIndex;
    private int endIndex;
    private bool isSelecting;

    private string originalText;

    void Start()
    {
        // Store the original text without any markup
        originalText = textMeshPro.text;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 startPos = Input.mousePosition;
            startIndex = TMP_TextUtilities.GetCursorIndexFromPosition(textMeshPro, startPos, null);
            isSelecting = true;
        }

        if (isSelecting && Input.GetMouseButton(0))
        {
            Vector2 currentPos = Input.mousePosition;
            endIndex = TMP_TextUtilities.GetCursorIndexFromPosition(textMeshPro, currentPos, null);
            HighlightText(startIndex, endIndex);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
        }
    }

    void HighlightText(int start, int end)
    {
        // Ensure startIndex <= endIndex
        if (start > end)
        {
            int temp = start;
            start = end;
            end = temp;
        }

        // Restore original text before applying new markup
        textMeshPro.text = originalText;

        // Extract the parts of the text before, within, and after the selection
        string before = textMeshPro.text.Substring(0, start);
        string selected = textMeshPro.text.Substring(start, end - start);
        string after = textMeshPro.text.Substring(end);

        // Apply markup to the selected text
        selected = $"<u><color=#FFFF00AA>{selected}</color></u>";

        // Combine the text parts back together
        textMeshPro.text = before + selected + after;
    }
}
