using System.Collections;
using System.Collections.Generic;
using KModkit;
using Rnd = UnityEngine.Random;
using UnityEngine;
using Words;
using System.Linq;
using System;

public class shapeCipherScript : MonoBehaviour {

    public TextMesh[] screenTexts;
    public KMBombInfo Bomb;
    public KMBombModule module;
    public Material[] ciphers;
    public Renderer Background;
    public KMAudio Audio;
    public AudioClip[] sounds;
    public TextMesh submitText;
    public KMSelectable leftArrow;
    public KMSelectable rightArrow;
    public KMSelectable submit;
    public KMSelectable[] keyboard;
    private List<List<string>> wordList = new List<List<string>>();
    private bool isSolved;
    private int moduleId;
    private static int moduleIdCounter = 1;
    private int cipher = 0;
    private string[][] pages;
    private string answer = "";
    private int page;
    private bool submitScreen;
    private string triangleClock = "";

    void Awake()
    {
        leftArrow.OnInteract += delegate { left(leftArrow); return false; };
        rightArrow.OnInteract += delegate { right(rightArrow); return false; };
        submit.OnInteract += delegate { submitWord(submit); return false; };
        foreach (KMSelectable keybutton in keyboard)
        {
            KMSelectable pressedButton = keybutton;
            pressedButton.OnInteract += delegate { letterPress(pressedButton); return false; };
        }
    }

    void Start ()
    {
        moduleId = moduleIdCounter++;
        Data data = new Data();
        wordList = data.allWords;
        submitText.text = "1";
        //Generating random word
        answer = wordList[2][Rnd.Range(0, wordList[2].Count)].ToUpper();
        wordList[2].Remove(answer);
        Debug.LogFormat("[Shape Cipher #{0}] Generated Word: {1}", moduleId, answer);
        pages = new string[7][];
        pages[0] = new string[3];
        pages[1] = new string[3];
        pages[2] = new string[3];
        pages[3] = new string[3];
        pages[4] = new string[3];
        pages[5] = new string[3];
        pages[6] = new string[3];
        pages[0][0] = "";
        pages[0][1] = "";
        pages[0][2] = "";
        string encrypt = shapecipher(answer);
        pages[0][0] = encrypt;
        page = 0;
        getScreens();
    }
    string shapecipher(string word)
    {
        Debug.LogFormat("[Shape Cipher #{0}] Begin Square Encryption", moduleId);
        string kw = wordList[2][Rnd.Range(0, wordList[2].Count)].ToUpper();
        wordList[2].Remove(kw);
        string encrypt = Square(word.ToUpper(), kw.ToUpper());
        int list = Rnd.Range(0, 5);
        Debug.LogFormat("[Shape Cipher #{0}] Begin Triangle Encryption", moduleId);
        kw = wordList[list][Rnd.Range(0, wordList[list].Count)].ToUpper();
        wordList[list].Remove(kw);
        encrypt = Triangle(encrypt.ToUpper(), kw.ToUpper());
        list = Rnd.Range(0, 5);
        Debug.LogFormat("[Shape Cipher #{0}] Begin Circle Encryption", moduleId);
        kw = wordList[list][Rnd.Range(0, wordList[list].Count)].ToUpper();
        wordList[list].Remove(kw);
        encrypt = Circle(encrypt.ToUpper(), kw.ToUpper());
        return encrypt;
    }

