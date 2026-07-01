#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ZillRunner.Core;
using ZillRunner.UI;

namespace ZillRunner.Editor
{
    public static class GameSetupEditor
    {
        [MenuItem("ZILL Runner/Setup Game Scene")]
        public static void SetupGameScene()
        {
            if (Object.FindObjectOfType<GameBootstrap>() == null)
            {
                var bootstrap = new GameObject("GameBootstrap");
                bootstrap.AddComponent<GameBootstrap>();
            }

            var bootstrapComp = Object.FindObjectOfType<GameBootstrap>();
            bootstrapComp.BuildScene();
            BuildUI();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("ZILL Runner scene setup complete. Press Play!");
        }

        private static void BuildUI()
        {
            if (Object.FindObjectOfType<UIManager>() == null)
            {
                var canvasGo = new GameObject("Canvas");
                var canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasGo.AddComponent<GraphicRaycaster>();
                canvasGo.AddComponent<UIManager>();
            }

            var ui = Object.FindObjectOfType<UIManager>();
            var canvasTransform = ui.GetComponent<RectTransform>();

            var mainMenu = CreatePanel(canvasTransform, "MainMenuPanel", new Color(0, 0, 0, 0.75f));
            CreateTMP(mainMenu, "Title", "ZILL RUNNER", 48, new Vector2(0, 180), new Vector2(600, 80), FontStyles.Bold);
            var nameInput = CreateInputField(mainMenu, "NameInput", "Enter player name...", new Vector2(0, 80));
            CreateTMP(mainMenu, "HighScore", "HIGH SCORE: 0", 22, new Vector2(0, 20), new Vector2(400, 40));
            CreateButton(mainMenu, "StartButton", "1. Start Game", new Vector2(0, -60), ui, "OnStartClicked");
            CreateButton(mainMenu, "InstructionsButton", "2. Instructions", new Vector2(0, -120), ui, "OnInstructionsClicked");
            CreateButton(mainMenu, "CreditsButton", "3. Credits", new Vector2(0, -180), ui, "OnCreditsClicked");
            CreateButton(mainMenu, "ExitButton", "4. Exit", new Vector2(0, -240), ui, "OnExitClicked");

            var hud = CreatePanel(canvasTransform, "HUDPanel", Color.clear);
            CreateTMP(hud, "PlayerName", "PLAYER: ---", 20, new Vector2(-320, 220), new Vector2(300, 30), FontStyles.Normal, TextAlignmentOptions.Left);
            CreateTMP(hud, "Lives", "LIVES: ♥♥♥", 20, new Vector2(-320, 190), new Vector2(300, 30), FontStyles.Normal, TextAlignmentOptions.Left);
            CreateTMP(hud, "Score", "SCORE: 0", 20, new Vector2(-320, 160), new Vector2(300, 30), FontStyles.Normal, TextAlignmentOptions.Left);
            CreateTMP(hud, "Coins", "COINS: 0", 20, new Vector2(320, 220), new Vector2(300, 30), FontStyles.Normal, TextAlignmentOptions.Right);
            CreateTMP(hud, "Speed", "SPEED: 12", 20, new Vector2(320, 190), new Vector2(300, 30), FontStyles.Normal, TextAlignmentOptions.Right);
            CreateTMP(hud, "Combo", "", 24, new Vector2(0, 220), new Vector2(200, 40), FontStyles.Bold, TextAlignmentOptions.Center);
            CreateTMP(hud, "Shield", "", 18, new Vector2(-100, -220), new Vector2(120, 30), FontStyles.Bold, TextAlignmentOptions.Center);
            CreateTMP(hud, "Magnet", "", 18, new Vector2(100, -220), new Vector2(120, 30), FontStyles.Bold, TextAlignmentOptions.Center);
            hud.gameObject.SetActive(false);

            var pause = CreateOverlay(canvasTransform, "PausePanel", "PAUSED", ui, "OnResumeClicked", "Resume", "OnQuitToMenuClicked", "Main Menu");
            var gameOver = CreateOverlay(canvasTransform, "GameOverPanel", "GAME OVER", ui, "OnRestartClicked", "Play Again", "OnQuitToMenuClicked", "Main Menu");
            var instructions = CreateInfoOverlay(canvasTransform, "InstructionsPanel",
                "INSTRUCTIONS\n\nW / Space — Jump\nS — Slide\nA / D — Move lanes\n\nBlue box — Jump OR Slide\nRed box — Jump only",
                ui);
            var credits = CreateInfoOverlay(canvasTransform, "CreditsPanel",
                "CREDITS\n\n22F-3422  Mahrukh\n22F-3279  Sania\n22F-3441  Wareesha\n\nUnity Edition — Advanced Port",
                ui);
            var loading = CreatePanel(canvasTransform, "LoadingPanel", new Color(0, 0, 0, 0.9f));
            CreateTMP(loading, "LoadingText", "LOADING THE GAME...", 32, Vector2.zero, new Vector2(500, 60), FontStyles.Bold);
            loading.gameObject.SetActive(false);

            WireUIManager(ui, mainMenu.gameObject, hud.gameObject, pause, gameOver, instructions, credits, loading.gameObject, nameInput);
        }

