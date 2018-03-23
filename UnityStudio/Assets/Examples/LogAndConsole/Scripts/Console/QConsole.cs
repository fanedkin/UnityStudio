using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace QFramework {
    /// <summary>
    /// 控制台GUI输出类
    /// 包括FPS，内存使用情况，日志GUI输出
    /// </summary>
    public class QConsole 
    {

        struct ConsoleMessage
        {
            public readonly string  message;
            public readonly string  stackTrace;
            public readonly LogType type;

            public ConsoleMessage (string message, string stackTrace, LogType type)
            {
                this.message    = message;
                this.stackTrace = stackTrace;
                this.type       = type;
            }
        }

        /// <summary>
        /// Update回调
        /// </summary>
        public delegate void OnUpdateCallback();
        /// <summary>
        /// OnGUI回调
        /// </summary>
        public delegate void OnGUICallback();

        public OnUpdateCallback onUpdateCallback = null;
        public OnGUICallback onGUICallback = null;
        /// <summary>
        /// FPS计数器
        /// </summary>
        private QFPSCounter fpsCounter = null;
        /// <summary>
        /// 内存监视器
        /// </summary>
        private QMemoryDetector memoryDetector = null;
        private bool showGUI = false;
        List<ConsoleMessage> entries = new List<ConsoleMessage>();
        Vector2 scrollPos;
        bool scrollToBottom = true;
        bool collapse;
        bool mTouching = false;

        const int margin = 20;
        Rect windowRect = new Rect(margin + Screen.width * 0.5f, margin, Screen.width * 0.5f - (2 * margin), Screen.height - (2 * margin));

        GUIContent clearLabel    = new GUIContent("Clear",    "Clear the contents of the console.");
        GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
        GUIContent scrollToBottomLabel = new GUIContent("ScrollToBottom", "Scroll bar always at bottom");


        public QConsole()
        {
            this.fpsCounter = new QFPSCounter(this);
            this.memoryDetector = new QMemoryDetector(this);
            //GameManager.instance.onUpdate += Update;
            //GameManager.instance.onGUI += OnGUI;
            Application.logMessageReceived += HandleLog;
        }

        ~QConsole()//析构函数
        {
            Application.logMessageReceived -= HandleLog;
        }


        void Update()
        {
            #if UNITY_EDITOR
            if (Input.GetKeyUp(KeyCode.F1))
                this.showGUI = !this.showGUI;
            #elif UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape))
                this.showGUI = !this.showGUI;
            #elif UNITY_IOS
            if (!mTouching && Input.touchCount == 4)
            {
                mTouching = true;
                this.showGUI = !this.showGUI;
            } else if (Input.touchCount == 0){
                mTouching = false;
            }
            #endif

            if (this.onUpdateCallback != null)
                this.onUpdateCallback();
        }

        void OnGUI()
        {
            if (!this.showGUI)
                return;

            if (this.onGUICallback != null)
                this.onGUICallback ();

            if (GUI.Button(new Rect(150, 150, 200, 100), "清空数据"))
            {
                PlayerPrefs.DeleteAll ();
                #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            }
            windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
        }


        /// <summary>
        /// A window displaying the logged messages.
        /// </summary>
        void ConsoleWindow (int windowID)
        {
            if (scrollToBottom) {
                GUILayout.BeginScrollView (Vector2.up * entries.Count * 100.0f);
            }
            else {
                scrollPos = GUILayout.BeginScrollView (scrollPos);
            }
            // Go through each logged entry
            for (int i = 0; i < entries.Count; i++) {
                ConsoleMessage entry = entries[i];
                // If this message is the same as the last one and the collapse feature is chosen, skip it
                if (collapse && i > 0 && entry.message == entries[i - 1].message) {
                    continue;
                }
                // Change the text colour according to the log type
                switch (entry.type) {
                    case LogType.Error:
                    case LogType.Exception:
                        GUI.contentColor = Color.red;
                        break;
                    case LogType.Warning:
                        GUI.contentColor = Color.yellow;
                        break;
                    default:
                        GUI.contentColor = Color.white;
                        break;
                }
                if (entry.type == LogType.Exception)
                {
                    GUILayout.Label(entry.message + " || " + entry.stackTrace);
                } else {
                    GUILayout.Label(entry.message);
                }
            }
            GUI.contentColor = Color.white;
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            // Clear button
            if (GUILayout.Button(clearLabel)) {
                entries.Clear();
            }
            // Collapse toggle
            collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));
            scrollToBottom = GUILayout.Toggle (scrollToBottom, scrollToBottomLabel, GUILayout.ExpandWidth (false));
            GUILayout.EndHorizontal();
            // Set the window to be draggable by the top title bar
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        void HandleLog (string message, string stackTrace, LogType type)
        {
            ConsoleMessage entry = new ConsoleMessage(message, stackTrace, type);
            entries.Add(entry);
        }
    }
}


