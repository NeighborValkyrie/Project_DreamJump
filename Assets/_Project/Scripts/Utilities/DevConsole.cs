using UnityEngine;
using System.Collections.Generic;

namespace PlatformerGame.Utilities
{
    /// <summary>
    /// 개발자 콘솔 (치트 명령어)
    /// v7.0: 비동기 수정
    /// </summary>
    public class DevConsole : MonoBehaviour
    {
        [Header("Console Settings")]
        [SerializeField] private bool enableConsole = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.BackQuote; // ~ 키

        private bool showConsole = false;
        private string input = "";
        private List<string> commandHistory = new List<string>();
        private Vector2 scrollPosition;

        private void Update()
        {
            if (!enableConsole) return;

            if (Input.GetKeyDown(toggleKey))
            {
                showConsole = !showConsole;
            }
        }

        private void OnGUI()
        {
            if (!showConsole || !enableConsole) return;

            float consoleHeight = Screen.height * 0.3f;

            // 배경
            GUI.Box(new Rect(0, 0, Screen.width, consoleHeight), "");

            // 명령어 히스토리
            GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, consoleHeight - 40));
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (string cmd in commandHistory)
            {
                GUILayout.Label(cmd);
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            // 입력 필드
            GUI.SetNextControlName("ConsoleInput");
            input = GUI.TextField(
                new Rect(10, consoleHeight - 25, Screen.width - 20, 20),
                input
            );

            GUI.FocusControl("ConsoleInput");

            // Enter 키로 명령 실행
            if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
            {
                if (!string.IsNullOrEmpty(input))
                {
                    ExecuteCommand(input);
                    commandHistory.Add($"> {input}");
                    input = "";
                }
            }
        }

        private void ExecuteCommand(string command)
        {
            string[] parts = command.Split(' ');
            string cmd = parts[0].ToLower();

            try
            {
                switch (cmd)
                {
                    case "help":
                        ShowHelp();
                        break;

                    case "tp": // 텔레포트
                        if (parts.Length >= 4)
                        {
                            float x = float.Parse(parts[1]);
                            float y = float.Parse(parts[2]);
                            float z = float.Parse(parts[3]);
                            TeleportPlayer(new Vector3(x, y, z));
                        }
                        else
                        {
                            commandHistory.Add("사용법: tp <x> <y> <z>");
                        }
                        break;

                    case "speed": // 속도 변경
                        if (parts.Length >= 2)
                        {
                            float multiplier = float.Parse(parts[1]);
                            SetPlayerSpeed(multiplier);
                        }
                        else
                        {
                            commandHistory.Add("사용법: speed <multiplier>");
                        }
                        break;

                    case "additem": // 아이템 추가
                        if (parts.Length >= 3)
                        {
                            string itemID = parts[1];
                            int quantity = int.Parse(parts[2]);
                            AddItem(itemID, quantity);
                        }
                        else
                        {
                            commandHistory.Add("사용법: additem <itemID> <quantity>");
                        }
                        break;

                    case "save": // 게임 저장
                        SaveGame();
                        break;

                    case "load": // 게임 로드
                        LoadGame();
                        break;

                    case "scene": // 씬 전환
                        if (parts.Length >= 2)
                        {
                            string sceneName = parts[1];
                            LoadScene(sceneName);
                        }
                        else
                        {
                            commandHistory.Add("사용법: scene <sceneName>");
                        }
                        break;

                    case "clear": // 콘솔 지우기
                        commandHistory.Clear();
                        break;

                    default:
                        commandHistory.Add($"알 수 없는 명령어: {cmd}");
                        commandHistory.Add("'help' 입력으로 명령어 목록 확인");
                        break;
                }
            }
            catch (System.Exception e)
            {
                commandHistory.Add($"오류: {e.Message}");
            }
        }

        private void ShowHelp()
        {
            commandHistory.Add("=== 개발자 콘솔 명령어 ===");
            commandHistory.Add("help - 명령어 목록");
            commandHistory.Add("tp <x> <y> <z> - 플레이어 텔레포트");
            commandHistory.Add("speed <multiplier> - 이동 속도 변경");
            commandHistory.Add("additem <itemID> <quantity> - 아이템 추가");
            commandHistory.Add("save - 게임 저장");
            commandHistory.Add("load - 게임 로드");
            commandHistory.Add("scene <sceneName> - 씬 전환");
            commandHistory.Add("clear - 콘솔 지우기");
        }

        private void TeleportPlayer(Vector3 position)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var movement = player.GetComponent<Core.Player.PlayerMovement>();
                if (movement != null)
                {
                    movement.Teleport(position);
                    commandHistory.Add($"플레이어 텔레포트: {position}");
                }
            }
            else
            {
                commandHistory.Add("플레이어를 찾을 수 없습니다.");
            }
        }

        private void SetPlayerSpeed(float multiplier)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var movement = player.GetComponent<Core.Player.PlayerMovement>();
                if (movement != null)
                {
                    movement.SetSpeedMultiplier(multiplier);
                    commandHistory.Add($"속도 배율 설정: {multiplier}x");
                }
            }
            else
            {
                commandHistory.Add("플레이어를 찾을 수 없습니다.");
            }
        }

        private void AddItem(string itemID, int quantity)
        {
            if (Systems.Inventory.InventoryManager.Instance != null)
            {
                bool success = Systems.Inventory.InventoryManager.Instance.AddItem(itemID, quantity);
                if (success)
                {
                    commandHistory.Add($"아이템 추가: {itemID} x{quantity}");
                }
                else
                {
                    commandHistory.Add("인벤토리가 가득 찼습니다.");
                }
            }
            else
            {
                commandHistory.Add("InventoryManager를 찾을 수 없습니다.");
            }
        }

        private void SaveGame()
        {
            if (Systems.Save.SaveManager.Instance != null)
            {
                Systems.Save.SaveManager.Instance.SaveGameAsync();
                commandHistory.Add("게임 저장 중...");
            }
            else
            {
                commandHistory.Add("SaveManager를 찾을 수 없습니다.");
            }
        }

        private void LoadGame()
        {
            if (Systems.Save.SaveManager.Instance != null)
            {
                Systems.Save.SaveManager.Instance.LoadGameAsync();
                commandHistory.Add("게임 로드 중...");
            }
            else
            {
                commandHistory.Add("SaveManager를 찾을 수 없습니다.");
            }
        }

        private void LoadScene(string sceneName)
        {
            if (Systems.Scene.SceneController.Instance != null)
            {
                Systems.Scene.SceneController.Instance.LoadScene(sceneName);
                commandHistory.Add($"씬 전환 중: {sceneName}");
            }
            else
            {
                commandHistory.Add("SceneController를 찾을 수 없습니다.");
            }
        }
    }
}