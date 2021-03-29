using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameScript : MonoBehaviour {

    [HideInInspector]
    public QuestionsList[] Questions;
    [HideInInspector]
    public int publicTimeCount = 20;
    [HideInInspector]
    public Color trueCC, falseCC,defaultCC;
    [HideInInspector]
    public int multiplierScore = 100;

    public Text questionText;
    public Button[] answerBttns = new Button[3];
    public Text[] answersText = new Text[3];
    public GameObject[] answersIcons; // 0 - trueIcon; 1 - falseIcon;
    public Image headPanel;
    public GameObject exitPanel;
    public GameObject finalText;
    public Text time;
    public Text recordText;
    public Text scoreText;
    private int timeCount = 20;
    private int score;
    private float scoreForRecord;
    private int currentQ = 1;
    private bool answerClicked;
    public Texture2D editorImg;
    public Image bg;

    private bool trueColor, falseColor,defaultColor;
    private int randQ;
    private List<object> qList;
    private QuestionsList crntQ;

    void Update ()
    {
        scoreText.text = string.Format("Ваш счёт: {0:0}", score);
        scoreForRecord = Mathf.Lerp(scoreForRecord, PlayerPrefs.GetInt("score"), 6 * Time.deltaTime);
        recordText.text = string.Format("Ваш рекорд: {0:0}", scoreForRecord);
        if (defaultColor) headPanel.color = Color.Lerp(headPanel.color, defaultCC, 8 * Time.deltaTime);
        else if (trueColor) headPanel.color = Color.Lerp(headPanel.color, trueCC, 8 * Time.deltaTime);
        else if (falseColor) headPanel.color = Color.Lerp(headPanel.color, falseCC, 8 * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Escape) && !exitPanel.activeSelf) { exitPanel.SetActive(true); Time.timeScale = 0; }
        else if (Input.GetKeyDown(KeyCode.Escape) && exitPanel.activeSelf) { exitPanel.SetActive(false); Time.timeScale = 1; }
    }

    public void playBttn()
    {
        qList = new List<object>(Questions);
        generateQuestion();
        headPanel.GetComponent<Animation>().Play("HeadAnim");
        score = 0;
        finalText.SetActive(false);
    }
    void generateQuestion()
    {
        if (qList.Count > 0)
        {
            if (scoreText.gameObject.activeSelf) scoreText.GetComponent<Animation>().Play("Bubble_Close_3");
            randQ = Random.Range(0, qList.Count);
            crntQ = qList[randQ] as QuestionsList;
            if (crntQ != null)
            {
                questionText.text = crntQ.Question;
                questionText.GetComponent<Animation>().Play("Bubble_Open_1");
                List<string> answers = new List<string>(crntQ.answers);
                for (int i = 0; i < crntQ.answers.Length; i++)
                {
                    int randA = Random.Range(0, answers.Count);
                    answersText[i].text = answers[randA];
                    answers.RemoveAt(randA);
                }
            }
            StartCoroutine(answersBttnsInAnim());
            timeCount = publicTimeCount;
            currentQ++;
        }
        else StartCoroutine(final());
    }
    public void answerBttn(int index)
    {
        answerClicked = true;
        StartCoroutine(trueOrFalse(answersText[index].text == crntQ.answers[0]));
    }
    IEnumerator final()
    {
        finalText.SetActive(true);
        yield return new WaitForSeconds(2);
        trueColor = false;
        defaultColor = true;
        headPanel.GetComponent<Animation>().Play("HeadAnimOut");
        scoreText.GetComponent<Animation>().Play("Bubble_Close_3");
        finalText.GetComponent<Animation>().Play("Bubble_Close_3");
        if (score > PlayerPrefs.GetInt("score")) PlayerPrefs.SetInt("score", score);
    }
    IEnumerator timer()
    {
        answerClicked = false;
        if (!time.gameObject.activeSelf) time.gameObject.SetActive(true);
        else time.GetComponent<Animation>().Play("Bubble_Open_3");
        while (timeCount > -1)
        {
            if (!answerClicked)
            {
                time.text = timeCount.ToString();
                timeCount--;
                yield return new WaitForSeconds(1);
            }
            else yield break;
        }
        foreach (Button t in answerBttns) t.interactable = false;
        if (!answerClicked) StartCoroutine(timeOut());
    }
    IEnumerator answersBttnsInAnim()
    {
        foreach (Button t in answerBttns) t.interactable = false;
        int i = 0;
        yield return new WaitForSeconds(1);
        while (i < answerBttns.Length)
        {
            if (!answerBttns[i].gameObject.activeSelf) answerBttns[i].gameObject.SetActive(true);
            else answerBttns[i].GetComponent<Animation>().Play("Bubble_Open_2");
            i++;
            yield return new WaitForSeconds(1);
        }
        foreach (Button t in answerBttns) t.interactable = true;
        yield return StartCoroutine(timer());
    }
    IEnumerator timeOut()
    {
        foreach (Button t in answerBttns) t.GetComponent<Animation>().Play("Bubble_Close_2");
        falseColor = true;
        yield return new WaitForSeconds(0.5f);
        if (!answersIcons[2].activeSelf) answersIcons[2].SetActive(true);
        else answersIcons[2].GetComponent<Animation>().Play("Bubble_Open_3");
        questionText.GetComponent<Animation>().Play("Bubble_Close_1");
        yield return new WaitForSeconds(0.5f);
        if (!scoreText.gameObject.activeSelf) scoreText.gameObject.SetActive(true);
        else scoreText.GetComponent<Animation>().Play("Bubble_Open_3");
        yield return new WaitForSeconds(2);
        answersIcons[2].GetComponent<Animation>().Play("Bubble_Close_3");
        time.GetComponent<Animation>().Play("Bubble_Close_3");
        falseColor = false;
        defaultColor = true;
        headPanel.GetComponent<Animation>().Play("HeadAnimOut");
        if (score > PlayerPrefs.GetInt("score")) PlayerPrefs.SetInt("score", score);
    }
    IEnumerator trueOrFalse(bool check)
    {
        defaultColor = false;
        foreach (Button t in answerBttns) t.interactable = false;
        yield return new WaitForSeconds(1);
        if (check)
        {
            score = score + (timeCount * multiplierScore) + 1;
            foreach (Button t in answerBttns) t.GetComponent<Animation>().Play("Bubble_Close_2");
            trueColor = true;
            yield return new WaitForSeconds(0.5f);
            if (!answersIcons[0].activeSelf) answersIcons[0].SetActive(true);
            else answersIcons[0].GetComponent<Animation>().Play("Bubble_Open_3");
            questionText.GetComponent<Animation>().Play("Bubble_Close_1");
            yield return new WaitForSeconds(0.5f);
            time.GetComponent<Animation>().Play("Bubble_Close_3");
            qList.RemoveAt(randQ);
            if (!scoreText.gameObject.activeSelf) scoreText.gameObject.SetActive(true);
            else scoreText.GetComponent<Animation>().Play("Bubble_Open_3");
            yield return new WaitForSeconds(1);
            answersIcons[0].GetComponent<Animation>().Play("Bubble_Close_3");
            trueColor = false;
            defaultColor = true;
            generateQuestion();
        }
        else
        {
            foreach (Button t in answerBttns) t.GetComponent<Animation>().Play("Bubble_Close_2");
            falseColor = true;
            yield return new WaitForSeconds(0.5f);
            if (!answersIcons[1].activeSelf) answersIcons[1].SetActive(true);
            else answersIcons[1].GetComponent<Animation>().Play("Bubble_Open_3");
            questionText.GetComponent<Animation>().Play("Bubble_Close_1");
            yield return new WaitForSeconds(0.5f);
            if (!scoreText.gameObject.activeSelf) scoreText.gameObject.SetActive(true);
            else scoreText.GetComponent<Animation>().Play("Bubble_Open_3");
            yield return new WaitForSeconds(1);
            answersIcons[1].GetComponent<Animation>().Play("Bubble_Close_3");
            time.GetComponent<Animation>().Play("Bubble_Close_3");
            falseColor = false;
            defaultColor = true;
            headPanel.GetComponent<Animation>().Play("HeadAnimOut");
            scoreText.GetComponent<Animation>().Play("Bubble_Close_3");
            if (score > PlayerPrefs.GetInt("score")) PlayerPrefs.SetInt("score", score);
            yield return new WaitForSeconds(1.5f);
            scoreText.gameObject.SetActive(false);
        }
    }
    public void exitPan(int bttn)
    {
        if (bttn == 0)
        {
            if (score > PlayerPrefs.GetInt("score")) PlayerPrefs.SetInt("score", score);
            Application.Quit();
        }
        else { exitPanel.SetActive(false); Time.timeScale = 1; }
    }
}

[System.Serializable]
public class QuestionsList
{
    public string Question;
    public string[] answers = new string[3];
}