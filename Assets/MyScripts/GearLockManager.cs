using UnityEngine;
using UnityEngine.UI;
using easyar;
using System.Collections;

public class GearLockManager : MonoBehaviour
{
    [Header("AR宝箱原始齿轮")]
    public GameObject[] gearObjs;
    [Header("UI放大副本齿轮")]
    public GameObject[] uiGearCopys;
    [Header("UI密码面板")]
    public GameObject passwordPanel;
    [Header("点击提示UI")]
    public UnityEngine.UI.Image tipBg;
    [Header("开锁动画配置")]
    public Transform lockHeadTrans;
    public AudioClip unlockSound;
    public AudioSource lockAudioSource;
    public GameObject lockModelObj;
    [Header("正确密码")]
    public string correctPwd = "1234";

    [Header("小型错误提示(Image)")]
    public GameObject errorTipImg;
    private Coroutine hideErrorCor;

    public int[] gearNum = new int[4] { 0, 0, 0, 0 };
    public string CurrentPassword => $"{gearNum[0]}{gearNum[1]}{gearNum[2]}{gearNum[3]}";

    private bool isAnimRunning = false;

    public void ShowClickTip()
    {
        Debug.Log($"调用显示提示， 底板空？{tipBg == null}");
        if (tipBg != null)
        {
            tipBg.gameObject.SetActive(true);
        }
    }

    // 点击宝箱隐藏提示
    public void HideClickTip()
    {
        if (tipBg != null)
            tipBg.gameObject.SetActive(false);
    }

    public void OnChestClick()
    {
        HideClickTip(); // 点击宝箱第一时间隐藏UI提示
        Debug.Log("===进入GearLockManager.OnChestClick===");

        if (TreasureManager.Instance == null)
        {
            Debug.LogError("TreasureManager单例不存在，无法执行开箱逻辑");
            return;
        }
        Debug.Log($"箱子当前开启状态 IsOpened：{TreasureManager.Instance.IsOpened}");
        Debug.Log($"passwordPanel引用是否赋值：{passwordPanel != null}");

        if (!TreasureManager.Instance.IsOpened && passwordPanel != null)
        {
            passwordPanel.SetActive(true);
            Canvas uiCanvas = FindObjectOfType<Canvas>();
            if (uiCanvas != null)
            {
                if (!uiCanvas.gameObject.activeSelf)
                    uiCanvas.gameObject.SetActive(true);

                Canvas.ForceUpdateCanvases();
                Debug.Log($" 画布刷新完成，已激活密码面板");
            }
            else
            {
                Debug.LogError("场景中未检索到Canvas组件，UI渲染失败");
            }
        }
        else
        {
            Debug.Log("不激活面板：箱子已开启 / 面板引用丢失");
        }
    }

    public void CheckPassword()
    {
        if (isAnimRunning)
        {
            Debug.Log("开锁动画正在进行，请勿重复操作");
            return;
        }
        if (TreasureManager.Instance == null)
        {
            Debug.LogError("TreasureManager单例不存在");
            return;
        }

        if (CurrentPassword == correctPwd)
        {
            Debug.Log($"✅ 密码校验成功 {CurrentPassword}，开始执行开锁流程");
            StartCoroutine(FullUnlockProcess());
        }
        else
        {
            Debug.Log($"❌ 密码错误，输入：{CurrentPassword}，正确密码：{correctPwd}");
            ShowErrorTip();
        }
    }

    // 弹出0.5秒自动消失的错误提示
    void ShowErrorTip()
    {
        if (errorTipImg == null) return;

        // 终止上一次未走完的关闭计时
        if (hideErrorCor != null)
        {
            StopCoroutine(hideErrorCor);
        }

        errorTipImg.SetActive(true);
        hideErrorCor = StartCoroutine(CloseTipAfterHalfSec());
    }

    IEnumerator CloseTipAfterHalfSec()
    {
        yield return new WaitForSeconds(0.5f);
        errorTipImg.SetActive(false);
        hideErrorCor = null;
    }

    IEnumerator FullUnlockProcess()
    {
        isAnimRunning = true;

        // 锁头瞬移
        Vector3 startPos = lockHeadTrans.localPosition;
        Vector3 endPos = new Vector3(startPos.x, startPos.y, -0.248f);
        lockHeadTrans.localPosition = endPos;
        Debug.Log("锁头瞬移完成");

        // 隐藏锁模型
        if (lockModelObj != null)
        {
            lockModelObj.SetActive(false);
            Debug.Log("宝箱锁模型已消失隐藏");
        }

        // 音频校验
        if (unlockSound == null)
            Debug.LogError("错误：unlockSound音频文件未拖拽赋值！");
        if (lockAudioSource == null)
            Debug.LogError("错误：lockAudioSource锁头音源组件未拖拽赋值！");

        // 播放开锁音效
        if (unlockSound != null && lockAudioSource != null)
        {
            lockAudioSource.PlayOneShot(unlockSound);
            Debug.Log("AudioSource播放开锁音效");
        }

        yield return new WaitForSeconds(0.5f);

        // 关闭密码面板
        yield return new WaitForSeconds(0.15f);
        if (passwordPanel != null)
        {
            passwordPanel.SetActive(false);
            Canvas uiCanvas = FindObjectOfType<Canvas>();
            if (uiCanvas != null) Canvas.ForceUpdateCanvases();
        }

        // 关闭AR追踪
        ImageTrackerFrameFilter tracker = FindObjectOfType<ImageTrackerFrameFilter>();
        if (tracker != null)
        {
            tracker.enabled = false;
            Debug.Log("图像追踪识别已关闭");
        }

        // 宝箱开盖
        TreasureManager.Instance.OpenChest();

        isAnimRunning = false;
    }

    public void ResetAllGear()
    {
        for (int i = 0; i < 4; i++)
        {
            gearNum[i] = 0;
            float resetAngle = 0f;
            if (gearObjs[i] != null)
                gearObjs[i].transform.localEulerAngles = new Vector3(0, resetAngle, 0);
            if (uiGearCopys[i] != null)
                uiGearCopys[i].transform.localEulerAngles = new Vector3(0, resetAngle, 0);
        }

        // 追踪丢失恢复锁
        if (lockModelObj != null)
            lockModelObj.SetActive(true);

        if (passwordPanel != null)
        {
            passwordPanel.SetActive(false);
            Canvas uiCanvas = FindObjectOfType<Canvas>();
            if (uiCanvas != null) Canvas.ForceUpdateCanvases();
        }

        // 追踪丢失后重新弹出提示UI
        ShowClickTip();
        Debug.Log("📌 图像跟踪丢失：齿轮归零、面板关闭、锁复原、提示重新显示");
    }

    // 修复点：启动仅隐藏提示，不再自动弹出，由TrackUICaller识别成功后调用ShowClickTip
    private void Start()
    {
        if (tipBg != null)
        {
            tipBg.gameObject.SetActive(false);
        }
    }
}