namespace QFramework {
    /// <summary>
    /// 帧率计算器
    /// </summary>
    public class QFPSCounter
    {
        // 帧率计算频率
        private const float calcRate = 0.5f;
        // 本次计算频率下帧数
        private int frameCount = 0;
        // 频率时长
        private float rateDuration = 0f;
        // 显示帧率
        private int fps = 0;

        public QFPSCounter(QConsole console)
        {
            console.onUpdateCallback += Update;
            console.onGUICallback += OnGUI;
        }

        void Start()
        {
            this.frameCount = 0;
            this.rateDuration = 0f;
            this.fps = 0;
        }

        void Update()
        {
            ++this.frameCount;
            this.rateDuration += Time.deltaTime;
            if (this.rateDuration > calcRate)
            {
                // 计算帧率
                this.fps = (int)(this.frameCount / this.rateDuration);
                this.frameCount = 0;
                this.rateDuration = 0f;
            }
        }

        void OnGUI()
        {
            GUI.color = Color.black;
            GUI.Label(new Rect(80, 20, 120, 20),"fps:" + this.fps.ToString());      
        }
    }

}
namespace QFramework
{
    /// <summary>
    /// 内存检测器，目前只是输出Profiler信息
    /// </summary>
    public class QMemoryDetector
    {
        private readonly static string TotalAllocMemroyFormation = "Alloc Memory : {0}M";
        private readonly static string TotalReservedMemoryFormation = "Reserved Memory : {0}M";
        private readonly static string TotalUnusedReservedMemoryFormation = "Unused Reserved: {0}M";
        private readonly static string MonoHeapFormation = "Mono Heap : {0}M";
        private readonly static string MonoUsedFormation = "Mono Used : {0}M";
        // 字节到兆
        private float ByteToM = 0.000001f;

        private Rect allocMemoryRect;
        private Rect reservedMemoryRect;
        private Rect unusedReservedMemoryRect;
        private Rect monoHeapRect;
        private Rect monoUsedRect;

        private int x = 0;
        private int y = 0;
        private int w = 0;
        private int h = 0;

        public QMemoryDetector(QConsole console)
        {
            this.x = 60;
            this.y = 60;
            this.w = 200;
            this.h = 20;

            this.allocMemoryRect = new Rect(x, y, w, h);
            this.reservedMemoryRect = new Rect(x, y + h, w, h);
            this.unusedReservedMemoryRect = new Rect(x, y + 2 * h, w, h);
            this.monoHeapRect = new Rect(x, y + 3 * h, w, h);
            this.monoUsedRect = new Rect(x, y + 4 * h, w, h);

            console.onGUICallback += OnGUI;
        }

        void OnGUI()
        {
            GUI.Label(this.allocMemoryRect,
                string.Format(TotalAllocMemroyFormation, Profiler.GetTotalAllocatedMemory() * ByteToM));
            GUI.Label(this.reservedMemoryRect,
                string.Format(TotalReservedMemoryFormation, Profiler.GetTotalReservedMemory() * ByteToM));
            GUI.Label(this.unusedReservedMemoryRect,
                string.Format(TotalUnusedReservedMemoryFormation, Profiler.GetTotalUnusedReservedMemory() * ByteToM));
            GUI.Label(this.monoHeapRect,
                string.Format(MonoHeapFormation, Profiler.GetMonoHeapSize() * ByteToM));
            GUI.Label(this.monoUsedRect,
                string.Format(MonoUsedFormation, Profiler.GetMonoUsedSize() * ByteToM));
        }
    }

}