        private static void WireUIManager(UIManager ui, GameObject mainMenu, GameObject hud, GameObject pause,
            GameObject gameOver, GameObject instructions, GameObject credits, GameObject loading, TMP_InputField nameInput)
        {
            var so = new SerializedObject(ui);
            so.FindProperty("mainMenuPanel").objectReferenceValue = mainMenu;
            so.FindProperty("hudPanel").objectReferenceValue = hud;
            so.FindProperty("pausePanel").objectReferenceValue = pause;
            so.FindProperty("gameOverPanel").objectReferenceValue = gameOver;
            so.FindProperty("instructionsPanel").objectReferenceValue = instructions;
            so.FindProperty("creditsPanel").objectReferenceValue = credits;
            so.FindProperty("loadingPanel").objectReferenceValue = loading;
            so.FindProperty("nameInput").objectReferenceValue = nameInput;
            so.FindProperty("playerNameText").objectReferenceValue = hud.transform.Find("PlayerName")?.GetComponent<TMP_Text>();
            so.FindProperty("livesText").objectReferenceValue = hud.transform.Find("Lives")?.GetComponent<TMP_Text>();
            so.FindProperty("scoreText").objectReferenceValue = hud.transform.Find("Score")?.GetComponent<TMP_Text>();
            so.FindProperty("coinsText").objectReferenceValue = hud.transform.Find("Coins")?.GetComponent<TMP_Text>();
            so.FindProperty("speedText").objectReferenceValue = hud.transform.Find("Speed")?.GetComponent<TMP_Text>();
            so.FindProperty("comboText").objectReferenceValue = hud.transform.Find("Combo")?.GetComponent<TMP_Text>();
            so.FindProperty("shieldIndicator").objectReferenceValue = hud.transform.Find("Shield")?.GetComponent<TMP_Text>();
            so.FindProperty("magnetIndicator").objectReferenceValue = hud.transform.Find("Magnet")?.GetComponent<TMP_Text>();
            so.FindProperty("highScoreMenuText").objectReferenceValue = mainMenu.transform.Find("HighScore")?.GetComponent<TMP_Text>();
            so.FindProperty("finalScoreText").objectReferenceValue = gameOver.transform.Find("Body")?.GetComponent<TMP_Text>();
            so.ApplyModifiedProperties();
        }

        private static RectTransform CreatePanel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            go.GetComponent<Image>().color = color;
            return rt;
        }

        private static TMP_Text CreateTMP(Transform parent, string name, string text, int size, Vector2 pos, Vector2 sizeDelta,
            FontStyles style = FontStyles.Normal, TextAlignmentOptions align = TextAlignmentOptions.Center)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = sizeDelta;
            var tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.fontStyle = style;
            tmp.alignment = align;
            tmp.color = Color.white;
            return tmp;
        }

        private static TMP_InputField CreateInputField(Transform parent, string name, string placeholder, Vector2 pos)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(TMP_InputField));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(320, 40);
            go.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f);

            var textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(go.transform, false);
            var textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = new Vector2(10, 0);
            textRt.offsetMax = new Vector2(-10, 0);
            var text = textGo.GetComponent<TextMeshProUGUI>();
            text.fontSize = 18;
            text.color = Color.white;

            var phGo = new GameObject("Placeholder", typeof(RectTransform), typeof(TextMeshProUGUI));
            phGo.transform.SetParent(go.transform, false);
            var phRt = phGo.GetComponent<RectTransform>();
            phRt.anchorMin = Vector2.zero;
            phRt.anchorMax = Vector2.one;
            phRt.offsetMin = new Vector2(10, 0);
            phRt.offsetMax = new Vector2(-10, 0);
            var ph = phGo.GetComponent<TextMeshProUGUI>();
            ph.text = placeholder;
            ph.fontSize = 18;
            ph.color = new Color(1, 1, 1, 0.4f);

            var input = go.GetComponent<TMP_InputField>();
            input.textComponent = text;
            input.placeholder = ph;
            return input;
        }

        private static void CreateButton(Transform parent, string name, string label, Vector2 pos, Object target, string methodName)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(280, 44);
            go.GetComponent<Image>().color = new Color(0.2f, 0.45f, 0.85f);
            CreateTMP(go.transform, "Label", label, 20, Vector2.zero, rt.sizeDelta);
            var button = go.GetComponent<Button>();
            var method = target.GetType().GetMethod(methodName);
            if (method != null)
            {
                var action = (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(
                    typeof(UnityEngine.Events.UnityAction), target, method);
                UnityEditor.Events.UnityEventTools.AddPersistentListener(button.onClick, action);
            }
        }

        private static GameObject CreateOverlay(Transform parent, string name, string title, Object target,
            string primaryMethod, string primaryLabel, string secondaryMethod, string secondaryLabel)
        {
            var panel = CreatePanel(parent, name, new Color(0, 0, 0, 0.85f));
            CreateTMP(panel, "Title", title, 40, new Vector2(0, 80), new Vector2(500, 60), FontStyles.Bold);
            CreateTMP(panel, "Body", "", 22, new Vector2(0, 0), new Vector2(500, 80));
            CreateButton(panel, "Primary", primaryLabel, new Vector2(0, -80), target, primaryMethod);
            CreateButton(panel, "Secondary", secondaryLabel, new Vector2(0, -140), target, secondaryMethod);
            panel.gameObject.SetActive(false);
            return panel.gameObject;
        }

        private static GameObject CreateInfoOverlay(Transform parent, string name, string body, Object target)
        {
            var panel = CreatePanel(parent, name, new Color(0, 0, 0, 0.9f));
            CreateTMP(panel, "Body", body, 22, Vector2.zero, new Vector2(520, 320));
            CreateButton(panel, "Close", "Close", new Vector2(0, -180), target, "OnCloseOverlayClicked");
            panel.gameObject.SetActive(false);
            return panel.gameObject;
        }
    }
}
#endif