    string Square(string word, string kw)
    {
        string alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ";
        string otherAlphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ";
        string[] morse = { ".-", "-...", "-.-.", "-..", ".", "..-.", "--.", "....", "..", "-.-", ".-..", "--", "-.", "---", ".--.", "--.-", ".-.", "...", "-", "..-", "...-", ".--", "-..-", "-.--", "--.." };
        pages[5][0] = morse[otherAlphabet.IndexOf(kw[0])];
        pages[5][1] = morse[otherAlphabet.IndexOf(kw[1])];
        pages[5][2] = morse[otherAlphabet.IndexOf(kw[2])];
        pages[6][0] = morse[otherAlphabet.IndexOf(kw[3])];
        pages[6][1] = morse[otherAlphabet.IndexOf(kw[4])];
        pages[6][2] = morse[otherAlphabet.IndexOf(kw[5])];
        Debug.LogFormat("[Shape Cipher #{0}] Morse-Encrypted Keyword: {1} {2} {3} {4} {5} {6}", moduleId, morse[otherAlphabet.IndexOf(kw[0])], morse[otherAlphabet.IndexOf(kw[1])], morse[otherAlphabet.IndexOf(kw[2])], morse[otherAlphabet.IndexOf(kw[3])], morse[otherAlphabet.IndexOf(kw[4])], morse[otherAlphabet.IndexOf(kw[5])]);
        List<string> mat = new List<string>();
        for (int i = 0; i < kw.Length; i++)
        {
            string elem = kw[i].ToString();
            if (!mat.Contains(elem))
            {
                mat.Add(kw[i].ToString());
            }
        }
        for (int i = 0; i < alphabet.Length; i++)
        {
            string elem = alphabet[i].ToString();
            if (!mat.Contains(elem))
            {
                mat.Add(alphabet[i].ToString());
            }
        }
        int col1 = 0;
        int row1 = 0;
        int col2 = 0;
        int row2 = 0;
        string cipherword = "";
        Debug.LogFormat("[Shape Cipher #{0}] Keyword: {1}", moduleId, kw);
        Debug.LogFormat("[Shape Cipher #{0}] Matrix:", moduleId);
        for (int i = 0; i < 5; i++)
        {
            Debug.LogFormat("[Shape Cipher #{0}] {1}{2}{3}{4}{5}", moduleId, mat[i*5], mat[i*5+1], mat[i*5+2], mat[i*5+3], mat[i*5+4]);
        }
        for (var i = 0; i < 6; i += 2)
        {
            var char1 = word[i];
            var char2 = word[i + 1];
            col1 = mat.IndexOf(word[i].ToString()) / 5;
            row1 = mat.IndexOf(word[i].ToString()) % 5;
            col2 = mat.IndexOf(word[i + 1].ToString()) / 5;
            row2 = mat.IndexOf(word[i + 1].ToString()) % 5;
            if ((col1 != col2) && (row1 != row2))
            {
                cipherword += mat[col2 * 5 + row1];
                cipherword += mat[col1 * 5 + row2];
            }
            else if ((col1 != col2) && (row1 == row2))
            {
                var differencerow = 0;
                if (row1 < 2)
                {
                    differencerow = 4 - row1;
                    row1 = differencerow;
                }
                else if (row1 > 2)
                {
                    differencerow = 4 - row1;
                    row1 = differencerow;
                }
                else
                {
                    row1 = 2;
                }
                if (row2 < 2)
                {
                    differencerow = 4 - row2;
                    row2 = differencerow;
                }
                else if (row2 > 2)
                {
                    differencerow = 4 - row2;
                    row2 = differencerow;
                }
                else
                {
                    row2 = 2;
                }
                cipherword += mat[col1 * 5 + row1];
                cipherword += mat[col2 * 5 + row2];
            }
            else if ((col1 == col2) && (row1 != row2))
            {
                var differencecol = 0;
                if (col1 < 2)
                {
                    differencecol = 4 - col1;
                    col1 = differencecol;
                }
                else if (col1 > 2)
                {
                    differencecol = 4 - col1;
                    col1 = differencecol;
                }
                else
                {
                    col1 = 2;
                }
                if (col2 < 2)
                {
                    differencecol = 4 - col2;
                    col2 = differencecol;
                }
                else if (col2 > 2)
                {
                    differencecol = 4 - col2;
                    col2 = differencecol;
                }
                else
                {
                    col2 = 2;
                }
                cipherword += mat[col1 * 5 + row1];
                cipherword += mat[col2 * 5 + row2];

            }
            else
            {
                cipherword += mat[col1 * 5 + row1];
                cipherword += mat[col2 * 5 + row2];
            }
        }
        for(int i = 0; i < 3; i++)
        {
            Debug.LogFormat("[Shape Cipher #{0}] {1}{2} -> {3}{4}", moduleId, word[i*2], word[i*2+1], cipherword[i*2], cipherword[i*2+1]);
        }
        Debug.LogFormat("[Shape Cipher #{0}] Ciphertext: {1}.", moduleId, cipherword);
        return cipherword;
    }

