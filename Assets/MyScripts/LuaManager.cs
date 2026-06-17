using UnityEngine;
using XLua;

public class LuaManager : MonoBehaviour
{
    public static LuaManager Instance;
    public LuaEnv luaEnv;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            luaEnv = new LuaEnv();
            // 这里是你原来的lua加载、绑定逻辑不变
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary> 安全释放Lua环境，先清理所有C#回调 </summary>
    public void SafeDisposeLua()
    {
        if (luaEnv == null) return;

        // 1. 先GC回收所有lua引用、解除C#回调绑定
        luaEnv.GC();
        // 2. 释放所有C#委托绑定（关键一步）
        luaEnv.Dispose(true);
        luaEnv = null;
    }

    void OnDestroy()
    {
        
        SafeDisposeLua();
    }

    void Update()
    {
        // 定时GC，减少内存堆积
        if (luaEnv != null)
            luaEnv.Tick();
    }
}