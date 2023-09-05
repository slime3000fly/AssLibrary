namespace AssLibrary;
/// <summary>
/// Library for processing .ass subtitle files.
/// Allows reading, modifying, and writing subtitles to a new file.
/// </summary>
public class AssLibrary
{
    private string pathToFile = "test.ass"; // Path to the .ass file
    public List<string> text = new List<string>(); // List to store subtitle content
    private string error = ""; // Error message

    /// <summary>
    /// Constructor for the AssLibrary class.
    /// </summary>
    /// <param name="path">The path to the .ass file to be processed.</param>
    public AssLibrary(string path)
    {
        pathToFile = path;
    }

    /// <summary>
    /// Reads subtitles from the .ass file and stores them in the list.
    /// </summary>
    public void GetSubtitles()
    {
        try
        {
            using (StreamReader sr = new StreamReader(pathToFile))
            {
                string? line = null;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("Dialogue:", StringComparison.OrdinalIgnoreCase))
                    {
                        // Split the line into parts using a comma.
                        List<string> parts = line.Split(',').ToList();

                        // Check if there are at least 10 parts (from 0 to 9).
                        if (parts.Count() >= 10)
                        {
                            if (parts.Count() > 10)
                            {
                                parts[9] = JoinParts(parts);
                            }
                            // Add the 10th part (index 9) to the list.
                            text.Add(parts[9]);
                        }
                    }
                }
            }
        }
        catch (FileNotFoundException)
        {
            error = "File doesn't exist";
            WriteError(error);
        }
        catch (Exception ex)
        {
            WriteError(ex.Message);
        }
    }

    /// <summary>
    /// Writes modified subtitles to a new file.
    /// </summary>
    /// <param name="output">The path to the new output file.</param>
    public void WriteFile(string output)
    {
        string[]? content = null;
        int index = 0;

        try
        {
            content = File.ReadAllLines(pathToFile);
        }
        catch (Exception ex)
        {
            WriteError(ex.Message);
        }

        if (File.Exists(output)) // check if file exist
        {
            File.Delete(output); // delete file
        }

        if (content != null)
        {
            for (int i = 0; i < content.Length; i++)
            {
                string line = content[i];
                if (line.StartsWith("Dialogue:", StringComparison.OrdinalIgnoreCase))
                {
                    List<string> parts = line.Split(',').ToList();
                    if (parts.Count >= 10)
                    {
                        if (parts.Count > 10)
                        {
                            parts[9] = JoinParts(parts);
                        }
                        parts[9] = text[index];
                        index += 1;

                        // Join parts into 1 line again
                        line = string.Join(",", parts);
                    }
                }

                //create file
                try
                {
                    using (StreamWriter writer = new StreamWriter(output, true))
                    {
                        // Możesz teraz pisać do pliku, np.:
                        writer.WriteLine(line);
                    }
                }
                catch (Exception ex)
                {
                    WriteError(ex.Message);
                }
            }
        }

    }

    /// <summary>
    /// Joins elements of a list starting from index 9 into a single string.
    /// </summary>
    /// <param name="parts">The list of parts to be joined.</param>
    /// <returns>The joined string.</returns>
    private string JoinParts(List<string> parts)
    {
        List<string> tmp = parts.Skip(9).ToList();
        string output = string.Join(",", tmp);
        if (parts.Count > 10)
        {
            parts.RemoveRange(10, parts.Count - 10);
        }
        return output;
    }

    /// <summary>
    /// Writes an error message to the console.
    /// </summary>
    /// <param name="error">The error message to be displayed.</param>
    private void WriteError(string error)
    {
        Console.WriteLine($"Error: {error}");
    }
}

