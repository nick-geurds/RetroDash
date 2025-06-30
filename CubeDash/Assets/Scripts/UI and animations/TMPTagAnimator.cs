using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPTagAnimator : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public Color color1 = Color.red;
    public Color color2 = Color.blue;

    [TextArea(3, 10)]
    public string rawText = "Reverses the {wave}gravity{/wave}";

    private TMP_TextInfo textInfo;
    private Vector3[][] originalVertices;

    private float time;

    [Header("Wave Animation Settings")]
    public float waveSpeed = 5f;       // Hoe snel de golf beweegt
    public float waveFrequency = 0.5f; // Hoeveel golfbewegingen per letter
    public float waveAmplitude = 5f;   // Hoogte van de golf in pixels

    [Header("Shake Animation Settings")]
    public float shakeSpeed = 50f;     // Hoe snel de shake
    public float shakeIntensity = 1.5f; // Hoe ver de shake gaat

    // Struct om een tag-range te bewaren
    private struct TagRange
    {
        public string tagName;
        public int startIndex; // Inclusief
        public int endIndex;   // Inclusief

        public TagRange(string tagName, int startIndex, int endIndex)
        {
            this.tagName = tagName;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }
    }

    private List<TagRange> tagRanges = new();

    void Start()
    {
        if (tmpText == null)
            tmpText = GetComponent<TextMeshProUGUI>();

        // Als rawText leeg is, gebruik de tekst uit het TextMeshPro-component
        if (string.IsNullOrWhiteSpace(rawText))
        {
            rawText = tmpText.text;
        }

        string processedText = ParseTags(rawText);

        tmpText.text = processedText;
        tmpText.ForceMeshUpdate();

        CacheOriginalVertices();
    }


    void Update()
    {
        AnimateText();
    }

    string ParseTags(string text)
    {
        tagRanges.Clear();

        var output = new System.Text.StringBuilder();
        var stack = new Stack<(string tagName, int startPos)>();

        int outputIndex = 0; // positie in de nieuwe tekst (zonder animatie-tags)
        for (int i = 0; i < text.Length;)
        {
            if (text[i] == '{')
            {
                int closeIndex = text.IndexOf('}', i);
                if (closeIndex == -1)
                {
                    output.Append(text[i]);
                    i++;
                    outputIndex++;
                    continue;
                }

                string tagContent = text.Substring(i + 1, closeIndex - i - 1).ToLower();

                // Tags die animatie triggeren (wave, shake)
                bool isAnimationTag = tagContent == "wave" || tagContent == "/wave" ||
                                      tagContent == "shake" || tagContent == "/shake";

                if (isAnimationTag)
                {
                    if (!tagContent.StartsWith("/"))
                    {
                        // open animatie-tag
                        stack.Push((tagContent, outputIndex));
                    }
                    else
                    {
                        // sluit animatie-tag
                        string closeTag = tagContent.Substring(1);
                        if (stack.Count > 0 && stack.Peek().tagName == closeTag)
                        {
                            var openTag = stack.Pop();
                            tagRanges.Add(new TagRange(closeTag, openTag.startPos, outputIndex - 1));
                        }
                        else
                        {
                            Debug.LogWarning($"Tag mismatch of closing tag zonder open: {closeTag}");
                        }
                    }
                    i = closeIndex + 1; // Animatie-tags worden niet toegevoegd aan output
                }
                else
                {
                    // Kleur/stijl tags: voeg ze wél toe maar zet ze om in TMP tags
                    string replacedTag = tagContent switch
                    {
                        "color1" => $"<color=#{ColorUtility.ToHtmlStringRGBA(color1)}>",
                        "/color1" => "</color>",
                        "color2" => $"<color=#{ColorUtility.ToHtmlStringRGBA(color2)}>",
                        "/color2" => "</color>",
                        "bold" => "<b>",
                        "/bold" => "</b>",
                        "italic" => "<i>",
                        "/italic" => "</i>",
                        _ => "{" + tagContent + "}", // onbekende tag gewoon erbij zetten
                    };

                    output.Append(replacedTag);
                    i = closeIndex + 1;
                    // outputIndex wordt NIET verhoogd, want tags tellen niet als letter in de uiteindelijke tekst
                }
            }
            else
            {
                output.Append(text[i]);
                i++;
                outputIndex++;
            }
        }

        if (stack.Count > 0)
        {
            Debug.LogWarning("Niet gesloten animatie-tags in tekst!");
        }

        return output.ToString();
    }

    void CacheOriginalVertices()
    {
        textInfo = tmpText.textInfo;
        originalVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            originalVertices[i] = new Vector3[textInfo.meshInfo[i].vertices.Length];
        }
        CopyVertices();
    }

    void CopyVertices()
    {
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var vertices = textInfo.meshInfo[i].vertices;
            for (int j = 0; j < vertices.Length; j++)
            {
                originalVertices[i][j] = vertices[j];
            }
        }
    }

    void AnimateText()
    {
        tmpText.ForceMeshUpdate();
        textInfo = tmpText.textInfo;

        CopyVertices();

        //  Gebruik unscaled tijd zodat animatie doorgaat bij pauze
        time += Time.unscaledDeltaTime;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;

            foreach (var tagRange in tagRanges)
            {
                if (i >= tagRange.startIndex && i <= tagRange.endIndex)
                {
                    if (tagRange.tagName == "wave")
                    {
                        float wave = Mathf.Sin(time * waveSpeed + i * waveFrequency) * waveAmplitude;
                        for (int j = 0; j < 4; j++)
                        {
                            vertices[vertexIndex + j] = originalVertices[meshIndex][vertexIndex + j] + new Vector3(0, wave, 0);
                        }
                    }
                    else if (tagRange.tagName == "shake")
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            float offsetX = Mathf.Sin(time * shakeSpeed + i * 10 + j) * shakeIntensity;
                            float offsetY = Mathf.Cos(time * shakeSpeed + i * 10 + j) * shakeIntensity;
                            vertices[vertexIndex + j] = originalVertices[meshIndex][vertexIndex + j] + new Vector3(offsetX, offsetY, 0);
                        }
                    }
                }
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            tmpText.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    public void SetAnimatedText(string newRawText)
    {
        rawText = newRawText;
        string processedText = ParseTags(rawText);
        tmpText.text = processedText;
        tmpText.ForceMeshUpdate();
        CacheOriginalVertices();
    }
}