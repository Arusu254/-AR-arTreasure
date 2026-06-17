using UnityEngine;
using DG.Tweening;
using XLua; 

public class TreasureManager : MonoBehaviour
{
    public static TreasureManager Instance;

    [Header("实体引用")]
    public Transform key;
    public Transform lid;
    public Transform lidLock;
    public ParticleSystem goldParticle;

    [Header("发光参数")]
    public Color glowGoldColor = Color.white;
    public float peakTint = 1f;
    public float glowKeepTime = 10f;
    public float fadeBackDuration = 1.2f;

    [Header("材质绑定")]
    public Material chestMaterial;
    public Material keyMaterial;

    private float currentTintValue;
    private Color originChestColor;
    private Color originKeyColor;
    public bool IsOpened { get; set; }

    void Awake()
    {
        Instance = this;
        Debug.Log($"【TreasureManager】Awake启动");
        // 自动读取材质原生颜色，杜绝变黑
        if (chestMaterial != null)
        {
            originChestColor = chestMaterial.GetColor("_Color");
            Debug.Log($"【宝箱材质】读取初始色 R:{originChestColor.r:F2} G:{originChestColor.g:F2} B:{originChestColor.b:F2}");
        }
        else
        {
            Debug.LogError("【严重】chestMaterial 未赋值！");
        }

        if (keyMaterial != null)
        {
            originKeyColor = keyMaterial.GetColor("_Color");
            Debug.Log($"【钥匙材质】读取初始色 R:{originKeyColor.r:F2} G:{originKeyColor.g:F2} B:{originKeyColor.b:F2}");
        }
        else
        {
            Debug.LogError("【严重】keyMaterial 未赋值！");
        }

        ResetAllGlow();
    }

    public void ResetAllGlow()
    {
        Debug.Log($"【ResetAllGlow】重置状态");
        currentTintValue = 0;
        IsOpened = false;

        if (chestMaterial != null)
            chestMaterial.SetColor("_Color", originChestColor);
        if (keyMaterial != null)
            keyMaterial.SetColor("_Color", originKeyColor);

        if (goldParticle != null)
        {
            goldParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            Debug.Log($"【粒子】重置停止");
        }
        else
        {
            Debug.LogWarning("【警告】goldParticle 粒子对象为空，不会播放粒子");
        }
    }

    void SetColorLerp(Material mat, Color originCol, float intensity)
    {
        if (mat == null)
        {
            Debug.LogError("SetColorLerp 传入材质为空");
            return;
        }
        Color targetCol = Color.Lerp(originCol, glowGoldColor, intensity);
        mat.SetColor("_Color", targetCol);
        Debug.Log($"【变色】强度:{intensity:F2} 目标色 R:{targetCol.r:F2} G:{targetCol.g:F2} B:{targetCol.b:F2}");
    }

    void PlayKeyEffect()
    {
        Debug.Log($"【PlayKeyEffect】钥匙动画+变色启动");
        if (key == null)
        {
            Debug.LogError("key Transform为空，钥匙动画跳过");
            return;
        }
        if (keyMaterial == null)
        {
            Debug.LogError("keyMaterial为空，钥匙变色跳过");
            return;
        }

        DOTween.To(() => currentTintValue, val =>
        {
            currentTintValue = val;
            SetColorLerp(keyMaterial, originKeyColor, currentTintValue);
        }, peakTint, 1.5f).OnComplete(() => {
            Debug.Log("【钥匙变色】渐变完成，达到峰值亮度");
        });

        Sequence keyAnim = DOTween.Sequence();
        keyAnim.Join(key.DOLocalMoveZ(-1f, 1.5f).SetEase(Ease.OutSine));
        keyAnim.Join(key.DOLocalRotate(new Vector3(-90, 0, 0), 1.5f, RotateMode.LocalAxisAdd));
        keyAnim.OnComplete(() => {
            Debug.Log("【钥匙位移动画】动作完成");
        });
    }

