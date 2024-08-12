using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class SimpleHighlightManager : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public GameObject cursorObject;  // Reference to the GameObject with a SpriteRenderer for the cursor

    private int startIndex;
    private int endIndex;
    private bool isSelecting;
    private string filename;
    private string originalText;

    void Start()
    {
        // Store the original text without any markup
        originalText = textMeshPro.text;

        // Generate a unique filename using the current timestamp
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        filename = Application.dataPath + $"/highlighted_text_{timestamp}.csv";

        // Initialize the CSV header
        File.WriteAllText(filename, "HighlightedText\n");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 startPos = Input.mousePosition;
            Vector2 offsetStartPos = startPos;  // Apply offset if needed

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
            Vector2 offsetCurrentPos = currentPos;  // Apply offset if needed

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

        // Extract the highlighted text from the original text
        string highlightedText = originalText.Substring(start, end - start);
        string cleanHighlightedText = highlightedText.Trim();

        // Debug log for extracted text
        Debug.Log($"Highlighted Text: {highlightedText}");

        // Write highlighted text to CSV
        WriteToCSV(highlightedText);
    }

    void WriteToCSV(string highlightedText)
    {
        // Append the highlighted text to the CSV file
        using (TextWriter tw = new StreamWriter(filename, true))
        {
            tw.WriteLine(highlightedText);
        }
    }
}
