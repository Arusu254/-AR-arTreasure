-- 打印脚本加载日志
print("===== Lua宝箱业务脚本 加载完成 =====")

-- 定义全局开箱回调函数，供C#直接调用
function OnBoxOpened()
    -- 基础打印日志（面试要求核心）
    print("[Lua事件] 宝箱解锁成功，触发开箱回调")
    print("[Lua事件] 时间戳:" .. os.time())

    -- 拓展演示：反向调用C#管理器（可选加分项）
    local treasureMgr = CS.TreasureManager.Instance
    if treasureMgr ~= nil then
        print("[Lua事件] 成功获取C# TreasureManager单例")
    end
end

-- 拓展：密码错误回调（可选，密码输错时C#调用）
function OnPasswordError(inputPwd)
    print(string.format("[Lua事件] 密码校验失败，输入密码：%s", inputPwd))
end