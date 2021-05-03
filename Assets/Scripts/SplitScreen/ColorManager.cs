using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
using System;
using WindowsInput;
using System.Diagnostics;

public class ColorManager : MonoBehaviour
{
    public TextMeshProUGUI log;
    Param objParam;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        objParam = new Param(Application.dataPath + "/Resources1/param.txt");
        log.color = new Color(0, 0, 0, 1);
#elif UNITY_STANDALONE
        objParam = new Param(Application.dataPath + "/Resources/param.txt");
        log.color = new Color(0,0,0,1);
#endif
        monitorWidth = Screen.resolutions[Screen.resolutions.Length - 1].width;
        monitorHeight = Screen.resolutions[Screen.resolutions.Length - 1].height;
        //CheckAndSet();

        for (int i = 0; i < transform.childCount; i++)
        {
            Color background = new Color(
                  UnityEngine.Random.Range(0f, 1f),
                  UnityEngine.Random.Range(0f, 1f),
                  UnityEngine.Random.Range(0f, 1f)
              );
            transform.GetChild(i).GetComponent<Image>().color = background;
        }
        //OpenExpLeft();
        //StartCoroutine(SplitScreen());

    }
    private bool isBrowserRunning;
    private bool isBrowserRunningStatus()
    {
        //NOTE: GetProcessByName() doesn't seem to work on Win7
        //Process[] running = Process.GetProcessesByName("notepad");
        Process[] running = Process.GetProcesses();
        foreach (Process process in running)
        {
            //print(process.ProcessName);
            try
            {
                if (!process.HasExited && process.ProcessName.ToLower() == objParam.defaultBrowser)
                {
                    //print(process.MainWindowHandle);

                    isBrowserRunning = true;

                }
                else
                {
                    isBrowserRunning = false;
                }
            }
            catch (System.InvalidOperationException)
            {
                //do nothing
                UnityEngine.Debug.Log("***** InvalidOperationException was caught!");
            }
        }
        return isBrowserRunning;
    }
    IEnumerator SplitScreen()
    {
        yield return new WaitForSeconds(0.2f);
        OpenExpLeft();
        yield return new WaitForSeconds(1);
        OpenURLFile();

    }
    bool callOnce = false;
    private void OnApplicationFocus(bool focus)
    {
        //if (!focus)
        //{
        //    if (!callOnce)
        //    {
        //        StartCoroutine(openChromeRightSide());
        //        callOnce = true;
        //    }
        //}
        //else {
        //    if (Screen.width <= monitorWidth/2)
        //    {
        //        SetFullScreen();
        //        isFullScreen = !isFullScreen;
        //        //callOnce = false;
        //    }
        //}
    }
    IEnumerator openChromeRightSide()
    {
        yield return new WaitForSeconds(3);
        //SetWindowed();
        print("isBrowserRunningStatus " + isBrowserRunningStatus());
        Process[] processes = Process.GetProcessesByName("chrome");

        Process lol = processes[0];
        lol.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
        IntPtr ptr = lol.MainWindowHandle;

        Rect NotepadRect = new Rect();
        NotepadRect.Left = 0;
        NotepadRect.Right = monitorWidth;
        NotepadRect.Top = 0;
        NotepadRect.Top = monitorHeight;
        log.text = lol.ProcessName + " " + lol.StartInfo.WindowStyle + " " + NotepadRect.Left + " " + NotepadRect.Right + " " + NotepadRect.Top + " " + NotepadRect.Bottom;
        GetWindowRect(ptr, ref NotepadRect);
        //print(GetWindowRect(ptr, ref NotepadRect));
        OpenExpRight();


        //StartCoroutine(pushBrowserRight());
    }
    IEnumerator pushBrowserRight()
    {
        yield return new WaitForSeconds(2);
        OpenExpLeft();
    }
    public void OpenExpLeft()
    {
        WindowsInput.Tests.InputSimulatorExamples a = new WindowsInput.Tests.InputSimulatorExamples();
        a.OpenWindowsExplorerLeft();
    }
    public void OpenExpRight()
    {
        WindowsInput.Tests.InputSimulatorExamples a = new WindowsInput.Tests.InputSimulatorExamples();
        a.OpenWindowsExplorerRight();
    }
    private void OpenURLFile()
    {
        Application.OpenURL(objParam.urlToOpen);
    }
    private bool isFullScreen;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isFullScreen = !isFullScreen;
            //Screen.fullScreen = !Screen.fullScreen;
            if (!isFullScreen)
            {
                print(Screen.currentResolution.width + " fullscreen " + Screen.currentResolution.height);
                //log.text = Screen.currentResolution.width + " fullscreen " + Screen.currentResolution.height;
                //Screen.SetResolution(Screen.width / 2, Screen.height, !Screen.fullScreen);
                SetFullScreen();
            }
            else
            {
                print(Screen.currentResolution.width + " not fullscreen " + Screen.currentResolution.height);
                //log.text = Screen.currentResolution.width + " not fullscreen " + Screen.currentResolution.height;
                //Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreen);
                SetWindowed();
                StartCoroutine(SplitScreen());
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            log.text = "A pressed";
            StartCoroutine(FindServerProcess());
        }
    }
    private bool inSplitScreen = false;
    private Process serverProcess = null;

    IEnumerator FindServerProcess()
    {
        while (true)
        {
            Process[] running = Process.GetProcessesByName("vWarehouse_Server_0.4");
            if (running.Length == 1 && !inSplitScreen)
            {
                serverProcess = running[0];
                serverProcess.EnableRaisingEvents = true;
                serverProcess.Exited += ServerProcess_Exited;
                print("Server Process Found !!");
                inSplitScreen = true;
            }
            else
            {
                yield return null;
            }
        }

    }

    private void ServerProcess_Exited(object sender, EventArgs e)
    {
        print("Server process killed !!");
    }
    private void LogValue(int size, int x, int y)
    {
        //log.text = "W " + monitorWidth + " H " + monitorHeight + " S " + size + " X " + x + " Y " + y;
    }
    public bool fullScreen = false;
    private static string fullScreenKey = "Full Screen";
    public int minDisplaySize = 384;
    private static int monitorWidth;
    private static int monitorHeight;

