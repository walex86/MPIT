using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(GameScript))]
public class QuizEditor : Editor
{

    public static GameScript gs;
    private bool developerMode;
    private int QAddCount = 1;

    private void OnEnable()
    {
        gs = target as GameScript;
    }

    public override void OnInspectorGUI()
    {
        /*var devbttn = GUILayout.Button("Включить режим разработчика");
        if (devbttn && !developerMode) developerMode = true;
        else if (devbttn && developerMode) developerMode = false;
        if (developerMode) DrawDefaultInspector();
        EditorGUILayout.Space();*/
        if (GUILayout.Button("Редактировать вопросы", GUILayout.Height(50))) QuestionWindow.ShowWindow();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal("box");
        QAddCount = EditorGUILayout.IntField("Добавление вопросов", QAddCount);
        if (QAddCount > 0)
        {
            if (GUILayout.Button("Добавить вопрос: " + (gs.Questions.Length + 1) + "-" + (gs.Questions.Length + QAddCount)))
            {
                List<object> QList = new List<object>(gs.Questions);
                gs.Questions = new QuestionsList[QList.Count + QAddCount];
                for (int i = 0; i < QList.Count; i++) gs.Questions[i] = QList[i] as QuestionsList;
                if (QAddCount <= 1) Debug.Log(gs.Questions.Length + " - вопрос, был успешно добавлен!");
                else Debug.Log(QAddCount + " - вопросов, было успешно добавлено!");
            }
        }
        else if (QAddCount <= 0) EditorGUILayout.HelpBox("Минимальное кол-во для добавления: 1.", MessageType.Error);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.BeginVertical();
        gs.publicTimeCount = EditorGUILayout.IntField("Время для ответа: ", gs.publicTimeCount);
        gs.multiplierScore = EditorGUILayout.IntField("Множитель для счёта: ", gs.multiplierScore);
        EditorGUILayout.Space();
        gs.trueCC = EditorGUILayout.ColorField("Цвет при правильном ответе", gs.trueCC);
        gs.falseCC = EditorGUILayout.ColorField("Цвет при неправильном ответе", gs.falseCC);
        gs.defaultCC = EditorGUILayout.ColorField("Стандартный цвет шапки", gs.defaultCC);
        if (!Application.isPlaying) gs.headPanel.color = new Color(gs.defaultCC.r, gs.defaultCC.g, gs.defaultCC.b);
        gs.bg.color = EditorGUILayout.ColorField("Цвет заднего фона", gs.bg.color);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
}
public class QuestionWindow : EditorWindow
{
    Vector2 scrollPos;

    public static void ShowWindow()
    {
        QuestionWindow qw = (QuestionWindow)GetWindow(typeof(QuestionWindow));
        qw.maxSize = new Vector2(850, 500); qw.minSize = new Vector2(850, 500);
        qw.Show();
    }
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(850), GUILayout.Height(500));
        if (QuizEditor.gs.Questions.Length > 0)
        {
            for (int i = 0; i < QuizEditor.gs.Questions.Length; i++)
            {
                QuizEditor.gs.Questions[i].Question = EditorGUILayout.TextField("Вопрос: " + (i + 1), QuizEditor.gs.Questions[i].Question);
                QuizEditor.gs.Questions[i].answers[0] = EditorGUILayout.TextField("Правильный ответ: ", QuizEditor.gs.Questions[i].answers[0]);
                for (int a = 1; a < QuizEditor.gs.Questions[i].answers.Length; a++) QuizEditor.gs.Questions[i].answers[a] = EditorGUILayout.TextField("Неправильный ответ: ", QuizEditor.gs.Questions[i].answers[a]);
                if (GUILayout.Button("Удалить вопрос: " + (i + 1), GUILayout.Height(25)))
                {
                    List<object> Qlist = new List<object>(QuizEditor.gs.Questions);
                    Qlist.RemoveAt(i);
                    QuizEditor.gs.Questions = new QuestionsList[Qlist.Count];
                    for (int a = 0; a < Qlist.Count; a++) QuizEditor.gs.Questions[a] = Qlist[a] as QuestionsList;
                }
                EditorGUILayout.Space();
            }
        }
        else EditorGUILayout.HelpBox("У вас отсутствуют вопросы, добавьте их, прежде чем редактировать.", MessageType.Info);
        Repaint();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}