    public void OpenChest()
    {
        Debug.Log($"【OpenChest】调用开箱函数，当前IsOpened={IsOpened}");
        if (IsOpened)
        {
            Debug.Log("已经开过箱子，直接返回");
            return;
        }
        IsOpened = true;

        // Lua开箱调用点 
        if (LuaEnvMgr.GlobalLua != null)
        {
            LuaEnvMgr.GlobalLua.DoString("OnBoxOpened()");
        }
        // Lua调用点结束

        Sequence fullOpenSeq = DOTween.Sequence();

        fullOpenSeq.AppendCallback(() =>
        {
            Debug.Log("【音频】尝试播放开箱音效");
            ChestAudioPlayer audioPlayer = key?.GetComponent<ChestAudioPlayer>();
            if (audioPlayer != null)
                audioPlayer.PlayOpenSound();
            else
                Debug.LogWarning("钥匙物体无ChestAudioPlayer组件");
        });

        if (lidLock != null)
        {
            fullOpenSeq.Append(lidLock.DOLocalRotate(new Vector3(135, 0, 0), 0.3f)).OnComplete(() => {
                Debug.Log("【锁扣动画】旋转完成");
            });
        }
        else
        {
            Debug.LogWarning("lidLock 锁扣物体为空，跳过锁动画");
        }

        if (lid != null)
        {
            fullOpenSeq.Append(lid.DOLocalRotate(new Vector3(120, 0, 0), 0.7f)).OnComplete(() => {
                Debug.Log("【箱盖动画】翻盖完成，准备播放粒子");
            });
        }
        else
        {
            Debug.LogWarning("lid 箱盖物体为空，跳过箱盖动画");
        }

        fullOpenSeq.AppendCallback(PlayGoldParticle);
        fullOpenSeq.AppendCallback(RunGlowAnim);
        fullOpenSeq.OnComplete(() => {
            Debug.Log("【完整开箱序列】所有步骤执行完毕");
        });
    }

    void PlayGoldParticle()
    {
        Debug.Log($"【PlayGoldParticle】执行粒子播放");
        if (goldParticle == null)
        {
            Debug.LogError("粒子组件为空，无法播放特效！必须把场景里ParticleSystem拖入GoldParticle框");
            return;
        }
        goldParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        goldParticle.Play();
        Debug.Log("【粒子】成功调用Play()");
    }

    void RunGlowAnim()
    {
        Debug.Log($"【RunGlowAnim】启动箱子+钥匙整体发光");
        PlayKeyEffect();

        DOTween.To(() => currentTintValue, val =>
        {
            currentTintValue = val;
            SetColorLerp(chestMaterial, originChestColor, currentTintValue);
        }, peakTint, 1.5f)
        .OnComplete(() =>
        {
            Debug.Log("【箱子变色】亮度拉满，等待10秒褪色");
            DOVirtual.DelayedCall(glowKeepTime, RestoreOriginColor);
        });
    }

    void RestoreOriginColor()
    {
        Debug.Log($"【RestoreOriginColor】10秒到，开始褪色回原色");
        DOTween.To(() => currentTintValue, val =>
        {
            currentTintValue = val;
            SetColorLerp(chestMaterial, originChestColor, currentTintValue);
            SetColorLerp(keyMaterial, originKeyColor, currentTintValue);
        }, 0f, fadeBackDuration).OnComplete(() => {
            Debug.Log("【褪色完成】恢复初始颜色");
        });
    }

    #region 外部赋值接口（你全套AR动态加载接口完整保留）
    public void SetChestMat(Material mat)
    {
        chestMaterial = mat;
        if (mat != null) originChestColor = mat.GetColor("_Color");
    }
    public void SetChestMaterial(Material mat) => SetChestMat(mat);

    public void SetLid(Transform t) => lid = t;
    public void SetLidLock(Transform t) => lidLock = t;

    public void SetKey(Transform t)
    {
        key = t;
        if (keyMaterial != null) originKeyColor = keyMaterial.GetColor("_Color");
    }
    #endregion

    // 额外拓展：密码错误回调接口（可选）
    public void CallLuaPwdError(string inputPwd)
    {
        if (LuaEnvMgr.GlobalLua == null) return;
        LuaEnvMgr.GlobalLua.DoString($"OnPasswordError('{inputPwd}')");
    }
}