    string Triangle(string word, string kw)
    {
        int triangleKey = 0;
        triangleKey = Rnd.Range(1,6);
        if (triangleKey == 5)
        {
            triangleKey++;
        }
        string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        List<string> alphabetKey = new List<string>();
        List<string> alphabetKey2 = new List<string>();

        string encrypt = "";
        for (int i = 0; i < kw.Length; i++)
        {
            var elem = kw[i];
            if (!alphabetKey.Contains(elem.ToString()))
            {
                alphabetKey.Add(kw[i].ToString());
                alphabetKey2.Add(kw[i].ToString());
            }
        }
        for (int i = 0; i < alphabet.Length; i++)
        {
            var elem = alphabet[i];
            if (!alphabetKey.Contains(elem.ToString()))
            {
                alphabetKey.Add(alphabet[i].ToString());
                alphabetKey2.Add(alphabet[i].ToString());
            }
        }
        for (int i = 26; i < 36; i++)
        {
            alphabetKey[i] = alphabetKey[i - 26];
            alphabetKey2[i] = alphabetKey2[i - 26];
        }
        Debug.LogFormat("[Shape Cipher #{0}] Keyword: {1}", moduleId, kw);
        Debug.LogFormat("[Shape Cipher #{0}] String: {1}", moduleId, alphabetKey2.Join(""));
        string[] triangles = { "", "", "", "", "", "", "", "", "", "", "", "" };
        for (var p = 0; p < triangles.Length; p++)
        {
            var elem = alphabetKey[p];
            var elem2 = alphabetKey[p + triangleKey];
            var elem3 = alphabetKey[p + (triangleKey * 2)];
            var l = 0;
            while (elem == ".")
            {
                l++;
                elem = alphabetKey[p + l];
                elem2 = alphabetKey[(p + triangleKey + l) % 36];
                elem3 = alphabetKey[(p + (2 * triangleKey) + l) % 36];
            }
            var l1 = l;
            if (!triangles[p].Contains(elem))
            {
                triangles[p] += elem;
                alphabetKey[p + l1] = ".";
                triangles[p] += elem2;
                alphabetKey[(p + l1 + triangleKey) % 36] = ".";
                triangles[p] += elem3;
                alphabetKey[p + l1 + (2 * triangleKey) % 36] = ".";
            }
        }
        
        bool[] clock = { false, false, false, false, false, false, false, false, false, false, false, false };
        for (var i = 0; i < 12; i++)
        {
            int boolean = Rnd.Range(0, 2);
            if(boolean %2 == 0)
            {
                clock[i] = true;
            }
            else
            {
                clock[i] = false;
            }

        }
        if(triangles[0] == triangles[10])
        {
            clock[10] = clock[0];
        }
        if (triangles[1] == triangles[11])
        {
            clock[11] = clock[1];
        }
        
        for (var i = 0; i < clock.Length; i++)
        {
            var e = clock[i];
            if (e)
            {
                triangleClock += "↺";
            }
            else
            {
                triangleClock += "↻";
            }
        }
        for (int i = 0; i < 12; i++)
        {
            Debug.LogFormat("[Shape Cipher #{0}] Triangle {1}: {2}, Rotation: {3}", moduleId, i, triangles[i], clock[i] ? "↻" : "↺");
        }
        pages[3][0] = kw;
        pages[3][1] = triangleClock[0] + " " + triangleClock[1] + " " + triangleClock[2];
        pages[3][2] = triangleClock[3] + " " + triangleClock[4] + " " + triangleClock[5];
        pages[4][0] = triangleClock[6] + " " + triangleClock[7] + " " + triangleClock[8];
        pages[4][1] = triangleClock[9] + " " + triangleClock[10] + " " + triangleClock[11];
        pages[4][2] = triangleKey.ToString();
        for (var j = 0; j < word.Length; j++)
        {
            var chosen = 0;
            var vertex = 0;
            for (var i = 0; i < 12; i++)
            {
                if (triangles[i].Contains(word[j]))
                {
                    chosen = i;
                }
            }
            var nani = triangles[chosen];
            for (var k = 0; k < 3; k++)
            {
                if (nani[k] == word[j])
                {
                    vertex = k;
                }
            }
            bool temp = clock[chosen];
            if (temp)
            {
                encrypt += nani[(vertex + 1) % 3];
            }
            if (!temp)
            {
                encrypt += nani[(vertex - 1 + 3) % 3];
            }
        }
        List<char> cipherword = new List<char>();
        for (var i = 0; i < encrypt.Length; i++)
        {
            cipherword.Add(encrypt[i]);
        }
        encrypt = "";
        for (var i = 0; i < cipherword.Count(); i++)
        {
            encrypt += cipherword[i].ToString();
        }
        for (int i = 0; i < 6; i++)
        {
            int chosen = 0;
            for (var j = 0; j < 12; j++)
            {
                if (triangles[j].Contains(word[i]))
                {
                    chosen = j;
                }
            }
            Debug.LogFormat("[Shape Cipher #{0}] {1} {2} {3}", moduleId, word[i], clock[chosen] ? "↻" : "↺", encrypt[i]);
        }
        Debug.LogFormat("[Shape Cipher #{0}] Ciphertext: {1}", moduleId, encrypt);
        return encrypt;
    }
    string Circle(string word, string kw)
    {
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        int[] pi = { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3, 2, 3, 8, 4, 6, 2, 6, 4, 3, 3, 8, 3, 2, 7, 9, 5, 0, 2, 8, 8, 4, 1, 9, 7, 1, 6, 9, 3, 9, 9, 3, 7, 5, 1, 0, 5, 8, 2, 0, 9, 7, 4, 9, 4, 4, 5, 9, 2, 3, 0, 7, 8, 1, 6, 4, 0, 6, 2, 8, 6, 2, 0, 8, 9, 9, 8, 6, 2, 8, 0, 3, 4, 8, 2, 5, 3, 4, 2, 1, 1, 7, 0, 6, 7, 9, 8, 2, 1, 4, 8, 0, 8, 6, 5, 1, 3, 2, 8, 2, 3, 0, 6, 6, 4, 7, 0, 9, 3, 8, 4, 4, 6, 0, 9, 5, 5, 0, 5, 8, 2, 2, 3, 1, 7, 2, 5, 3, 5, 9, 4, 0, 8, 1, 2, 8, 4, 8, 1, 1, 1, 7, 4, 5, 0, 2, 8, 4, 1, 0, 2, 7, 0, 1, 9, 3, 8, 5, 2, 1, 1, 0, 5, 5, 5, 9, 6, 4, 4, 6, 2, 2, 9, 4, 8, 9, 5, 4, 9, 3, 0, 3, 8, 1, 9, 6, 4, 4, 2, 8, 8, 1, 0, 9, 7, 5, 6, 6, 5, 9, 3, 3, 4, 4, 6, 1, 2, 8, 4, 7, 5, 6, 4, 8, 2, 3, 3, 7, 8, 6, 7, 8, 3, 1, 6 };
        List<string> caesar = new List<string>();
        string ciphertext = "";
        for (int i = 0; i < kw.Length; i++)
        {
            string elem = kw[i].ToString();
            if (!caesar.Contains(elem))
            {
                caesar.Add(kw[i].ToString());
            }
        }
        for (int i = 0; i < alphabet.Length; i++)
        {
            string elem = alphabet[i].ToString();
            if (!caesar.Contains(elem))
            {
                caesar.Add(alphabet[i].ToString());
            }
        }
        Debug.LogFormat("[Shape Cipher #{0}] String: {1}", moduleId, string.Join("", caesar.ToArray()));
        int total = 0;
        List<string> numbers = new List<string>();
        List<string> loggingNumbers = new List<string>();
        for (int i = 0; i < word.Length; i++)
        {
            int letter = caesar.IndexOf(word[i].ToString());
            int pie = pi[letter + total];
            loggingNumbers.Add(pie.ToString());
            ciphertext += caesar[(letter + pie) % 26];
            total = letter + total;
            Debug.LogFormat("[Shape Cipher #{0}] Pi Position: {2} Number: {1}", moduleId, pie, total);
            numbers.Add(total.ToString());
        }
        pages[0][1] = kw;
        pages[1][0] = numbers[0].ToString();
        pages[1][1] = numbers[1].ToString();
        pages[1][2] = numbers[2].ToString();
        pages[2][0] = numbers[3].ToString();
        pages[2][1] = numbers[4].ToString();
        pages[2][2] = numbers[5].ToString();
        for(int i = 0; i < 6; i++)
        {
            Debug.LogFormat("[Shape Cipher #{0}] {1} + {2} = {3}", moduleId, word[i], loggingNumbers[i], ciphertext[i]);
        }
        Debug.LogFormat("[Shape Cipher #{0}] Ciphertext: {1}", moduleId, ciphertext);
        return ciphertext;
    }

