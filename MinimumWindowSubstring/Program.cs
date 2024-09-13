string s = "ADOBECODEBANC";
string t = "ABC";

Console.WriteLine($"Bruteforce result: {MinWindowBruteForce(s, t)}");

Console.WriteLine($"Hashmap result: {MinWindow(s, t)}");

string MinWindowBruteForce(string s, string t)
{
    if (string.IsNullOrEmpty(t)) return string.Empty;

    long minLength = long.MaxValue;
    string minWindow = string.Empty;

    // Bruteforce way: iterate through both strings and compare each character
    for (int i = 0; i < s.Length; i++)
    {
        for (int j = i; j < s.Length; j++)
        {
            string substring = s.Substring(i, j - i + 1);
            if (ContainsAllCharacters(substring, t) &&
            substring.Length < minLength)
            { // We've got a new "lowscore"
                minLength = substring.Length;
                minWindow = substring;
            }
        }
    }

    return minWindow;
}

bool ContainsAllCharacters(string s, string t)
{
    // Dictionary containing the characters we need and their count to form a valid substring
    var charCount = new Dictionary<char, int>();

    foreach (char c in t)
    {
        if (!charCount.ContainsKey(c))
        {
            charCount[c] = 0;
        }
        charCount[c]++;
    }

    // Iterate through string t -- our substring to check if we have all required characters. 
    foreach (char c in s)
    {
        if (charCount.ContainsKey(c))
        {
            charCount[c]--;
            if (charCount[c] == 0)
            {
                charCount.Remove(c); // Remove this character from the dictionary as we have satisifed our need for this character to help contribute to forming a valid substring
            }
        }
    }

    return charCount.Count == 0; // If all counts are 0, we've got a viable candidate, else we've not got a matching substring
}

string MinWindow(string s, string t)
{
    // Resources used:
    // https://www.youtube.com/watch?v=jSto0O4AJbM
    // https://leetcodethehardway.com/tutorials/basic-topics/sliding-window
    // https://medium.com/@shanemarchan/demystifying-sliding-window-through-leetcode-exercises-eb01e4b8c495

    if (string.IsNullOrEmpty(t)) return string.Empty;

    // Hashmap containing the characters we need: char, count
    // We need to consider the scenario where multiple values exist for same key 
    var needHashMap = new Dictionary<char, int>();

    // Add each character of the substring t to hashmap 
    for (int i = 0; i < t.Length; i++)
    {
        AddCharToDictionary(t[i], needHashMap);
    }

    // Hashmap containing the characters we have: char, count
    var haveHashMap = new Dictionary<char, int>();

    // So at this stage we have the characters we need in the needHashMap, let's track the number of characters we have vs the number we need
    int charactersWeNeed = needHashMap.Count;
    int charactersWeHave = 0;
    int left = 0;

    // Keep track of the result for the shortest string: give it a silly high value because it's likely to get shorter from there when we find a viable candidate
    long shortestResult = long.MaxValue;

    // Keep track of the shortest string result too
    int[]? result = [-1, -1];

    // Now we need to go through what characters we actually have in s and add them to the hash map
    for (int i = 0; i < s.Length; i++)
    {
        // Add to have hashMap
        AddCharToDictionary(s[i], haveHashMap);

        // Now we need to check if we have all of the characters we need

        // So if the need hash map has a key we have, what do we want to do?
        // Here we can perhaps track the number of characters we have vs the number we need 

        if (needHashMap.ContainsKey(s[i]) && haveHashMap[s[i]] == needHashMap[s[i]])
        {
            charactersWeHave++;
        }

        // So now we want to keep track of the sliding window count and find the shortest string
        while (charactersWeNeed == charactersWeHave)
        {
            // We have a result candidate, it might not be the shortest one but it's worthy of comparison

            // Is the latest result shorter than the one we currently have? Let's keep track
            var windowSize = i - left + 1;

            if (windowSize < shortestResult)
            {
                // Update the result to the current window's boundaries
                result = [left, i];

                // Update the length of the smallest window found
                shortestResult = windowSize;
            }

            // Remove the character at the left boundary of our window: i.e. we now have one character less:
            // Make our window as small as possible whilst keeping the same number of characters we have and need
            haveHashMap[s[left]]--;

            if (needHashMap.ContainsKey(s[left]) && haveHashMap[s[left]] < needHashMap[s[left]])
            {
                charactersWeHave--;
            }

            left++;
        }

    }

    return shortestResult == long.MaxValue
       ? string.Empty
       : s.Substring(result[0], result[1] - result[0] + 1);
}

void AddCharToDictionary(char characterToAdd, IDictionary<char, int> dict)
{
    if (dict.ContainsKey(characterToAdd))
    {
        dict[characterToAdd]++; //if it does increment the value to this key as this is the count
    }
    else
    {
        dict.Add(characterToAdd, 1); // New value
    }
}