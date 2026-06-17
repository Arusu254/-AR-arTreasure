using UnityEngine;
using XLua;

public class LuaEnvMgr : MonoBehaviour
{
    public static LuaEnv GlobalLua { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        GlobalLua = new LuaEnv();

        // 直接把lua代码写在C#字符串里，完全不需要读取外部文件，不会报找不到模块
        string boxLuaCode = @"
print(""===== Lua宝箱事件脚本 加载成功 ====="")
-- 开箱回调函数，TreasureManager会调用这个
function OnBoxOpened()
    print(""[Lua打印日志] 宝箱成功打开，触发开箱事件点"")
end
-- 密码错误回调
function OnPasswordError(inputPwd)
    print(string.format(""[Lua打印日志] 输入密码错误：%s"", inputPwd))
end
";
        // 执行这段lua代码
        GlobalLua.DoString(boxLuaCode);
        Debug.Log("内嵌Lua脚本加载完成，无文件路径依赖");
    }

    void Update()
    {
        // xLua标准每帧GC，防内存泄漏
        if (GlobalLua != null)
        {
            GlobalLua.Tick();
        }
    }

    void OnDestroy()
    {
        if (GlobalLua != null)
        {
            GlobalLua.Dispose();
            GlobalLua = null;
        }
    }
}