    // Update is called once per frame
    void Update () {
        if (page == 0 || page == 1 || page == 2)
        {
            cipher = 0;
        }
        if (page == 3 || page == 4)
        {
            cipher = 1;
        }
        if (page == 5 || page == 6)
        {
            cipher = 2;
        }
        Background.material = ciphers[cipher];
	}
    int correction(int p, int max)
    {
        while (p < 0)
            p += max;
        while (p >= max)
            p -= max;
        return p;
    }
    private void getScreens()
    {
        submitText.text = (page + 1) + "";
        screenTexts[0].text = pages[page][0];
        screenTexts[1].text = pages[page][1];
        screenTexts[2].text = pages[page][2];
        if (page == 0)
        {
            screenTexts[0].fontSize = 40;
            screenTexts[1].fontSize = 35;
            screenTexts[2].fontSize = 40;
        }
        else if (page == 3)
        {
            screenTexts[0].fontSize = 35;
            screenTexts[1].fontSize = 40;
            screenTexts[2].fontSize = 40;
        }
        else
        {
            screenTexts[0].fontSize = 40;
            screenTexts[1].fontSize = 40;
            screenTexts[2].fontSize = 40;
        }
    }
    void left(KMSelectable arrow)
    {
        if (!isSolved)
        {
            Audio.PlaySoundAtTransform(sounds[0].name, transform);
            submitScreen = false;
            arrow.AddInteractionPunch();
            page--;
            page = correction(page, pages.Length);
            getScreens();
        }
    }
    void right(KMSelectable arrow)
    {
        if (!isSolved)
        {
            Audio.PlaySoundAtTransform(sounds[0].name, transform);
            submitScreen = false;
            arrow.AddInteractionPunch();
            page++;
            page = correction(page, pages.Length);
            getScreens();
        }
    }
    void submitWord(KMSelectable submitButton)
    {
        if (!isSolved)
        {
            submitButton.AddInteractionPunch();
            if (screenTexts[2].text.Equals(answer))
            {
                if (!isSolved)
                {
                    Audio.PlaySoundAtTransform(sounds[2].name, transform);
                    module.HandlePass();
                    page = 69;
                    cipher = 3;
                    isSolved = true;
                    screenTexts[2].text = "";
                    StartCoroutine(SolveAnim());
                }
            }
            else
            {
                Audio.PlaySoundAtTransform(sounds[3].name, transform);
                module.HandleStrike();
                page = 0;
                getScreens();
                submitScreen = false;
            }
        }
    }
    void letterPress(KMSelectable pressed)
    {
        if (!isSolved)
        {
            pressed.AddInteractionPunch();
            Audio.PlaySoundAtTransform(sounds[1].name, transform);
            if (submitScreen)
            {
                if (screenTexts[2].text.Length < 6)
                {
                    screenTexts[2].text = screenTexts[2].text + "" + pressed.GetComponentInChildren<TextMesh>().text;
                }
            }
            else
            {
                submitText.text = "SUB";
                screenTexts[0].text = "";
                screenTexts[1].text = "";
                screenTexts[2].text = pressed.GetComponentInChildren<TextMesh>().text;
                screenTexts[2].fontSize = 40;
                submitScreen = true;
            }
        }
    }

