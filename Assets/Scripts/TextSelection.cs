using UnityEngine;
using TMPro;

public class TextSelectionWithOffsetCursor : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public GameObject cursorObject;  // Reference to the GameObject with a SpriteRenderer for the cursor
    public float cursorOffset = 1f;  // Vertical offset for the cursor (in screen units)

    private int startIndex;
    private int endIndex;
    private bool isSelecting;

    private string originalText;

    void Start()
    {
        // Store the original text without any markup
        originalText = textMeshPro.text;

        // Hide the cursor initially
        cursorObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 startPos = Input.mousePosition;
            Vector2 offsetStartPos = startPos + new Vector2(0, cursorOffset); // Apply the offset in screen space

            // Use the offset cursor position for selecting text
            startIndex = TMP_TextUtilities.GetCursorIndexFromPosition(textMeshPro, offsetStartPos, null);
            isSelecting = true;

            // Update the cursor position for display purposes
            Vector3 worldCursorPos = Camera.main.ScreenToWorldPoint(new Vector3(offsetStartPos.x, offsetStartPos.y, Camera.main.nearClipPlane));
            cursorObject.transform.position = worldCursorPos;
            cursorObject.SetActive(true);
        }

        if (isSelecting && Input.GetMouseButton(0))
        {
            Vector2 currentPos = Input.mousePosition;
            Vector2 offsetCurrentPos = currentPos + new Vector2(0, cursorOffset); // Apply the offset in screen space

            // Update the cursor position visually with offset
            Vector3 worldCursorPos = Camera.main.ScreenToWorldPoint(new Vector3(offsetCurrentPos.x, offsetCurrentPos.y, Camera.main.nearClipPlane));
            cursorObject.transform.position = worldCursorPos;

            // Update the end index for highlighting using the offset cursor position
            endIndex = TMP_TextUtilities.GetCursorIndexFromPosition(textMeshPro, offsetCurrentPos, null);

            // Highlight text while selecting
            HighlightText(startIndex, endIndex);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;

            // Hide the cursor when selection ends
            cursorObject.SetActive(false);
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
        selected = $"<u><color=#808080AA>{selected}</color></u>";

        // Combine the text parts back together
        textMeshPro.text = before + selected + after;
    }
}