#if UNITY_STANDALONE_WIN

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindow(string className, string windowName);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

    public struct Rect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }


    public static IEnumerator SetWindowPosition(int x, int y)
    {
        yield return new WaitForEndOfFrame();
        //SetWindowPos(FindWindow(null, Application.productName), 0, x, y, 0, 0, 5);
        SetWindowPos(FindWindow(null, "chrome"), 0, x, y, 0, 0, 5);
    }


#endif


    //private void CheckAndSet()
    //{
    //    if (PlayerPrefs.GetInt(fullScreenKey, 0) >= 1)
    //    {
    //        SetFullScreen();
    //    }
    //    else
    //    {
    //        SetWindowed();
    //    }
    //}

    public void SetFullScreen()
    {
        Screen.SetResolution(monitorWidth, monitorHeight, true);
        //fullScreen = true;
        //PlayerPrefs.SetInt(fullScreenKey, 1);
        //PlayerPrefs.Save();
    }

    public void SetWindowed()
    {
        SetWindowResolution();
        fullScreen = false;
        //PlayerPrefs.SetInt(fullScreenKey, 0);
        //PlayerPrefs.Save();
    }

    private void SetWindowResolution()
    {
        int multiplier = 1;
        if (monitorWidth >= monitorHeight)
        {
            multiplier = monitorHeight / minDisplaySize;
            if ((monitorHeight % minDisplaySize) == 0)
            {
                multiplier--;
            }
        }
        else
        {
            multiplier = monitorWidth / minDisplaySize;
            if ((monitorWidth % minDisplaySize) == 0)
            {
                multiplier--;
            }
        }
        int size = minDisplaySize * multiplier;
        if (size < minDisplaySize)
        {
            size = minDisplaySize;
        }
        Screen.SetResolution(monitorWidth / 2, monitorHeight, false);

#if UNITY_STANDALONE_WIN

        int x = monitorWidth / 2;
        //x -= size / 2;
        int y = monitorHeight / 2;
        //y -= size / 2;
        x = 0;
        y = 0;
        //log.text = "W " + monitorWidth + " H " + monitorHeight + " S " + size + " X " + x + " Y " + y;

        StartCoroutine(SetWindowPosition(x, y));
#endif
    }
}