    IEnumerator SolveAnim()
    {
        cipher = 0;
        yield return new WaitForSecondsRealtime(1.0f);
        cipher = 1;
        yield return new WaitForSecondsRealtime(1.0f);
        cipher = 2;
        yield return new WaitForSecondsRealtime(1.0f);
        cipher = 3;
        isSolved = true;
        yield return new WaitForSecondsRealtime(1.0f);
    }
#pragma warning disable 414
    private string TwitchHelpMessage = "Move to other screens using !{0} right|left|r|l|. Submit the decrypted word with !{0} submit qwertyuiopasdfghjklzxcvbnm";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {

        if (command.EqualsIgnoreCase("right") || command.EqualsIgnoreCase("r"))
        {
            yield return null;
            rightArrow.OnInteract();
            yield return new WaitForSeconds(0.1f);

        }
        if (command.EqualsIgnoreCase("left") || command.EqualsIgnoreCase("l"))
        {
            yield return null;
            leftArrow.OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        string[] split = command.ToUpperInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        if (split.Length != 2 || !split[0].Equals("SUBMIT") || split[1].Length != 6) yield break;
        int[] buttons = split[1].Select(getPositionFromChar).ToArray();
        if (buttons.Any(x => x < 0)) yield break;

        yield return null;

        yield return new WaitForSeconds(0.1f);
        foreach (char let in split[1])
        {
            keyboard[getPositionFromChar(let)].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
        submit.OnInteract();
        yield return new WaitForSeconds(0.1f);
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        if (submitScreen && !answer.StartsWith(screenTexts[2].text))
        {
            KMSelectable[] arrows = new KMSelectable[] { leftArrow, rightArrow };
            arrows[UnityEngine.Random.Range(0, 2)].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        int start = submitScreen ? screenTexts[2].text.Length : 0;
        for (int i = start; i < 6; i++)
        {
            keyboard[getPositionFromChar(answer[i])].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        submit.OnInteract();
        yield return new WaitForSeconds(0.1f);
    }
    private int getPositionFromChar(char c)
    {
        return "QWERTYUIOPASDFGHJKLZXCVBNM".IndexOf(c);
    }
}
