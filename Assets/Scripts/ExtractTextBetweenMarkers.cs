using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class DisplayTextBetweenTags : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public string csvFileName = "extractedText.csv"; // Default file name
    public List<string> targetWords = new List<string>(); // List of target words
    public enum CursorType { Direct, Offset }
    public CursorType cursorType;
    public string participantNumber = "000"; // Default participant number

    private string previousText = "";
    private float? lastHighlightTime = null;
    private float currentHighlightStartTime;
    private float? lastCorrectHighlightTime = null;

    void Start()
    {
        // Write headers to the CSV file
        WriteHeadersToCSV();
    }

    void Update()
    {
        if (textMeshPro != null)
        {
            string currentText = textMeshPro.text;

            // Check if the text has changed
            if (currentText != previousText)
            {
                // Extract the text between <u> and </u> tags
                string extractedText = ExtractTextBetweenTags(currentText, "<u>", "</u>");

                // Clean the extracted text to remove <color=#808080AA> tags
                string cleanedText = RemoveColorTags(extractedText);

                // Display the cleaned text in the console
                if (!string.IsNullOrEmpty(cleanedText))
                {
                    Debug.Log(cleanedText);

                    // Track time for highlighting
                    if (lastHighlightTime.HasValue)
                    {
                        float timeTaken = Time.time - currentHighlightStartTime;
                        float timeBetweenHighlights = lastCorrectHighlightTime.HasValue ? Time.time - lastCorrectHighlightTime.Value : 0;
                        bool isCorrect = IsTargetWord(cleanedText);

                        // Log the results to CSV
                        LogResults(cleanedText, timeTaken, timeBetweenHighlights, isCorrect);

                        // Update the time of the last highlight
                        lastHighlightTime = Time.time;

                        // Update the time of the last correct highlight if the current highlight is correct
                        if (isCorrect)
                        {
                            lastCorrectHighlightTime = Time.time;
                        }
                    }
                    else
                    {
                        // Start tracking time for the first highlight
                        currentHighlightStartTime = Time.time;
                        lastHighlightTime = Time.time;
                    }

                    // Check if the cleaned text matches any target words
                    if (IsTargetWord(cleanedText))
                    {
                        // Write the cleaned text to a CSV file
                        WriteTextToCSV(cleanedText);
                    }
                }

                // Update the previousText to the current text
                previousText = currentText;
            }
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component is not assigned.");
        }
    }

    string ExtractTextBetweenTags(string inputText, string startTag, string endTag)
    {
        int startIndex = inputText.IndexOf(startTag);
        if (startIndex != -1)
        {
            startIndex += startTag.Length;
            int endIndex = inputText.IndexOf(endTag, startIndex);
            if (endIndex != -1)
            {
                return inputText.Substring(startIndex, endIndex - startIndex);
            }
        }
        return string.Empty;
    }

    string RemoveColorTags(string text)
    {
        // Regex to remove <color=#808080AA> and </color> tags
        string pattern = @"<color=#808080AA>|</color>";
        return System.Text.RegularExpressions.Regex.Replace(text, pattern, string.Empty);
    }

    bool IsTargetWord(string text)
    {
        // Check if the cleaned text matches any of the target words
        foreach (string targetWord in targetWords)
        {
            if (text.Equals(targetWord, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    void LogResults(string text, float timeTaken, float timeBetweenHighlights, bool isCorrect)
    {
        string filePath = Path.Combine(Application.dataPath, csvFileName);

        using (StreamWriter writer = new StreamWriter(filePath, true)) // Append mode
        {
            writer.WriteLine($"{text},{timeTaken},{timeBetweenHighlights},{isCorrect},{cursorType},{participantNumber}");
        }

        Debug.Log($"Results logged to CSV file: {filePath}");
    }

    void WriteTextToCSV(string text)
    {
        string filePath = Path.Combine(Application.dataPath, csvFileName);

        using (StreamWriter writer = new StreamWriter(filePath, true)) // Append mode
        {
            writer.WriteLine(text);
        }

        Debug.Log($"Text written to CSV file: {filePath}");
    }

    void WriteHeadersToCSV()
    {
        string filePath = Path.Combine(Application.dataPath, csvFileName);

        if (!File.Exists(filePath))
        {
            using (StreamWriter writer = new StreamWriter(filePath, false)) // Overwrite mode
            {
                writer.WriteLine("Text,TimeToHighlight,TimeBetweenCorrectHighlights,IsCorrect,CursorType,ParticipantNumber");
            }
            Debug.Log($"Headers written to CSV file: {filePath}");
        }
    }
}
